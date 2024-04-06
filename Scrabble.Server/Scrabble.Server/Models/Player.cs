namespace Scrabble.Server.Models;

public class Player
{
    public string PlayerName { get; }
    public List<string> Letters { get; }
    public string ConnectionId { get; set; }

    public Player(string playerName)
    {
        PlayerName = playerName;
        Letters = new List<string>();
    }

    public override bool Equals(object? obj)
    {
        if (obj is Player player)
        {
            return player.PlayerName == PlayerName;
        }

        return false;
    }
}
