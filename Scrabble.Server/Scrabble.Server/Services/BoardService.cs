using Scrabble.Server.Models;

namespace Scrabble.Server.Services;

public class BoardService
{
    private readonly BoardServiceConfiguration _configuration;
    private readonly Dictionary<Guid, BoardState> _boards = new();

    public BoardService(BoardServiceConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Guid CreateSession()
    {
        var board = GetNewBoard();
        var boardState = new BoardState
        {
            Board = board,
            CurrentPlayer = 0,
            LastPlayed = DateTimeOffset.Now
        };
        var sessionId = Guid.NewGuid();
        _boards.Add(sessionId, boardState);
        return sessionId;
    }

    private Board GetNewBoard()
    {
        return new Board(15);
    }

    private bool PlaceLetter(Guid sessionId, int x, int y, string letter)
    {
        
    }

    private bool ValidatePendingWord(Guid sessionId)
    {

    }

    private bool CommitPendingWord(Guid sessionId)
    {

    }

    public Board GetBoardForSession(Guid sessionId)
    {
        if (!_boards.TryGetValue(sessionId, out var value))
        {
            throw new ArgumentException("Invalid session ID");
        }

        return value.Board;
    }

    public void PurgeOldBoards()
    {
        var sessionIds = _boards.Keys.ToList();
        var currentTime = DateTimeOffset.Now;
        foreach (var sessionId in sessionIds)
        {
            if (currentTime - _boards[sessionId].LastPlayed > TimeSpan.FromMinutes(_configuration.SessionTimeoutInMinutes))
            {
                _boards.Remove(sessionId);
            }
        }
    }
}

internal class BoardState
{
    public Board Board { get; set; }
    public int CurrentPlayer { get; set; }
    public DateTimeOffset LastPlayed { get; set; }
}
