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

    public bool TryGetPlayerConnectionForSession(string sessionKey, string playerName, out string? connectionId)
    {
        connectionId = null;
        if (!_gameSessionsByKey.TryGetValue(sessionKey, out var gameSession))
            return false;

        connectionId = gameSession.Players.FirstOrDefault(x => x.PlayerName == playerName)?.ConnectionId;
        return connectionId != null;
    }

    public Result<Board> JoinGameSession(string sessionKey, string playerName, string connectionId)
    {
        if (!_gameSessionsByKey.TryGetValue(sessionKey, out var gameSession))
            return Result.Fail<Board>($"Cannot find session with key: {sessionKey}");

        if (gameSession.IsGameInProgress)
            return Result.Fail<Board>("Game is already in progress");

        var player = gameSession.Players.FirstOrDefault(x => x.PlayerName == playerName);
        if (player == null)
        {
            gameSession.Players.Add(new(playerName)
            {
                ConnectionId = connectionId
            });
        }
        else
        {
            player.ConnectionId = connectionId;
        }

        return Result.Ok(gameSession.Board);
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

    public Result ClearLetter(string sessionKey, int x, int y)
    {
        if (!_gameSessionsByKey.TryGetValue(sessionKey, out var gameSession))
            return Result.Fail($"Cannot find session with key: {sessionKey}");

        // if (!gameSession.IsGameInProgress)
        //     return Result.Fail("Game is not in progress");

        var cell = gameSession.Board.Cells[x][y];
        cell.Value = null;

        return Result.Ok();
    }

    public Result PlaceLetter(string sessionKey, int x, int y, string letter)
    {
        if (!_gameSessionsByKey.TryGetValue(sessionKey, out var gameSession))
            return Result.Fail($"Cannot find session with key: {sessionKey}");

        // if (!gameSession.IsGameInProgress)
        //     return Result.Fail("Game is not in progress");

        var cell = gameSession.Board.Cells[x][y];
        cell.Value = letter;

        return Result.Ok();
    }

    public Result<(Board Board, int Score)> CommitWord(string sessionKey)
    {
        if (!_gameSessionsByKey.TryGetValue(sessionKey, out var gameSession))
            return Result.Fail<(Board Board, int Score)>($"Cannot find session with key: {sessionKey}");

        if (!gameSession.IsGameInProgress)
            return Result.Fail<(Board Board, int Score)>("Game is not in progress");

        var result = _boardValidator.TryValidateBoard(gameSession.Board, gameSession.CurrentPlayer.Letters.ToArray());
        if (!result.IsSuccess)
            return Result.Fail<(Board Board, int Score)>(result.Message);

        var score = _boardService.ScoreAllWords(gameSession.Board);

        gameSession.Board.GetAllCells().ToList().ForEach(x => x.CommittedValue = x.Value);
        gameSession.NextPlayer();

        return Result.Ok((gameSession.Board, score));
    }
}
