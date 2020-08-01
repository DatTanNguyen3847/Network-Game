using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


using NativeWebSocket;
using SimpleJSON;
public class Client : MonoBehaviour
{
    public DebuggingScreen debugger;
    public PlayerControl player;
    public Transform player2;

    public NetworkStats networkStats;

    WebSocket websocket;

    public float fps = 60.0f;

    private void Awake()
    {
    }

    void Start()
    {
        networkStats = new NetworkStats();

        ConnectToServer();
        // create internal monitor network every 1000 ms
        InvokeRepeating("SendPing", 0.0f, 1.0f);

        // monitor cpu usage every 1000ms
        InvokeRepeating("SendCPUUsage", 0.0f, 1.0f);

    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket.DispatchMessageQueue();
#endif
        // Debug.Log("Update time :" + Time.deltaTime);
    }

    void FixedUpdate()
    {
        // Debug.Log("FixedUpdate time :" + Time.deltaTime);
    }

    async void SendPing()
    {
        SendMsg(Events.PING + "::" + DateTime.Now.Millisecond.ToString());
        debugger.Log();
    }

    async void SendCPUUsage()
    {
        SendMsg(Events.CPU_USAGE + "::" + DateTime.Now.Millisecond.ToString());
    }

    private async void ConnectToServer()
    {
        websocket = new WebSocket("ws://localhost:3000");

        websocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
        };

        websocket.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed!");
        };

        websocket.OnMessage += (bytes) =>
        {
            // Reading a plain text message
            var message = System.Text.Encoding.UTF8.GetString(bytes);
            // Debug.Log("OnMessage! " + message);
            string[] parts = message.Split(new string[] { "::" }, StringSplitOptions.None);
            string cmd = parts[0];
            string msg = parts[1];
            switch (cmd)
            {
                case Events.CONNECTION:
                    this.onConnect(msg);
                    break;
                case Events.DISCONNECT:
                    this.onDisconnect(msg);
                    break;
                case Events.CONNECTED:
                    this.onConnected(msg);
                    break;
                case Events.ADD_PLAYER:
                    this.onAddPlayer(msg);
                    break;
                case Events.REMOVE_PLAYER:
                    this.onRemovePlayer(msg);
                    break;
                case Events.PONG:
                    this.onPong(msg);
                    break;
                case Events.CLIENT_UPDATE:
                    this.onClientUpdate(msg);
                    break;
                case Events.SERVER_UPDATE:
                    this.onServerUpdate(msg);
                    break;
                case Events.CPU_USAGE:
                    this.onCpuUsage(msg);
                    break;
            }
        };
        await websocket.Connect();
    }

    async void onConnect(string msg)
    {
        networkStats.msgRcv += 1;
        var jsonNode = JSON.Parse(msg);
        Game game = new Game(jsonNode["gid"]);
        game.addPlayer(new Player(jsonNode["uid"]));
    }

    async void onDisconnect(string msg)
    {
        networkStats.msgRcv += 1;
    }

    async void onConnected(string msg)
    {
        networkStats.msgRcv += 1;
    }

    async void onAddPlayer(string msg)
    {
        networkStats.msgRcv += 1;
    }

    async void onRemovePlayer(string msg)
    {
        networkStats.msgRcv += 1;
    }

    async void onPong(string msg)
    {
        networkStats.msgRcv += 1;
    }

    async void onClientUpdate(string msg)
    {
        networkStats.msgRcv += 1;
    }

    async void onServerUpdate(string msg)
    {
        var jsonNode = JSON.Parse(msg);
        // Debug.Log(jsonNode);
        // var serverUpdate = jsonNode["sup"];
        var playerJson = jsonNode["a9f6c206"];
        Debug.Log(playerJson);
        Vector3 p = player2.transform.position;
        p.x = playerJson["po"]["x"];
        p.z = playerJson["po"]["z"];
        player2.transform.position = p;
        networkStats.msgRcv += 1;
    }

    async void onCpuUsage(string msg)
    {
        networkStats.msgRcv += 1;
    }

    async void SendMsg(string msg)
    {
        if (websocket.State == WebSocketState.Open)
        {
            await websocket.SendText(msg);
        }
    }


    async void SendWebSocketMessage()
    {
        if (websocket.State == WebSocketState.Open)
        {
            // Sending bytes
            // await websocket.Send(new byte[] { 10, 20, 30 });

            // Sending plain text
            await websocket.SendText("plain text message");
        }
    }

    private async void OnApplicationQuit()
    {
        await websocket.Close();
    }
}