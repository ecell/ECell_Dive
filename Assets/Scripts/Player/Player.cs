using Unity.Netcode;
using UnityEngine;
using TMPro;
using ECellDive.Interfaces;
using ECellDive.Multiplayer;
using ECellDive.Utility;
using ECellDive.SceneManagement;

namespace ECellDive.PlayerComponents
{
	/// <summary>
	/// The logic behind the player gameobject.
	/// </summary>
	public class Player : NetworkBehaviour,
						  INamed,
						  IMlprVisibility
	{
		/// <summary>
		/// The gameobject encapsulating the head of the player.
		/// </summary>
		[Header("Visibility objects")]
		public GameObject head;

		/// <summary>
		/// The gameobject encapsulating the controllers of the player.
		/// </summary>
		public GameObject rootControllers;

		#region - INamed Members -
		/// <summary>
		/// The field of the <see cref="nameTextFieldContainer"/> property.
		/// </summary>
		[Header("Name")]
		[SerializeField] private GameObject m_nameTextFieldContainer;
		
		/// <inheritdoc/>
		public GameObject nameTextFieldContainer
		{
			get => m_nameTextFieldContainer;
			private set => m_nameTextFieldContainer = value;
		}

		/// <inheritdoc/>
		public TextMeshProUGUI nameField
		{
			get;
			private set;
		}
		#endregion

		#region - IMlrpVisibility Members -
		/// <summary>
		/// The field of the <see cref="isActivated"/> property.
		/// </summary>
		private NetworkVariable<bool> m_isActivated = new NetworkVariable<bool>(true);

		/// <inheritdoc/>
		public NetworkVariable<bool> isActivated
		{
			get => m_isActivated;
			protected set => m_isActivated = value;
		}
		#endregion

		private void Awake()
		{
			nameField = m_nameTextFieldContainer.GetComponentInChildren<TextMeshProUGUI>();
		}

		public override void OnNetworkSpawn()
		{
			if (IsLocalPlayer)
			{
				NetworkManager.Singleton.OnClientConnectedCallback += HandleNameTargetCamera;
				GameNetDataManager.Instance.OnClientReceivedAllPlayerNetData += UpdatePlayerNamesInContainers;
				GameNetDataManager.Instance.OnClientReceivedAllModules += SpawnInDiveScene;
			}
		}

		public override void OnNetworkDespawn()
		{
			if (IsLocalPlayer)
			{
				NetworkManager.Singleton.OnClientConnectedCallback -= HandleNameTargetCamera;
				GameNetDataManager.Instance.OnClientReceivedAllPlayerNetData -= UpdatePlayerNamesInContainers;
				GameNetDataManager.Instance.OnClientReceivedAllModules -= SpawnInDiveScene;
			}
		}

		/// <summary>
		/// A callback to make sure that the name canvas of the replicated
		/// players in the environment of the local player face the camera
		/// of the local player. Called back when on new client connection.
		/// </summary>
		/// <param name="_clientID">
		/// The ID of the client that just connected to the server.
		/// </param>
		private void HandleNameTargetCamera(ulong _clientID)
		{
			Debug.Log($"Player HandleNameTargetCamera {_clientID}, localClientID {NetworkManager.Singleton.LocalClientId}");

			if (_clientID == NetworkManager.LocalClientId)
			{
				HandleNameTargetCameraServerRpc(_clientID);
			}
		}

		/// <summary>
		/// Instructs replicated players <paramref name="_playerObj"/> to have its
		/// name canvas face the camera of the local player.
		/// </summary>
		/// <param name="_playerObj">
		/// Network reference (to be converted to GameObject) of the player which
		/// we want to have its name canvas face the camera of the local player.
		/// </param>
		/// <param name="_clientRpcParams">
		/// The client RPC params that allows to reach specific clients.
		/// </param>
		[ClientRpc]
		private void HandleNameTargetCameraClientRpc(NetworkObjectReference _playerObj, ClientRpcParams _clientRpcParams)
		{
			//Debug.Log($"Local player object ={NetworkManager.Singleton.LocalClient.PlayerObject.gameObject.GetComponentInChildren<Camera>()}", NetworkManager.Singleton.LocalClient.PlayerObject);
			GameObject playerGO = _playerObj;
			//Debug.Log($"Replicated Copy ILookAt={playerGO.GetComponentInChildren<ILookAt>()}", playerGO);
			playerGO.GetComponentInChildren<FaceCamera>().SetTargets(NetworkManager.Singleton.LocalClient.PlayerObject.gameObject.GetComponentInChildren<Camera>().transform);
		}

		/// <summary>
		/// Sends various orders between the clients to have each the name of replicated
		/// player copy of each local client face the camera of the local player.
		/// Called back when a new client connects to the server.
		/// </summary>
		/// <param name="_expeditorClientId">
		/// The id of the client that just connected to the server.
		/// </param>
		[ServerRpc(RequireOwnership = false)]
		private void HandleNameTargetCameraServerRpc(ulong _expeditorClientId)
		{
			if (IsServer)
			{
				ulong[] otherClientId = new ulong[NetworkManager.Singleton.ConnectedClientsIds.Count - 1];
				ClientRpcParams clientRpcParams = new ClientRpcParams
				{
					Send = new ClientRpcSendParams
					{
						TargetClientIds = new ulong[1] { _expeditorClientId }
					}
				};

				ushort i = 0;
				foreach (ulong _clientId in NetworkManager.Singleton.ConnectedClientsIds)
				{
					if (_clientId != _expeditorClientId)
					{
						//Send the order to the already connected replicated clients copies in the 
						//expeditor client environment to face the camera of the local player (expeditor)

						HandleNameTargetCameraClientRpc(NetworkManager.Singleton.ConnectedClients[_clientId].PlayerObject,
							clientRpcParams);

						otherClientId[i] = _clientId;
						i++;
					}
				}

				//Send the order to the replicated copy of the expeditor client in the already connected
				//clients environment to face the camera of the local player (other client)
				clientRpcParams = new ClientRpcParams
				{
					Send = new ClientRpcSendParams
					{
						TargetClientIds = otherClientId
					}
				};
				HandleNameTargetCameraClientRpc(NetworkManager.Singleton.ConnectedClients[_expeditorClientId].PlayerObject,
					clientRpcParams);
			}
		}

        /// <summary>
        /// A callback to drop the player in the dive scene registered last
        /// in its Dive travel list. Called back when <see cref="GameNetDataManager.Instance.OnClientReceivedAllModules"/>
        /// is triggered. This typically happens when the client is a new
        /// player in the game and the server has finished synchronizing
        /// all the modules data with him.
        /// </summary>
        /// <param name="_clientID">
        /// The ID of the client that just received all the modules data.
        /// </param>
        private void SpawnInDiveScene(ulong _clientID)
        {
            Debug.Log($"Player SpawnInDiveScene {_clientID}, localClientID {NetworkManager.Singleton.LocalClientId}");
            if (_clientID == NetworkManager.Singleton.LocalClientId)
            {
                DiveScenesManager.Instance.DropClientInSceneServerRpc(_clientID);
            }
        }

		/// <summary>
		/// A callback to update the name in the name containers of the replicated
		/// payers in the environment of the local player. Called back when <see cref="GameNetDataManager.Instance.OnClientReceivedAllPlayerNetData"/>
		/// is triggered. This typically happens when the client is a new
		/// player in the game and the server has finished synchronizing
		/// all the player net data with him.
		/// </summary>
		/// <param name="_clientID">
		/// The ID of the client that just received all the player net data.
		/// </param>
        private void UpdatePlayerNamesInContainers(ulong _clientID)
		{
			Debug.Log($"Player UpdatePlayerNamesInContainers {_clientID}, localClientID {NetworkManager.Singleton.LocalClientId}");

			if (_clientID == NetworkManager.LocalClientId)
			{
				UpdatePlayerNamesInContainersServerRpc(_clientID);
			}
		}

		/// <summary>
		/// Updates the name in the name container of the player <paramref name="_playerObj"/>.
		/// Uses the <paramref name="_playerClientID"/> to retrieve the name of the player
		/// in the <see cref="GameNetDataManager"/>.
		/// </summary>
		/// <param name="_playerObj">
		/// The network reference of the player whose name we want to update.
		/// </param>
		/// <param name="_playerName">
		/// The name of the client to set.
		/// </param>
		/// <param name="_clientRpcParams">
		/// The client RPC params that allows to reach specific clients.
		/// </param>
		[ClientRpc]
		private void UpdatePlayerNamesInContainersClientRpc(NetworkObjectReference _playerObj, string _playerName, ClientRpcParams _clientRpcParams)
		{
			GameObject playerGO = _playerObj;
			Debug.Log($"The local client {NetworkManager.Singleton.LocalClientId} udpates the name of playerGO to: " + _playerName, playerGO);
			playerGO.GetComponentInChildren<Player>().SetName(_playerName);
		}

		/// <summary>
		/// Requests the server to order the clients to update the name in the name containers
		/// of the replicated player copies of each local client.
		/// </summary>
		/// <param name="_expeditorClientId">
		/// The id of the client that just connected to the server.
		/// </param>
		[ServerRpc(RequireOwnership = false)]
		private void UpdatePlayerNamesInContainersServerRpc(ulong _expeditorClientId)
		{
			ulong[] otherClientId = new ulong[NetworkManager.Singleton.ConnectedClientsIds.Count - 1];
			ClientRpcParams clientRpcParams = new ClientRpcParams
			{
				Send = new ClientRpcSendParams
				{
					TargetClientIds = new ulong[1] { _expeditorClientId }
				}
			};

			ushort i = 0;
			foreach (ulong _clientId in NetworkManager.Singleton.ConnectedClientsIds)
			{
				if (_clientId != _expeditorClientId)
				{
					//Send the order to the already connected replicated clients copies in the 
					//expeditor client environment to update their names
					//Debug.Log($"Sending name ({GameNetDataManager.Instance.GetClientName(_clientId)})" +
					//	$"of already connected clients {_clientId} " +
					//	$"to the new client {_expeditorClientId}", gameObject);
					UpdatePlayerNamesInContainersClientRpc(NetworkManager.Singleton.ConnectedClients[_clientId].PlayerObject,
						GameNetDataManager.Instance.GetClientName(_clientId),
						clientRpcParams);

					otherClientId[i] = _clientId;
					i++;
				}
			}

			//Send the order to the replicated copy of the expeditor client in the already connected
			//clients environment to update their names
			clientRpcParams = new ClientRpcParams
			{
				Send = new ClientRpcSendParams
				{
					TargetClientIds = otherClientId
				}
			};

			//Debug.Log($"Sending name ({GameNetDataManager.Instance.GetClientName(_expeditorClientId)})" +
			//	$"of new client {_expeditorClientId} " +
			//	$"to the already connected clients", gameObject);
			UpdatePlayerNamesInContainersClientRpc(NetworkManager.Singleton.ConnectedClients[_expeditorClientId].PlayerObject,
				GameNetDataManager.Instance.GetClientName(_expeditorClientId),
				clientRpcParams);
		}

		#region - INamed Methods -
		/// <inheritdoc/>
		public virtual void DisplayName()
		{
			m_nameTextFieldContainer.gameObject.SetActive(true);
		}

		/// <inheritdoc/>
		public string GetName()
		{
			return nameField.text;
		}

		/// <inheritdoc/>
		public void HideName()
		{
			m_nameTextFieldContainer.gameObject.SetActive(false);
		}

		/// <inheritdoc/>
		public void SetName(string _name)
		{
			nameField.text = _name;
		}

		/// <inheritdoc/>
		public void ShowName()
		{
			m_nameTextFieldContainer.GetComponent<ILookAt>().LookAt();
		}
		#endregion

		#region - IMlrpVisibility Methods -
		/// <inheritdoc/>
		public virtual void ManageActivationStatus(bool _previous, bool _current)
		{
			gameObject.SetActive(isActivated.Value);
		}

		/// <inheritdoc/>
		public virtual void NetHide()
		{
			Debug.Log($"Player {NetworkManager.Singleton.LocalClientId} becomes invisible");
			LogSystem.AddMessage(LogMessageTypes.Debug,
				$"Player {NetworkManager.Singleton.LocalClientId} becomes invisible");
			nameField.gameObject.SetActive(false);
			head.SetActive(false);
			rootControllers.SetActive(false);
		}

		/// <inheritdoc/>
		[ClientRpc]
		public virtual void NetHideClientRpc(ClientRpcParams _clientRpcParams)
		{
			NetHide();
		}

		/// <inheritdoc/>
		public virtual void NetShow()
		{
			Debug.Log($"Player {NetworkManager.Singleton.LocalClientId} becomes visible");
			LogSystem.AddMessage(LogMessageTypes.Debug, 
				$"Player {NetworkManager.Singleton.LocalClientId} becomes visible");

			nameField.gameObject.SetActive(true);

			head.SetActive(true);

			rootControllers.SetActive(true);
		}

		/// <inheritdoc/>
		[ClientRpc]
		public virtual void NetShowClientRpc(ClientRpcParams _clientRpcParams)
		{
			NetShow();
		}

		/// <inheritdoc/>
		[ServerRpc(RequireOwnership = false)]
		public void RequestSetActiveServerRpc(bool _active)
		{
			isActivated.Value = _active;
		}
		#endregion
	}
}