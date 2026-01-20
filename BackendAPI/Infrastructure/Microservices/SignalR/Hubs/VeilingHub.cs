using System.Security.Claims;
using System.Threading; // Added for completeness if tokens are ever added back
using Application.Services;
using Infrastructure.Microservices.SignalR.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

// NOTE: Test for Concurrent User trying to join clock group at the same time

namespace Infrastructure.Microservices.SignalR.Hubs;

// Manages in-memory state for active auction clocks
[Authorize]
public class VeilingHub : Hub<IVeilingHubClient>
{
    private readonly IVeilingKlokEngine _veilingKlokEngine;

    public VeilingHub(IVeilingKlokEngine veilingVeilingKlokEngine)
    {
        _veilingKlokEngine = veilingVeilingKlokEngine;
    }

    private Guid GetUserIdFromClaims()
    {
        var userIdClaim =
            Context.User?.FindFirst(ClaimTypes.NameIdentifier)
            ?? Context.User?.FindFirst("sub")
            ?? Context.User?.FindFirst("userId");

        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            throw new HubException("User ID not found in token claims");

        return userId;
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var connectionId = Context.ConnectionId;

        // Check if we have a mapping for this connection
        var connectionInfo = _veilingKlokEngine.GetConnectionInfo(connectionId);
        if (connectionInfo != null)
            await LeaveClock(connectionInfo.Value.KlokId);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinRegion(string country, string region)
    {
        var connectionId = Context.ConnectionId;

        await Groups.AddToGroupAsync(
            connectionId,
            _veilingKlokEngine.GetRegionConnectionGroupName(country, region)
        );
    }

    public async Task LeaveRegion(string country, string region)
    {
        var connectionId = Context.ConnectionId;
        await Groups.RemoveFromGroupAsync(
            connectionId,
            _veilingKlokEngine.GetRegionConnectionGroupName(country, region)
        );
    }

    public async Task JoinClock(Guid klokId)
    {
        var userId = GetUserIdFromClaims();
        var connectionId = Context.ConnectionId;

        var groupName = _veilingKlokEngine.GetConnectionGroupName(klokId);
        await _veilingKlokEngine.AddSocketConnection(connectionId, klokId, userId);
        await Groups.AddToGroupAsync(connectionId, groupName);
    }

    public async Task LeaveClock(Guid klokId)
    {
        var userId = GetUserIdFromClaims();
        var connectionId = Context.ConnectionId;

        var groupName = _veilingKlokEngine.GetConnectionGroupName(klokId);
        await _veilingKlokEngine.RemoveSocketConnection(connectionId, klokId, userId);
        await Groups.RemoveFromGroupAsync(connectionId, groupName, CancellationToken.None);
    }
}
