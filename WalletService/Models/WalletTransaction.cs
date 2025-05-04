namespace WalletService.Models;

public class WalletTransaction
{
    public Guid TransactionId { get; set; } = Guid.NewGuid();
    public string UserId { get; set; } = null!;
    public decimal Amount { get; set; }
    public bool RolledBack { get; set; } = false;
}