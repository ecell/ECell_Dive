using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ECellDive
{
    namespace UI
    {
        /// <summary>
        /// UI class used to compute the position of the anchor 
        /// of the line connecting the information panel to its object.
        /// </summary>
        public class ConnectionAnchorPosition : MonoBehaviour
        {
            public Transform refMasterObject;
            public RectTransform refAnchoredObject;

            private Vector3[] anchoredObjectCorners;
            private int closestCornerIndex = 0;

            /// <summary>
            /// Moves the anchor to the corner of refAnchoredObject that is
            /// the closest to refMasterObject.
            /// </summary>
            IEnumerator SetClosestCornerC()
            {
                yield return new WaitForEndOfFrame();

                anchoredObjectCorners = new Vector3[4];
                refAnchoredObject.GetWorldCorners(anchoredObjectCorners);

                float dist = Mathf.Infinity;
                for (var i = 0; i < 4; i++)
                {
                    float testDist = Vector3.Distance(anchoredObjectCorners[i], refMasterObject.position);
                    if ( testDist < dist)
                    {
                        dist = testDist;
                        closestCornerIndex = i;
                    }
                }
                transform.position = anchoredObjectCorners[closestCornerIndex];
            }

            /// <summary>
            /// The public method to fire the coroutine
            /// </summary>
            public void SetClosestCorner()
            {
                StartCoroutine(SetClosestCornerC());
            }

            private void Start()
            {
                SetClosestCorner();
            }
        }
    }
}

