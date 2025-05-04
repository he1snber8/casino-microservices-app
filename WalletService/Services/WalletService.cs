using CasinoGame.Facade.Exceptions;
using WalletService.Interfaces;
using WalletService.Models;

namespace WalletService.Services;

public class WalletService : IWalletService
{
    private readonly Dictionary<string, decimal> _balances = new();
    private readonly List<WalletTransaction> _transactions = new();

    public decimal GetBalance(string userId)
    {
        _balances.TryGetValue(userId, out var balance);
        return balance;
    }
    
    public List<WalletTransaction> GetTransactions(string userId)
    {
        return _transactions.Where(w => w.UserId == userId).ToList();
    }

    public WalletTransaction FillUpBalance(string userId, decimal amount)
    {
        _balances.TryAdd(userId, 0);

        _balances[userId] += amount;

        var tx = new WalletTransaction { UserId = userId, Amount = amount};
        _transactions.Add(tx);

        return tx;
    }

    public WalletTransaction RollBack(string userId, Guid transactionId)
    {
        _balances.TryAdd(userId, 0);
        
        var originalTTransaction = _transactions
            .LastOrDefault(t => t.TransactionId == transactionId && t.RolledBack is false);

        if (originalTTransaction != null)
        {
            _balances[userId] -= originalTTransaction.Amount;
            originalTTransaction.RolledBack = true;
            return originalTTransaction;
        }
        
        throw new Exception("Original transaction not found to refund.");
    }

    public WalletTransaction DeductBalance(string userId, decimal amount)
    {
        _balances.TryAdd(userId, 0);

        // if (_balances[userId] - amount < 0)
        //     throw new InsufficientFundsException(userId, amount);

        _balances[userId] -= amount;

        var tx = new WalletTransaction { UserId = userId, Amount = amount};
        _transactions.Add(tx);

        return tx;
    }
    
}