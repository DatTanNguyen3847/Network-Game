using System;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using GameEngine;
using UnityEngine;
using UnityEngine.PlayerLoop;
using static System.String;

namespace Models
{
    public class Host : Player
    {
        private CharacterController controller;
        private Vector3 _playerVelocity;
        private bool _groundedPlayer;
        private float playerSpeed = 20.0f;
        private float jumpHeight = 1.0f;
        private float gravityValue = -9.81f;
        private float rotationSpeed = 25f;
        private float lerpSpeed = 22f;
        private GameObject ghost;
        private Camera camera;
        private float _fow;
        private float _mouseX, _mouseY;
        private List<Dictionary<String, String>> inputs = new List<Dictionary<string, string>>();
        private int inputSeq;
        public Host(Component renderer, string playerId) : base(renderer, playerId)
        {
            ghost = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ghost.transform.parent = renderer.transform;
            ghost.transform.position = new Vector3(0.5f, 0.5f, 0.5f);
            ghost.GetComponent<Renderer>().material.color = UnityEngine.Color.yellow;
            ghost.name = "ghost-" + playerId;

            // create a camera for host player
            camera = gameObject.AddComponent<Camera>();
            camera.transform.parent = gameObject.transform;

            controller = gameObject.AddComponent<CharacterController>();
        }

        public new void Update()
        {
            MovePlayer();
            ControlCamHead();
            AdjustFowCam();
        }

        public new void SetPosition(float serverX, float serverY)
        {
            ghost.transform.position = new Vector3(serverY / BOX_UNIT + 0.5f, 0.5f, serverX / BOX_UNIT + 0.5f);
        }
        private void MovePlayer()
        {
            // _groundedPlayer = controller.isGrounded;
            // if (_groundedPlayer && _playerVelocity.y < 0)
            // {
            //     _playerVelocity.y = 0f;
            // }
            //
            // var move = gameObject.transform.forward * Input.GetAxis("Vertical") + gameObject.transform.right * Input.GetAxis("Horizontal");
            // controller.Move(move * Time.deltaTime * playerSpeed);
            //
            // // Changes the height position of the player..
            // if (Input.GetButtonDown("Jump") && _groundedPlayer)
            // {
            //     _playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            // }
            //
            // _playerVelocity.y += gravityValue * Time.deltaTime;
            // controller.Move(_playerVelocity * Time.deltaTime);
            var cmdInputs = new ArrayList();
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                cmdInputs.Add("u");
            }
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                cmdInputs.Add("r");
            }
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                cmdInputs.Add("d");
            }
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftAlt))
            {
                cmdInputs.Add("l");
            }

            if (cmdInputs.Count <= 0) return;
            var input = Join("_", cmdInputs.ToArray());
            var msg = new string[]
            {
                "c", DateTime.Now.Millisecond.ToString(), this.playerId, input, inputSeq.ToString()
            };
            AddInputs(input);
            ProcessInputs();
            Debug.Log(Events.ClientUpdate + "::" + Join(".", msg));
            game.gameClient.SendMsg(Events.ClientUpdate + "::\"" + Join(".", msg) + "\"");
        }

        private void AddInputs(string input)
        {
            inputSeq += 1;
            Dictionary<String, String> obj = new Dictionary<string, string>();
            obj.Add(Field.Input, input);
            obj.Add(Field.InputSeq, this.inputSeq.ToString());
            inputs.Add(obj);
        }

        private void ProcessInputs()
        {
            var dX = 0;
            var dZ = 0;
            foreach (var obj in inputs)
            {
                var input = obj[Field.Input];
                foreach (var ch in input)
                {
                    if (ch.Equals('u'))
                    {
                        dX -= 1;
                    }
                    if (ch.Equals('d'))
                    {
                        dX += 1;
                    }
                    if (ch.Equals('l'))
                    {
                        dZ -= 1;
                    }
                    if (ch.Equals('r'))
                    {
                        dZ += 1;
                    }
                }
            }

            var position = gameObject.transform.position;
            var newX = position.x + dX * playerSpeed * Time.deltaTime;
            var newZ = position.z + dZ * playerSpeed * Time.deltaTime;
            var newY = position.y;
            position = new Vector3(newX, newY, newZ);
            gameObject.transform.position = position;
            inputs = new List<Dictionary<string, string>>();
        }

        private void AdjustFowCam()
        {
            _fow = Mathf.Lerp(_fow, 60f, 8f * Time.deltaTime);
            camera.fieldOfView = _fow;
        }

        private void ControlCamHead()
        {
            _mouseX += Input.GetAxis("Mouse X") * rotationSpeed * 10f * Time.deltaTime;
            _mouseY -= Input.GetAxis("Mouse Y") * rotationSpeed * 10f * Time.deltaTime;
            _mouseY = Mathf.Clamp(_mouseY, -70f, 70f);
            Quaternion rot = Quaternion.Euler(new Vector3(_mouseY, _mouseX, 0f));
            Transform transform;
            (transform = camera.transform).rotation = Quaternion.Lerp(camera.transform.rotation, rot, lerpSpeed * Time.deltaTime);
            gameObject.transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
        }
    }
}