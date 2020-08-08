using System.Transactions;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Models
{
    public class Host : Player
    {
        public Host(Component renderer, string playerId) : base(renderer, playerId)
        {
        }

        public Host(string playerId) : base(playerId)
        {
        }

        public void Update()
        {
        }
    }
}