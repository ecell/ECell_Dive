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
            public Camera refCamera;

            public TextMeshProUGUI refInfoTextMesh;
            public ConnectionAnchorPosition refConnectionAnchorHandler;
            public LinePositionHandler refConnectionLineHandler;

            public bool alwaysShowInfoToPlayer = false;

            public void SetText(string _text)
            {
                refInfoTextMesh.text = _text;
            }

            /// <summary>
            /// Computes the info tag position, connection anchor and 
            /// connection start/end so that the info tag is readable
            /// from the player point of view.
            /// </summary>
            public void ShowInfoToPlayer()
            {
                Positioning.UIFaceTarget(gameObject, refCamera.transform);
                refConnectionAnchorHandler.SetClosestCorner();
                refConnectionLineHandler.RefreshLinePositions();
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

