namespace Scrabble.Server.Models;

public class Board
{
    public Cell[][] Cells { get; }
    public int Size { get; }

    public Board(int size, Dictionary<Point, MultiplierData> multipliers)
    {
        Size = size;
        Cells = new Cell[size][];
        for (var row = 0; row < size; row++)
        {
            Cells[row] = new Cell[size];
            for (var col = 0; col < size; col++)
            {
                var point = new Point(row, col);
                var multiplier = multipliers.GetValueOrDefault(point, new MultiplierData(MultiplierType.None, 1));
                Cells[row][col] = new Cell(point, multiplier.Type, multiplier.Multiplier);
            }
        }
    }
}
