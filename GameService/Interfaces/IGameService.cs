using CasinoGame.Facade.Models;

namespace GameService.Interfaces;

public interface IGameService
{
    GameTable CreateGameTable(string gameName, string createdBy, decimal entryFee, List<string> players, Dictionary<
        string, object>? rules = null);
    Task<GameResult> StartGameAsync(string gameTableId);
    GameResult? GetGameResult(string gameTableId);
    
    Task HandleGameResultAsync(Guid tableId, GameResult result);
}