namespace Scrabble.Server.Models;

public class BoardServiceConfiguration
{
    public int SessionTimeoutInMinutes { get; set; } = 10;
    public int Size { get; set; }
    public MultiplierDto[] Multipliers { get; set; } = [];
    public Dictionary<string, int> Scores { get; set; } = new();
    public Dictionary<string, int> LetterDistribution { get; set; } = new();
}

public class MultiplierDto
{
    public int Row { get; set; }
    public int Col { get; set; }
    public MultiplierType Type { get; set; }
    public int Multiplier { get; set; }
}
