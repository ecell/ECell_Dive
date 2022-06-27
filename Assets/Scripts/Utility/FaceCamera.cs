using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using ECellDive.Interfaces;

namespace ECellDive
{
    namespace Utility
    {
        public class FaceCamera : MonoBehaviour,
                                    ILookAt
        {
            
            public bool showOnEnable = true;

            #region - ILookAt Members -
            [SerializeField] private bool m_isUI = false;
            public bool isUI
            {
                get => m_isUI;
                private set => m_isUI = value;
            }
            public Transform target { get; private set; }
            #endregion

            private void Start()
            {
                target = NetworkManager.Singleton.LocalClient.PlayerObject.gameObject.
                        GetComponentInChildren<Camera>().transform;
                LookAt();
            }

            private void OnEnable()
            {
                if (showOnEnable)
                {
                    LookAt();
                }
            }

            #region - ILookAt Methods -
            public void LookAt()
            {
                if (isUI)
                {
                    Positioning.UIFaceTarget(gameObject, target);
                }
                else
                {
                    gameObject.transform.LookAt(target);
                }
            }
            #endregion
        }
    }
}

