using Microsoft.Extensions.Options;
using Scrabble.Server.Models;
using Scrabble.Server.Models.Extensions;

namespace Scrabble.Server.Services;

public class BoardService
{

    private readonly int _boardSize;
    private readonly Dictionary<(int Row, int Col), string> _multipliers;
    private readonly Dictionary<string, int> _scores;

    public BoardService(IOptions<BoardServiceConfiguration> configuration)
    {
        var config = configuration?.Value ?? throw new ArgumentNullException(nameof(configuration));
        _boardSize = config.Size;
        _multipliers = config.Multipliers.SelectMany(x => x.Value.Select(y => (y.Row, y.Col, Multiplier: x.Key))).ToDictionary(x => (x.Row, x.Col), x => x.Multiplier);
        _scores = config.Scores ?? throw new ArgumentNullException(nameof(config.Scores));
    }

    public Board GetNewBoard()
    {
        return new Board(_boardSize, _multipliers);
    }

    public int ScoreAllWords(Board board)
    {
        var words = GetAllCreatedWords(board);
        return words.Sum(x => ScoreWord(board, x));
    }

    private int ScoreWord(Board board, Word word)
    {
        Cell[] cells;
        if (word.IsHorizontal)
        {
            cells = board.Cells[word.StartRow][word.StartCol..word.EndCol];
        }
        else
        {
            cells = board.Cells[word.StartRow..word.EndRow].Select(x => x[word.StartCol]).ToArray();
        }

        var uncommittedCells = cells.Where(x => !x.IsCommitted()).ToArray();
        var wordMultiplier = uncommittedCells.Aggregate(1, (m, cell) => m * (cell.Multiplier == "DW" ? 2 : cell.Multiplier == "TW" ? 3 : 1));
        var bonus = uncommittedCells.Length == 7 ? 50 : 0;
        return wordMultiplier * cells.Sum(ScoreCell) + bonus;
    }

    private int ScoreCell(Cell cell)
    {
        var multiplier = 1;
        if (!cell.IsCommitted())
        {
            if (cell.Multiplier == "DL")
                multiplier = 2;
            else if (cell.Multiplier == "TL")
                multiplier = 3;
        }

        return _scores.GetValueOrDefault(cell.Value) * multiplier;
    }

    public List<Word> GetAllCreatedWords(Board board)
    {
        var words = new List<Word>();

        var cellCoordinates = board.GetAllCells().Where(x => !x.IsCommitted()).Select(board.GetCellPosition).ToList();
        var firstRow = cellCoordinates.First().Row;
        var firstCol = cellCoordinates.First().Col;
        if (cellCoordinates.Select(x => x.Row).Distinct().Count() == 1) // Main word is horizontal
        {
            var word = EvaluateHorizontalWord(board, firstRow, firstCol);

            if (word.Length > 1)
                words.Add(word);

            // iterate through the vertical words
            foreach (var column in cellCoordinates.Select(x => x.Col))
            {
                var verticalWord = EvaluateVerticalWord(board, firstRow, column);
                if (verticalWord.Length >= 1)
                    words.Add(verticalWord);
            }
        }
        else // Main word is vertical
        {
            var word = EvaluateVerticalWord(board, firstRow, firstCol);

            if (word.Length > 1)
                words.Add(word);

            // iterate through the horizontal words
            foreach (var row in cellCoordinates.Select(x => x.Row))
            {
                var horizontalWord = EvaluateHorizontalWord(board, row, firstCol);
                if (horizontalWord.Length > 1)
                    words.Add(horizontalWord);
            }
        }

        return words;
    }



    private static Word EvaluateVerticalWord(Board board, int row, int col)
    {
        var startRow = row;
        var endRow = row;

        while (startRow > 0 && !board.Cells[startRow - 1][col].IsEmpty())
            startRow--;

        while (endRow < board.Size - 1 && !board.Cells[endRow + 1][col].IsEmpty())
            endRow++;

        return new Word
        {
            StartRow = startRow,
            EndRow = endRow,
            StartCol = col,
            EndCol = col,
            Value = string.Join("", board.Cells[startRow..(endRow + 1)].Select(x => x[col].Value))
        };
    }

    private static Word EvaluateHorizontalWord(Board board, int row, int col)
    {
        var startColumn = col;
        var endColumn = col;

        while (startColumn > 0 && !board.Cells[row][startColumn - 1].IsEmpty())
            startColumn--;

        while (endColumn < board.Size - 1 && !board.Cells[row][endColumn + 1].IsEmpty())
            endColumn++;

        return new Word
        {
            StartRow = row,
            EndRow = row,
            StartCol = startColumn,
            EndCol = endColumn,
            Value = string.Join("", board.Cells[row][startColumn..(endColumn + 1)].Select(x => x.Value))
        };
    }

    public class Word
    {
        public int StartRow { get; set; }
        public int StartCol { get; set; }
        public int EndRow { get; set; }
        public int EndCol { get; set; }
        public bool IsHorizontal => StartRow == EndRow;
        public string Value { get; set; }
        public int Length => Value.Length;
    }
}
