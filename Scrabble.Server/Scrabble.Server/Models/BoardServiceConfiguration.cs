namespace Scrabble.Server.Models;

public class BoardServiceConfiguration
{
    public int SessionTimeoutInMinutes { get; set; } = 10;
    public int Size { get; set; }
    public Dictionary<string, List<GridCoordinate>> Multipliers { get; set; } = new();
    public Dictionary<string, int> Scores { get; set; } = new();

    public class GridCoordinate
    {
        public int Row { get; set; }
        public int Col { get; set; }
    }
}
