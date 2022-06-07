using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

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
        /// Wraps the invocation of NetworkManager.StartClient, including our GUID as the payload.
        /// </summary>
        /// <remarks>
        /// This method must be static because, when it is invoked, the client still doesn't know it's a client yet, and in particular, GameNetPortal hasn't
        /// yet initialized its client and server GameNetPortal objects yet (which it does in OnNetworkSpawn, based on the role that the current player is performing).
        /// </remarks>
        /// <param name="ipaddress">the IP address of the host to connect to. (currently IPV4 only)</param>
        /// <param name="port">The port of the host to connect to. </param>
        public void StartClient(string ipaddress, int port)
        {
            Debug.Log("Setting up target IP & port on the Client Side");
            UnityTransport unityTransport = NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();
            NetworkManager.Singleton.NetworkConfig.NetworkTransport = unityTransport;

            //switch (chosenTransport)
            //{
            //    case UNetTransport unetTransport:
            //        unetTransport.ConnectAddress = ipaddress;
            //        unetTransport.ConnectPort = port;
            //        break;
            //    case UnityTransport unityTransport:
            //        // TODO: once this is exposed in the adapter we will be able to change it
            //        unityTransport.SetConnectionData(ipaddress, (ushort)port);
            //        break;
            //    default:
            //        throw new ArgumentOutOfRangeException(nameof(chosenTransport));
            //}
            unityTransport.SetConnectionData(ipaddress, (ushort)port);
            StartClient();
        }

        /// <summary>
        /// Starts the client with the default IP and port registered from the inspection
        /// of the Unity Transport component in the editor.
        /// </summary>
        public void StartClient()
        {

            Debug.Log("Building Client payload and connecting.");
            var payload = JsonUtility.ToJson(new ConnectionPayload()
            {
                psw = "1234",
                playerId = m_Portal.GetPlayerId(),
                //clientScene = SceneManager.GetActiveScene().buildIndex,
                playerName = m_Portal.PlayerName,
                //isDebug = Debug.isDebugBuild
            });

            var payloadBytes = System.Text.Encoding.UTF8.GetBytes(payload);

            m_Portal.NetManager.NetworkConfig.ConnectionData = payloadBytes;
            m_Portal.NetManager.NetworkConfig.ClientConnectionBufferTimeout = k_TimeoutDuration;

            //and...we're off! Netcode will establish a socket connection to the host.
            //  If the socket connection fails, we'll hear back by getting an ReceiveServerToClientSetDisconnectReason_CustomMessage callback for ourselves and get a message telling us the reason
            //  If the socket connection succeeds, we'll get our ReceiveServerToClientConnectResult_CustomMessage invoked. This is where game-layer failures will be reported.
            UnityTransport unityTransport = NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();
            Debug.Log($"Client will try to connect to {unityTransport.ConnectionData.Address}:" +
                $"{unityTransport.ConnectionData.Port}");
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

        IEnumerator Restart()
        {
            Debug.Log($"Shutting down");
            m_Portal.NetManager.Shutdown();
            yield return new WaitForEndOfFrame();
            Debug.Log("Restarting as a client");
            m_Portal.NetManager.StartClient();
        }
    }
}