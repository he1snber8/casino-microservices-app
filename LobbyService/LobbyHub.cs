using CasinoGame.Facade.Models;
using LobbyService.Interfaces;
using Microsoft.AspNetCore.SignalR;


namespace LobbyService;

public class LobbyHub(IGameTableManager gameTableManager) : Hub
{
    public List<string> GetPendingApprovals(Guid tableId, string player)
    {
        Console.WriteLine("getting pending approvals");
        var result = gameTableManager.GetPendingApprovals(tableId, player);;
        Console.WriteLine(result);
        return result;
    }

    public GameTable? GetTableInformation(Guid tableId)
    {
        return gameTableManager.GetTableInformation(tableId);
    }

    public async Task GetAvailableTables()
    {
        var tables = gameTableManager.GetAllTables();
        Console.WriteLine("Getting available tables");
        Console.WriteLine(tables);
        await Clients.Caller.SendAsync("AvailableTables", tables);
    }

    public async Task GetAvailableGames()
    {
        var games = gameTableManager.GetAvailableGames();
        await Clients.All.SendAsync("AvailableGames", games);
    }

    public async Task CreateGameTable(GameTableRequest request)
    {
        var table = await gameTableManager.CreateTable(request);

        await Clients.All.SendAsync("GameTableCreated", table);
    }
    
    public async Task NotifyGameResult(GameTableRequest request)
    {
        var table = await gameTableManager.CreateTable(request);

        await Clients.All.SendAsync("GameResultConcluded", table);
    }

    public async Task JoinTable(Guid tableId, string userId)
    {
        var result = gameTableManager.JoinTable(tableId, userId);
        if (result)
            Console.WriteLine("Player Joined!!!");

        await Clients.All.SendAsync("PlayerJoined", tableId, userId);
    }

    public async Task ApproveJoin(Guid tableId, string player)
    {
        var result = gameTableManager.ApproveJoin(tableId, player);

        var table = gameTableManager.GetTableInformation(tableId);

        if (result)
            await Clients.All.SendAsync("JoinApproved", table);
    }

    public async Task CancelGame(Guid tableId)
    {
        var refundResult = gameTableManager.CancelTable(tableId);
        await Clients.All.SendAsync("GameCanceled", tableId);
    }
}