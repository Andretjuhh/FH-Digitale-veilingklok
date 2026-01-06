using Application.Common.Models;
using Infrastructure.Microservices.SignalR.Models;

namespace Infrastructure.Microservices.SignalR.Interfaces;

public interface IVeilingKlokNotifier
{
    /// <summary>
    /// Notify users in a region that a veiling has started
    /// </summary>
    Task NotifyRegionVeilingStarted(string regionGroupName, VeilingKlokState state);

    /// <summary>
    /// Notify users in a region that a veiling has ended
    /// </summary>
    Task NotifyRegionVeilingEnded(string regionGroupName);

    /// <summary>
    /// Notify clients that a bid was placed
    /// </summary>
    Task NotifyBidPlaced(string groupName, VeilingProductState bid);

    /// <summary>
    /// Notify clients that product has changed
    /// </summary>
    Task NotifyProductChanged(string groupName, VeilingProductState productChange);

    /// <summary>
    /// Notify clients that auction has ended
    /// </summary>
    Task NotifyAuctionEnded(string groupName);

    /// <summary>
    /// Notify clients of viewer count update
    /// </summary>
    Task NotifyViewerCountChanged(string groupName, int count);

    /// <summary>
    /// Notify clients of price tick (price decrease over time)
    /// </summary>
    Task NotifyPriceTick(string groupName, VeilingKlokState state);

    /// <summary>
    /// Notify clients that current product time expired and waiting for veilingmeester
    /// </summary>
    Task NotifyProductWaitingForNext(string groupName, Guid klokId, Guid completedProductId);

    /// <summary>
    /// Disconnect specific users from a group (when veiling ends)
    /// </summary>
    Task DisconnectUsersFromGroup(string groupName, IEnumerable<string> connectionIds);
}