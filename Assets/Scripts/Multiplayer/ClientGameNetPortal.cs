using System.Collections;
using Unity.Netcode;
using UnityEngine;
using ECellDive.Utility;
using ECellDive.Modules;

namespace ECellDive.Multiplayer
{
	/// <summary>
	/// The Logic handling the connection of a client to a host using Unity Netcode for
	/// gameobjects with the default Unity Transport.
	/// </summary>
	/// <remarks>
	/// This code is copied and adapted from the official Untiy multiplayer sample project
	/// "Boss Room":
	/// - https://docs-multiplayer.unity3d.com/netcode/current/learn/bossroom
	/// - https://github.com/Unity-Technologies/com.unity.multiplayer.samples.coop/releases v1.2.0-pre
	/// </remarks>
	[RequireComponent(typeof(GameNetPortal))]
	public class ClientGameNetPortal : MonoBehaviour
	{
		/// <summary>
		/// The singleton instance of this class.
		/// </summary>
		public static ClientGameNetPortal Instance;

		/// <summary>
		/// Reference to the GameNetPortal component on this scene.
		/// </summary>
		private GameNetPortal m_Portal;

		/// <summary>
		/// Time in seconds before the client considers a lack of server response a timeout
		/// </summary>
		private const int k_TimeoutDuration = 10;

		private void Awake()
		{
			Instance = this;
			m_Portal = GetComponent<GameNetPortal>();
		}

		/// <summary>
		/// What to do immediately after the client connection protocol has finished.
		/// </summary>
		/// <param name="status">
		/// The status of the connection attempt.
		/// </param>
		public void OnConnectFinished(ConnectStatus status)
		{
			//on success, there is nothing to do (the Netcode for GameObjects (Netcode) scene management system will take us to the next scene).
			//on failure, we must raise an event so that the UI layer can display something.
			Debug.Log("RecvConnectFinished Got status: " + status);

			//if (status != ConnectStatus.Success)
			//{
			//    //this indicates a game level failure, rather than a network failure. See note in ServerGameNetPortal.
			//    DisconnectReason.SetDisconnectReason(status);
			//}
			//else
			//{
			//    m_ConnectStatusPub.Publish(status);
			//    if (m_LobbyServiceFacade.CurrentUnityLobby != null)
			//    {
			//        m_LobbyServiceFacade.BeginTracking();
			//    }
			//}
		}

		/// <summary>
		/// Debug utility to print the reason of the disconnection.
		/// </summary>
		/// <param name="status">
		/// The status of the disconnection.
		/// </param>
		private void OnDisconnectReasonReceived(ConnectStatus status)
		{
			Debug.Log("Got disconnected with status: " + status);
			//DisconnectReason.SetDisconnectReason(status);
		}

		/// <summary>
		/// Called by <see cref="ECellDive.Multiplayer.GameNetPortal"/> when the network is ready.
		/// In this case we check the satus of the NetManager. Since this component is only
		/// enabled for clients, if the NetManager is not a client, we disable the component.
		/// </summary>
		public void OnNetworkReady()
		{
			Debug.Log("OnNetworkReady for ClientGameNetPortal component");
			if (!m_Portal.NetManager.IsClient)
			{
				Debug.Log("This NetManager is NOT a client: the component is disabled.");
				enabled = false;
			}
		}

		/// <summary>
		/// A custom message handler for the client to receive the result of the connection
		/// attempt.
		/// </summary>
		/// <param name="clientID">
		/// The ID of the client that should receive the message.
		/// </param>
		/// <param name="reader">
		/// The optimized data struct to encapsulate the data to be sent.
		/// </param>
		public static void ReceiveServerToClientConnectResult_CustomMessage(ulong clientID, FastBufferReader reader)
		{
			reader.ReadValueSafe(out ConnectStatus status);
			Instance.OnConnectFinished(status);
		}

		/// <summary>
		/// A custom message handler for the client to receive the reason of the
		/// disconnection.
		/// </summary>
		/// <param name="clientID">
		/// The ID of the client that should receive the message.
		/// </param>
		/// <param name="reader">
		/// The optimized data struct to encapsulate the data to be sent.
		/// </param>
		public static void ReceiveServerToClientSetDisconnectReason_CustomMessage(ulong clientID, FastBufferReader reader)
		{
			reader.ReadValueSafe(out ConnectStatus status);
			Instance.OnDisconnectReasonReceived(status);
		}

		/// <summary>
		/// Shuts down the current session and starts a new one as a client.
		/// </summary>
		private IEnumerator Restart()
		{
			m_Portal.NetManager.Shutdown();

			//We make sure the previous instance of the host is closed
			yield return new WaitWhile(() => m_Portal.NetManager.IsListening);

			//We try hosting at the new address already stored in Unity Transport Connection Data.
			m_Portal.NetManager.StartClient();

			float startTime = Time.time;
			yield return new WaitUntil(() => m_Portal.NetManager.IsConnectedClient || Time.time-startTime > 1);

			string msgStr;
			if (!m_Portal.NetManager.IsConnectedClient)
			{
				msgStr = "<color=red>Client couldn't connect to " + m_Portal.settings.IP + ":" + m_Portal.settings.port +
					   ". Falling back to single player on 127.0.0.1:7777</color>";

				LogSystem.AddMessage(LogMessageTypes.Errors,
					"Client couldn't connect to " + m_Portal.settings.IP + ":" + m_Portal.settings.port +
					". Falling back to single player on 127.0.0.1:7777");

				MultiplayerModule.Instance.OnConnectionFails();
				yield return new WaitForSeconds(1f);

				m_Portal.SetConnectionSettings(m_Portal.settings.playerName, "127.0.0.1", 7777, m_Portal.settings.password);
				m_Portal.SetUnityTransport();

				//There is no shutdown if client doesn't connect so we are shutting down
				//ourselves.
				m_Portal.NetManager.Shutdown();

				//We are in the case where joining to the new address failed.
				//We wait until the failed client connection has properly shut down.
				yield return new WaitWhile(() => m_Portal.NetManager.IsListening);

				m_Portal.NetManager.StartHost();
			}
			else
			{
				msgStr = "<color=green>Successfully joined at " + m_Portal.settings.IP + ":" + m_Portal.settings.port + "</color>";

				LogSystem.AddMessage(LogMessageTypes.Trace,
					"Successfully joinet at " + m_Portal.settings.IP + ":" + m_Portal.settings.port);
				MultiplayerModule.Instance.OnConnectionSuccess();
				yield return new WaitForSeconds(1f);
			}
		}

		/// <summary>
		/// Starts the client with the IP and port registered in <see cref="GameNetPortal.settings"/>
		/// </summary>
		public void StartClient()
		{
			string payload = JsonUtility.ToJson(m_Portal.GetConnectionPayload());
			byte[] payloadBytes = System.Text.Encoding.UTF8.GetBytes(payload);

			m_Portal.NetManager.NetworkConfig.ConnectionData = payloadBytes;
			m_Portal.NetManager.NetworkConfig.ClientConnectionBufferTimeout = k_TimeoutDuration;

			m_Portal.SetUnityTransport();

			//and...we're off! Netcode will establish a socket connection to the host.
			//  If the socket connection fails, we'll hear back by getting an ReceiveServerToClientSetDisconnectReason_CustomMessage callback for ourselves and get a message telling us the reason
			//  If the socket connection succeeds, we'll get our ReceiveServerToClientConnectResult_CustomMessage invoked. This is where game-layer failures will be reported.
			if (m_Portal.NetManager.IsHost || m_Portal.NetManager.IsClient)
			{
				Debug.Log("NetManager was already running so we are shutting down before re-launching");
				StartCoroutine(Restart());
			}
			else
			{
				if (m_Portal.NetManager.StartClient())
				{
					MultiplayerModule.Instance.OnConnectionSuccess();
				}
			}
			//SceneLoaderWrapper.Instance.AddOnSceneEventCallback();

			// should only do this once StartClient has been called (start client will initialize CustomMessagingManager
			//NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler(nameof(ReceiveServerToClientConnectResult_CustomMessage), ReceiveServerToClientConnectResult_CustomMessage);
			//NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler(nameof(ReceiveServerToClientSetDisconnectReason_CustomMessage), ReceiveServerToClientSetDisconnectReason_CustomMessage);
		}
	}
}