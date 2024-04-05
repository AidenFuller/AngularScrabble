namespace Scrabble.Server.Models;

public class Board
{
    public Cell[][] Cells { get; }
    public int Size { get; }

    public Board(int size)
    {
        Size = size;
        Cells = new Cell[size][];
        for (var i = 0; i < size; i++)
        {
            Cells[i] = new Cell[size];
            for (var j = 0; j < size; j++)
            {
                Cells[i][j] = new Cell();
            }
        }
    }
}
