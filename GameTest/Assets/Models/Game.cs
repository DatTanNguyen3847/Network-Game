using System.Collections.Generic;
using GameEngine;

namespace Models
{
    public class Game
    {
        public string gameId;
        public readonly LinkedList<Player> players = new LinkedList<Player>();
        public Host host;
        public GameClient gameClient;
        public Game(string gameId)
        {
            this.gameId = gameId;
        }

        public void AddPlayer(Player player)
        {
            this.players.AddLast(player);
            player.SetGame(this);
        }

        public void SetHost(Host hostPlayer)
        {
            this.players.AddLast(hostPlayer);
            host = hostPlayer;
            host.game = this;
        }

        public void Update()
        {
            host.Update();
        }
    }
}