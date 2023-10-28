using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using ECellDive.Interfaces;
using ECellDive.Utility.Data.Multiplayer;
using ECellDive.PlayerComponents;
using UnityEngine.Events;
using ECellDive.SceneManagement;

namespace ECellDive.Multiplayer
{
	/// <summary>
	/// A class to manage shared data between all players in a multiplayer session.
	/// </summary>
	public class GameNetDataManager : NetworkBehaviour
	{
		/// <summary>
		/// A map between the network client id of a player and its unique player ID.
		/// Use this as an indirection to get the network data of a player in 
		/// <see cref="playerGUIDToPlayerNetData"/>.
		/// </summary>
		private Dictionary<ulong, System.Guid> clientIDToPlayerGUIDMap = new Dictionary<ulong, System.Guid>();

		/// <summary>
		/// The singleton instance of this class.
		/// </summary>
		public static GameNetDataManager Instance;

		/// <summary>
		/// The list of the data modules in the scene.
		/// </summary>
		public List<IMlprData> dataModules = new List<IMlprData>();

		/// <summary>
		/// The list of modules that can be modified by the user.
		/// </summary>
		/// <remarks>
		/// Even if it's convenient that it's here, it's probably not the best place
		/// because it's not really related to data of the multiplayer session.
		/// </remarks>
		public List<IModifiable> modifiables = new List<IModifiable>();

		/// <summary>
		/// A map between a unique player ID and the data of the player.
		/// </summary>
		/// <remarks>
		/// Client IDs are assigned whenever a client connects to the server. So, 
		/// if a player leaves and reconnects, it will have a different client ID.
		/// That is why we use the unique player ID to retrieve the network data of
		/// the player. But the unique player ID is only known by the player itself.
		/// So you can use <see cref="clientIDToPlayerGUIDMap"/>(updated when
		/// a client connects to the server) to get the unique player ID of a client.
		/// </remarks>
		private Dictionary<System.Guid, PlayerNetData> playerGUIDToPlayerNetData = new Dictionary<System.Guid, PlayerNetData>();

		/// <summary>
		/// The list of modules that can be saved.
		/// </summary>
		/// <remarks>
		/// Even if it's convenient that it's here, it's probably not the best place
		/// because it's not really related to data of the multiplayer session.
		/// </remarks>
		public List<ISaveable> saveables = new List<ISaveable>();

		/// <summary>
		/// Triggered <b>CLIENT SIDE</b>.
		/// 
		/// Event triggered when the data modules already loaded in a multiplayer
		/// session has been shared with a new player.
		/// Triggered at the end of <see cref="ShareModuleDataC(ulong)"/>.
		/// </summary>
		public UnityAction<ulong> OnClientReceivedAllModules;

        /// <summary>
        /// Triggered <b>CLIENT SIDE</b>.
        /// 
        /// Event triggered when the player net data of all players has been shared
        /// on the network. This includes "Already connected players" to "New player",
		/// and "New player" to "Already connected players".
        /// It is triggered at the end of <see cref="SharePlayerNetDataClientRpc(string, ECellDive.Utility.Data.Multiplayer.PlayerNetData)"/>
        /// and <see cref="SharePlayerNetDataClientRpc(string, ECellDive.Utility.Data.Multiplayer.PlayerNetData, ClientRpcParams)"/>.
        /// It is triggered when the number of player net data loaded on the client
		/// side is equal to <see cref="playerNetDataCount"/>.
        /// </summary>
        public UnityAction<ulong> OnClientReceivedAllPlayerNetData;
		
		/// <summary>
		/// Synchronoizes with all players the number of player net data that
		/// the server is managing. This is used to know when the server has
		/// finished to send all player net data to a new player.
		/// </summary>
		private NetworkVariable<int> playerNetDataCount = new NetworkVariable<int>(0);

		private void Awake()
		{
			Instance = this;
		}

		public override void OnNetworkSpawn()
		{
			//In the particular case of the host, we directly set the data because 
			//it is the first entity to be spawned in the scene so there is no
			//data to synchronize.
			//Note that this might change in the future if we decide to spawn
			//the host in a dive scene with loaded data.
			if (IsClient & !IsHost)
			{
				Clear();

				SharePlayerNetDataServerRpc(NetworkManager.Singleton.LocalClientId, PlayerPrefsWrap.GetPlayerName(), PlayerPrefsWrap.GetGUID());

				ShareModuleDataServerRpc(NetworkManager.Singleton.LocalClientId);
			}

			else if (IsServer)
			{
				System.Guid guid = System.Guid.Parse(PlayerPrefsWrap.GetGUID());
				clientIDToPlayerGUIDMap[NetworkManager.Singleton.LocalClientId] = guid;
				playerGUIDToPlayerNetData[guid] = new PlayerNetData
				{
					clientId = NetworkManager.Singleton.LocalClientId,
					playerName = PlayerPrefsWrap.GetPlayerName(),
					scenes = new ListInt32Network(1) { 0 }
				}; 
				playerNetDataCount.Value++;

				//We make sure the root dive scene is added to the bank before we 
				//trigger the events.
				DiveScenesManager.Instance.AddNewDiveScene(-1, "Root");

                //We trigger the events here because there is no need to check.
                OnClientReceivedAllPlayerNetData?.Invoke(NetworkManager.Singleton.LocalClientId);
				OnClientReceivedAllModules?.Invoke(NetworkManager.Singleton.LocalClientId);
			}
		}

		/// <summary>
		/// Adds a new scene ID to the list of scenes for the player with
		/// the given client ID.
		/// </summary>
		/// <param name="_clientID">
		/// The client ID of the player.
		/// </param>
		/// <param name="_newSceneID">
		/// The new scene ID to add to the list of scenes. Presumably the
		/// scene where the player is currently or has just dived in.
		/// </param>
		public void AddToTrace(ulong _clientID, int _newSceneID)
		{
			playerGUIDToPlayerNetData[clientIDToPlayerGUIDMap[_clientID]].scenes.Add(_newSceneID);
		}

		/// <summary>
		/// Clears all the data of this class. This should be called when the
		/// player switches to another multiplayer session.
		/// </summary>
		public void Clear()
		{
			dataModules.Clear();
			modifiables.Clear();
			clientIDToPlayerGUIDMap.Clear();
			playerGUIDToPlayerNetData.Clear();
			saveables.Clear();
		}

		/// <summary>
		/// Gets the name of the player with the given client ID.
		/// </summary>
		/// <param name="_clientID">
		/// The client ID of the player.
		/// </param>
		/// <returns>
		/// The name of the player.
		/// </returns>
		public string GetClientName(ulong _clientID)
		{
			return playerGUIDToPlayerNetData[clientIDToPlayerGUIDMap[_clientID]].playerName;
		}

		/// <summary>
		/// Gets the scene ID of the current dive scene the player with the given
		/// client ID is in.
		/// </summary>
		/// <param name="_clientID">
		/// The client ID of the player.
		/// </param>
		/// <returns>
		/// The scene ID of the current dive scene the player is in.
		/// </returns>
		public int GetCurrentScene(ulong _clientID)
		{
			return playerGUIDToPlayerNetData[clientIDToPlayerGUIDMap[_clientID]].scenes.GetBack();
		}

		/// <summary>
		/// Gets all scenes the player with the given client ID has been in.
		/// </summary>
		/// <param name="_clientID">
		/// The client ID of the player.
		/// </param>
		/// <returns>
		/// The list of all scenes the player has been in.
		/// </returns>
		public List<int> GetSceneTrace(ulong _clientID)
		{
			return playerGUIDToPlayerNetData[clientIDToPlayerGUIDMap[_clientID]].scenes.Values;
		}

        /// <summary>
        /// The server notifies the client that there is no more data module
        /// to share. This is triggered at the end of <see cref="ShareModuleDataC(ulong)"/>.
        /// </summary>
        /// <param name="_clientRpcParams">
        /// The client RPC parameters allowing to reach specific clients.
        /// </param>
        [ClientRpc]
        private void OnClientReceivedAllModulesClientRpc(ClientRpcParams _clientRpcParams)
        {
            OnClientReceivedAllModules.Invoke(NetworkManager.LocalClientId);
        }

        /// <summary>
        /// Notifies the server that the player with the given client ID would
        /// like to receive the data modules already loaded in the scene.
        /// </summary>
        /// <param name="_expeditorClientID">
        /// The client ID of the player that would like to receive the data modules.
        /// </param>
        /// <seealso cref="ShareModuleDataC"/>
        [ServerRpc(RequireOwnership = false)]
		private void ShareModuleDataServerRpc(ulong _expeditorClientID)
		{
			StartCoroutine(ShareModuleDataC(_expeditorClientID));
		}

		/// <summary>
		/// <b>SERVER SIDE</b>
		/// 
		/// Synchronizes the content of the data modules that already exist
		/// in the scene (and stored in <see cref="dataModules"/>) for the
		/// client with id <paramref name="_expeditorClientID"/>.
		/// </summary>
		/// <param name="_expeditorClientID">The id of the target client to which
		/// we send the content of the data modules in the scene.</param>
		/// <remarks>This should be used only with (or after) <see
		/// cref="OnNetworkReady(ulong)"/> to be sure that the data modules
		/// have been spawned in the scene of the target client.</remarks>
		private IEnumerator ShareModuleDataC(ulong _expeditorClientID)
		{
			int nbClientReadyLoaded;
			foreach (IMlprData mlprData in dataModules)
			{
				Debug.Log($"Sending data {mlprData.sourceDataName}");
				nbClientReadyLoaded = mlprData.nbClientReadyLoaded.Value;
				StartCoroutine(mlprData.SendSourceDataC(_expeditorClientID));

				//We wait for the current data to be completely loaded
				//in the scene of the new client before starting to load
				//the next one (next step of the foreach loop).
				//We know when the data has been loaded once the network
				//variable storing the number of client that has loaded
				//the data has been incremented by 1.
				yield return new WaitUntil(() => mlprData.nbClientReadyLoaded.Value == nbClientReadyLoaded + 1);
			}

			ClientRpcParams clientRpcParams = new ClientRpcParams
			{
				Send = new ClientRpcSendParams
				{
					TargetClientIds = new ulong[] { _expeditorClientID }
				}
			};

			//We send the event to the target client to notify it that
			//it has received all the data modules.
			OnClientReceivedAllModulesClientRpc(clientRpcParams);
		}

		/// <summary>
		/// Broadcasts a player's net data to all clients.
		/// </summary>
		/// <param name="_playerGUID">
		/// The unique player ID of the player which data is broadcasted.
		/// </param>
		/// <param name="_playerNetData">
		/// The data of the player which is broadcasted.
		/// </param>
		[ClientRpc]
		public void SharePlayerNetDataClientRpc(string _playerGUID, PlayerNetData _playerNetData)
		{
			Debug.Log($"SharePlayerNetDataClientRpc in client " +
				$"{NetworkManager.Singleton.LocalClientId} about " +
				$"[_playerGUID: {_playerGUID}, " + _playerNetData + "]");

			System.Guid guid = System.Guid.Parse(_playerGUID);
			clientIDToPlayerGUIDMap[_playerNetData.clientId] = guid;
			playerGUIDToPlayerNetData[guid] = _playerNetData;

			Debug.Log($"playerNetDataCount.Value: {playerNetDataCount.Value}, " +
				$"playerGUIDToPlayerNetData.Count: " +
				$"{playerGUIDToPlayerNetData.Count}");
			if (playerNetDataCount.Value == playerGUIDToPlayerNetData.Count)
			{
				OnClientReceivedAllPlayerNetData?.Invoke(NetworkManager.Singleton.LocalClientId);
			}
		}

		/// <summary>
		/// Broadcasts a player's net data to a subset of clients defined by
		/// <paramref name="_clientRpcParams"/>.
		/// </summary>
		/// <param name="_playerGUID">
		/// The unique player ID of the player which data is broadcasted.
		/// </param>
		/// <param name="_playerNetData">
		/// The data of the player which is broadcasted.
		/// </param>
		/// <param name="_clientRpcParams">
		/// The client RPC parameters allowing to reach specific clients.
		/// </param>
		[ClientRpc]
		public void SharePlayerNetDataClientRpc(string _playerGUID, PlayerNetData _playerNetData, ClientRpcParams _clientRpcParams)
		{
			Debug.Log($"SharePlayerNetDataClientRpc in client " +
				$"{NetworkManager.Singleton.LocalClientId} about " +
				$"[_playerGUID: {_playerGUID}, " + _playerNetData + "]");

			System.Guid guid = System.Guid.Parse(_playerGUID);
			clientIDToPlayerGUIDMap[_playerNetData.clientId] = guid;
			playerGUIDToPlayerNetData[guid] = _playerNetData;

			Debug.Log($"playerNetDataCount.Value: {playerNetDataCount.Value}, " +
				$"playerGUIDToPlayerNetData.Count: " +
				$"{playerGUIDToPlayerNetData.Count}");
			if (playerNetDataCount.Value == playerGUIDToPlayerNetData.Count)
			{
				OnClientReceivedAllPlayerNetData?.Invoke(NetworkManager.Singleton.LocalClientId);
			}
		}

		/// <summary>
		/// Notifies the server that the player with the given client ID would
		/// like to receive the player net data of all players already connected.
		/// </summary>
		/// <param name="_connectingClientID">
		/// The client ID of the connecting player.
		/// </param>
		/// <param name="_playerName">
		/// The name of the connecting player.
		/// </param>
		/// <param name="_playerGUID">
		/// The unique player ID of the connecting player.
		/// </param>
		[ServerRpc(RequireOwnership = false)]
		public void SharePlayerNetDataServerRpc(ulong _connectingClientID, string _playerName, string _playerGUID)
		{
			//If the player is indeed new (not a reconnecting player), we increment
			//the number of player net data to synchronize.
			//We must increment this number before the server starts to send the
			//player net data to the connecting client. Otherwise, the client will
			//trigger the OnClientReceivedAllPlayerNetData event before the server
			//has finished to send the connecting player's data to itself.
			if (!playerGUIDToPlayerNetData.ContainsKey(System.Guid.Parse(_playerGUID)))
			{
				playerNetDataCount.Value++;
			}

			ClientRpcParams clientRpcParams = new ClientRpcParams
			{
				Send = new ClientRpcSendParams
				{
					TargetClientIds = new ulong[] { _connectingClientID }
				}
			};

			//We send the player net data of already connected players to the new player.
			//The "already connected players" may include the new player itself if it is
			//a reconnecting player.
			foreach (KeyValuePair<System.Guid, PlayerNetData> pnd in playerGUIDToPlayerNetData)
			{
				Debug.Log($"Server is broadcasting already connected player " +
										$"{pnd.Value.playerName} with GUID {pnd.Key} to new player");
				SharePlayerNetDataClientRpc(pnd.Key.ToString(), pnd.Value, clientRpcParams);
			}

			//If the player is not a reconnecting player, we send its player net data to
			//all players including itself.
			if (!playerGUIDToPlayerNetData.ContainsKey(System.Guid.Parse(_playerGUID)))
			{
				Debug.Log("Server is Broadcasting new player data to all clients");
				SharePlayerNetDataClientRpc(_playerGUID, new PlayerNetData
				{
					clientId = _connectingClientID,
					playerName = _playerName,
					scenes = new ListInt32Network(1) { 0 }
				});

				//In the case the server is not in a Host/Client architecture,
				//we must update the data maps of the server here because it
				//won't be done via the clientRpc above sent above.
				if (!IsHost)
				{
					System.Guid guid = System.Guid.Parse(_playerGUID);
					clientIDToPlayerGUIDMap[_connectingClientID] = guid;
					playerGUIDToPlayerNetData[guid] = new PlayerNetData
					{
						clientId = _connectingClientID,
						playerName = _playerName,
						scenes = new ListInt32Network(1) { 0 }
					};
				}
			}
		}
	}
}
