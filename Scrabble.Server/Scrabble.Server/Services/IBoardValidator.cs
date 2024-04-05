using Scrabble.Server.Models;

namespace Scrabble.Server.Services;

public interface IBoardValidator
{
    Result IsBoardValid(Board board, string[] availableLetters);
}
