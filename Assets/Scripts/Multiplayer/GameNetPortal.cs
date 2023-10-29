using System.Collections;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

using ECellDive.PlayerComponents;
using ECellDive.Modules;
using ECellDive.Utility;
using ECellDive.Utility.Data.Multiplayer;

namespace ECellDive.Multiplayer
{
	public enum ConnectStatus
	{
		Undefined,
		IncorrectPassword,
		Success,                  //client successfully connected. This may also be a successful reconnect.
		ServerFull,               //can't join, server is already at capacity.
		LoggedInAgain,            //logged in on a separate client, causing this one to be kicked out.
		UserRequestedDisconnect,  //Intentional Disconnect triggered by the user.
		GenericDisconnect,        //server disconnected, but no specific reason given.
		IncompatibleBuildType,    //client build type is incompatible with server.
		HostEndedSession,         //host intentionally ended the session.
	}

	/// <summary>
	/// The logic handling the creation of a host using Unity Netcode for
	/// gameobjects with the default Unity Transport. This intends to be a wrapper of the
	/// communications between server and client upon creation and termination.
	/// </summary>
	/// <remarks>
	/// This code is copied and adapted from the official Untiy multiplayer sample project
	/// "Boss Room":
	/// - https://docs-multiplayer.unity3d.com/netcode/current/learn/bossroom
	/// - https://github.com/Unity-Technologies/com.unity.multiplayer.samples.coop/releases v1.2.0-pre
	/// </remarks>
	public class GameNetPortal : MonoBehaviour
	{
		/// <summary>
		/// The reference to the network manager of the scene.
		/// </summary>
		public NetworkManager NetManager;

		/// <summary>
		/// The singleton instance of the GameNetPortal.
		/// </summary>
		public static GameNetPortal Instance;

		/// <summary>
		/// The reference to the client portal.
		/// </summary>
		private ClientGameNetPortal m_ClientPortal;

		/// <summary>
		/// The refence to the server portal.
		/// </summary>
		private ServerGameNetPortal m_ServerPortal;

		/// <summary>
		/// The field for the <see cref="settings"/> property.
		/// </summary>
		[SerializeField] ConnectionSettings m_settings;
		
		/// <summary>
		/// The settings of the connection to the server.
		/// </summary>
		public ConnectionSettings settings
		{
			get => m_settings;
			private set => m_settings = value;
		}

		private void Awake()
		{
			Debug.Assert(Instance == null);
			Instance = this;
			m_ClientPortal = GetComponent<ClientGameNetPortal>();
			m_ServerPortal = GetComponent<ServerGameNetPortal>();

			NetManager.OnClientConnectedCallback += OnNetworkReady;
		}

		private void Start()
		{
			//default connection payload
			string payload = JsonUtility.ToJson(GetConnectionPayload());
			byte[] payloadBytes = System.Text.Encoding.UTF8.GetBytes(payload);
			NetManager.NetworkConfig.ConnectionData = payloadBytes;

			StartHost();
		}

		private void OnDestroy()
		{
			if (NetManager != null)
			{
				NetManager.OnClientConnectedCallback -= OnNetworkReady;
			}

			Instance = null;
		}

		/// <summary>
		/// Generates a unique player id.
		/// </summary>
		/// <returns>
		/// The new player id.
		/// </returns>
		private string GetPlayerId()
		{
			//if (UnityServices.State != ServicesInitializationState.Initialized)
			//{
			//    return ClientPrefs.GetGUID() + ProfileManager.Profile;
			//}

			//return AuthenticationService.Instance.IsSignedIn ? AuthenticationService.Instance.PlayerId : ClientPrefs.GetGUID() + ProfileManager.Profile;
			return PlayerPrefsWrap.GetGUID();
		}

		private string GetPlayerName()
		{
#if UNITY_EDITOR
			return m_settings.playerName;
#else
			return PlayerPrefsWrap.GetPlayerName();
#endif
        }

        /// <summary>
        /// Encapsulate the connection data into a <see cref="ConnectionPayload"/>
        /// </summary>
        /// <returns>
        /// The connection payload.
        /// </returns>
        public ConnectionPayload GetConnectionPayload()
		{
			return new ConnectionPayload
			{
				playerId = GetPlayerId(),
				playerName = GetPlayerName(),
				psw = m_settings.password
			};
		}

		/// <summary>
		/// Checks if the password is correct.
		/// </summary>
		/// <param name="_password">
		/// The string to check agains the password.
		/// </param>
		/// <returns>
		/// True if the password is correct, false otherwise.
		/// </returns>
		public bool CheckPassword(string _password)
		{
			return m_settings.password == _password;
		}

		/// <summary>
		/// This method is subscribed to Unity's NetworkManager.OnClientConnectedCallback.
		/// So, it runs when NetworkManager has started up (following a succesful
		/// connect on the client, or directly after StartHost is invoked on the host).
		/// It is named to match NetworkBehaviour.OnNetworkSpawn, and serves the same
		/// role, even though GameNetPortal itself isn't a NetworkBehaviour.
		/// </summary>
		private void OnNetworkReady(ulong _clientID)
		{
			Debug.Log($"OnNetworkReady called from Client Connection with id {_clientID}");
			if (_clientID == NetManager.LocalClientId)
			{
				if (NetManager.IsHost)
				{
					//special host code. This is what kicks off the flow that happens on a regular client
					//when it has finished connecting successfully. A dedicated server would remove this.

					m_ClientPortal.OnConnectFinished(ConnectStatus.Success);
				}

				m_ClientPortal.OnNetworkReady();
				m_ServerPortal.OnNetworkReady();
			}
		}

		/// <summary>
		/// Shuts down the current session and starts a new one as a host.
		/// </summary>
		private IEnumerator Restart()
		{
			NetManager.Shutdown();
			
			//We make sure the previous instance of the host is closed
			yield return new WaitWhile(()=>NetManager.IsListening);

			//We try hosting at the new address alredy stored in Unity Transport Connection Data.
			bool isHostStarted = NetManager.StartHost();
			string msgStr;
			if (!isHostStarted)
			{
				msgStr = "<color=red>Host couldn't be started: bind and listening to " + m_settings.IP + ":" +
					 m_settings.port + " failed.\n" + "Falling back to 127.0.0.1:7777</color>";

				LogSystem.AddMessage(LogMessageTypes.Errors,
					"Host couldn't be started: bind and listening to " + m_settings.IP + ":" +
					 m_settings.port + " failed.\n" + "Falling back to 127.0.0.1:7777");

				SetConnectionSettings(m_settings.playerName, "127.0.0.1", 7777, m_settings.password);
				SetUnityTransport();

				MultiplayerModule.Instance.OnConnectionFails();

				//We are in the case where hosting to the new address failed.
				//We wait until the failed server has properly shut down.
				yield return new WaitWhile(() => NetManager.IsListening);

				NetManager.StartHost();
			}
			else
			{
				msgStr = "<color=green>Successfully hosting at " + m_settings.IP + ":" + m_settings.port+ "</color>";
				
				MultiplayerModule.Instance.OnConnectionSuccess();

				LogSystem.AddMessage(LogMessageTypes.Trace,
					"Successfully hosting at " + m_settings.IP + ":" + m_settings.port);

			}
			yield return new WaitForSeconds(1f);
		}

		/// <summary>
		/// Sets the connection settings.
		/// </summary>
		/// <param name="_name">
		/// The name of the player.
		/// </param>
		/// <param name="_ip">
		/// The IPv4 of the server.
		/// </param>
		/// <param name="_port">
		/// The port of the server.
		/// </param>
		/// <param name="_password">
		/// The password of the server.
		/// </param>
		public void SetConnectionSettings(string _name, string _ip, ushort _port, string _password)
		{
			PlayerPrefsWrap.SetPlayerName(_name);

			m_settings.SetPlayerName(_name);

			if (_ip != "")
			{
				m_settings.SetIP(_ip);
			}

			if (_port != 0)
			{
				m_settings.SetPort(_port);
			}

			m_settings.SetPassword(_password);
		}

		/// <summary>
		/// Retrieves the UnityTransport of the scene.
		/// </summary>
		/// <param name="_verbose">
		/// A boolean to print the retrieved connection data in the console.
		/// </param>
		public void SetUnityTransport(bool _verbose=false)
		{
			UnityTransport unityTransport = NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();
			unityTransport.SetConnectionData(m_settings.IP, m_settings.port); 

			if (_verbose)
			{
				Debug.Log($"Setting up transport connection to {unityTransport.ConnectionData.Address}:" +
				$"{unityTransport.ConnectionData.Port} and server listen address is {unityTransport.ConnectionData.ServerListenAddress}");

				LogSystem.AddMessage(LogMessageTypes.Debug, $"Setting up transport connection to {unityTransport.ConnectionData.Address}:" +
				$"{unityTransport.ConnectionData.Port} and server listen address is {unityTransport.ConnectionData.ServerListenAddress}");
			}
		}

		/// <summary>
		/// Public interface to start a client.
		/// </summary>
		/// <remarks>
		/// Forwards a call to <see cref= "ClientGameNetPortal.StartClient"/> 
		/// through <see cref="m_ClientPortal"/> which is private.
		/// </remarks>
		public void StartClient()
		{
			m_ClientPortal.StartClient();
		}

		/// <summary>
		/// Initializes host mode on this client. Call this and then other clients 
		/// should connect to us!
		/// Uses the IP and port registered in <see cref="m_settings"/>.
		/// </summary>
		public void StartHost()
		{
			GameNetDataManager.Instance.Clear();
			if (NetManager.IsHost)
			{
				ConnectionPayload payload = GetConnectionPayload();
				byte[] payloadBytes = System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(payload));
				NetManager.NetworkConfig.ConnectionData = payloadBytes;

				LogSystem.AddMessage(LogMessageTypes.Debug, 
					$"Network: a session was already running so we are shutting it down before re-launching");
				SetUnityTransport(true);
				StartCoroutine(Restart());
			}
			else
			{
				SetUnityTransport();
				if (NetManager.StartHost())
				{
					//The instance might be null if this is the first connection
					//when launching the app.
					MultiplayerModule.Instance?.OnConnectionSuccess();
				}
			}
		}
	}
}