using UnityEngine;
using TMPro;
using ECellDive.Multiplayer;
using ECellDive.Modules;

namespace ECellDive.UI
{
	/// <summary>
	/// The class to manage the multiplayer menu UI.
	/// It is used both to join and host a multiplayer session.
	/// </summary>
	public class MultiplayerMenuManager : MonoBehaviour
	{
		/// <summary>
		/// The reference to the input field for the player name.
		/// </summary>
		public TMP_InputField playerName;

		/// <summary>
		/// The reference to the input field for the IPv4 address of the
		/// multiplayer server.
		/// </summary>
		public TMP_InputField ip;

		/// <summary>
		/// The reference to the input field for the port of the multiplayer
		/// server.
		/// </summary>
		public TMP_InputField port;

		/// <summary>
		/// The reference to the input field for the password of the
		/// multiplayer server.
		/// </summary>
		public TMP_InputField password;

		void Start()
		{
			playerName.gameObject.GetComponent<VirtualKeyboardLinker>().GetSetVKManager();
			ip.gameObject.GetComponent<VirtualKeyboardLinker>().GetSetVKManager();
			port.gameObject.GetComponent<VirtualKeyboardLinker>().GetSetVKManager();
			password.gameObject.GetComponent<VirtualKeyboardLinker>().GetSetVKManager();
		}

		/// <summary>
		/// Replaces the empty fields (<see cref="playerName"/>, <see cref="ip"/>,
		/// <see cref="port"/>, <see cref="password"/>) with default values.
		/// The default values are:
		/// <list type="bullet">
		/// <item><description><see cref="playerName"/>: "NewPlayer"</description></item>
		/// <item><description><see cref="ip"/>: ""</description></item>
		/// <item><description><see cref="port"/>: "0"</description></item>
		/// <item><description><see cref="password"/>: "1234"</description></item>
		/// </list>
		/// </summary>
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

		/// <summary>
		/// Starts a multiplayer session as a host.
		/// </summary>
		public void Host()
		{
			MultiplayerModule.Instance.OnConnectionStart();

			CheckFieldsContent();

			GameNetPortal.Instance.SetConnectionSettings(playerName.text,
														ip.text, System.Convert.ToUInt16(port.text),
														password.text);
			GameNetPortal.Instance.StartHost();
		}

		/// <summary>
		/// Joins a multiplayer session as a client.
		/// </summary>
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