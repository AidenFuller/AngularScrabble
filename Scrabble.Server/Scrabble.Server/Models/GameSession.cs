namespace Scrabble.Server.Models;

public class GameSession
{
    public Board Board { get; set; }
    public List<Player> Players { get; } = new();
    public Player CurrentPlayer { get; set; }
    public bool IsGameInProgress { get; set; }

    public void NextPlayer()
    {
        var currentPlayerIndex = Players.IndexOf(CurrentPlayer);
        CurrentPlayer = Players[(currentPlayerIndex + 1) % Players.Count];
    }
}
