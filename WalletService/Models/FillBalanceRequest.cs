namespace WalletService.Models;

public class DeductRequest
{
    public string UserId { get; set; } = default!;
    public decimal Amount { get; set; }
}

