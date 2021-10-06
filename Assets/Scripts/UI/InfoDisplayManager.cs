using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ECellDive.Utility;


namespace ECellDive
{
    namespace UI
    {
        public class InfoDisplayManager : MonoBehaviour
        {
            public TextMeshProUGUI refInfoTextMesh;
            public ConnectionAnchorPosition refConnectionAnchorHandler;
            public LinePositionHandler refConnectionLineHandler;

            public bool alwaysShowInfoToPlayer = false;
            [HideInInspector] public bool globalHide = false;
            public bool hideOnStart = false;

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
                GetComponentInChildren<CanvasGroup>().alpha = 0f;
                GetComponentInChildren<BoxCollider>().enabled = false;
                refConnectionLineHandler.gameObject.GetComponent<LineRenderer>().enabled = false;
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
                GetComponentInChildren<CanvasGroup>().alpha = 1f;
                GetComponentInChildren<BoxCollider>().enabled = true;
                refConnectionLineHandler.gameObject.GetComponent<LineRenderer>().enabled = true;
            }

            /// <summary>
            /// Computes the info tag position, connection anchor and 
            /// connection start/end so that the info tag is readable
            /// from the player point of view.
            /// </summary>
            public void ShowInfoToPlayer()
            {
                Positioning.UIFaceTarget(gameObject, Camera.main.transform);
                refConnectionAnchorHandler.SetClosestCorner();
                refConnectionLineHandler.RefreshLinePositions();
            }

            private void Start()
            {
                if (hideOnStart)
                {
                    Hide();
                }
            }

            private void Update()
            {
                if (alwaysShowInfoToPlayer)
                {
                    ShowInfoToPlayer();
                }
            }
        }
    }
}

