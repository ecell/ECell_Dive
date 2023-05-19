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

            [SerializeField] private float m_popupHeightOffset;
            public float popupHeightOffset
            {
                get => m_popupHeightOffset;
                private set => m_popupHeightOffset = value;
            }
            public Transform popupTarget { get; private set; }
            #endregion

            private void Start()
            {
                if (Camera.main != null)
                {
                    SetTargets(Camera.main.transform);
                }
                
                LookAt();
            }

            private void OnEnable()
            {
                if (showOnEnable)
                {
                    LookAt();
                }
            }

            /// <summary>
            /// Sets the value of <see cref="lookAtTarget"/> and <see cref="popupTarget"/>
            /// to <paramref name="target"/>.
            /// </summary>
            /// <param name="target">The target of the <see cref="LookAt"/> and
            /// <see cref="PopUp"/> methods.</param>
            public void SetTargets(Transform target)
            {
                lookAtTarget = target;
                popupTarget = target;
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
                Vector3 pos = Positioning.PlaceInFrontOfTarget(popupTarget, m_popupDistance, m_popupHeightOffset);
                transform.position = pos;
                LookAt();
            }
            #endregion
        }
    }
}