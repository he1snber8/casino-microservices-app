namespace CasinoGame.Facade.Models;

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

public class GameSettings
{
    public List<Game> Games { get; set; }
}
public class Game
{
    public string GameName { get; set; }
    public string GameAddress { get; set; }
    public List<Rule> Rules { get; set; }
}

public class Rule
{
    public string RuleName { get; set; }
    public object ValueType { get; set; } // will be string or List<string>
}





