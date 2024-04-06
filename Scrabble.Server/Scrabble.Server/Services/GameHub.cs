using Microsoft.AspNetCore.SignalR;
using Scrabble.Server.Models;

namespace Scrabble.Server.Services;

public class GameHub : Hub
{
    private readonly GameSessionManager _gameSessionManager;

    public GameHub(GameSessionManager gameSessionManager)
    {
        _gameSessionManager = gameSessionManager ?? throw new ArgumentNullException(nameof(gameSessionManager));
    }

    public async Task<Result<bool>> CanJoinGame(string sessionKey)
    {
        return _gameSessionManager.CanJoinGameSession(sessionKey);
    }

    public async Task<Result> JoinGame(string sessionKey, string playerName)
    {
        if (_gameSessionManager.TryGetPlayerConnectionForSession(sessionKey, playerName, out var connectionId))
        {
            await Groups.RemoveFromGroupAsync(connectionId!, sessionKey);
            await Clients.Clients(connectionId!).SendAsync("PlayerKicked");
        }

        var result = _gameSessionManager.JoinGameSession(sessionKey, playerName, Context.ConnectionId);
        if (result.IsSuccess)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, sessionKey);
            await Clients.Group(sessionKey).SendAsync("PlayerJoined", playerName);

            await Clients.Caller.SendAsync("BoardChanged", result.Value);
        }

        return Result.Ok();
    }

    public async Task<Result> PlaceLetter(string sessionKey, int row, int col, string letter)
    {
        var result = _gameSessionManager.PlaceLetter(sessionKey, row, col, letter);
        if (result.IsSuccess)
        {
            await Clients.Group(sessionKey).SendAsync("LetterChanged", new LetterChange(row, col, letter));
        }

        return result;
    }

    public async Task<Result> CommitWord(string sessionKey)
    {
        var result = _gameSessionManager.CommitWord(sessionKey);
        if (result.IsSuccess)
        {
            await Clients.Group(sessionKey).SendAsync("BoardChanged", result.Value.Board);
            await Clients.Group(sessionKey).SendAsync("ScoreChanged", result.Value.Score);
        }

        return Result.Ok();
    }

    public async Task<Result> StartGame(string sessionKey)
    {
        return _gameSessionManager.StartGame(sessionKey);
    }

    private class LetterChange(int row, int col, string letter)
    {
        public int Row { get; set; } = row;
        public int Col { get; set; } = col;
        public string Value { get; set; } = letter;
    }

    private class LetterClear(int row, int col)
    {
        public int Row { get; set; } = row;
        public int Col { get; set; } = col;
    }
}
