namespace CasinoGame.Facade.Exceptions;

public class InsufficientFundsException : Exception
{
    public InsufficientFundsException(string userId, decimal amount)
        : base($"User {userId} has insufficient funds. Tried to deduct {amount}.")
    {
    }
}
