using Microsoft.AspNetCore.SignalR;
using Scrabble.Server.Models;

namespace Scrabble.Server.Services;

public class GameHub(GameSessionManager gameSessionManager) : Hub
{
    private readonly GameSessionManager _gameSessionManager = gameSessionManager ?? throw new ArgumentNullException(nameof(gameSessionManager));

    private static class Command
    {
        public const string PlayerJoined = "PlayerJoined";
        public const string PlayerKicked = "PlayerKicked";
        public const string BoardChanged = "BoardChanged";
        public const string LetterChanged = "LetterChanged";
        public const string ScoreChanged = "ScoreChanged";
    }

    public Task<Result<bool>> CanJoinGame(string sessionKey)
    {
        return Task.FromResult(_gameSessionManager.CanJoinGameSession(sessionKey));
    }

    public async Task<Result> JoinGame(string sessionKey, string playerName)
    {
        if (_gameSessionManager.TryGetPlayerConnectionForSession(sessionKey, playerName, out var connectionId))
        {
            await Groups.RemoveFromGroupAsync(connectionId!, sessionKey);
            await Clients.Clients(connectionId!).SendAsync(Command.PlayerKicked);
        }

        var result = _gameSessionManager.JoinGameSession(sessionKey, playerName, Context.ConnectionId);
        if (!result.IsSuccess)
        {
            return result;
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, sessionKey);
        await Clients.Group(sessionKey).SendAsync(Command.PlayerJoined, playerName);
        await Clients.Caller.SendAsync(Command.BoardChanged, result.Value);

        return Result.Ok();
    }

    public async Task<Result> PlaceLetter(string sessionKey, int row, int col, string letter)
    {
        var isPlayersTurnResult = _gameSessionManager.IsPlayersTurn(sessionKey, Context.ConnectionId);
        if (!isPlayersTurnResult.IsSuccess)
        {
            return isPlayersTurnResult;
        }

        var result = _gameSessionManager.PlaceLetter(sessionKey, row, col, letter);
        if (result.IsSuccess)
        {
            await Clients.Group(sessionKey).SendAsync(Command.LetterChanged, new LetterChange(row, col, letter));
        }

        return result;
    }

    public async Task<Result> CommitWord(string sessionKey)
    {
        var result = _gameSessionManager.CommitWord(sessionKey);
        if (!result.IsSuccess)
            return result;

        await Clients.Group(sessionKey).SendAsync(Command.BoardChanged, result.Value.Board);
        await Clients.Group(sessionKey).SendAsync(Command.ScoreChanged, result.Value.Score);

        return Result.Ok();
    }

    public Task<Result> StartGame(string sessionKey)
    {
        return Task.FromResult(_gameSessionManager.StartGame(sessionKey));
    }

    private record LetterChange(int Row, int Column, string Letter);
}
