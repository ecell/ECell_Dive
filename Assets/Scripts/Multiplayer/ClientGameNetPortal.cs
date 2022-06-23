using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using ECellDive.UI;


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
        public static ClientGameNetPortal Instance;
        GameNetPortal m_Portal;

        /// <summary>
        /// Time in seconds before the client considers a lack of server response a timeout
        /// </summary>
        private const int k_TimeoutDuration = 10;

        private void Awake()
        {
            Instance = this;
            m_Portal = GetComponent<GameNetPortal>();
        }

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

        private void OnDisconnectReasonReceived(ConnectStatus status)
        {
            Debug.Log("Got disconnected with status: " + status);
            //DisconnectReason.SetDisconnectReason(status);
        }

        public void OnNetworkReady()
        {
            Debug.Log("OnNetworkReady for ClientGameNetPortal component");
            if (!m_Portal.NetManager.IsClient)
            {
                Debug.Log("This NetManager is NOT a client: the component is disabled.");
                enabled = false;
            }
        }

        public static void ReceiveServerToClientConnectResult_CustomMessage(ulong clientID, FastBufferReader reader)
        {
            reader.ReadValueSafe(out ConnectStatus status);
            Instance.OnConnectFinished(status);
        }

        public static void ReceiveServerToClientSetDisconnectReason_CustomMessage(ulong clientID, FastBufferReader reader)
        {
            reader.ReadValueSafe(out ConnectStatus status);
            Instance.OnDisconnectReasonReceived(status);
        }

        /// <summary>
        /// Shuts down the current session and starts a new one as a client.
        /// </summary>
        IEnumerator Restart()
        {
            Debug.Log($"Shutting down");
            m_Portal.NetManager.Shutdown();
            //yield return new WaitForEndOfFrame();
            yield return new WaitWhile(() => m_Portal.NetManager.IsListening);
            //SetUnityTransport(true);
            bool isClientStarted = m_Portal.NetManager.StartClient();
            yield return new WaitForEndOfFrame();
            string msg;
            if (!isClientStarted)
            {
                msg = "<color=red>Host couldn't be joined at" + m_Portal.settings.IP + ":" + m_Portal.settings.port +
                       "Falling back to single player on 127.0.0.1:7777</color>";
                m_Portal.SetConnectionSettings(m_Portal.settings.playerName, "127.0.0.1", 7777, m_Portal.settings.password);
                m_Portal.SetUnityTransport();
                yield return new WaitForSeconds(5);
                m_Portal.NetManager.StartHost();
            }
            else
            {
                msg = "<color=green>Successfully joined at " + m_Portal.settings.IP + ":" + m_Portal.settings.port + "</color>";
            }
            yield return new WaitForSeconds(1f);
            MultiplayerMenuManager.SetMessage(msg);
        }

        /// <summary>
        /// Starts the client with the IP and port registered in <see cref="GameNetPortal.settings"/>
        /// </summary>
        public void StartClient()
        {
            Debug.Log("Building Client payload and connecting.");
            string payload = JsonUtility.ToJson(m_Portal.GetConnectionPayload());

            Debug.Log("Client is connecting with payload:\n" + payload);

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
                m_Portal.NetManager.StartClient();
            }
            //SceneLoaderWrapper.Instance.AddOnSceneEventCallback();

            // should only do this once StartClient has been called (start client will initialize CustomMessagingManager
            //NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler(nameof(ReceiveServerToClientConnectResult_CustomMessage), ReceiveServerToClientConnectResult_CustomMessage);
            //NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler(nameof(ReceiveServerToClientSetDisconnectReason_CustomMessage), ReceiveServerToClientSetDisconnectReason_CustomMessage);
        }
    }
}