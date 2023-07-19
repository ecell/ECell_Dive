using UnityEngine;
using ECellDive.Utility;
using ECellDive.Interfaces;

namespace ECellDive
{
    namespace Utility
    {
        /// <summary>
        /// Public interface to add module data on callback in
        /// in the Unity Editor.
        /// </summary>
        public class GameObjectConstructor : MonoBehaviour, IPopUp
        {
            public GameObject refPrefab;

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

            [Tooltip("If popup target is left to null, then the Main Camera's transform is used.")]
            [SerializeField] private Transform m_popupTarget;
            public Transform popupTarget
            {
                get => m_popupTarget;
                set => m_popupTarget = value;
            }
            #endregion

            public void Constructor()
            {
                PopUp();
            }

            #region - IPopUp Methods -
            public void PopUp()
            {
                if (popupTarget == null)
                {
                    popupTarget = Camera.main.transform;
                }
                Vector3 pos = Positioning.PlaceInFrontOfTarget(popupTarget, m_popupDistance, m_popupHeightOffset);
                Instantiate(refPrefab, pos, Quaternion.identity);
            }
            #endregion
        }
    }
}

