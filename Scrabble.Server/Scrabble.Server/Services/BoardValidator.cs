using Scrabble.Server.Models;
using Scrabble.Server.Models.Extensions;

namespace Scrabble.Server.Services;

public class BoardValidator : IBoardValidator
{
    private readonly DictionaryLookupService _dictionaryLookupService;

    public BoardValidator(DictionaryLookupService dictionaryLookupService)
    {
        _dictionaryLookupService = dictionaryLookupService ?? throw new ArgumentNullException(nameof(dictionaryLookupService));
    }

    public Result IsBoardValid(Board board, string[] availableLetters)
    {
        if (!AllUncommittedLettersInRack(board, availableLetters))
            return Result.Fail("All letters must be in the rack");

        if (!IsConnectedToExistingWord(board))
            return Result.Fail("At least one letter must be connected to existing words");

        if (!IsInLine(board))
            return Result.Fail("All letters must be in a straight line");

        if (!AreLettersConnectedTogether(board))
            return Result.Fail("All letters must be connected together");

        if (!WordsInDictionary(board))
            return Result.Fail("All words must be in the dictionary");

        return Result.Ok();
    }

    private bool AllUncommittedLettersInRack(Board board, string[] availableLetters)
    {
        var uncommittedLetters = board.Cells.SelectMany(x => x.Select(y => y)).Where(x => !x.IsCommitted).ToList();
        var availableLettersCounts = availableLetters.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());

        foreach (var letter in uncommittedLetters)
        {
            if (!availableLettersCounts.ContainsKey(letter.Letter))
            {
                return false;
            }
            availableLettersCounts[letter.Letter]--;
            if (availableLettersCounts[letter.Letter] < 0)
            {
                return false;
            }
        }

        return true;
    }

    private bool IsConnectedToExistingWord(Board board)
    {
        // Make sure it isn't the first move
        if (board.GetStartingCell().IsEmpty && !board.GetStartingCell().IsCommitted)
        {
            return true;
        }

        return board
            .GetAllCells()
            .Where(cell => !cell.IsCommitted)
            .Any(cell => board.GetSurroundingCells(cell)
                .Any(surroundingCell => surroundingCell is { IsCommitted: true, IsEmpty: false }));
    }

    private bool AreLettersConnectedTogether(Board board)
    {
        var uncommittedCells = board.GetAllCells().Where(x => !x.IsCommitted);
        var cellCoordinates = uncommittedCells.Select(board.GetCellPosition).ToList();
        if (cellCoordinates.Select(x => x.Row).Distinct().Count() == 1) // Horizontal
        {
            var row = cellCoordinates.First().Row;
            var columns = cellCoordinates.Select(x => x.Col).ToList();
            var start = cellCoordinates.Min(x => x.Col);
            var end = cellCoordinates.Max(x => x.Col);

            for (int column = start; column <= end; column++)
            {
                if (columns.Contains(column)) continue;

                if (board.Cells[row][column].IsEmpty)
                    return false;
            }
        }

        return true;
    }

    private bool IsInLine(Board board)
    {
        var uncommittedCells = board.GetAllCells().Where(x => !x.IsCommitted);
        var cellCoordinates = uncommittedCells.Select(board.GetCellPosition).ToList();
        return cellCoordinates.Select(x => x.Row).Distinct().Count() == 1 || cellCoordinates.Select(x => x.Col).Distinct().Count() == 1;
    }

    public bool WordsInDictionary(Board board)
    {
        var cellCoordinates = board.GetAllCells().Select(board.GetCellPosition).ToList();
        var firstRow = cellCoordinates.First().Row;
        var firstCol = cellCoordinates.First().Col;
        if (cellCoordinates.Select(x => x.Row).Distinct().Count() == 1) // Main word is horizontal
        {
            var word = EvaluateHorizontalWord(board, firstRow, firstCol);

            if (word.Length == 1 || !_dictionaryLookupService.IsValidWord(word))
                return false;

            // iterate through the vertical words
            foreach (var column in cellCoordinates.Select(x => x.Col))
            {
                var verticalWord = EvaluateVerticalWord(board, firstRow, column);
                if (verticalWord.Length == 1 || !_dictionaryLookupService.IsValidWord(verticalWord))
                    return false;
            }
        }
        else // Main word is vertical
        {
            var word = EvaluateVerticalWord(board, firstRow, firstCol);

            if (word.Length == 1 || !_dictionaryLookupService.IsValidWord(word))
                return false;

            // iterate through the horizontal words
            foreach (var row in cellCoordinates.Select(x => x.Row))
            {
                var horizontalWord = EvaluateHorizontalWord(board, row, firstCol);
                if (horizontalWord.Length == 1 || !_dictionaryLookupService.IsValidWord(horizontalWord))
                    return false;
            }
        }

        return true;
    }

    private string EvaluateVerticalWord(Board board, int row, int col)
    {
        var startRow = row;
        var endRow = row;

        while (startRow > 0 && !board.Cells[startRow - 1][col].IsEmpty)
            startRow--;

        while (endRow < board.Size - 1 && !board.Cells[endRow + 1][col].IsEmpty)
            endRow++;

        return string.Join("", board.Cells[startRow..(endRow + 1)].Select(x => x[col].Letter));
    }

    private string EvaluateHorizontalWord(Board board, int row, int col)
    {
        var startColumn = col;
        var endColumn = col;

        while (startColumn > 0 && !board.Cells[row][startColumn - 1].IsEmpty)
            startColumn--;

        while (endColumn < board.Size - 1 && !board.Cells[row][endColumn + 1].IsEmpty)
            endColumn++;

        return string.Join("", board.Cells[row][startColumn..(endColumn + 1)].Select(x => x.Letter));
    }
}
