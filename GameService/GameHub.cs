using Microsoft.AspNetCore.Authorization;

namespace GameService;

using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

[Authorize(Policy = "LobbyOnly")]
public class GameHub : Hub
{
    public async Task NotifyWinner(string winnerName)
    {
        await Clients.All.SendAsync("ReceiveWinnerNotification", winnerName);
    }
    
    public async Task CreateGameTable(string gameRules)
    {
        await Clients.All.SendAsync("GameTableCreated", gameRules);
    }
    
    public async Task ApproveJoinRequest(string userName, string gameTableId)
    {
        await Clients.Group(gameTableId).SendAsync("UserJoined", userName);
    }
    
    public async Task JoinGameTable(string userName, string gameTableId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, gameTableId);
        await Clients.Group(gameTableId).SendAsync("UserJoined", userName);
    }
    
    public async Task LeaveGameTable(string userName, string gameTableId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameTableId);
        await Clients.Group(gameTableId).SendAsync("UserLeft", userName);
    }
}
