namespace Scrabble.Server.Models;

public class Board
{
    public Cell[][] Cells { get; }
    public int Size { get; }

    public Board(int size, Dictionary<(int Row, int Col), string> multipliers)
    {
        Size = size;
        Cells = new Cell[size][];
        for (var i = 0; i < size; i++)
        {
            Cells[i] = new Cell[size];
            for (var j = 0; j < size; j++)
            {
                var multiplier = multipliers.GetValueOrDefault((i, j));
                Cells[i][j] = new Cell(i, j, multiplier);
            }
        }
    }
}
