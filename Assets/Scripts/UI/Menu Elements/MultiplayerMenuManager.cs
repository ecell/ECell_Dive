using UnityEngine;
using TMPro;
using ECellDive.Multiplayer;

namespace ECellDive.UI
{
    public class MultiplayerMenuManager : MonoBehaviour
    {
        private static MultiplayerMenuManager Instance;

        public TMP_InputField playerName;
        public TMP_InputField ip;
        public TMP_InputField port;
        public TMP_InputField password;
        public TMP_Text message;

        public static string messageContent = "";
        // Start is called before the first frame update
        void Start()
        {
            Instance = this;
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
                ip.text = "";
            }

            if (port.text.Length == 0)
            {
                port.text = "0";
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
                                                        ip.text, System.Convert.ToUInt16(port.text),
                                                        password.text);
            GameNetPortal.Instance.StartHost();
        }

        /// <summary>
        /// Will set the link to the VirtualKeyboard
        /// </summary>
        public void Initialize()
        {
            playerName.gameObject.GetComponent<VirtualKeyboardLinker>().GetSetVKManager();
            ip.gameObject.GetComponent<VirtualKeyboardLinker>().GetSetVKManager();
            port.gameObject.GetComponent<VirtualKeyboardLinker>().GetSetVKManager();
            password.gameObject.GetComponent<VirtualKeyboardLinker>().GetSetVKManager();
        }

        public void Join()
        {
            CheckFieldsContent();

            GameNetPortal.Instance.SetConnectionSettings(playerName.text,
                                                        ip.text, System.Convert.ToUInt16(port.text),
                                                        password.text);
            GameNetPortal.Instance.StartClient();
        }

        public static void SetMessage(string _msg)
        {
            messageContent = _msg;
            Instance.Display();
        }

        private void Display()
        {
            message.text = messageContent;
        }

    }
}