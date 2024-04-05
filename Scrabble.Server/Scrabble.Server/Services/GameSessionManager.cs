using Scrabble.Server.Models;
using Scrabble.Server.Models.Extensions;

namespace Scrabble.Server.Services;

public class GameSessionManager
{
    private readonly BoardService _boardService;
    private readonly IBoardValidator _boardValidator;
    private readonly Dictionary<string, GameSession> _gameSessionsByKey = new(StringComparer.OrdinalIgnoreCase);

    public GameSessionManager(BoardService boardService, IBoardValidator boardValidator)
    {
        _boardService = boardService ?? throw new ArgumentNullException(nameof(boardService));
        _boardValidator = boardValidator ?? throw new ArgumentNullException(nameof(boardValidator));

        _gameSessionsByKey.Add("TEST", new GameSession()
        {
            Board = _boardService.GetNewBoard(),
            IsGameInProgress = false
        });
    }

    public Result<bool> CanJoinGameSession(string sessionKey)
    {
        if (!_gameSessionsByKey.TryGetValue(sessionKey, out var gameSession))
            return Result.Ok(true);

        return Result.Ok(!gameSession.IsGameInProgress);
    }

    public Result JoinGameSession(string sessionKey, string playerName)
    {
        if (!_gameSessionsByKey.TryGetValue(sessionKey, out var gameSession))
            return Result.Fail($"Cannot find session with key: {sessionKey}");

        if (gameSession.IsGameInProgress)
            return Result.Fail("Game is already in progress");

        gameSession.Players.Add(new Player(playerName));
        return Result.Ok();
    }

    public Result StartGame(string sessionKey)
    {
        if (!_gameSessionsByKey.TryGetValue(sessionKey, out var gameSession))
            return Result.Fail($"Cannot find session with key: {sessionKey}");

        if (gameSession.IsGameInProgress)
            return Result.Fail("Game is already in progress");

        if (gameSession.Players.Count < 2)
            return Result.Fail("Not enough players to start game");

        gameSession.IsGameInProgress = true;
        gameSession.CurrentPlayer = gameSession.Players.First();
        return Result.Ok();
    }

    public Result PlaceLetter(string sessionKey, int x, int y, string letter)
    {
        if (!_gameSessionsByKey.TryGetValue(sessionKey, out var gameSession))
            return Result.Fail($"Cannot find session with key: {sessionKey}");

        // if (!gameSession.IsGameInProgress)
        //     return Result.Fail("Game is not in progress");

        var cell = gameSession.Board.Cells[x][y];
        cell.Letter = letter;
        cell.IsCommitted = false;

        return Result.Ok();
    }

    public Result CommitWord(string sessionKey)
    {
        if (!_gameSessionsByKey.TryGetValue(sessionKey, out var gameSession))
            return Result.Fail<bool>($"Cannot find session with key: {sessionKey}");

        if (!gameSession.IsGameInProgress)
            return Result.Fail<bool>("Game is not in progress");

        var result = _boardValidator.IsBoardValid(gameSession.Board, gameSession.CurrentPlayer.Letters.ToArray());
        if (!result.IsSuccess)
            return result;

        gameSession.Board.GetAllCells().ToList().ForEach(x => x.IsCommitted = true);
        gameSession.NextPlayer();

        return Result.Ok();
    }
}
