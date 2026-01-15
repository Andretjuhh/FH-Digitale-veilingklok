using Domain.Entities;

namespace Application.Services;

public interface IVeilingKlokEngine
{
    protected Task InitializeVeilingKlokAsync(CancellationToken cancellationToken);
    public Task AddActiveVeilingKlokAsync(VeilingKlok veilingKlok, List<Product> products);


    // Manage Veiling Klok State
    public bool IsVeillingRunning(Guid klokId);
    public Task StartVeilingAsync(Guid klokId);
    public Task PauseVeilingAsync(Guid klokId);
    public Task StopVeilingAsync(Guid klokId);
    public Task PlaceVeilingBidAsync(Guid klokId, Guid productId, DateTimeOffset placedAt, int quantity);
    public Task ChangeVeilingProductAsync(Guid klokId, Guid newProductId);
    public decimal GetCurrentPrice(Guid klokId, DateTimeOffset atTime);

    public string GetConnectionGroupName(Guid klokId);
    public string GetRegionConnectionGroupName(string country, string region);
    public (Guid KlokId, Guid UserId)? GetConnectionInfo(string connectionId);

    // Manage Socket Connections
    public Task AddSocketConnection(string connectionId, Guid klokId, Guid userId);
    public Task RemoveSocketConnection(string connectionId, Guid klokId, Guid userId);
}
