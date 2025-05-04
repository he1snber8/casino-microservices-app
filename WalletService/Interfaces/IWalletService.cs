using WalletService.Models;

namespace WalletService.Interfaces;

public interface IWalletService
{
    decimal GetBalance(string userId);
    WalletTransaction FillUpBalance(string userId, decimal amount);
    
    WalletTransaction DeductBalance(string userId, decimal amount);
    // WalletTransaction Debit(string userId, decimal amount);
    WalletTransaction RollBack(string userId, Guid transactionId);
    List<WalletTransaction> GetTransactions(string userId);
}