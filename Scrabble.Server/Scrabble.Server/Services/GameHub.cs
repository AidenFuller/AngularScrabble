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

    public async Task<Result<bool>> CanJoinGameSession(string sessionKey)
    {
        return _gameSessionManager.CanJoinGameSession(sessionKey);
    }

    public async Task<Result> JoinGame(string sessionKey, string playerName)
    {
        var result = _gameSessionManager.JoinGameSession(sessionKey, playerName);
        if (result.IsSuccess)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, sessionKey);
            await Clients.Group(sessionKey).SendAsync("PlayerJoined", playerName);
        }

        return result;
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
}
