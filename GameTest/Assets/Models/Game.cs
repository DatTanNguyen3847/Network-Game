using System.Collections.Generic;

namespace Models
{
    public class Game
    {
        public string gameId;
        public readonly LinkedList<Player> players = new LinkedList<Player>();
        public Player host;
        public Game(string gameId)
        {
            this.gameId = gameId;
        }

        public void AddPlayer(Player player)
        {
            this.players.AddLast(player);
            player.SetGame(this);
        }

        public void SetHost(Player hostPlayer)
        {
            host = hostPlayer;
        }

        public void Update()
        {
            host.Update();
        }
    }
}