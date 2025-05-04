using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using WalletService.Interfaces;
using WalletService.Models;

namespace WalletService
{
    [Authorize(Policy = "LobbyOnly")]
    public class WalletHub(IWalletService walletService) : Hub
    {
        public async Task FillUpBalance(string userId, decimal amount)
        {
            var transaction = walletService.FillUpBalance(userId, amount);
            
            await Clients.User(userId).SendAsync("TransactionAdded", transaction);
        }
        
        public async Task DeductBalance(string userId, decimal amount)
        {
            var transaction = walletService.DeductBalance(userId, amount);
            
            await Clients.User(userId).SendAsync("TransactionAdded", transaction);
        }
        
        public async Task RollbackTransaction(string userId, Guid transactionId)
        {
            var success = walletService.RollBack(userId, transactionId);
            var data = new
            {
                UserId = userId,
                TransactionId = transactionId.ToString(),
                Success = success
            };

            await Clients.User(userId).SendAsync("TransactionRolledBack", data);
        }

        public Task<decimal> GetBalance(string userId)
        {
            var balance = walletService.GetBalance(userId);
            return Task.FromResult(balance);
        }

        public Task<List<WalletTransaction>> GetTransactions(string userId)
        {
            var transactions = walletService.GetTransactions(userId);
            
            return Task.FromResult(transactions);
        }
    }
}