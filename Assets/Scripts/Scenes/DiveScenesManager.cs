using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

using ECellDive.Modules;
using ECellDive.Multiplayer;
using ECellDive.PlayerComponents;
using ECellDive.Utility;
using ECellDive.Utility.Data.Dive;

namespace ECellDive.SceneManagement
{
	/// <summary>
	/// Implements the logic for the scene transitions when a player is diving
	/// or resurfacing.
	/// </summary>
	/// <remarks>
	/// Synchronized with multiplayer.
	/// </remarks>
	public class DiveScenesManager : NetworkBehaviour
	{
		/// <summary>
		/// The singleton instance of this class.
		/// </summary>
		public static DiveScenesManager Instance { get; private set; }

		/// <summary>
		/// A list of prefabs that may be instantiated by the user at runtime.
		/// Every prefab MUST have a <see cref="ECellDive.Modules.GameNetModule"/> and Unity.Netcode.NetworkObject.
		/// component. We advise to use this list for root data modules (e.g. CyJsonModule) only.
		/// "Submodules" that may be instantiated as a result of a module's data generation
		/// should be referenced in the script detailing the said data generation process.
		/// </summary>
		public GameObject[] modulePrefabs;

		/// <summary>
		/// Data to control the dive animation.
		/// </summary>
		public DivingAnimationData divingAnimationData;

		/// <summary>
		/// The list of dive scenes.
		/// </summary>
		public List<SceneData> scenesBank = new List<SceneData>();

		/// <summary>
		/// A boolean to indicate that a scene has finished hiding thanks to
		/// <see cref="HideScene(int, ulong)"/>.
		/// </summary>
		private bool currentSceneisHidden = false;

		/// <summary>
		/// A boolean to indicate that a scene has finished showing thanks to
		/// <see cref="ShowScene(int, ulong)"/>.
		/// </summary>
		private bool targetSceneIsVisible = false;

		public override void OnNetworkSpawn()
		{
			Instance = this;
			if (IsServer)
			{
				Debug.Log("Spawning Scene Management");
				if (scenesBank.Count == 0)
				{
					AddNewDiveScene(-1, "Root");

					//TODO1: This forces the player in the root scene on spwan
					//But this does not account for player reconnecting to the game
					//which may have left from a different scene. 
					//TODO2 (maybe): The scene bank is not synchronized with the clients
					//but is only managed by the server because currently the "AddNewDiveScene"
					//method is only called within ServerRPC. But this there are no guarentees
					//for the future. And we should really think whether the players might
					//need it a lot in the future that it would be worth synchronizing it.
					//Otherwise, we should have a Client->Server->Client RPCs to send the 
					//scene's information to the clients.
					DiverGetsInServerRpc(0, NetworkManager.Singleton.LocalClientId);
					currentSceneisHidden = true;
					StartCoroutine(ShowScene(0, NetworkManager.Singleton.LocalClientId));
					StartCoroutine(UpdatePlayerDataC(0, NetworkManager.Singleton.LocalClientId));
				}
			}
		}

		/// <summary>
		/// Remove the reference to the diver with id <paramref name="_clientIdToClear"/>
		/// from every scene in the <see cref="scenesBank"/> (in and out divers' lists).
		/// </summary>
		/// <param name="_clientIdToClear">
		/// The ID of the client to remove from every scene.
		/// </param>
		[ServerRpc(RequireOwnership = false)]
		public void ClearPlayerFromSessionServerRpc(ulong _clientIdToClear)
		{
			foreach(SceneData _scene in scenesBank)
			{
				_scene.inDivers.Remove(_clientIdToClear);
				_scene.outDivers.Remove(_clientIdToClear);
			}
		}

		/// <summary>
		///  Checks whether a dive scenes has at least one player in.
		/// </summary>
		/// <param name="_sceneId">The index of the scene in the <see cref="scenesBank"/>.</param>
		/// <returns>Returns true if the scene contains at least a diver. False, otherwise.</returns>
		public bool CheckIfDiveSceneHasPlayers(int _sceneId)
		{
			return scenesBank[_sceneId].inDivers.Count > 0;
		}

		/// <summary>
		/// Removes the scene at index <paramref name="_sceneId"/> from the <see cref="scenesBank"/>.
		/// </summary>
		/// <param name="_sceneId">The index of the scene to remove from the <see cref="scenesBank"/>.</param>
		public void DestroyDiveScene(int _sceneId)
		{
			scenesBank.RemoveAt(_sceneId);
		}

		/// <summary>
		/// A client notifies the server that it is entering a scene.
		/// </summary>
		/// <param name="_sceneId">
		/// The scene id of the scene the diver is entering.
		/// </param>
		/// <param name="_diverClientId">
		/// The client id of the diver entering the scene.
		/// </param>
		[ServerRpc]
		public void DiverGetsInServerRpc(int _sceneId, ulong _diverClientId)
		{
			Debug.Log($"Diver {_diverClientId} is entering scene {_sceneId}");
			scenesBank[_sceneId].DiverGetsIn(_diverClientId);
		}

		/// <summary>
		/// A client notifies the server that it is leaving a scene.
		/// </summary>
		/// <param name="_sceneId">
		/// The scene id of the scene the diver is leaving.
		/// </param>
		/// <param name="_diverClientId">
		/// The client id of the diver leaving the scene.
		/// </param>
		[ServerRpc]
		public void DiverGetsOutServerRpc(int _sceneId, ulong _diverClientId)
		{
			Debug.Log($"Diver {_diverClientId} is leaving scene {_sceneId}");
			scenesBank[_sceneId].DiverGetsOut(_diverClientId);
		}

		/// <summary>
		/// Called to instantiate a new scene upon diving in a module
		/// </summary>
		public int AddNewDiveScene(int _parentSceneId, string _sceneName)
		{
			SceneData newScene = new SceneData(scenesBank.Count, _parentSceneId, _sceneName);
			foreach (ulong _clientId in NetworkManager.Singleton.ConnectedClientsIds)
			{
				newScene.AddOutDiver(_clientId);
			}
			scenesBank.Add(newScene);
			return newScene.sceneID;
		}

#if UNITY_EDITOR
		/// <summary>
		/// Outputs the content of the <see cref="scenesBank"/>.
		/// </summary>
		/// <remarks>Used for Debug purposes.</remarks>
		public void DebugScene()
		{
			foreach (SceneData sceneData in scenesBank)
			{
				Debug.Log(sceneData);
				LogSystem.AddMessage(LogMessageTypes.Debug, sceneData.ToString());
			}
		}
#endif
		/// <summary>
		/// Calls <see cref="GameNetModule.NetHide"/> for every <see cref=
		/// "GameNetModule"/> of the scene with <paramref name="_sceneID"/>
		/// for the local client.
		/// </summary>
		/// <param name="_sceneID">Index of the scene in <see cref="scenesBank"/></param>
		/// <param name="_outDiverClientId">Client Id of the diver leaving the scene
		/// with id <paramref name="_sceneID"/>.</param>
		/// <remarks>
		/// This is used server-side.
		/// </remarks>
		private IEnumerator HideScene(int _sceneID, ulong _outDiverClientId)
		{
			Debug.Log($"Original scene information:");
			LogSystem.AddMessage(LogMessageTypes.Debug,
				$"Original scene information:");
			DebugScene();
			//Updating divers for the scene in the Scene bank
			scenesBank[_sceneID].DiverGetsOut(_outDiverClientId);

			Debug.Log($"Hiding scene {_sceneID} for client {_outDiverClientId}");
			LogSystem.AddMessage(LogMessageTypes.Debug,
				$"Hiding scene {_sceneID} for client {_outDiverClientId}");
			DebugScene();

			ClientRpcParams outDiverClientRpcParams = new ClientRpcParams
			{
				Send = new ClientRpcSendParams
				{
					TargetClientIds = new ulong[] { _outDiverClientId }
				}
			};

			int moduleCounter = 0;
			//Hide every module of the scene to the diver that is leaving
			foreach (GameNetModule _gameNetMod in scenesBank[_sceneID].loadedModules)
			{
				_gameNetMod.NetHideClientRpc(outDiverClientRpcParams);
				moduleCounter++;

				if (moduleCounter == 50)
				{
					//Debug.Log("Waiting for end of Frame");
					yield return new WaitForEndOfFrame();
					moduleCounter = 0;
				}
			}

			ClientRpcParams inDiversRpcParams = new ClientRpcParams
			{
				Send = new ClientRpcSendParams
				{
					TargetClientIds = scenesBank[_sceneID].inDivers.ToArray()
				}
			};

			//out-diver GameObject
			GameObject diverGo = NetworkManager.Singleton.ConnectedClients[_outDiverClientId].PlayerObject.gameObject;
			//Hide the out diver from all the in-divers
			diverGo.GetComponent<Player>().NetHideClientRpc(inDiversRpcParams);

			//Hide all the in-divers from the out-diver
			foreach (ulong _inDiverCliendId in scenesBank[_sceneID].inDivers)
			{
				Debug.Log($"Hiding {_inDiverCliendId} to {_outDiverClientId}");
				LogSystem.AddMessage(LogMessageTypes.Debug,
				$"Hiding {_inDiverCliendId} to {_outDiverClientId}");
				//in-Diver gameObject
				diverGo = NetworkManager.Singleton.ConnectedClients[_inDiverCliendId].PlayerObject.gameObject;
				diverGo.GetComponent<Player>().NetHideClientRpc(outDiverClientRpcParams);
			}

			currentSceneisHidden = true;
		}

		/// <summary>
		/// Calls <see cref="GameNetModule.NetShow"/> for every <see cref=
		/// "GameNetModule"/> of the scene with <paramref name="_sceneID"/>
		/// for the local client.
		/// </summary>
		/// <param name="_sceneID">Index of the scene in <see cref="scenesBank"/></param>
		/// <param name="_newInDiverClientId">Client Id of the diver entering the scene
		/// with id <paramref name="_sceneID"/>.</param>
		/// <remarks>
		/// This is used server-side.
		/// </remarks>
		private IEnumerator ShowScene(int _sceneID, ulong _newInDiverClientId)
		{
			yield return new WaitUntil(() => currentSceneisHidden);
			currentSceneisHidden = false;
			Debug.Log($"Showing scene {_sceneID} for client {_newInDiverClientId}");
			LogSystem.AddMessage(LogMessageTypes.Debug,
				$"Showing scene {_sceneID} for client {_newInDiverClientId}");
			DebugScene();
			ClientRpcParams newInDiverClientRpcParams = new ClientRpcParams
			{
				Send = new ClientRpcSendParams
				{
					TargetClientIds = new ulong[] { _newInDiverClientId }
				}
			};

			int moduleCounter = 0;
			//Show every modules of the new scene to the new diver
			foreach (GameNetModule _gameNetMod in scenesBank[_sceneID].loadedModules)
			{
				_gameNetMod.NetShowClientRpc(newInDiverClientRpcParams);
				moduleCounter++;

				if(moduleCounter == 50)
				{
					yield return new WaitForEndOfFrame();
					moduleCounter = 0;
				}
			}

			ClientRpcParams oldInDiversRpcParams = new ClientRpcParams
			{
				Send = new ClientRpcSendParams
				{
					TargetClientIds = scenesBank[_sceneID].inDivers.ToArray()
				}
			};

			GameObject diverGo = NetworkManager.Singleton.ConnectedClients[_newInDiverClientId].PlayerObject.gameObject;

			//Show the the new diver to all the already present in-Divers
			diverGo.GetComponent<Player>().NetShowClientRpc(oldInDiversRpcParams);

			//Show all already present in-Divers to the new in Diver
			foreach (ulong _oldInDiverCliendId in scenesBank[_sceneID].inDivers)
			{
				Debug.Log($"Showing {_oldInDiverCliendId} to {_newInDiverClientId}");
				LogSystem.AddMessage(LogMessageTypes.Debug,
				$"Showing {_oldInDiverCliendId} to {_newInDiverClientId}");
				diverGo = NetworkManager.Singleton.ConnectedClients[_oldInDiverCliendId].PlayerObject.gameObject;
				diverGo.GetComponent<Player>().NetShowClientRpc(newInDiverClientRpcParams);
			}

			//Updating the scene's data in the scene bank once we finished showing everyone
			scenesBank[_sceneID].DiverGetsIn(_newInDiverClientId);

			LogSystem.AddMessage(LogMessageTypes.Debug,
				$"After showing scene, the scene data is: (look at next messages)");
			DebugScene();

			targetSceneIsVisible = true;
		}

		/// <summary>
		/// Instantiates a game object and spawns it for replication across the network.
		/// This method can only be called on the SERVER. So, be sure to call from a server
		/// object or called inside a ServerRpc.
		/// </summary>
		/// <param name="_sceneId">
		/// The Id of the scene in which it the new game object is instantiated.
		/// </param>
		/// <param name="_prefab">
		/// The gameobject to instantiate and spawn.
		/// It MUST be a <see cref="ECellDive.Modules.GameNetModule"/> and a Unity.Netcode.NetworkObject.
		/// </param>
		/// <param name="_position">
		/// The position at which to instantiate the gameobject.
		/// </param>
		/// <returns>The game object that got instantiated &amp; spawned.</returns>
		public GameObject SpawnModuleInScene(int _sceneId, GameObject _prefab, Vector3 _position)
		{
			GameObject go = Instantiate(_prefab, _position, Quaternion.identity);

			GameNetModule gameNetModule = go.GetComponent<GameNetModule>();
			gameNetModule.rootSceneId.Value = _sceneId;

			go.GetComponent<NetworkObject>().Spawn();
			scenesBank[_sceneId].AddModule(gameNetModule);

			return go;
		}

		/// <summary>
		/// Instantiates a game object and spawns it for replication across the network.
		/// This method can only be called on the SERVER. So, be sure to call from a server object
		/// or called inside a ServerRpc.
		/// </summary>
		/// <param name="_sceneId">The Id of the scene in which it the new game object is instantiated.</param>
		/// <param name="_prefabIdx">The index of the game object in the.
		/// It MUST be a <see cref="ECellDive.Modules.GameNetModule"/> and a Unity.Netcode.NetworkObject.</param>
		/// <param name="_position">The position at which to instantiate the gameobject.</param>
		/// <returns>The game object that got instantiated &amp; spawned.</returns>
		public GameObject SpawnModuleInScene(int _sceneId, int _prefabIdx, Vector3 _position)
		{
			GameObject go = Instantiate(modulePrefabs[_prefabIdx], _position, Quaternion.identity);

			GameNetModule gameNetModule = go.GetComponent<GameNetModule>();
			gameNetModule.rootSceneId.Value = _sceneId;

			go.GetComponent<NetworkObject>().Spawn();
			scenesBank[_sceneId].AddModule(gameNetModule);

			return go;
		}

		/// <summary>
		/// The server notification that the diver with id <paramref name="_clientId"/>
		/// is going from scene ID <paramref name="_from"/> to scene ID <paramref name="_to"/>.
		/// </summary>
		/// <param name="_from">
		/// The ID of the scene the diver is leaving.
		/// </param>
		/// <param name="_to">
		/// The ID of the scene the diver is entering.
		/// </param>
		/// <param name="_clientId">
		/// The client ID of the diver switching scenes.
		/// </param>
		[ServerRpc(RequireOwnership = false)]
		public void SwitchingScenesServerRpc(int _from, int _to, ulong _clientId)
		{
			StartCoroutine(HideScene(_from, _clientId));

			StartCoroutine(ShowScene(_to, _clientId));

			StartCoroutine(UpdatePlayerDataC(_to, _clientId));
		}

		/// <summary>
		/// The server call to resurface a diver.
		/// </summary>
		/// <param name="_surfacingDiverId">The clientID of the diver asking to resurface
		/// from his current dive scene.</param>
		[ServerRpc(RequireOwnership = false)]
		public void ResurfaceServerRpc(ulong _surfacingDiverId)
		{
			StartCoroutine(ResurfaceC(_surfacingDiverId));
		}

		/// <summary>
		/// The coroutine performing the work to actually move a diver from its
		/// current dive scene to the parent of that dive scene (resurface).
		/// </summary>
		/// <param name="_surfacingDiverId">The clientID of the diver asking to resurface
		/// from his current dive scene.</param>
		private IEnumerator ResurfaceC(ulong _surfacingDiverId)
		{
			yield return null;
			int from = GameNetDataManager.Instance.GetCurrentScene(_surfacingDiverId);
			int to = scenesBank[from].parentSceneID;

			Debug.Log($"Resurfacing client {_surfacingDiverId} from {from} to {to}");

			if (to >= 0)
			{
				SwitchingScenesServerRpc(from, to, _surfacingDiverId);
			}
		}

		/// <summary>
		/// Checks if the scene switch is finished (the target scene is visible).
		/// </summary>
		/// <returns>
		/// Returns True is <see cref="targetSceneIsVisible"/> is true. False, otherwise.
		/// </returns>
		public bool SceneSwitchIsFinished()
		{
			if (targetSceneIsVisible)
			{
				targetSceneIsVisible = false;
				return true;
			}
			return false;
		}

		/// <summary>
		/// A coroutine waiting until the target scene is visible for the client
		/// from the perspective of the server before broadcasting the scene
		/// switch information to the other clients.
		/// </summary>
		
		/// <param name="_clientId">
		/// The ID of the client switching scenes.
		/// </param>
		/// <remarks>
		/// This is used server-side.
		/// </remarks>
		private IEnumerator UpdatePlayerDataC(int _to, ulong _clientId)
		{
			yield return new WaitUntil(() => targetSceneIsVisible);
			targetSceneIsVisible = false;

			GameNetDataManager.Instance.AddToTrace(_clientId, _to);

			UpdatePlayerDataClientRPC(_clientId, _to);
		}

		/// <summary>
		/// Updates data in all clients about the player who just switched scenes.
		/// </summary>
		/// <param name="_diverID">
		/// The player who just switched scenes.
		/// </param>
		/// <param name="_to">
		/// The ID of the scene the diver is entering.
		/// </param>
		[ClientRpc]
		private void UpdatePlayerDataClientRPC(ulong _diverID, int _to)
		{
			//The diver data in case of a host is already updated in the server
			if (!IsHost)
			{
				//We update the diver data for other clients
				GameNetDataManager.Instance.AddToTrace(_diverID, _to);
			}

			if (NetworkManager.Singleton.LocalClientId == _diverID)
			{
				targetSceneIsVisible = true;
			}
		}
	}
}