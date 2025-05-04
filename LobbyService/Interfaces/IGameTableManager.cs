using CasinoGame.Facade.Models;

namespace LobbyService.Interfaces;

public interface IGameTableManager
{
    List<string> GetPendingApprovals(Guid tableId, string player);
    Task<GameTable> CreateTable(GameTableRequest request);
    List<Game> GetAvailableGames();
    bool QuitTable(Guid tableId, string userId);
    GameTable GetTableInformation(Guid tableId);
    List<GameTable> GetAllTables();
    bool JoinTable(Guid tableId, string player);
    bool ApproveJoin(Guid tableId, string player);
    bool CancelTable(Guid tableId);
     Task HandleGameResultAsync(GameResult result);
}