using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using ECellDive.SceneManagement;

namespace ECellDive.Multiplayer
{
    /// <summary>
    /// The logic handling the creation of a server and approval checks when a client is
    /// trying to connect. It uses Unity Netcode for gameobjects with the default Unity Transport.
    /// </summary>
    /// <remarks>
    /// This code is copied and adapted from the official Untiy multiplayer sample project
    /// "Boss Room":
    /// - https://docs-multiplayer.unity3d.com/netcode/current/learn/bossroom
    /// - https://github.com/Unity-Technologies/com.unity.multiplayer.samples.coop/releases v1.2.0-pre
    /// </remarks>
    [RequireComponent(typeof(GameNetPortal))]
    public class ServerGameNetPortal : MonoBehaviour
    {
        private GameNetPortal m_Portal;

        // used in ApprovalCheck. This is intended as a bit of light protection against DOS attacks that rely on sending silly big buffers of garbage.
        const int k_MaxConnectPayload = 1024;


        private void Awake()
        {
            m_Portal = GetComponent<GameNetPortal>();

            // we add ApprovalCheck callback BEFORE OnNetworkSpawn to avoid spurious Netcode for GameObjects (Netcode)
            // warning: "No ConnectionApproval callback defined. Connection approval will timeout"
            m_Portal.NetManager.ConnectionApprovalCallback += ApprovalCheck;
        }


        private void OnDestroy()
        {
            if (m_Portal != null)
            {
                if (m_Portal.NetManager != null)
                {
                    m_Portal.NetManager.ConnectionApprovalCallback -= ApprovalCheck;
                    //m_Portal.NetManager.OnServerStarted -= ServerStartedHandler;
                }
            }
        }

        /// <summary>
        /// This logic plugs into the "ConnectionApprovalCallback" exposed by Netcode.NetworkManager, and is run every time a client connects to us.
        /// See ClientGameNetPortal.StartClient for the complementary logic that runs when the client starts its connection.
        /// </summary>
        /// <remarks>
        /// Since our game doesn't have to interact with some third party authentication service to validate the identity of the new connection, our ApprovalCheck
        /// method is simple, and runs synchronously, invoking "callback" to signal approval at the end of the method. Netcode currently doesn't support the ability
        /// to send back more than a "true/false", which means we have to work a little harder to provide a useful error return to the client. To do that, we invoke a
        /// custom message in the same channel that Netcode uses for its connection callback. Since the delivery is NetworkDelivery.ReliableSequenced, we can be
        /// confident that our login result message will execute before any disconnect message.
        /// </remarks>
        /// <param name="connectionData">binary data passed into StartClient. In our case this is the client's GUID, which is a unique identifier for their install of the game that persists across app restarts. </param>
        /// <param name="clientId">This is the clientId that Netcode assigned us on login. It does not persist across multiple logins from the same client. </param>
        /// <param name="connectionApprovedCallback">The delegate we must invoke to signal that the connection was approved or not. </param>
        void ApprovalCheck(byte[] connectionData, ulong clientId, NetworkManager.ConnectionApprovedDelegate connectionApprovedCallback)
        {
            Debug.Log("Server is checking for approval.");
            if (connectionData.Length > k_MaxConnectPayload)
            {
                // If connectionData too high, deny immediately to avoid wasting time on the server. This is intended as
                // a bit of light protection against DOS attacks that rely on sending silly big buffers of garbage.
                connectionApprovedCallback(false, 0, false, null, null);
                return;
            }

            string payload = System.Text.Encoding.UTF8.GetString(connectionData);
            ConnectionPayload connectionPayload = JsonUtility.FromJson<ConnectionPayload>(payload); // https://docs.unity3d.com/2020.2/Documentation/Manual/JSONSerialization.html

            // Approval check happens for Host too, but obviously we want it to be approved
            if (clientId == NetworkManager.Singleton.LocalClientId)
            {
                Debug.Log("Client ID corresponds to local object: this is the host");

                //Storing about the Host relevant data (as a player)
                m_Portal.netSessionPlayersDataMap[clientId] = new NetSessionPlayerData(connectionPayload.playerName, clientId, 0);

                connectionApprovedCallback(true, null, true, null, null);
                return;
            }

            ConnectStatus gameReturnStatus = GetConnectStatus(connectionPayload);

            if (gameReturnStatus == ConnectStatus.Success)
            {

                Debug.Log($"New Connection was a success. The new player's name is " +
                    $"{connectionPayload.playerName}, with GUID {connectionPayload.playerId}");

                //Storing about the new client relevant data (who is a player)
                m_Portal.netSessionPlayersDataMap[clientId] = new NetSessionPlayerData(connectionPayload.playerName, clientId, 0);
                GameNetScenesManager.Instance.DiverGetsInServerRpc(0, clientId);
                
                SendServerToClientConnectResult(clientId, gameReturnStatus);

                //Populate our client scene map
                //m_ClientSceneMap[clientId] = connectionPayload.clientScene;

                // connection approval will create a player object for you
                connectionApprovedCallback(true, null, true, Vector3.zero, Quaternion.identity);

                // m_ConnectionEventPublisher.Publish(new ConnectionEventMessage() { ConnectStatus = ConnectStatus.Success, PlayerName = SessionManager<SessionPlayerData>.Instance.GetPlayerData(clientId)?.PlayerName });
            }
            else
            {
                //TODO-FIXME:Netcode Issue #796. We should be able to send a reason and disconnect without a coroutine delay.
                //TODO:Netcode: In the future we expect Netcode to allow us to return more information as part of the
                //approval callback, so that we can provide more context on a reject. In the meantime we must provide
                //the extra information ourselves, and then wait a short time before manually close down the connection.
                SendServerToClientConnectResult(clientId, gameReturnStatus);
                SendServerToClientSetDisconnectReason(clientId, gameReturnStatus);
                //StartCoroutine(WaitToDenyApproval(connectionApprovedCallback));
                //if (m_LobbyServiceFacade.CurrentUnityLobby != null)
                //{
                //    m_LobbyServiceFacade.RemovePlayerFromLobbyAsync(connectionPayload.playerId, m_LobbyServiceFacade.CurrentUnityLobby.Id);
                //}
            }
        }

        ConnectStatus GetConnectStatus(ConnectionPayload connectionPayload)
        {
            //if (m_Portal.NetManager.ConnectedClientsIds.Count >= CharSelectData.k_MaxLobbyPlayers)
            //{
            //    return ConnectStatus.ServerFull;
            //}

            //if (connectionPayload.isDebug != Debug.isDebugBuild)
            //{
            //    return ConnectStatus.IncompatibleBuildType;
            //}

            //return SessionManager<SessionPlayerData>.Instance.IsDuplicateConnection(connectionPayload.playerId) ?
            //    ConnectStatus.LoggedInAgain : ConnectStatus.Success;
            if (m_Portal.CheckPassword(connectionPayload.psw))
            {
                Debug.Log("This is a password match");
                return ConnectStatus.Success;
            }
            else
            {
                Debug.Log("Password does not match");
                return ConnectStatus.IncorrectPassword;
            }
        }

        public void OnNetworkReady()
        {
            Debug.Log("OnNetworkReady for ServerGameNetPortal componenent");
            if (!m_Portal.NetManager.IsServer)
            {
                Debug.Log("This NetManager is NOT the server: the component is disabled.");
                enabled = false;
            }
            //else
            //{
            //    //O__O if adding any event registrations here, please add an unregistration in OnClientDisconnect.
            //    m_Portal.NetManager.OnClientDisconnectCallback += OnClientDisconnect;

            //    if (m_Portal.NetManager.IsHost)
            //    {
            //        m_ClientSceneMap[m_Portal.NetManager.LocalClientId] = ServerScene;
            //    }
            //}
        }

        /// <summary>
        /// Sends a DisconnectReason to the indicated client. This should only be done on the server, prior to disconnecting the client.
        /// </summary>
        /// <param name="clientID"> id of the client to send to </param>
        /// <param name="status"> The reason for the upcoming disconnect.</param>
        static void SendServerToClientSetDisconnectReason(ulong clientID, ConnectStatus status)
        {
            Debug.Log("Notifying the disconnection reason to the client.");
            var writer = new FastBufferWriter(sizeof(ConnectStatus), Allocator.Temp);
            writer.WriteValueSafe(status);
            NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage(nameof(ClientGameNetPortal.ReceiveServerToClientSetDisconnectReason_CustomMessage), clientID, writer);
        }

        /// <summary>
        /// Responsible for the Server->Client custom message of the connection result.
        /// </summary>
        /// <param name="clientID"> id of the client to send to </param>
        /// <param name="status"> the status to pass to the client</param>
        static void SendServerToClientConnectResult(ulong clientID, ConnectStatus status)
        {
            Debug.Log("Notifying the conneciton results to the client");
            var writer = new FastBufferWriter(sizeof(ConnectStatus), Allocator.Temp);
            writer.WriteValueSafe(status);
            NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage(nameof(ClientGameNetPortal.ReceiveServerToClientConnectResult_CustomMessage), clientID, writer);
        }
    }
}