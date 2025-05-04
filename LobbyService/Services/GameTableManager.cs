using CasinoGame.Facade.Models;
using LobbyService.Interfaces;
using Microsoft.Extensions.Options;

namespace LobbyService.Services;

public class GameTableManager(IOptions<GameSettings> gameSettings, HttpClient httpClient) : IGameTableManager
{
    private readonly List<GameTable> _tables = [];

    public async Task<GameTable> CreateTable(GameTableRequest request)
    {
        var table = new GameTable
        {
            GameName = request.GameName,
            CreatedBy = request.CreatedBy,
            EntryFee = request.EntryFee,
            Rules = request.Rules,
            Players = new List<string> { request.CreatedBy }
        };
        _tables.Add(table);
        
        Console.WriteLine($"Calling wallet endpoint with: {httpClient.BaseAddress}");

        
        await httpClient.PostAsJsonAsync("api/balance/deduct", new
        {
            UserId = request.CreatedBy,
            Amount = request.EntryFee
        });

        
        return table;
    }
    
    public async Task HandleGameResultAsync(GameResult result)
    {
        if (!Guid.TryParse(result.GameTableId, out var tableId))
            throw new ArgumentException("Invalid table ID format", nameof(result.GameTableId));

        var table = _tables.FirstOrDefault(t => t.Id == tableId);
        if (table == null)
            throw new InvalidOperationException($"Game table with ID {result.GameTableId} not found.");

        Console.WriteLine($"Handling result for table {tableId}: Winner = {result.WinnerUserId}, Prize = {result.WinnerAmount}, Platform Margin = {result.PlatformMargin}");
        
        await httpClient.PostAsJsonAsync("api/balance/reward", new
        {
            UserId = result.WinnerUserId,
            Amount = result.WinnerAmount
        });
        
        if (result.PlatformMargin > 0)
        {
            await httpClient.PostAsJsonAsync("api/balance/reward", new
            {
                UserId = "platform",
                Amount = result.PlatformMargin
            });
        }
        
        _tables.Remove(table);
    }

    
    public async Task<GameTable> UpdateWinnerBalance(GameTableRequest request)
    {
        var table = new GameTable
        {
            GameName = request.GameName,
            CreatedBy = request.CreatedBy,
            EntryFee = request.EntryFee,
            Rules = request.Rules,
            Players = new List<string> { request.CreatedBy }
        };
        _tables.Add(table);
        
        Console.WriteLine($"Calling wallet endpoint with: {httpClient.BaseAddress}");

        
        await httpClient.PostAsJsonAsync("api/balance/reward", new
        {
            UserId = request.CreatedBy,
            Amount = request.EntryFee
        });

        
        return table;
    }
    
    public GameTable GetTableInformation(Guid tableId)
    {
        return _tables.SingleOrDefault(t => t.Id == tableId)
               ?? throw new ArgumentNullException();
    }

    public List<GameTable> GetAllTables() => _tables;

    public bool JoinTable(Guid tableId, string player)
    {
        var table = _tables.FirstOrDefault(t => t.Id == tableId);
        if (table == null || table.Players.Contains(player)) return false;
        table.PendingApprovals.Add(player);
        return true;
    }

    public List<string> GetPendingApprovals(Guid tableId, string player)
    {
        return _tables.Where(t => t.Id == tableId && t.CreatedBy == player)
            .SelectMany(p => p.PendingApprovals)
            .ToList();
    }
    
    public List<Game> GetAvailableGames()
    {
         return gameSettings.Value.Games.Select(g => new Game
        {
            GameName = g.GameName,
            GameAddress = g.GameAddress,
            Rules = g.Rules.Select(r => new Rule
            {
                RuleName = r.RuleName,
                ValueType = ParseValueType(r.ValueType)
            }).ToList()
        }).ToList();
    }
    
    private static object ParseValueType(object valueType)
    {
        if (valueType is string str && str.Contains(";"))
            return str.Split(';').ToList();
        
        return valueType.ToString();
    }


    public bool ApproveJoin(Guid tableId, string player)
    {
        var table = _tables.FirstOrDefault(t => t.Id == tableId);
        if (table == null || !table.PendingApprovals.Contains(player)) return false;
        table.Players.Add(player);
        table.PendingApprovals.Remove(player);
        return true;
    }

    public bool CancelTable(Guid tableId)
    {
        var table = _tables.FirstOrDefault(t => t.Id == tableId);
        if (table == null) return false;
        _tables.Remove(table);
        
        // foreach (var player in table.Players)
        // {
        //     walletService.Refund(player, table.EntryFee);
        // }
        
        return true;
    }
    
    public bool QuitTable(Guid tableId, string userId)
    {
        var table = _tables.FirstOrDefault(t => t.Id == tableId);
        if (table == null) return false;

        table.Players.Remove(userId);
        // Call WalletService to refund players...
        return true;
    }
}