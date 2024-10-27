namespace Scrabble.Server.Models;

public class Cell(Point point, MultiplierType multiplierType, int multiplierValue)
{
    public Point Point { get; } = point;
    public MultiplierType MultiplierType { get; } = multiplierType;
    public int MultiplierValue { get; } = multiplierValue;
    public string? Value { get; set; }
    public string? CommittedValue { get; set; }
}
