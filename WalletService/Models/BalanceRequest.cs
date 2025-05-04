namespace WalletService.Models;

public class BalanceRequest
{
    public string UserId { get; set; } = default!;
}

public class TransactionRequest
{
    public string UserId { get; set; } = default!;
}

public class DeductBalanceRequest
{
    public string UserId { get; set; } = default!;
    public decimal Amount { get; set; } = default!;
}

public class TopUpBalanceRequest
{
    public string UserId { get; set; } = default!;
    public decimal Amount { get; set; } = default!;
}
