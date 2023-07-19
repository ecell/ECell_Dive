using UnityEngine;
using TMPro;
using ECellDive.Multiplayer;
using ECellDive.Modules;

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
            playerName.gameObject.GetComponent<VirtualKeyboardLinker>().GetSetVKManager();
            ip.gameObject.GetComponent<VirtualKeyboardLinker>().GetSetVKManager();
            port.gameObject.GetComponent<VirtualKeyboardLinker>().GetSetVKManager();
            password.gameObject.GetComponent<VirtualKeyboardLinker>().GetSetVKManager();
        }

		private void CheckFieldsContent()
		{
			if (playerName.text.Length == 0)
			{
				playerName.text = "NewPlayer";
			}

			if (ip.text.Length == 0)
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
			MultiplayerModule.Instance.OnConnectionStart();

			CheckFieldsContent();

			GameNetPortal.Instance.SetConnectionSettings(playerName.text,
														ip.text, System.Convert.ToUInt16(port.text),
														password.text);
			GameNetPortal.Instance.StartHost();
		}

		public void Join()
		{
            MultiplayerModule.Instance.OnConnectionStart();

            CheckFieldsContent();

			GameNetPortal.Instance.SetConnectionSettings(playerName.text,
														ip.text, System.Convert.ToUInt16(port.text),
														password.text);
			GameNetPortal.Instance.StartClient();
		}
	}
}