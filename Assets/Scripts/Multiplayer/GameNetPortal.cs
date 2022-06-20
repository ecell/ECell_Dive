using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using ECellDive.UI;
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
    public struct ConnectionPayload
    {
        public string playerId;
        public string psw;
        //public int clientScene = -1;
        public string playerName;
        //public bool isDebug;
    }

    [Serializable]
    public struct ConnectionSettings
    {
        public string playerName;
        public string IP;
        public int port;
        public string password;
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
        public NetworkManager NetManager;

        public static GameNetPortal Instance;
        private ClientGameNetPortal m_ClientPortal;
        private ServerGameNetPortal m_ServerPortal;
        
        [SerializeField] private ConnectionSettings m_Settings;

        public Dictionary<ulong, NetSessionPlayerData> netSessionPlayersDataMap = new Dictionary<ulong, NetSessionPlayerData>();


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

        private string GetPlayerId()
        {
            //if (UnityServices.State != ServicesInitializationState.Initialized)
            //{
            //    return ClientPrefs.GetGuid() + ProfileManager.Profile;
            //}

            //return AuthenticationService.Instance.IsSignedIn ? AuthenticationService.Instance.PlayerId : ClientPrefs.GetGuid() + ProfileManager.Profile;
            return PlayerPrefsWrap.GetGuid();
        }

        public ConnectionPayload GetConnectionPayload()
        {
            return new ConnectionPayload
            {
                playerId = GetPlayerId(),
                playerName = m_Settings.playerName,
                psw = m_Settings.password
            };
        }

        public bool CheckPassword(string _password)
        {
            return m_Settings.password == _password;
        }

        /// <summary>
        /// This method runs when NetworkManager has started up (following a
        /// succesful connect on the client, or directly after StartHost is invoked
        /// on the host). It is named to match NetworkBehaviour.OnNetworkSpawn,
        /// and serves the same role, even though GameNetPortal itself isn't a NetworkBehaviour.
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
        /// Shuts down the current session and starts a new one as a host.
        /// </summary>
        IEnumerator Restart()
        {
            Debug.Log($"Shutting down");
            NetManager.Shutdown();
            yield return new WaitForEndOfFrame();
            Debug.Log($"Restarting a host");
            bool isHostStarted = NetManager.StartHost();

            if (!isHostStarted)
            {
                string msg = "Host couldn't be started: bind and listening to " + m_Settings.IP + ":" + m_Settings.port + " failed.\n" +
                    "Falling back to 127.0.0.1:7777";

                SetConnectionSettings(m_Settings.playerName, "127.0.0.1", 7777, m_Settings.password);
                StartHost();
                yield return new WaitForSeconds(1f);
                MultiplayerMenuManager.SetMessage(msg);
            }
        }

        public void SetConnectionSettings(string _name, string _ip, int _port, string _password)
        {
            m_Settings = new ConnectionSettings
            {
                playerName = name,
                IP = _ip,
                port = _port,
                password = _password
            };
        }

        public void SetUnityTransport()
        {
            UnityTransport unityTransport = NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();
            unityTransport.SetConnectionData(m_Settings.IP, (ushort)m_Settings.port);

            Debug.Log($"Setting up transport  connection to {unityTransport.ConnectionData.Address}:" +
                $"{unityTransport.ConnectionData.Port}");
        }

        /// <summary>
        /// Public interface to start a client. Forwards a call to <see cref=
        /// "ClientGameNetPortal.StartClient"/>.
        /// </summary>
        public void StartClient()
        {
            m_ClientPortal.StartClient();

        }

        /// <summary>
        /// Initializes host mode on this client. Call this and then other clients 
        /// should connect to us!
        /// Uses the IP and port registered in <see cref="m_Settings"/>.
        /// </summary>
        public void StartHost()
        {
            Debug.Log("Net Manager call to Start host");

            SetUnityTransport();
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
    }
}