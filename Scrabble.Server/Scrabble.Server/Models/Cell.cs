namespace Scrabble.Server.Models;

public class Cell(int row, int col, string? multiplier)
{
    public int Row { get; set; } = row;
    public int Col { get; set; } = col;
    public string? Multiplier { get; set; } = multiplier;
    public string? Value { get; set; }
    public string? CommittedValue { get; set; }
}
