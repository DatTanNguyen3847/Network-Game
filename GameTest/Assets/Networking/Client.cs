using System;
using Debugging;
using SimpleJSON;
using UnityEngine;

namespace Networking
{
    public class Client : MonoBehaviour
    {
        public DebuggingScreen debugger;
        public PlayerControl player;
        public Transform player2;
        public float moveSpeed = 0.2f;

        public NetworkStats networkStats;

        WebSocket _websocket;

        public float fps = 60.0f;
        private String _player1Key = "";

        private void Awake()
        {
        }

        void Start()
        {
            networkStats = new NetworkStats();

            ConnectToServer();
            // create internal monitor network every 1000 ms
            InvokeRepeating(nameof(SendPing), 0.0f, 1.0f);

            // monitor cpu usage every 1000ms
            InvokeRepeating(nameof(SendCpuUsage), 0.0f, 1.0f);

        }

        private void Update()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            _websocket.DispatchMessageQueue();
#endif
            // Debug.Log("Update time :" + Time.deltaTime);
        }

        void FixedUpdate()
        {
            // Debug.Log("FixedUpdate time :" + Time.deltaTime);
        }

        public async void SendPing()
        {
            SendMsg(Events.Ping + "::" + DateTime.Now.Millisecond.ToString());
            debugger.Log();
        }

        public async void SendCpuUsage()
        {
            SendMsg(Events.CpuUsage + "::" + DateTime.Now.Millisecond.ToString());
        }

        private async void ConnectToServer()
        {
            _websocket = new WebSocket("ws://localhost:3000");

            _websocket.OnOpen += () =>
            {
                Debug.Log("Connection open!");
            };

            _websocket.OnError += (e) =>
            {
                Debug.Log("Error! " + e);
            };

            _websocket.OnClose += (e) =>
            {
                Debug.Log("Connection closed!");
            };

            _websocket.OnMessage += (bytes) =>
            {
                // Reading a plain text message
                var message = System.Text.Encoding.UTF8.GetString(bytes);
                // Debug.Log("OnMessage! " + message);
                string[] parts = message.Split(new string[] { "::" }, StringSplitOptions.None);
                string cmd = parts[0];
                string msg = parts[1];
                switch (cmd)
                {
                    case Events.Connection:
                        this.OnConnect(msg);
                        break;
                    case Events.Disconnect:
                        this.OnDisconnect(msg);
                        break;
                    case Events.Connected:
                        this.OnConnected(msg);
                        break;
                    case Events.AddPlayer:
                        this.OnAddPlayer(msg);
                        break;
                    case Events.RemovePlayer:
                        this.OnRemovePlayer(msg);
                        break;
                    case Events.Pong:
                        this.OnPong(msg);
                        break;
                    case Events.ClientUpdate:
                        this.OnClientUpdate(msg);
                        break;
                    case Events.ServerUpdate:
                        this.OnServerUpdate(msg);
                        break;
                    case Events.CpuUsage:
                        this.OnCpuUsage(msg);
                        break;
                }
            };
            await _websocket.Connect();
        }

        private async void OnConnect(string msg)
        {
            networkStats.msgRcv += 1;
            var jsonNode = JSON.Parse(msg);
            Game.Game game = new Game.Game(jsonNode["gid"]);
            game.addPlayer(new Player(jsonNode["uid"]));
            _player1Key = jsonNode["uid"];
        }

        private async void OnDisconnect(string msg)
        {
            networkStats.msgRcv += 1;
        }

        private async void OnConnected(string msg)
        {
            networkStats.msgRcv += 1;
        }

        private async void OnAddPlayer(string msg)
        {
            networkStats.msgRcv += 1;
        }

        private async void OnRemovePlayer(string msg)
        {
            networkStats.msgRcv += 1;
        }

        private async void OnPong(string msg)
        {
            networkStats.msgRcv += 1;
        }

        private async void OnClientUpdate(string msg)
        {
            networkStats.msgRcv += 1;
        }

        private async void OnServerUpdate(string msg)
        {
            var jsonNode = JSON.Parse(msg);
            String player2Key = "";
            foreach (var key in jsonNode.Keys)
            {
                if (key != null && !key.Equals("ts") && !key.Equals(_player1Key))
                {
                    player2Key = key;
                    break;
                }
            }
            if (!player2Key.Equals("")) {
                var playerJson = jsonNode[player2Key];
                Debug.Log(playerJson);
                var transform1 = player2.transform;
                Vector3 p = transform1.position;
                p.x = playerJson["po"]["x"] * moveSpeed;
                p.z = playerJson["po"]["z"] * moveSpeed;
                transform1.position = p;
            }
            networkStats.msgRcv += 1;
        }

        private async void OnCpuUsage(string msg)
        {
            networkStats.msgRcv += 1;
        }

        private async void SendMsg(string msg)
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