using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using TMPro;
using ECellDive.Interfaces;
using ECellDive.Multiplayer;
using ECellDive.SceneManagement;
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
        [SerializeField] private TextMeshProUGUI m_nameField;
        public TextMeshProUGUI nameField
        {
            get => m_nameField;
            set => m_nameField = value;
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

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            string _nameStr = GameNetPortal.Instance.settings.playerName;
            byte[] _nameB = System.Text.Encoding.UTF8.GetBytes(_nameStr);

            ExchangeNamesServerRpc(_nameB, NetworkManager.Singleton.LocalClientId);
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

        [ServerRpc(RequireOwnership = false)]
        private void ExchangeNamesServerRpc(byte[] _nameB, ulong _expeditorClientId)
        {
            ReceiveNameClientRpc(_nameB);

            ClientRpcParams expiditorClientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { _expeditorClientId }
                }
            };

            string name;
            NetworkObjectReference netObjRef;
            foreach (ulong _clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                name = GameNetPortal.Instance.netSessionPlayersDataMap[_clientId].playerName;
                netObjRef = NetworkManager.Singleton.ConnectedClients[_clientId].PlayerObject;

                ReceiveNameClientRpc(netObjRef, System.Text.Encoding.UTF8.GetBytes(name), expiditorClientRpcParams);
            }
        }

        #region - INamed Methods -
        public string GetName()
        {
            return m_nameField.text;
        }

        public void SetName(string _name)
        {
            m_nameField.text = _name;
        }
        #endregion

        #region - IMlrpVisibility Methods -
        public virtual void ManageActivationStatus(bool _previous, bool _current)
        {
            gameObject.SetActive(isActivated.Value);
        }

        public virtual void NetHide()
        {
            Debug.Log($"{NetworkManager.Singleton.LocalClientId} devient invisible");
            LogSystem.refLogManager.AddMessage(LogSystem.MessageTypes.Debug,
                $"{NetworkManager.Singleton.LocalClientId} devient invisible");
            m_nameField.gameObject.SetActive(false);
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
            Debug.Log($"{NetworkManager.Singleton.LocalClientId} devient visible");
            LogSystem.refLogManager.AddMessage(LogSystem.MessageTypes.Debug, 
                $"{NetworkManager.Singleton.LocalClientId} devient visible");

            m_nameField.gameObject.SetActive(true);

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