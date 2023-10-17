using Unity.Netcode;
using UnityEngine;
using TMPro;
using ECellDive.Interfaces;
using ECellDive.Multiplayer;
using ECellDive.Utility;

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

		public override void OnNetworkDespawn()
		{
			base.OnNetworkDespawn();
			NetworkManager.Singleton.OnClientConnectedCallback -= ExchangeNames;
			NetworkManager.Singleton.OnClientConnectedCallback -= HandleNameTargetCamera;
		}

		public override void OnNetworkSpawn()
		{
			base.OnNetworkSpawn();
			nameField = m_nameTextFieldContainer.GetComponentInChildren<TextMeshProUGUI>();
			NetworkManager.Singleton.OnClientConnectedCallback += ExchangeNames;
			NetworkManager.Singleton.OnClientConnectedCallback += HandleNameTargetCamera;
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
			Debug.Log($"Local player object ={NetworkManager.Singleton.LocalClient.PlayerObject.gameObject.GetComponentInChildren<Camera>()}", NetworkManager.Singleton.LocalClient.PlayerObject);
			GameObject playerGO = _playerObj;
			Debug.Log($"Replicated Copy ILookAt={playerGO.GetComponentInChildren<ILookAt>()}", playerGO);
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
		private void HandleNameTargetCamera(ulong _expeditorClientId)
		{
			if (IsServer)
			{
				ClientRpcParams clientRpcParams;
				ulong[] expeditorClientId = new ulong[1] { _expeditorClientId };
				ulong[] otherClientId = new ulong[NetworkManager.Singleton.ConnectedClientsIds.Count - 1];
				
				NetworkObjectReference netObjRef;
				ushort i = 0;
				foreach (ulong _clientId in NetworkManager.Singleton.ConnectedClientsIds)
				{
					if (_clientId != _expeditorClientId)
					{
						//Send the order to the already connected replicated clients copies in the 
						//expeditor client environment to face the camera of the local player (expeditor)
						clientRpcParams = new ClientRpcParams
						{
							Send = new ClientRpcSendParams
							{
								TargetClientIds = expeditorClientId
							}
						};
						netObjRef = NetworkManager.Singleton.ConnectedClients[_clientId].PlayerObject;
						HandleNameTargetCameraClientRpc(netObjRef, clientRpcParams);

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
				netObjRef = NetworkManager.Singleton.ConnectedClients[_expeditorClientId].PlayerObject;
				HandleNameTargetCameraClientRpc(netObjRef, clientRpcParams);
			}
		}

		/// <summary>
		/// Assigns a name that has transited in the network to the name field
		/// of this player object.
		/// </summary>
		/// <param name="_name">
		/// The new name of the player.
		/// </param>
		[ClientRpc]
		private void ReceiveNameClientRpc(byte[] _name)
		{
			SetName(System.Text.Encoding.UTF8.GetString(_name));
		}

		/// <summary>
		/// Assigns a name to a replicated copy of a player object in target
		/// clients session.
		/// </summary>
		/// <param name="_playerObj">
		/// The reference to the replicated copy of the player object which we 
		/// want to assign a name to.
		/// </param>
		/// <param name="_name">
		/// The name to assign to the replicated copy of the player object.
		/// </param>
		/// <param name="_clientRpcParams">
		/// The client RPC params that allows to reach specific clients.
		/// </param>
		[ClientRpc]
		private void ReceiveNameClientRpc(NetworkObjectReference _playerObj, byte[] _name, ClientRpcParams _clientRpcParams)
		{
			GameObject playerGO = _playerObj;
			playerGO.GetComponent<Player>().SetName(System.Text.Encoding.UTF8.GetString(_name));
		}

		/// <summary>
		/// Send the name of the local player for distribution to the
		/// other clients.
		/// Called back when a new client connects to the server.
		/// </summary>
		/// <param name="_clientId">
		/// The id of the client that just connected to the server.
		/// </param>
		private void ExchangeNames(ulong _clientId)
		{
			string _nameStr = GameNetPortal.Instance.settings.playerName;
			byte[] _nameB = System.Text.Encoding.UTF8.GetBytes(_nameStr);
			ExchangeNamesServerRpc(_nameB, _clientId);
		}

		/// <summary>
		/// Notifies the server that a client wants to exchange names with
		/// other clients.
		/// </summary>
		/// <param name="_nameB">
		/// The name of the client that expedited the request
		/// </param>
		/// <param name="_expeditorClientId">
		/// The id of the client that expedited the request.
		/// </param>
		[ServerRpc(RequireOwnership = false)]
		private void ExchangeNamesServerRpc(byte[] _nameB, ulong _expeditorClientId)
		{
			//Send the name info to the replicated copies on the other clients
			ReceiveNameClientRpc(_nameB);
			
			ClientRpcParams expeditorClientRpcParams = new ClientRpcParams
			{
				Send = new ClientRpcSendParams
				{
					TargetClientIds = new ulong[] { _expeditorClientId }
				}
			};

			//Send the name of the already connected clients to the newly
			//connecting client
			string name;
			NetworkObjectReference netObjRef;
			foreach (ulong _clientId in NetworkManager.Singleton.ConnectedClientsIds)
			{
				name = GameNetPortal.Instance.netSessionPlayersDataMap[_clientId].playerName;
				netObjRef = NetworkManager.Singleton.ConnectedClients[_clientId].PlayerObject;
				

				ReceiveNameClientRpc(netObjRef, System.Text.Encoding.UTF8.GetBytes(name), expeditorClientRpcParams);
			}
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
			Debug.Log($"{NetworkManager.Singleton.LocalClientId} becomes invisible");
			LogSystem.AddMessage(LogMessageTypes.Debug,
				$"{NetworkManager.Singleton.LocalClientId} becomes invisible");
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
			Debug.Log($"{NetworkManager.Singleton.LocalClientId} becomes visible");
			LogSystem.AddMessage(LogMessageTypes.Debug, 
				$"{NetworkManager.Singleton.LocalClientId} becomes visible");

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