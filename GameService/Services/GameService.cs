using CasinoGame.Facade.Models;
using GameService.Interfaces;
using System.Net.Http.Json;

namespace GameService.Services;

public class GameService(HttpClient httpClient) : IGameService
{
    private readonly Dictionary<string, GameTable> _gameTables = new();
    private readonly List<GameResult> _results = new();

    public GameTable CreateGameTable(string gameName, string createdBy, decimal entryFee, List<string> players, Dictionary<string, object>? rules = null)
    {
        var table = new GameTable
        {
            GameName = gameName,
            CreatedBy = createdBy,
            EntryFee = entryFee,
            Players = players,
            Rules = rules,
            PrizePool = players.Count * entryFee
        };

        _gameTables[table.Id.ToString()] = table;
        return table;
    }
    
    public async Task HandleGameResultAsync(Guid tableId, GameResult result)
    {
        Console.WriteLine($"Handling game result for table {tableId}...");
        Console.WriteLine($"Winner: {result.WinnerUserId}, Prize: {result.WinnerAmount}");
        
        await httpClient.PostAsJsonAsync("api/notify/winner", new
        {
            UserId = result.WinnerUserId,
            Amount = result.WinnerAmount
        });
    }

    public async Task<GameResult> StartGameAsync(string gameTableId)
    {
        if (!_gameTables.TryGetValue(gameTableId, out var table))
            throw new Exception("GameTable not found.");

        if (table.Players.Count < 2)
            throw new Exception("Not enough players.");
        
        if (table.Rules != null && table.Rules.TryGetValue("DelayTime", out var delayObj) && int.TryParse(delayObj?.ToString(), out int delaySeconds))
        {
            await Task.Delay(delaySeconds * 1000);
        }

        var random = new Random();
        var winnerIndex = random.Next(table.Players.Count);
        var winnerId = table.Players[winnerIndex];

        var result = new GameResult
        {
            GameTableId = table.Id.ToString(),
            WinnerUserId = winnerId,
            PrizePool = table.PrizePool,
            WinnerAmount = table.PrizePool * 0.95m,
            PlatformMargin = table.PrizePool * 0.05m
        };

        _results.Add(result);

        Console.WriteLine($"Calling wallet endpoint at: {httpClient.BaseAddress}");

        await httpClient.PostAsJsonAsync("api/balance/reward", new
        {
            UserId = result.WinnerUserId,
            Amount = result.WinnerAmount
        });

        return result;
    }

    public GameResult? GetGameResult(string gameTableId)
    {
        return _results.LastOrDefault(r => r.GameTableId == gameTableId);
    }
}
