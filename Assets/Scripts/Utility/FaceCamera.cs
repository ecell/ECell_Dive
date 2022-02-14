using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ECellDive
{
    namespace Utility
    {
        public class FaceCamera : MonoBehaviour
        {
            public bool showOnEnable = true;

            private void OnEnable()
            {
                if (showOnEnable)
                {
                    ShowBackToPlayer();
                }
            }

            public void ShowBackToPlayer()
            {
                Positioning.UIFaceTarget(gameObject, Camera.main.transform);
            }
        }
    }
}

