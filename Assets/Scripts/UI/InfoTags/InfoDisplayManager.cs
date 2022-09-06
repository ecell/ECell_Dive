using Unity.Netcode;
using UnityEngine;
using TMPro;
using ECellDive.Interfaces;
using ECellDive.Utility;


namespace ECellDive
{
    namespace UI
    {
        public class InfoDisplayManager : MonoBehaviour,
                                            ILookAt
        {
            public Transform refMaster;
            public Vector3 defaultPositionOffset;
            public TextMeshProUGUI refInfoTextMesh;
            public ConnectionAnchorPosition refConnectionAnchorHandler;
            public LinePositionHandler refConnectionLineHandler;

            public bool alwaysShowInfoToPlayer = false;
            [HideInInspector] public bool globalHide = false;
            public bool hideOnStart = false;

            #region - ILookAt Members-
            [SerializeField] private bool m_isUI = false;
            public bool isUI
            {
                get => m_isUI;
                private set => m_isUI = value;
            }
            public Transform lookAtTarget{ get; protected set; }
            #endregion


            private void Start()
            {
                if (refMaster == null)
                {
                    refMaster = transform.parent;
                }
                transform.position = refMaster.transform.position + defaultPositionOffset;

                if (hideOnStart)
                {
                    Hide();
                }

                lookAtTarget = NetworkManager.Singleton.LocalClient.PlayerObject.gameObject.
                            GetComponentInChildren<Camera>().transform;
                LookAt();
            }

            private void Update()
            {
                if (alwaysShowInfoToPlayer)
                {
                    LookAt();
                }
            }

            /// <summary>
            /// Switches the value of the globalHide field on every call.
            /// If set to false, it resets the visibility of the gameobject
            /// based on the value of hideOnStart.
            /// </summary>
            public void GlobalHide()
            {
                globalHide = !globalHide;
                if (globalHide)
                {
                    Hide();
                }
                else
                {
                    if (!hideOnStart)
                    {
                        Show();
                    }
                }
            }

            /// <summary>
            /// Hides the Info tag without setting it inactive
            /// </summary>
            protected virtual void Hide()
            {
                gameObject.SetActive(false);
            }

            public void SetText(string _text)
            {
                refInfoTextMesh.text = _text;
            }

            /// <summary>
            /// Controls the visibility of the gameobject only when
            /// globalHide is set to false.
            /// </summary>
            /// <param name="_show">True => Show, False => Hide</param>
            public void SetVisibility(bool _show)
            {
                if (!globalHide)
                {
                    if (_show)
                    {
                        Show();
                    }

                    else
                    {
                        Hide();
                    }
                }
            }

            /// <summary>
            /// Shows the info tag without setting it active
            /// </summary>
            protected virtual void Show()
            {
                gameObject.SetActive(true);
            }

            #region - ILookAt Methods-
            /// <summary>
            /// Computes the info tag position, connection anchor and 
            /// connection start/end so that the info tag is readable
            /// from the player point of view.
            /// </summary>
            public void LookAt()
            {
                Positioning.UIFaceTarget(gameObject, lookAtTarget);
                refConnectionAnchorHandler.SetClosestCorner();
                refConnectionLineHandler.RefreshLinePositions();
            }
            #endregion
        }
    }
}