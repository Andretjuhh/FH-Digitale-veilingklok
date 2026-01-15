using System.Collections.Concurrent;
using Application.Common.Exceptions;
using Application.Common.Models;
using Application.Repositories;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Microservices.SignalR.Interfaces;
using Infrastructure.Microservices.SignalR.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Microservices.SignalR.Services;

public class VeilingKlokEngine : IVeilingKlokEngine, IHostedService
{
    // Services provider for the manager
    private readonly IServiceProvider _serviceProvider;
    private readonly IVeilingKlokNotifier _notifier;
    private readonly ILogger<VeilingKlokEngine> _logger;
    private readonly ConcurrentDictionary<Guid, VeilingKlokState> _activeVeilingClocks = new();
    private readonly ConcurrentDictionary<Guid, Timer> _clockTimers = new();

    // Track Klok Viewers are connected
    private static readonly ConcurrentDictionary<
        Guid,
        ConcurrentDictionary<Guid, byte>
    > KlokViewers = new();

    // Track user connection id with their clock and user IDs
    private static readonly ConcurrentDictionary<
        string,
        (Guid KlokId, Guid UserId)
    > KlokConnections = new();

    public VeilingKlokEngine(
        IServiceProvider serviceProvider,
        IVeilingKlokNotifier notifier,
        ILogger<VeilingKlokEngine> logger
    )
    {
        _serviceProvider = serviceProvider;
        _notifier = notifier;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Initialize the veiling klok engine on startup
        return InitializeVeilingKlokAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        // Disable error WARNING CS4014
#pragma warning disable CS4014
        // Stop all veiling klok activities and clean up timers
        foreach (var veilingKlok in _activeVeilingClocks)
            StopVeilingAsync(veilingKlok.Key);
#pragma warning  restore CS4014

        // Dispose all timers
        foreach (var timer in _clockTimers.Values)
            timer?.Dispose();
        _clockTimers.Clear();

        return Task.CompletedTask;
    }

    public async Task InitializeVeilingKlokAsync(CancellationToken cancellationToken)
    {
        // Create a scope to get scoped services (repositories)
        using var scope = _serviceProvider.CreateScope();
        // Add necessary repositories here
        var veilingKlokRepository =
            scope.ServiceProvider.GetRequiredService<IVeilingKlokRepository>();
        var productRepository = scope.ServiceProvider.GetRequiredService<IProductRepository>();

        // Load active veiling clocks from the repository
        var activeKloks = (
            await veilingKlokRepository.GetAllByStatusAsync(
                VeilingKlokStatus.Started,
                cancellationToken
            )
        ).ToList();

        if (activeKloks.Count == 0)
        {
            _logger.LogInformation("No active auctions to recover");
            return;
        }

        // Initialize each active clock (materialized list avoids multiple enumeration)
        foreach (var veilingKlok in activeKloks)
        {
            // Load products for the klok (ordered by position)
            var productIds = veilingKlok.GetOrderedProductIds();

            var products = await productRepository.GetAllByIds(productIds);
            await AddActiveVeilingKlokAsync(veilingKlok, products.ToList());
        }

        _logger.LogInformation($"Initialized with {activeKloks.Count} active auctions");
    }

    #region Veiling Klok Management

    public bool IsVeillingRunning(Guid klokId)
    {
        // Check if there's an active ticker (timer) for this clock
        return _clockTimers.ContainsKey(klokId);
    }

    // Add a new active veiling klok
    public Task AddActiveVeilingKlokAsync(VeilingKlok veilingKlok, List<Product> products)
    {
        var state = new VeilingKlokState(veilingKlok, products);
        _activeVeilingClocks[veilingKlok.Id] = state;
        _logger.LogInformation($"Added active veiling klok {veilingKlok.Id} to engine");
        return Task.CompletedTask;
    }

    // Start the veiling klok
    public async Task StartVeilingAsync(Guid klokId)
    {
        if (!_activeVeilingClocks.TryGetValue(klokId, out var state))
        {
            _logger.LogError($"Cannot start veiling klok {klokId}: not found in active clocks");
            return;
        }

        // Start the veiling klok
        state.StartVeiling();

        // Notify region users that veiling has started
        await _notifier.NotifyRegionVeilingStarted(
            GetRegionConnectionGroupName(state.Country, state.RegionOrState),
            state
        );

        // Notify connected users of the update
        await _notifier.NotifyKlokUpdate(
            GetConnectionGroupName(klokId),
            state,
            GetViewerCount(klokId)
        );

        _logger.LogInformation($"Started veiling klok {klokId}");
    }

    // Pause the veiling klok
    public async Task PauseVeilingAsync(Guid klokId)
    {
        if (!_activeVeilingClocks.TryGetValue(klokId, out var state))
        {
            _logger.LogError($"Cannot pause veiling klok {klokId}: not found in active clocks");
            return;
        }

        // Update status
        state.Status = VeilingKlokStatus.Paused;

        // Stop the price ticker
        StopPriceTicker(klokId);

        // Notify clients about the pause (state update)
        await _notifier.NotifyKlokUpdate(
            GetConnectionGroupName(klokId),
            state,
            GetViewerCount(klokId)
        );

        _logger.LogInformation($"Paused veiling klok {klokId}");
    }

    // Stop the veiling klok
    public async Task StopVeilingAsync(Guid klokId)
    {
        if (!_activeVeilingClocks.TryGetValue(klokId, out var state))
        {
            _logger.LogWarning($"Cannot stop veiling klok {klokId}: not found in active clocks");
            return;
        }

        // Update the klok status
        state.Status = VeilingKlokStatus.Stopped;

        // Stop the price ticker
        StopPriceTicker(klokId);

        // Notify all users that veiling has ended (they receive this before disconnection)
        await _notifier.NotifyAuctionEnded(GetConnectionGroupName(klokId));
        await _notifier.NotifyRegionVeilingEnded(
            GetRegionConnectionGroupName(state.Country, state.RegionOrState)
        );

        // Disconnect all users from this clock (removes from SignalR group)
        await DisconnectAllKlokConnections(klokId);

        // Remove from active clocks
        _activeVeilingClocks.TryRemove(klokId, out _);

        _logger.LogInformation($"Stopped and cleaned up veiling klok {klokId}");
    }

    // Place a bid on the veiling klok
    public async Task PlaceVeilingBidAsync(
        Guid klokId,
        Guid productId,
        DateTimeOffset placedAt,
        int quantity
    )
    {
        var state = _activeVeilingClocks[klokId];
        var productState = state.GetCurrentProduct();
        var bidPrice = state.GetCurrentPriceByDate(placedAt);

        // Update product state
        if (quantity > productState.RemainingStock)
            throw CustomException.InsufficientStock();

        // Place the bid on the product
        productState.PlaceBid(bidPrice, quantity);

        // Notify clients about the bid placement
        await _notifier.NotifyBidPlaced(GetConnectionGroupName(klokId), productState);
    }

    public async Task ChangeVeilingProductAsync(Guid klokId, Guid newProductId)
    {
        _logger.LogInformation(
            "Changing product to {ProductId} for klok {KlokId}",
            newProductId,
            klokId
        );

        if (!_activeVeilingClocks.TryGetValue(klokId, out var state))
        {
            _logger.LogError("Cannot change product for klok {KlokId}: not found", klokId);
            throw CustomException.VeilingKlokNotStarted();
        }

        // Stop existing ticker before starting a new one for the new product
        StopPriceTicker(klokId);

        state.StartProductVeiling(newProductId);
        var currentProduct = state.GetCurrentProduct();

        // Restart the price ticker for the new product
        StartPriceTicker(klokId, state);

        await _notifier.NotifyProductChanged(GetConnectionGroupName(klokId), currentProduct);

        _logger.LogInformation(
            "Changed product to {ProductId} for klok {KlokId}",
            newProductId,
            klokId
        );
    }

    #endregion

    #region Price Ticking Mechanism

    private void StartPriceTicker(Guid klokId, VeilingKlokState state)
    {
        // Create a timer that ticks every second
        var timer = new Timer(
            async _ =>
            {
                try
                {
                    if (_activeVeilingClocks.TryGetValue(klokId, out var currentState))
                    {
                        // Update the current price based on time
                        currentState.Tick();
                        var currentProduct = currentState.GetCurrentProduct();

                        // Debug log to confirm ticker is running
                        // _logger.LogInformation(
                        //     "Ticker running for {KlokId}. Price: {Price}",
                        //     klokId,
                        //     currentState.CurrentPrice
                        // );

                        // Notify clients about the price tick
                        await _notifier.NotifyPriceTick(
                            GetConnectionGroupName(klokId),
                            currentState
                        );

                        // Check if current product time has expired
                        if (currentState.CurrentProductVeilingEnded())
                        {
                            // Current product time expired - notify clients and wait for veilingmeester
                            _logger.LogInformation(
                                "Product {ProductId} time expired for klok {KlokId}. Waiting for veilingmeester to select next product.",
                                currentProduct.ProductId,
                                klokId
                            );

                            // Notify clients that we're waiting for next product
                            await _notifier.NotifyProductWaitingForNext(
                                GetConnectionGroupName(klokId),
                                klokId,
                                currentProduct.ProductId
                            );

                            // Stop ticker as product ended
                            StopPriceTicker(klokId);
                        }
                    }
                    else
                    {
                        // Safely cleanup if clock is gone
                        StopPriceTicker(klokId);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in price ticker for klok {KlokId}", klokId);
                }
            },
            null,
            TimeSpan.FromSeconds(1),
            TimeSpan.FromSeconds(1)
        );

        _clockTimers[klokId] = timer;
        _logger.LogInformation("Started price ticker for klok {KlokId}", klokId);
    }

    private void StopPriceTicker(Guid klokId)
    {
        if (_clockTimers.TryRemove(klokId, out var timer))
        {
            timer?.Dispose();
            _logger.LogInformation("Stopped price ticker for klok {KlokId}", klokId);
        }
    }

    #endregion

    # region Socket Management

    public string GetConnectionGroupName(Guid klokId)
    {
        return $"VeilingKlok-{klokId}";
    }

    public string GetRegionConnectionGroupName(string country, string region)
    {
        return $"VeilingRegion-{country}-{region}";
    }

    public (Guid KlokId, Guid UserId)? GetConnectionInfo(string connectionId)
    {
        // Try to get the mapping
        KlokConnections.TryGetValue(connectionId, out var mapping);
        return mapping;
    }

    private int GetViewerCount(Guid klokId)
    {
        return KlokViewers.TryGetValue(klokId, out var viewers) ? viewers.Count : 0;
    }

    public Task AddSocketConnection(string connectionId, Guid klokId, Guid userId)
    {
        // Add the connection mapping
        KlokConnections[connectionId] = (klokId, userId);

        // Add user to viewers set (thread-safe)
        var viewers = KlokViewers.GetOrAdd(klokId, _ => new ConcurrentDictionary<Guid, byte>());
        viewers[userId] = 0; // value is irrelevant

        _notifier.NotifyViewerCountChanged(GetConnectionGroupName(klokId), viewers.Count);
        return Task.CompletedTask;
    }

    public Task RemoveSocketConnection(string connectionId, Guid klokId, Guid userId)
    {
        // Remove user from viewers set (thread-safe)
        KlokConnections.TryRemove(connectionId, out _);

        // Remove user from viewers set (thread-safe)
        if (KlokViewers.TryGetValue(klokId, out var viewers))
        {
            viewers.TryRemove(userId, out _);

            // 3. CLEANUP: If no one is watching this Klok anymore, remove the Klok entry entirely
            if (viewers.IsEmpty)
                // Note: In high-concurrency, another user might join right here.
                // We use the overload that ensures we only remove it if it's still empty.
                KlokViewers.TryRemove(KeyValuePair.Create(klokId, viewers));

            _notifier.NotifyViewerCountChanged(GetConnectionGroupName(klokId), viewers.Count);
        }

        return Task.CompletedTask;
    }

    private async Task DisconnectAllKlokConnections(Guid klokId)
    {
        var groupName = GetConnectionGroupName(klokId);

        // Get all connections for this clock before removing them
        var connectionsToRemove = KlokConnections
            .Where(kvp => kvp.Value.KlokId == klokId)
            .Select(kvp => kvp.Key)
            .ToList();

        // Remove users from SignalR group (actual disconnection)
        if (connectionsToRemove.Any())
        {
            await _notifier.DisconnectUsersFromGroup(groupName, connectionsToRemove);
            _logger.LogInformation(
                "Disconnected {ConnectionCount} connections from klok {KlokId} SignalR group",
                connectionsToRemove.Count,
                klokId
            );
        }

        // Clean up tracking data
        foreach (var connectionId in connectionsToRemove)
            KlokConnections.TryRemove(connectionId, out _);

        // Remove all viewers for this clock
        if (KlokViewers.TryRemove(klokId, out var viewers))
            _logger.LogInformation(
                "Removed {ViewerCount} viewers from klok {KlokId}",
                viewers.Count,
                klokId
            );
    }

    #endregion
}
