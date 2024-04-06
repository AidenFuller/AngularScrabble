using Scrabble.Server.Models;
using Scrabble.Server.Models.Extensions;

namespace Scrabble.Server.Services;

public interface IBoardValidator
{
    Result<List<string>> TryValidateBoard(Board board, string[] availableLetters);
}

public class BoardValidator : IBoardValidator
{
    private readonly DictionaryLookupService _dictionaryLookupService;
    private readonly BoardService _boardService;

    public BoardValidator(DictionaryLookupService dictionaryLookupService, BoardService boardService)
    {
        _dictionaryLookupService = dictionaryLookupService ?? throw new ArgumentNullException(nameof(dictionaryLookupService));
        _boardService = boardService ?? throw new ArgumentNullException(nameof(boardService));
    }

    public Result<List<string>> TryValidateBoard(Board board, string[] availableLetters)
    {
        // if (!AllUncommittedLettersInRack(board, availableLetters))
        //     return Result.Fail<List<string>>("All letters must be in the rack");

        if (!IsConnectedToExistingWord(board))
            return Result.Fail<List<string>>("At least one letter must be connected to existing words");

        if (!IsInLine(board))
            return Result.Fail<List<string>>("All letters must be in a straight line");

        if (!AreLettersConnectedTogether(board))
            return Result.Fail<List<string>>("All letters must be connected together");

        var words = _boardService.GetAllCreatedWords(board).Select(x => x.Value).ToList();

        if (!words.All(_dictionaryLookupService.IsValidWord))
            return Result.Fail<List<string>>("All words must be in the dictionary");

        return Result.Ok(words);
    }

    private static bool AllUncommittedLettersInRack(Board board, string[] availableLetters)
    {
        var uncommittedLetters = board.Cells.SelectMany(x => x.Select(y => y)).Where(x => !x.IsCommitted()).ToList();
        var availableLettersCounts = availableLetters.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());

        foreach (var letter in uncommittedLetters)
        {
            if (!availableLettersCounts.ContainsKey(letter.Value))
            {
                return false;
            }
            availableLettersCounts[letter.Value]--;
            if (availableLettersCounts[letter.Value] < 0)
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsConnectedToExistingWord(Board board)
    {
        // Make sure it isn't the first move
        if (!board.GetStartingCell().IsCommitted())
        {
            return true;
        }

        return board
            .GetAllCells()
            .Where(cell => !cell.IsCommitted())
            .Any(cell => board.GetSurroundingCells(cell)
                .Any(surroundingCell => surroundingCell.IsCommitted() && !surroundingCell.IsEmpty())); // Cell is committed and not populated
    }

    private static bool AreLettersConnectedTogether(Board board)
    {
        var uncommittedCells = board.GetAllCells().Where(x => !x.IsCommitted());
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

                if (board.Cells[row][column].IsEmpty())
                    return false;
            }
        }

        return true;
    }

    private bool IsInLine(Board board)
    {
        var uncommittedCells = board.GetAllCells().Where(x => !x.IsCommitted());
        var cellCoordinates = uncommittedCells.Select(board.GetCellPosition).ToList();
        return cellCoordinates.Select(x => x.Row).Distinct().Count() == 1 || cellCoordinates.Select(x => x.Col).Distinct().Count() == 1;
    }
}
