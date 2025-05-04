namespace CasinoGame.Facade.Models;

public class GameTable
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? GameName { get; set; }
    public string? CreatedBy { get; set; }
    public decimal EntryFee { get; set; }
    public Dictionary<string, object>? Rules { get; set; }
    public List<string> Players { get; set; } = new();
    
    public decimal PrizePool { get; set; }
    public List<string> PendingApprovals { get; set; } = new();
}


public class GameResult
{
    public string GameTableId { get; set; }
    public string WinnerUserId { get; set; }
    public decimal PrizePool { get; set; }
    public decimal WinnerAmount { get; set; }
    public decimal PlatformMargin { get; set; }
}

// public class GameTable
// {
//     public string Id { get; set; }
//     public List<string> PlayerIds { get; set; } = new();
//     public int DelayTime { get; set; } // in seconds
//     public decimal PrizePool { get; set; }
// }

public class GameDefinition
{
    public string? GameName { get; set; }
    public string? GameAddress { get; set; }
    public List<GameRule>? Rules { get; set; }
}

public class GameRule
{
    public string? RuleName { get; set; }
    public object? ValueType { get; set; }
}

public class GameTableRequest
{
    public string? GameName { get; set; }
    public string? CreatedBy { get; set; }
    public decimal EntryFee { get; set; }
    public Dictionary<string, object>? Rules { get; set; }
}

