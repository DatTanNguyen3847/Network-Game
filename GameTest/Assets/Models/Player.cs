using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Models
{
    public class Player
    {
        protected const float BOX_UNIT = 16.0f;
        public readonly string playerId;
        public int x = 0;
        public int y = 0;
        public Game game;
        public readonly GameObject gameObject;
        public static readonly int Color = Shader.PropertyToID("_Color");

        public Player(Component renderer, string playerId)
        {
            this.playerId = playerId;
            gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            gameObject.transform.parent = renderer.transform;
            gameObject.transform.position = new Vector3(0.5f, 0.5f, 0.5f);
            gameObject.name = "player-" + playerId;
        }

        public void Update()
        {
        }

        public Player(string playerId)
        {
            this.playerId = playerId;
        }

        public void SetGame(Models.Game gameObj)
        {
            this.game = gameObj;
        }

        public void SetColor(string hextString)
        {
            if (!ColorUtility.TryParseHtmlString(hextString, out var color)) return;
            var renderer = gameObject.GetComponent<Renderer>();
            renderer.material.SetColor(Color, color);
        }

        public void SetPosition(float serverX, float serverY)
        {
            gameObject.transform.position = new Vector3(serverY / BOX_UNIT + 0.5f, 0.5f, serverX / BOX_UNIT + 0.5f);
        }
    }
}