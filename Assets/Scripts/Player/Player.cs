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

		private void Awake()
		{
			nameField = m_nameTextFieldContainer.GetComponentInChildren<TextMeshProUGUI>();
			GameNetDataManager.Instance.OnDataShared += UpdatePlayerNamesInContainers;
			NetworkManager.Singleton.OnClientConnectedCallback += HandleNameTargetCamera;
		}

		public override void OnNetworkDespawn()
		{
			base.OnNetworkDespawn();
			GameNetDataManager.Instance.OnDataShared -= UpdatePlayerNamesInContainers;
			NetworkManager.Singleton.OnClientConnectedCallback -= HandleNameTargetCamera;
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
		/// Uses the names stored in  the values of <see cref="GameNetDataManager.playerGUIDToPlayerNetData"/>
		/// to updated the text mesh displaying the names of the players in this multiplayer session.
		/// So, it includes the local player and all replicated players.
		/// </summary>
		/// <param name="_clientId">
		/// The id of the client that just connected to the server.
		/// </param>
		private void UpdatePlayerNamesInContainers(ulong _clientId)
		{
			//The following also includes the local player even if we could
			//directly call SetName() here. It avoids having an If statement
			//in the loop.
			foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
			{
				NetworkManager.Singleton.ConnectedClients[_clientId].PlayerObject.GetComponent<Player>().SetName(
					GameNetDataManager.Instance.GetClientName(_clientId));
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