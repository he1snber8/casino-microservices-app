using CasinoGame.Facade.Exceptions;
using CasinoGame.Facade.Models;
using LobbyService.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;


namespace LobbyService.Controllers;

[ApiController]
[Route("api")]
public class LobbyController(IGameTableManager gameTableManager, IHubContext<LobbyHub> hubContext) : Controller
{
    [HttpPost("notify/winner")]
    public async Task<IActionResult> NotifyGameWinner([FromBody] GameResult gameResult)
    {
        try
        {
            var token = Request.Headers["Authorization"].ToString();
            Console.WriteLine($"Received JWT Token: {token}");

            var transaction = gameTableManager.HandleGameResultAsync(gameResult);

            await hubContext.Clients.User(gameResult.WinnerUserId).SendAsync("GameResultConcluded", transaction);

            Console.WriteLine("TRANSACRTION ADDED!!!");

            return Ok(transaction);
        }

        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
}