using Microsoft.Extensions.Options;
using Scrabble.Server.Models;
using Scrabble.Server.Models.Extensions;

namespace Scrabble.Server.Services;

public class BoardService
{
    private readonly int _boardSize;
    private readonly Dictionary<Point, MultiplierData> _multipliers;
    private readonly Dictionary<string, int> _scores;

    public BoardService(IOptions<BoardServiceConfiguration> configuration)
    {
        var config = configuration.Value ?? throw new ArgumentNullException(nameof(configuration));
        _boardSize = config.Size;
        _multipliers = config.Multipliers.ToDictionary(dto => new Point(dto.Row, dto.Col), dto => new MultiplierData(dto.Type, dto.Multiplier));
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
        var cells = word.IsHorizontal
            ? board.Cells[word.Start.Row][word.Start.Column..word.End.Column]
            : board.Cells[word.Start.Row..word.End.Row].Select(x => x[word.Start.Column]).ToArray();

        var uncommittedCells = cells.Where(x => !x.IsCommitted()).ToArray();
        var wordMultiplier = uncommittedCells.Aggregate(1, (multiple, cell) => multiple * (cell.MultiplierType == MultiplierType.Word ? cell.MultiplierValue : 1));
        var bonus = uncommittedCells.Length == 7 ? 50 : 0;
        return wordMultiplier * cells.Sum(ScoreCell) + bonus;
    }

    private int ScoreCell(Cell cell)
    {
        var multiplier = 1;
        if (!cell.IsCommitted() && cell.MultiplierType == MultiplierType.Letter)
        {
            multiplier = cell.MultiplierValue;
        }

        return _scores.GetValueOrDefault(cell.Value!) * multiplier;
    }

    public static List<Word> GetAllCreatedWords(Board board)
    {
        var words = new List<Word>();

        var cellCoordinates = board.GetAllCells().Where(x => !x.IsCommitted()).Select(cell => cell.Point).ToList();
        var firstRow = cellCoordinates.First().Row;
        var firstColumn = cellCoordinates.First().Column;
        if (cellCoordinates.Select(x => x.Row).Distinct().Count() == 1) // Main word is horizontal
        {
            var word = EvaluateHorizontalWord(board, firstRow, firstColumn);

            if (word.Length > 1)
                words.Add(word);

            // iterate through the vertical words
            var verticalWords = cellCoordinates
                .Select(point => EvaluateVerticalWord(board, firstRow, point.Column))
                .Where(verticalWord => verticalWord.Length >= 1);

            words.AddRange(verticalWords);
        }
        else // Main word is vertical
        {
            var word = EvaluateVerticalWord(board, firstRow, firstColumn);

            if (word.Length > 1)
                words.Add(word);

            // iterate through the horizontal words
            var horizontalWords = cellCoordinates
                .Select(point => EvaluateHorizontalWord(board, point.Row, firstColumn))
                .Where(horizontalWord => horizontalWord.Length > 1);

            words.AddRange(horizontalWords);
        }

        return words;
    }



    private static Word EvaluateVerticalWord(Board board, int row, int column)
    {
        var startRow = row;
        var endRow = row;

        while (startRow > 0 && !board.Cells[startRow - 1][column].IsEmpty())
            startRow--;

        while (endRow < board.Size - 1 && !board.Cells[endRow + 1][column].IsEmpty())
            endRow++;

        var startPoint = new Point(startRow, column);
        var endPoint = new Point(endRow, column);

        return new Word(startPoint, endPoint, string.Join("", board.Cells[startRow..(endRow + 1)].Select(x => x[column].Value)));
    }

    private static Word EvaluateHorizontalWord(Board board, int row, int column)
    {
        var startColumn = column;
        var endColumn = column;

        while (startColumn > 0 && !board.Cells[row][startColumn - 1].IsEmpty())
            startColumn--;

        while (endColumn < board.Size - 1 && !board.Cells[row][endColumn + 1].IsEmpty())
            endColumn++;

        var startPoint = new Point(row, startColumn);
        var endPoint = new Point(row, endColumn);

        return new Word(startPoint, endPoint, string.Join("", board.Cells[row][startColumn..(endColumn + 1)].Select(x => x.Value)));
    }

    public class Word(Point start, Point end, string value)
    {
        public Point Start { get; } = start;
        public Point End { get; } = end;
        public bool IsHorizontal => Start.Row == End.Row;
        public string Value { get; } = value;
        public int Length => Value.Length;
    }
}
