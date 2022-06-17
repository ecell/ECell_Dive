using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using TMPro;
using ECellDive.Interfaces;


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