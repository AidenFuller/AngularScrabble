namespace Scrabble.Server.Models.Extensions;

public static class BoardExtensions
{
    public static IEnumerable<Cell> GetAllCells(this Board board)
    {
        return board.Cells.SelectMany(row => row);
    }

    public static Cell GetStartingCell(this Board board)
    {
        return board.Cells[board.Size / 2][board.Size / 2];
    }

    public static Cell[] GetSurroundingCells(this Board board, Cell cell)
    {
        // Return the 3x3 grid of cells surrounding the given cell excluding the cell itself
        var (row, col) = board.GetCellPosition(cell);
        var cells = new List<Cell>();
        for (var i = row - 1; i <= row + 1; i++)
        {
            for (var j = col - 1; j <= col + 1; j++)
            {
                if (i >= 0 && i < board.Size && j >= 0 && j < board.Size && (i != row || j != col))
                {
                    cells.Add(board.Cells[i][j]);
                }
            }
        }

        return cells.ToArray();
    }

    public static (int Row, int Col) GetCellPosition(this Board board, Cell cell)
    {
        for (var row = 0; row < board.Size; row++)
        {
            for (var col = 0; col < board.Size; col++)
            {
                if (board.Cells[row][col] == cell)
                {
                    return (row, col);
                }
            }
        }

        throw new ArgumentException("Cell not found on board");
    }
}
