namespace Scrabble.Server.Models;

public record Point(int Row, int Column)
{
    public static Point At(int row, int column) => new(row, column);
}