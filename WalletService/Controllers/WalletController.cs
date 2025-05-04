using CasinoGame.Facade.Exceptions;
using WalletService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using WalletService.Interfaces;
using WalletService.Models;

namespace WalletService.Controllers;

[ApiController]
[Route("api")]
[Authorize(Policy = "LobbyOnly")]
public class WalletController(IWalletService walletService, IHubContext<WalletHub> hubContext) : Controller
{
    [HttpPost("balance/deduct")]
    public async Task<IActionResult> DeductBalance([FromBody] DeductBalanceRequest deductBalanceRequest)
    {
        try
        {
            var token = Request.Headers["Authorization"].ToString();
            Console.WriteLine($"Received JWT Token: {token}");

            var transaction = walletService.DeductBalance(deductBalanceRequest.UserId, deductBalanceRequest.Amount);

            await hubContext.Clients.User(deductBalanceRequest.UserId).SendAsync("TransactionAdded", transaction);

            Console.WriteLine("TRANSACTION ADDED!!!");

            return Ok(transaction);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpPost("balance/reward")]
    public async Task<IActionResult> DeductBalance([FromBody] TopUpBalanceRequest deductBalanceRequest)
    {
        try
        {
            var token = Request.Headers["Authorization"].ToString();
            Console.WriteLine($"Received JWT Token: {token}");
            
            var transaction = walletService.DeductBalance(deductBalanceRequest.UserId, deductBalanceRequest.Amount);

            await hubContext.Clients.User(deductBalanceRequest.UserId).SendAsync("TransactionAdded", transaction);

            Console.WriteLine("TRANSACTION ADDED!!!");

            return Ok(transaction);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
}