using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using ECellDive.UserActions;


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

    [Serializable]
    public class ConnectionPayload
    {
        public string playerId;
        public string psw;
        //public int clientScene = -1;
        public string playerName;
        //public bool isDebug;
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
        /// the name of the player chosen at game start
        /// </summary>
        public string PlayerName;

        public NetworkManager NetManager;

        public static GameNetPortal Instance;
        private ClientGameNetPortal m_ClientPortal;
        private ServerGameNetPortal m_ServerPortal;


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

        public string GetPlayerId()
        {
            //if (UnityServices.State != ServicesInitializationState.Initialized)
            //{
            //    return ClientPrefs.GetGuid() + ProfileManager.Profile;
            //}

            //return AuthenticationService.Instance.IsSignedIn ? AuthenticationService.Instance.PlayerId : ClientPrefs.GetGuid() + ProfileManager.Profile;
            return PlayerPrefsWrap.GetGuid();
        }

        /// <summary>
        /// This method runs when NetworkManager has started up (following a succesful connect on the client, or directly after StartHost is invoked
        /// on the host). It is named to match NetworkBehaviour.OnNetworkSpawn, and serves the same role, even though GameNetPortal itself isn't a NetworkBehaviour.
        /// </summary>
        private void OnNetworkReady(ulong clientId)
        {
            Debug.Log("OnNetworkReady called from Client Connection");
            if (clientId == NetManager.LocalClientId)
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
        /// Initializes host mode on this client. Call this and then other clients should connect to us!
        /// </summary>
        /// <remarks>
        /// See notes in ClientGameNetPortal.StartClient about why this must be static.
        /// </remarks>
        /// <param name="ipaddress">The IP address to connect to (currently IPV4 only).</param>
        /// <param name="port">The port to connect to. </param>
        public void StartHost(string ipaddress, int port)
        {
            Debug.Log("Setting up IP & port on the Host Side");
            UnityTransport unityTransport = NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();
            //NetworkManager.Singleton.NetworkConfig.NetworkTransport = chosenTransport;

            // Note: In most cases, this switch case shouldn't be necessary. It becomes necessary when having to deal with multiple transports like this
            // sample does, since current Transport API doesn't expose these fields.
            //switch (chosenTransport)
            //{
            //    case UNetTransport unetTransport:
            //        unetTransport.ConnectAddress = ipaddress;
            //        unetTransport.ServerListenPort = port;
            //        break;
            //    case UnityTransport unityTransport:
            //        unityTransport.SetConnectionData(ipaddress, (ushort)port);
            //        break;
            //    default:
            //        throw new Exception($"unhandled IpHost transport {chosenTransport.GetType()}");
            //}
            unityTransport.SetConnectionData(ipaddress, (ushort)port);
            StartHost();
        }

        /// <summary>
        /// Starts the host with the default IP and port registered from the inspection
        /// of the Unity Transport component in the editor.
        /// </summary>
        public void StartHost()
        {
            Debug.Log("Net Manager call to Start host");
            UnityTransport unityTransport = NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();
            Debug.Log($"Host is initialized on {unityTransport.ConnectionData.Address}:" +
                $"{unityTransport.ConnectionData.Port}");

            if (NetManager.IsHost)
            {
                Debug.Log("NetManager was already running so we are shutting down before re-launching");
                StartCoroutine(Restart());
            }
            else
            {
                NetManager.StartHost();
            }

        }

        IEnumerator Restart()
        {
            Debug.Log($"Shutting down");
            NetManager.Shutdown();
            yield return new WaitForEndOfFrame();
            Debug.Log($"Restarting a host");
            NetManager.StartHost();
        }
    }
}