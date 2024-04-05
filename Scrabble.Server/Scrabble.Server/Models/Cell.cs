namespace Scrabble.Server.Models;

public class Cell
{
    public string? Letter { get; set; }
    public bool IsEmpty => Letter == null;
    public bool IsBlank => Letter == string.Empty;
    public bool IsCommitted { get; set; }
}
