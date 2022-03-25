using UnityEngine;

namespace ECellDive
{
    namespace Utility
    {
        public class AnchoredContinousMoveHelper : MonoBehaviour
        {
            public GameObject refSphere;

            [Header("Line X")]
            public LineRenderer refXLineRenderer;
            public Gradient XLineNonValidGradient;
            public Gradient XLineValidGradient;

            [Header("Line Y")]
            public LineRenderer refYLineRenderer;
            public Gradient YLineNonValidGradient;
            public Gradient YLineValidGradient;

            [Header("Line Z")]
            public LineRenderer refZLineRenderer;
            public Gradient ZLineNonValidGradient;
            public Gradient ZLineValidGradient;

            private void Start()
            {
                refXLineRenderer.colorGradient = XLineNonValidGradient;
                refYLineRenderer.colorGradient = YLineNonValidGradient;
                refZLineRenderer.colorGradient = ZLineNonValidGradient;
            }

            /// <summary>
            /// Changes the color gradients of the lines to the valid ones
            /// if their respective length are above the thresholds.
            /// </summary>
            /// <param name="_linesLength">Length of line X at position _linesLength.x
            /// (same for Y and Z).</param>
            /// <param name="_thresholds">Threshold for line X at position _threshold.x
            /// (same for Y and Z).</param>
            public void CheckValidity(Vector3 _linesLength, Vector3 _thresholds)
            {
                if (Mathf.Abs(_linesLength.x) > _thresholds.x)
                {
                    refXLineRenderer.colorGradient = XLineValidGradient;
                }
                else
                {
                    refXLineRenderer.colorGradient = XLineNonValidGradient;
                }

                if (Mathf.Abs(_linesLength.y) > _thresholds.y)
                {
                    refYLineRenderer.colorGradient = YLineValidGradient;
                }
                else
                {
                    refYLineRenderer.colorGradient = YLineNonValidGradient;
                }

                if (Mathf.Abs(_linesLength.z) > _thresholds.z)
                {
                    refZLineRenderer.colorGradient = ZLineValidGradient;
                }
                else
                {
                    refZLineRenderer.colorGradient = ZLineNonValidGradient;
                }
            }

            /// <summary>
            /// Place the helper in front of the camera with only the rotation on the y axis
            /// </summary>
            public void FlatPositioning()
            {
                transform.position = Positioning.PlaceInFrontOfTargetLocal(Camera.main.transform, 0.5f, -0.2f);
                transform.LookAt(new Vector3(Camera.main.transform.position.x,
                                             transform.position.y,
                                             Camera.main.transform.position.z));
            }

            /// <summary>
            /// The lines have only 2 positions. We set the end of the line 
            /// with the positions at index 1 in the LineRenderer.
            /// </summary>
            /// <param name="_endPositions">_endPositions.x is the length of XLine
            /// along the X axis. it's the same for Y and Z.</param>
            public void SetLinesEndPositions(Vector3 _endPositions)
            {
                refXLineRenderer.SetPosition(1, _endPositions.x * Vector3.right);
                refYLineRenderer.SetPosition(1, _endPositions.y * Vector3.up);
                refZLineRenderer.SetPosition(1, _endPositions.z * Vector3.forward);
            }

            /// <summary>
            /// Sets the local scale of the sphere (which can be an ellipsoid).
            /// </summary>
            /// <param name="_scale"></param>
            public void SetSphereScale(Vector3 _scale)
            {
                refSphere.transform.localScale = _scale;
            }
        }
    }
}

