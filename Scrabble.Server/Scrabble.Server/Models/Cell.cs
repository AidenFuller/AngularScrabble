namespace Scrabble.Server.Models;

public class Cell
{
    public char? Letter { get; }
    public bool IsEmpty => !Letter.HasValue;
}