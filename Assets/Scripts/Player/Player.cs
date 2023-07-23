using Unity.Netcode;
using UnityEngine;
using TMPro;
using ECellDive.Interfaces;
using ECellDive.Multiplayer;
using ECellDive.Utility;

namespace ECellDive.PlayerComponents
{
    public class Player : NetworkBehaviour,
                          INamed,
                          IMlprVisibility
    {
        [Header("Visibility objects")]
        public GameObject head;
        public GameObject rootControllers;

        #region - INamed Members -
        [Header("Name")]
        [SerializeField] private GameObject m_nameTextFieldContainer;
        public GameObject nameTextFieldContainer
        {
            get => m_nameTextFieldContainer;
            private set => m_nameTextFieldContainer = value;
        }

        public TextMeshProUGUI nameField
        {
            get;
            private set;
        }
        #endregion

        #region - IMlrpVisibility Members -
        private NetworkVariable<bool> m_isActivated = new NetworkVariable<bool>(true);
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
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            nameField = m_nameTextFieldContainer.GetComponentInChildren<TextMeshProUGUI>();
            NetworkManager.Singleton.OnClientConnectedCallback += ExchangeNames;
        }

        [ClientRpc]
        private void ReceiveNameClientRpc(byte[] _name)
        {
            SetName(System.Text.Encoding.UTF8.GetString(_name));
        }

        [ClientRpc]
        private void ReceiveNameClientRpc(NetworkObjectReference _playerObj, byte[] _name, ClientRpcParams _clientRpcParams)
        {
            GameObject playerGO = _playerObj;
            playerGO.GetComponent<Player>().SetName(System.Text.Encoding.UTF8.GetString(_name));
        }

        private void ExchangeNames(ulong _clientId)
        {
            string _nameStr = GameNetPortal.Instance.settings.playerName;
            byte[] _nameB = System.Text.Encoding.UTF8.GetBytes(_nameStr);
            ExchangeNamesServerRpc(_nameB, _clientId);
        }

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
        public virtual void ManageActivationStatus(bool _previous, bool _current)
        {
            gameObject.SetActive(isActivated.Value);
        }

        public virtual void NetHide()
        {
            Debug.Log($"{NetworkManager.Singleton.LocalClientId} becomes invisible");
            LogSystem.AddMessage(LogMessageTypes.Debug,
                $"{NetworkManager.Singleton.LocalClientId} becomes invisible");
            nameField.gameObject.SetActive(false);
            head.SetActive(false);
            rootControllers.SetActive(false);
        }

        [ClientRpc]
        public virtual void NetHideClientRpc(ClientRpcParams _clientRpcParams)
        {
            NetHide();
        }

        public virtual void NetShow()
        {
            Debug.Log($"{NetworkManager.Singleton.LocalClientId} becomes visible");
            LogSystem.AddMessage(LogMessageTypes.Debug, 
                $"{NetworkManager.Singleton.LocalClientId} becomes visible");

            nameField.gameObject.SetActive(true);

            head.SetActive(true);

            rootControllers.SetActive(true);
        }

        [ClientRpc]
        public virtual void NetShowClientRpc(ClientRpcParams _clientRpcParams)
        {
            NetShow();
        }

        [ServerRpc(RequireOwnership = false)]
        public void RequestSetActiveServerRpc(bool _active)
        {
            isActivated.Value = _active;
        }
        #endregion
    }
}