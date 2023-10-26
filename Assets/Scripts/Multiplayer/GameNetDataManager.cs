using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using ECellDive.Interfaces;
using ECellDive.Utility.Data.Multiplayer;
using ECellDive.PlayerComponents;
using UnityEngine.Events;

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
		/// Event triggered when the data to be shared when a new client connects
		/// has been shared ammong all players.
		/// Currently triggered at the end of <see cref="ShareModuleDataC(ulong)"/>.
		/// </summary>
		public UnityAction<ulong> OnDataShared;

		private void Awake()
		{
			Instance = this;
		}

		public override void OnNetworkSpawn()
		{
			if (IsClient)
			{
				ShareNetworkDataServerRPC(NetworkManager.Singleton.LocalClientId,
					PlayerPrefsWrap.GetPlayerName(), PlayerPrefsWrap.GetGUID());

				StartCoroutine(ShareModuleDataC(NetworkManager.Singleton.LocalClientId));

				//TODO: once data sharing is finished:
				//- we need to spawn the new player in its dive scene.
				//- we need to authorize all players and replicated copies to assign the
				//names to their player prefabs name container.
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
		/// A coroutine to control the speed at which the server sends the data in
		/// <see cref="clientIDToPlayerGUIDMap"/> and <see cref="playerGUIDToPlayerNetData"/>
		/// using <see cref="SetPlayerGUIDToPlayerNetDataClientRpc(ulong, string, ClientRpcParams)"/>
		/// and <see cref="SetPlayerGUIDToPlayerNetDataClientRpc(string, PlayerNetData, ClientRpcParams)"/>.
		/// </summary>
		/// <param name="_expeditorID">
		/// The ID of the client that should receive the data.
		/// </param>
		/// <remarks>
		/// Executed Server-side in <see cref="ShareNetworkDataServerRPC(ulong, string, string)"/>.
		/// This one will overide the data set in the new player via ShareNetworkDataServerRPC.
		/// But it's not a problem because either it is the same data (real new player) or the
		/// server has data that is more up to date (reconnected player).
		/// </remarks>
		private IEnumerator SetPlayerNetDataC(ulong _expeditorID)
		{
			ClientRpcParams expeditorRpcParams = new ClientRpcParams
			{
				Send = new ClientRpcSendParams
				{
					TargetClientIds = new ulong[] { _expeditorID }
				}
			};

			//We send the content clientIDToPlayerGUIDMap to the new player
			foreach (KeyValuePair<ulong, System.Guid> pair in clientIDToPlayerGUIDMap)
			{
				SetClientIDToPlayerGUIDClientRpc(pair.Key, pair.Value.ToString(), expeditorRpcParams);
			}

			yield return new WaitForEndOfFrame();

			//We send the content of playerGUIDToPlayerNetData to the new player. 
			foreach (KeyValuePair<System.Guid, PlayerNetData> pair in playerGUIDToPlayerNetData)
			{
				SetPlayerGUIDToPlayerNetDataClientRpc(pair.Key.ToString(), pair.Value, expeditorRpcParams);
			}

		}

		/// <summary>
		/// Send data from the server to all clients to update their <see cref="clientIDToPlayerGUIDMap"/>.
		/// </summary>
		/// <param name="_clientID">
		/// The client ID of the client for which we are setting the data. It will be
		/// the key in <see cref="clientIDToPlayerGUIDMap"/> of the client targeted by
		/// <paramref name="clientRpcSendParams"/>.
		/// </param>
		/// <param name="_playerGUID">
		/// The player GUID of the client for which we are setting the data. It will be
		/// the value in <see cref="clientIDToPlayerGUIDMap"/>.
		/// </param>
		[ClientRpc]
		private void SetClientIDToPlayerGUIDClientRpc(ulong _clientID, string _playerGUID)
		{
			//Host side has already been updated in the server.
			if (!IsHost)
			{
				clientIDToPlayerGUIDMap[_clientID] = System.Guid.Parse(_playerGUID);
			}
		}

		/// <summary>
		/// The client targeted via <paramref name="clientRpcSendParams"/> receives data
		/// from the server to update its <see cref="clientIDToPlayerGUIDMap"/>.
		/// </summary>
		/// <param name="_clientID">
		/// The client ID of the client for which we are setting the data. It will be
		/// the key in <see cref="clientIDToPlayerGUIDMap"/> of the client targeted by
		/// <paramref name="clientRpcSendParams"/>.
		/// </param>
		/// <param name="_playerGUID">
		/// The player GUID of the client for which we are setting the data. It will be
		/// the value in <see cref="clientIDToPlayerGUIDMap"/> of the client targeted by
		/// <paramref name="clientRpcSendParams"/>.
		/// </param>
		/// <param name="clientRpcSendParams">
		/// The parameters to let the server know which client to target.
		/// </param>
		[ClientRpc]
		private void SetClientIDToPlayerGUIDClientRpc(ulong _clientID, string _playerGUID, ClientRpcParams clientRpcSendParams)
		{
			//Host side has already been updated in the server.
			if (!IsHost)
			{
				clientIDToPlayerGUIDMap[_clientID] = System.Guid.Parse(_playerGUID);
			}
		}

		/// <summary>
		/// Sends data from the server to all clients to update their <see cref="playerGUIDToPlayerNetData"/>.
		/// </summary>
		/// <param name="_playerGUID">
		/// The player GUID of the client for which we are setting the data. It will be
		/// the key in <see cref="playerGUIDToPlayerNetData"/>.
		/// </param>
		/// <param name="_playerNetData">
		/// The player net data of the client for which we are setting the data. It will be
		/// the value in <see cref="playerGUIDToPlayerNetData"/>.
		/// </param>
		[ClientRpc]
		private void SetPlayerGUIDToPlayerNetDataClientRpc(string _playerGUID, PlayerNetData _playerNetData)
		{
			//Host side has already been updated in the server.
			if (!IsHost)
			{
				Debug.Log($"SetPlayerGUIDToPlayerNetDataClientRpc in client {NetworkManager.Singleton.LocalClientId}");
				playerGUIDToPlayerNetData[System.Guid.Parse(_playerGUID)] = _playerNetData;
			}
		}

		/// <summary>
		/// The client targeted via <paramref name="clientRpcSendParams"/> receives data
		/// from the server to update its <see cref="playerGUIDToPlayerNetData"/>.
		/// </summary>
		/// <param name="_playerGUID">
		/// The player GUID of the client for which we are setting the data. It will be
		/// the key in <see cref="playerGUIDToPlayerNetData"/> of the client targeted by
		/// <paramref name="clientRpcSendParams"/>.
		/// </param>
		/// <param name="_playerNetData">
		/// The player net data of the client for which we are setting the data. It will be
		/// the value in <see cref="playerGUIDToPlayerNetData"/> of the client targeted by
		/// <paramref name="clientRpcSendParams"/>.
		/// </param>
		/// <param name="clientRpcSendParams">
		/// The parameters to let the server know which client to target.
		/// </param>
		[ClientRpc]
		private void SetPlayerGUIDToPlayerNetDataClientRpc(string _playerGUID, PlayerNetData _playerNetData, ClientRpcParams clientRpcSendParams)
		{
			//Host side has already been updated in the server.
			if (!IsHost)
			{
				playerGUIDToPlayerNetData[System.Guid.Parse(_playerGUID)] = _playerNetData;
			}
		}

		/// <summary>
		/// Synchronizes the content of the data modules that already exist
		/// in the scene (and stored in <see cref="dataModules"/>) for the
		/// client with id <paramref name="_targetClientID"/>.
		/// </summary>
		/// <param name="_targetClientID">The id of the target client to which
		/// we send the content of the data modules in the scene.</param>
		/// <remarks>This should be used only with (or after) <see
		/// cref="OnNetworkReady(ulong)"/> to be sure that the data modules
		/// have been spawned in the scene of the target client.</remarks>
		private IEnumerator ShareModuleDataC(ulong _targetClientID)
		{
			int nbClientReadyLoaded;
			foreach (IMlprData mlprData in dataModules)
			{
				Debug.Log($"Sending data {mlprData.sourceDataName}");
				nbClientReadyLoaded = mlprData.nbClientReadyLoaded.Value;
				StartCoroutine(mlprData.SendSourceDataC(_targetClientID));

				//We wait for the current data to be completely loaded
				//in the scene of the new client before starting to load
				//the next one (next step of the foreach loop).
				//We know when the data has been loaded once the network
				//variable storing the number of client that has loaded
				//the data has been incremented by 1.
				yield return new WaitUntil(() => mlprData.nbClientReadyLoaded.Value == nbClientReadyLoaded + 1);
			}

			OnDataShared.Invoke(_targetClientID);
		}

		/// <summary>
		/// Updates and/or initializes the data <see cref="clientIDToPlayerGUIDMap"/>" and
		/// <see cref="playerGUIDToPlayerNetData"/> for the server and then all the clients.
		/// Checks whether the player is new or reconnecting by comparing the player GUID
		/// with the one in <see cref="clientIDToPlayerGUIDMap"/>. If the player is new,
		/// then we add its info in <see cref="playerGUIDToPlayerNetData"/> for the server
		/// and all clients
		/// </summary>
		/// <param name="_expeditorID"></param>
		/// <param name="_playerName"></param>
		/// <param name="_playerGUID"></param>
		[ServerRpc(RequireOwnership = false)]
		private void ShareNetworkDataServerRPC(ulong _expeditorID, string _playerName, string _playerGUID)
		{
			System.Guid playerGUID = System.Guid.Parse(_playerGUID);
			clientIDToPlayerGUIDMap[_expeditorID] = playerGUID;//for the server

			SetClientIDToPlayerGUIDClientRpc(_expeditorID, _playerGUID);//for all clients

			//If the player is really new, we add its info in the
			//playerGUIDToPlayerNetData map of the server (since this is a server RPC).
			if (!playerGUIDToPlayerNetData.ContainsKey(playerGUID))
			{
				Debug.Log($"New player {_playerName} with GUID {_playerGUID} and clientID {_expeditorID} connected to the server.");
				PlayerNetData newPlayerNewData = new PlayerNetData
				{
					playerName = _playerName,
					clientId = _expeditorID,
					scenes = new ListInt32Network(0)
				};

				playerGUIDToPlayerNetData[playerGUID] = newPlayerNewData;

				SetPlayerGUIDToPlayerNetDataClientRpc(_playerGUID, newPlayerNewData);//for all clients
			}

			foreach (KeyValuePair<ulong, System.Guid> pair in clientIDToPlayerGUIDMap)
			{
				Debug.Log($"ClientID {pair.Key} has GUID {pair.Value}");
			}

			foreach (KeyValuePair<System.Guid, PlayerNetData> pair in playerGUIDToPlayerNetData)
			{
				Debug.Log($"PlayerGUID {pair.Key} has PlayerNetData [{pair.Value.playerName}, {pair.Value.clientId}]");
			}

			//We send data about all other players to the new player
			StartCoroutine(SetPlayerNetDataC(_expeditorID));
		}

	}
}
