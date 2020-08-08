using System;
using System.Text;
using Debugging;
using Models;
using Networking;
using SimpleJSON;
using UnityEngine;

namespace GameEngine
{
    public class GameClient
    {
        public PlayerControl player;
        public Transform player2;
        public float moveSpeed = 0.2f;

        public NetworkStats networkStats;

        WebSocket _websocket;

        public float fps = 60.0f;
        public Player host;
        public Game game;

        private readonly MonoBehaviour _renderer;

        public GameClient(MonoBehaviour gameRenderer)
        {
            _renderer = gameRenderer;
        }

        public void Start()
        {
            networkStats = new NetworkStats();
            ConnectToServer();
        }

        public void Update()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            _websocket.DispatchMessageQueue();
#endif
            // Debug.Log("Update time :" + Time.deltaTime);
        }

        public void FixedUpdate()
        {
            // Debug.Log("FixedUpdate time :" + Time.deltaTime);
        }

        private async void ConnectToServer()
        {
            _websocket = new WebSocket("ws://localhost:3000");

            _websocket.OnOpen += () =>
            {
                Debug.Log("Connection open!");
            };

            _websocket.OnError += e =>
            {
                Debug.Log("Error! " + e);
            };

            _websocket.OnClose += e =>
            {
                Debug.Log("Connection closed!");
            };

            _websocket.OnMessage += bytes =>
            {
                // Reading a plain text message
                var message = Encoding.UTF8.GetString(bytes);
                var parts = message.Split(new[] { "::" }, StringSplitOptions.None);
                var cmd = parts[0];
                var msg = parts[1];
                switch (cmd)
                {
                    case Events.Connection:
                        OnConnect(msg);
                        break;
                    case Events.Disconnect:
                        OnDisconnect(msg);
                        break;
                    case Events.Connected:
                        OnConnected(msg);
                        break;
                    case Events.AddPlayer:
                        OnAddPlayer(msg);
                        break;
                    case Events.RemovePlayer:
                        OnRemovePlayer(msg);
                        break;
                    case Events.Pong:
                        OnPong(msg);
                        break;
                    case Events.ClientUpdate:
                        OnClientUpdate(msg);
                        break;
                    case Events.ServerUpdate:
                        OnServerUpdate(msg);
                        break;
                    case Events.CpuUsage:
                        OnCpuUsage(msg);
                        break;
                }
            };
            await _websocket.Connect();
        }

        private void OnConnect(string msg)
        {
            networkStats.msgRcv += 1;
        }

        private void OnDisconnect(string msg)
        {
            networkStats.msgRcv += 1;
        }

        private void OnConnected(string msg)
        {
            var jsonNode = JSON.Parse(msg);
            // parsing the game information to create a host and a game
            game = new Game(jsonNode[Field.GameId]);
            host = new Player(_renderer, jsonNode[Field.UserId]);
            game.AddPlayer(host);
            foreach (string playerId in jsonNode[Field.PlayerIds].Values)
            {
                if (!playerId.Equals(host.playerId))
                {
                    game.AddPlayer(new Player(_renderer, playerId));
                }
            }
            networkStats.msgRcv += 1;
        }

        private void OnAddPlayer(string msg)
        {
            networkStats.msgRcv += 1;
        }

        private void OnRemovePlayer(string msg)
        {
            networkStats.msgRcv += 1;
        }

        private void OnPong(string msg)
        {
            networkStats.msgRcv += 1;
        }

        private void OnClientUpdate(string msg)
        {
            networkStats.msgRcv += 1;
        }

        private void OnServerUpdate(string msg)
        {
            // Debug.Log(msg);
            var jsonNode = JSON.Parse(msg);
            foreach (var localPlayer in game.players)
            {
                var info = jsonNode[localPlayer.playerId];
                localPlayer.SetColor(info[Field.Color]);
                localPlayer.SetPosition(info[Field.Pos]["x"], info[Field.Pos]["y"]);
            }
            networkStats.msgRcv += 1;
        }

        private void OnCpuUsage(string msg)
        {
            networkStats.msgRcv += 1;
        }

        public async void SendMsg(string msg)
        {
            if (_websocket.State == WebSocketState.Open)
            {
                await _websocket.SendText(msg);
            }
        }


        private async void SendWebSocketMessage()
        {
            if (_websocket.State == WebSocketState.Open)
            {
                // Sending bytes
                // await websocket.Send(new byte[] { 10, 20, 30 });

                // Sending plain text
                await _websocket.SendText("plain text message");
            }
        }

        private async void OnApplicationQuit()
        {
            await _websocket.Close();
        }
    }
}