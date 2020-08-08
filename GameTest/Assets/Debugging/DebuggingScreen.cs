using GameEngine;
using Networking;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Debugging
{
    public class DebuggingScreen : MonoBehaviour
    {
        public Text text;

        [FormerlySerializedAs("clientGame")] public GameClient gameClient;

        public void setClient(GameClient gameClient) {
            this.gameClient = gameClient;
        }

        private void Update()
        {

        }

        public void Log()
        {
            string msg = "[network stats]\n"
                         + "latency\t=\t" + "\n"
                         + "msg_rcv_count\t=\t" + gameClient.networkStats.msgRcv +"\n"
                         + "cpu_usage\t=\t" + "\n"
                         + "rrs\t=\t" + "\n"
                         + "heap_total\t=\t" + "\n"
                         + "heap_usages\t=\t" + "\n"
                         + "\n"
                         + "[game state]\n"
                         + "game_id\t=\t" + "\n"
                         + "num_players\t=\t" + "\n"
                         + "my_id\t=\t" + "\n"
                         + "my_pos\t=\t" + "\n"
                         + "client_time=\t=\t" + "\n"
                         + "server_time=\t=\t" + "\n"
                         + "ts_difference\t=\t" + "\n"
                         + "dt\t=\t" + "\n"
                         + "dte\t=\t" + "\n"
                         + "\n"
                         + "[client_prediction]\n"
                         + "my_input_seq\t=\t" + "\n"
                         + "my_processed_input_seq\t=\t" + "\n"
                         + "server_processed_input_seq\t=\t" + "\n"
                         + "total_local_inputs\t=\t" + "\n"
                         + "\n"
                         + "[client interpolation]\n"
                         + "difference\t=\t" + "\n"
                         + "max_difference\t=\t" + "\n"
                         + "p\t=\t" + "\n"
                ;

            this.text.text = msg;
        }
    }
}