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
                                    ILookAt,
                                    IPopUp
        {
            
            public bool showOnEnable = true;

            #region - ILookAt Members -
            [SerializeField] private bool m_isUI = false;
            public bool isUI
            {
                get => m_isUI;
                private set => m_isUI = value;
            }
            public Transform lookAtTarget { get; private set; }
            #endregion

            #region - IPopUp Members -
            [SerializeField] private float m_popupDistance;
            public float popupDistance
            {
                get => m_popupDistance;
                private set => m_popupDistance = value;
            }

            [SerializeField] private float m_popupRelativeHeight;
            public float popupRelativeHeight
            {
                get => m_popupRelativeHeight;
                private set => m_popupRelativeHeight = value;
            }
            public Transform popupTarget { get; private set; }
            #endregion

            private void Start()
            {
                lookAtTarget = NetworkManager.Singleton.LocalClient.PlayerObject.gameObject.
                        GetComponentInChildren<Camera>().transform;
                popupTarget = lookAtTarget;
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
                    Positioning.UIFaceTarget(gameObject, lookAtTarget);
                }
                else
                {
                    gameObject.transform.LookAt(lookAtTarget);
                }
            }
            #endregion

            #region - IPopUp Methods -
            public void PopUp()
            {
                Vector3 pos = Positioning.PlaceInFrontOfTargetLocal(popupTarget, m_popupDistance, m_popupRelativeHeight);
                transform.position = pos;
                LookAt();
            }
            #endregion
        }
    }
}

