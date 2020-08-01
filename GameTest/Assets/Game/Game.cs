using System.Collections.Generic;

public class Game 
{

    public string gameId;
    public LinkedList<Player> players = new LinkedList<Player>();
    public Game(string gameId)
    {
        this.gameId = gameId;
    }

    public void addPlayer(Player player) 
    {
        this.players.AddLast(player);
    }
}