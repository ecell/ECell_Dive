using UnityEngine;
using TMPro;
using ECellDive.Multiplayer;

namespace ECellDive.UI
{
    public class MultiplayerMenuManager : MonoBehaviour
    {
        public TMP_InputField playerName;
        public TMP_InputField ip;
        public TMP_InputField port;
        public TMP_InputField password;

        // Start is called before the first frame update
        void Start()
        {
            gameObject.SetActive(false);
        }

        private void CheckFieldsContent()
        {
            if(playerName.text.Length == 0)
            {
                playerName.text = "NewPlayer";
            }
            
            if(ip.text.Length == 0)
            {
                ip.text = "127.0.0.1";
            }

            if (port.text.Length == 0)
            {
                port.text = "7777";
            }

            if (password.text.Length == 0)
            {
                password.text = "1234";
            }
        }

        public void Host()
        {
            CheckFieldsContent();

            GameNetPortal.Instance.SetConnectionSettings(playerName.text,
                                                        ip.text, System.Convert.ToInt32(port.text),
                                                        password.text);
            GameNetPortal.Instance.StartHost();
        }

        public void Join()
        {
            CheckFieldsContent();

            GameNetPortal.Instance.SetConnectionSettings(playerName.text,
                                                        ip.text, System.Convert.ToInt32(port.text),
                                                        password.text);
            GameNetPortal.Instance.StartClient();
        }

    }
}