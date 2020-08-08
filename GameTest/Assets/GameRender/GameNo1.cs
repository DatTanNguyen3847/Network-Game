using System;
using GameEngine;
using UnityEngine;

namespace GameRender
{
    public class GameNo1 : MonoBehaviour
    {
        private const float BOX_UNIT = 16.0f; // 16 pixels on web
        private const float X_SIZE = 480.0f; // y size on web 480 pixels
        private const float Y_SIZE = 720.0f; // x size on web 720 pixels
        private const float PLANE_SIZE = 10f;

        public GameClient gameClient;

        private void Awake()
        {
            gameClient = new GameClient(this);
        }

        private void Start()
        {
            // Create a plane for game
            GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            plane.transform.parent = this.transform;
            plane.transform.localScale = new Vector3(X_SIZE / BOX_UNIT / PLANE_SIZE, 1, Y_SIZE / BOX_UNIT / PLANE_SIZE);
            plane.transform.position = new Vector3(X_SIZE / BOX_UNIT / 2, 0, Y_SIZE / BOX_UNIT / 2);

            gameClient.Start();

            // create internal monitor network every 1000 ms
            InvokeRepeating(nameof(SendPing), 0.0f, 1.0f);

            // monitor cpu usage every 1000ms
            InvokeRepeating(nameof(SendCpuUsage), 0.0f, 1.0f);
        }

        void Update()
        {
            gameClient.Update();
        }

        private void FixedUpdate()
        {
            gameClient.FixedUpdate();
        }


        public void SendPing()
        {
            gameClient.SendMsg(Events.Ping + "::" + DateTime.Now.Millisecond);
        }

        public void SendCpuUsage()
        {
            gameClient.SendMsg(Events.CpuUsage + "::" + DateTime.Now.Millisecond);
        }
    }
}