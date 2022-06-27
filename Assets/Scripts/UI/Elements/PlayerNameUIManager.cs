using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using ECellDive.Interfaces;
using ECellDive.Utility;


namespace ECellDive.UI
{
    [System.Obsolete("FaceCamera does the same job")]
    public class PlayerNameUIManager : MonoBehaviour,
                                        ILookAt
    {
        #region - ILookAt Members-
        [SerializeField] private bool m_isUI = false;
        public bool isUI
        {
            get => m_isUI;
            private set => m_isUI = value;
        }
        public Transform target { get; protected set; }
        #endregion

        // Start is called before the first frame update
        void Start()
        {
            target = NetworkManager.Singleton.LocalClient.PlayerObject.gameObject.
                        GetComponentInChildren<Camera>().transform;
        }

        private void Update()
        {
            LookAt();
        }

        #region - ILookAt Methods-
        /// <summary>
        /// Computes the info tag position, connection anchor and 
        /// connection start/end so that the info tag is readable
        /// from the player point of view.
        /// </summary>
        public void LookAt()
        {
            Positioning.UIFaceTarget(gameObject, target);
        }
        #endregion
    }
}

