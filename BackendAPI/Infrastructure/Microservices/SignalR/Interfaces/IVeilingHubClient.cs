using Application.Common.Models;

namespace Infrastructure.Microservices.SignalR.Interfaces;

// make strongly-typed communication between the server and clients.
// The generic type on Hub<T> only types the client methods.
// The data you send is typed normally, just like any other C# method parameter or return value.
public interface IVeilingHubClient
{
    Task RegionVeilingStarted(VeilingNotifications.RegionVeilingStartedNotification notification);
    Task RegionVeilingEnded();
    Task VeilingEnded();
    Task VeilingViewerCountChanged(int viewerCount);
    Task VeilingKlokUpdated(VeilingNotifications.VeilingKlokStateNotification notification);
    Task VeilingBodPlaced(VeilingNotifications.VeilingBodNotification notification);
    Task VeilingProductChanged(VeilingNotifications.VeilingProductChangedNotification notification);
    Task VeilingPriceTick(VeilingNotifications.VeilingPriceTickNotification notification);
    Task VeilingProductWaiting(VeilingNotifications.VeilingProductWaitingNotification notification);
}
