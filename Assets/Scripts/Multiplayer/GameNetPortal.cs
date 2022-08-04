using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using ECellDive.Interfaces;
using ECellDive.Modules;
using ECellDive.UI;
using ECellDive.UserActions;
using ECellDive.Utility;

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
        public ushort port;
        public string password;

        public void SetPlayerName(string _playerName)
        {
            playerName = _playerName;
        }
        
        public void SetIP(string _IP)
        {
            IP = _IP;
        }

        public void SetPort(ushort _port)
        {
            port = _port;
        }

        public void SetPassword(string _password)
        {
            password = _password;
        }
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

        [SerializeField] ConnectionSettings m_settings;
        public ConnectionSettings settings
        {
            get => m_settings;
            private set => m_settings = value;
        }

        public List<IMlprData> dataModules = new List<IMlprData>();
        public List<IModifiable> modifiables = new List<IModifiable>();
        public List<ISaveable> saveables = new List<ISaveable>();
        public Dictionary<ulong, NetSessionPlayerData> netSessionPlayersDataMap = new Dictionary<ulong, NetSessionPlayerData>();

        private List<int> successFullPorts = new List<int>();

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
                playerName = m_settings.playerName,
                psw = m_settings.password
            };
        }

        public bool CheckPassword(string _password)
        {
            return m_settings.password == _password;
        }

        /// <summary>
        /// This method runs when NetworkManager has started up (following a
        /// succesful connect on the client, or directly after StartHost is invoked
        /// on the host). It is named to match NetworkBehaviour.OnNetworkSpawn,
        /// and serves the same role, even though GameNetPortal itself isn't a NetworkBehaviour.
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

            if (NetManager.IsServer)
            {
                StartCoroutine(SendData(_clientID));
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

                LogSystem.refLogManager.AddMessage(LogSystem.MessageTypes.Errors,
                    "Host couldn't be started: bind and listening to " + m_settings.IP + ":" +
                     m_settings.port + " failed.\n" + "Falling back to 127.0.0.1:7777");

                SetConnectionSettings(m_settings.playerName, "127.0.0.1", 7777, m_settings.password);
                SetUnityTransport();

                //We are in the case where hosting to the new address failed.
                //We wait until the failed server has properly shut down.
                yield return new WaitWhile(() => NetManager.IsListening);

                NetManager.StartHost();
            }
            else
            {
                msgStr = "<color=green>Successfully hosting at " + m_settings.IP + ":" + m_settings.port+ "</color>";
                
                LogSystem.refLogManager.AddMessage(LogSystem.MessageTypes.Trace,
                    "Successfully hosting at " + m_settings.IP + ":" + m_settings.port);

            }
            yield return new WaitForSeconds(1f);
            MultiplayerMenuManager.SetMessage(msgStr);
        }

        /// <summary>
        /// Synchronizes the content of the data modules that already exist
        /// in the scene (and stored in <see cref="dataModules"/>) for the
        /// client with id <paramref name="_clientID"/>.
        /// </summary>
        /// <param name="_clientID">The id of the target client to which
        /// we send the content of the data modules in the scene.</param>
        /// <remarks>This should be used only with (or after) <see
        /// cref="OnNetworkReady(ulong)"/> to be sure that the data modules
        /// have been spawned in the scene of the target client.</remarks>
        private IEnumerator SendData(ulong _clientID)
        {
            int nbClientReadyLoaded;
            foreach (IMlprData mlprData in dataModules)
            {
                Debug.Log($"Sending data {mlprData.sourceDataName}");
                nbClientReadyLoaded = mlprData.nbClientReadyLoaded.Value;
                StartCoroutine(mlprData.SendSourceDataC(_clientID));

                //We wait for the current data to be completely loaded
                //in the scene of the new client before starting to load
                //the next one (next step of the foreach loop.
                //We know when the data has been loaded once the network
                //variable storing the number of client that has loaded
                //the data has been incremented by 1.
                yield return new WaitUntil(() => mlprData.nbClientReadyLoaded.Value == nbClientReadyLoaded + 1);
            }
        }

        public void SetConnectionSettings(string _name, string _ip, ushort _port, string _password)
        {
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

        public void SetUnityTransport(bool _verbose=false)
        {
            UnityTransport unityTransport = NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();
            unityTransport.SetConnectionData(m_settings.IP, m_settings.port); 

            if (_verbose)
            {
                Debug.Log($"Setting up transport connection to {unityTransport.ConnectionData.Address}:" +
                $"{unityTransport.ConnectionData.Port} and server listen address is {unityTransport.ConnectionData.ServerListenAddress}");

                LogSystem.refLogManager.AddMessage(LogSystem.MessageTypes.Debug, $"Setting up transport connection to {unityTransport.ConnectionData.Address}:" +
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
            if (NetManager.IsHost)
            {
                string payload = JsonUtility.ToJson(GetConnectionPayload());
                byte[] payloadBytes = System.Text.Encoding.UTF8.GetBytes(payload);
                NetManager.NetworkConfig.ConnectionData = payloadBytes;

                LogSystem.refLogManager.AddMessage(LogSystem.MessageTypes.Debug, 
                    $"Network: a session was already running so we are shutting it down before re-launching");
                SetUnityTransport(true);
                StartCoroutine(Restart());
            }
            else
            {
                SetUnityTransport();
                NetManager.StartHost();
            }
        }
    }
}