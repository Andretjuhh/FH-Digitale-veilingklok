using Application.Common.Models;
using Infrastructure.Microservices.SignalR.Hubs;
using Infrastructure.Microservices.SignalR.Interfaces;
using Infrastructure.Microservices.SignalR.Models;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Microservices.SignalR.Services;

// Handles SignalR notifications - broadcasts to clients
public class VeilingKlokNotifier : IVeilingKlokNotifier
{
    private readonly IHubContext<VeilingHub, IVeilingHubClient> _hubContext;

    public VeilingKlokNotifier(IHubContext<VeilingHub, IVeilingHubClient> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyRegionVeilingStarted(string regionGroupName, VeilingKlokState state)
    {
        var notification = new VeilingNotifications.RegionVeilingStartedNotification
        {
            ClockId = state.ClockId,
            Country = state.Country,
            Region = state.RegionOrState,
            StartTime = state.VeilingStartTime
        };
        await _hubContext.Clients.Group(regionGroupName).RegionVeilingStarted(notification);
    }

    public async Task NotifyRegionVeilingEnded(string regionGroupName)
    {
        await _hubContext.Clients.Group(regionGroupName).RegionVeilingEnded();
    }

    public async Task NotifyBidPlaced(string groupName, VeilingProductState bid)
    {
        var notification = new VeilingNotifications.VeilingBodNotification
        {
            ProductId = bid.ProductId,
            Price = bid.LastBidPrice,
            Quantity = bid.InitialStock - bid.RemainingStock,
            RemainingQuantity = bid.RemainingStock
        };
        await _hubContext.Clients.Group(groupName).VeilingBodPlaced(notification);
    }

    public async Task NotifyProductChanged(string groupName, VeilingProductState productChange)
    {
        var notification = new VeilingNotifications.VeilingProductChangedNotification
        {
            ProductId = productChange.ProductId,
            StartingPrice = productChange.StartingPrice,
            Quantity = productChange.InitialStock
        };
        await _hubContext.Clients.Group(groupName).VeilingProductChanged(notification);
    }

    public async Task NotifyAuctionEnded(string groupName)
    {
        await _hubContext.Clients.Group(groupName).VeilingEnded();
    }

    public async Task NotifyViewerCountChanged(string groupName, int count)
    {
        await _hubContext.Clients.Group(groupName).VeilingViewerCountChanged(count);
    }

    public async Task NotifyKlokUpdate(string groupName, VeilingKlokState state, int viewerCount)
    {
        var currentProduct = state.GetCurrentProduct();
        var notification = new VeilingNotifications.VeilingKlokStateNotification
        {
            Status = state.Status,
            ClockId = state.ClockId,
            CurrentProductId = currentProduct.ProductId,
            CurrentPrice = state.CurrentPrice,
            StartingPrice = currentProduct.StartingPrice,
            LowestPrice = currentProduct.LowestPrice,
            RemainingQuantity = currentProduct.RemainingStock,
            LiveViewerCount = viewerCount,
            EndTime = state.VeilingEndTime,
            TotalRounds = state.TotalRounds
        };
        await _hubContext.Clients.Group(groupName).VeilingKlokUpdated(notification);
    }

    public async Task NotifyPriceTick(string groupName, VeilingKlokState state)
    {
        var currentProduct = state.GetCurrentProduct();
        var notification = new VeilingNotifications.VeilingPriceTickNotification
        {
            ClockId = state.ClockId,
            ProductId = currentProduct.ProductId,
            CurrentPrice = state.CurrentPrice,
            TickTime = DateTimeOffset.UtcNow
        };
        await _hubContext.Clients.Group(groupName).VeilingPriceTick(notification);
    }

    public async Task NotifyProductWaitingForNext(
        string groupName,
        Guid klokId,
        Guid completedProductId
    )
    {
        var notification = new VeilingNotifications.VeilingProductWaitingNotification
        {
            ClockId = klokId,
            CompletedProductId = completedProductId
        };
        await _hubContext.Clients.Group(groupName).VeilingProductWaiting(notification);
    }

    public async Task DisconnectUsersFromGroup(string groupName, IEnumerable<string> connectionIds)
    {
        // Remove each connection from the group
        foreach (var connectionId in connectionIds)
            await _hubContext.Groups.RemoveFromGroupAsync(connectionId, groupName);
    }
}