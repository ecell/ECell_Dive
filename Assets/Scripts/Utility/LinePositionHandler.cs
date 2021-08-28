using System;
using System.Collections;
using UnityEngine;

namespace ECellDive
{
    namespace Utility
    {
        /// <summary>
        /// Utility class to set the start and end positions of a LineRenderer.
        /// It is particularly usefull to update the connections between an
        /// info display and its module, node, edge, etc...
        /// </summary>
        public class LinePositionHandler : MonoBehaviour
        {
            [Serializable]
            public enum PositionScope { local, world, delta}
            public PositionScope startPositionScope;
            public Transform start;
            public PositionScope endPositionScope;
            public Transform end;
            public LineRenderer refLineRenderer;
            
            public void RefreshLineStartPosition()
            {
                switch (startPositionScope)
                {
                    case (PositionScope.local):
                        refLineRenderer.SetPosition(0, start.localPosition);
                        break;
                    case (PositionScope.world):
                        refLineRenderer.SetPosition(0, start.position);
                        break;
                    case (PositionScope.delta):
                        refLineRenderer.SetPosition(0, start.position - end.position);
                        break;
                }
                
            }

            public void RefreshLineEndPosition()
            {
                switch (endPositionScope)
                {
                    case (PositionScope.local):
                        refLineRenderer.SetPosition(1, end.localPosition);
                        break;
                    case (PositionScope.world):
                        refLineRenderer.SetPosition(1, end.position);
                        break;
                    case (PositionScope.delta):
                        refLineRenderer.SetPosition(1, end.position - start.position);
                        break;
                }
            }

            /// <summary>
            /// Performs both RefreshLineStartPosition() and 
            /// RefreshLineEndPosition().
            /// </summary>
            public void RefreshLinePositions()
            {
                RefreshLineStartPosition();
                RefreshLineEndPosition();
            }

            private IEnumerator Start()
            {
                yield return new WaitForEndOfFrame();
                RefreshLinePositions();
            }
        }
    }
}
