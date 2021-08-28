using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECellDive.Utility;
using ECellDive.Utility.SettingsModels;
using ECellDive.IInteractions;
using ECellDive.INetworkComponents;


namespace ECellDive
{
    namespace NetworkComponents
    {
        [RequireComponent(typeof(LineRenderer))]
        public class EdgeGO : LivingObject, IEdgeGO, IHighlightable
        {
            public IEdge edgeData { get; set; }
            public float defaultStartWidth { get; protected set; }
            public float defaultEndWidth { get; protected set; }
            public LineRenderer refLineRenderer { get; protected set; }
            public bool highlighted { get; protected set; }

            public EdgeGOSettings edgeGOSettingsModels;

            private void Awake()
            {
                highlighted = false;
            }

            public void SetEdgeData(IEdge _IEdge)
            {
                edgeData = _IEdge;
            }

            public void SetDefaultWidth(float _start, float _end)
            {
                defaultStartWidth = _start;
                defaultEndWidth = _end;
            }

            public void SetPosition(Vector3 _start, Vector3 _end)
            {
                refLineRenderer.SetPosition(0, _start);
                refLineRenderer.SetPosition(1, _end);
            }

            public void SetHighlight()
            {
                refLineRenderer.sharedMaterial.SetInt("Vector1_22F9BCB6", 1);
                refLineRenderer.startWidth = edgeGOSettingsModels.startHWidthFactor * defaultStartWidth;
                refLineRenderer.endWidth = edgeGOSettingsModels.endHWidthFactor * defaultEndWidth;
                highlighted = true;
            }

            public void UnsetHighlight()
            {
                refLineRenderer.sharedMaterial.SetInt("Vector1_22F9BCB6", 0);
                refLineRenderer.startWidth = edgeGOSettingsModels.startWidthFactor * defaultStartWidth;
                refLineRenderer.endWidth = edgeGOSettingsModels.endWidthFactor * defaultEndWidth;
                highlighted = false;
            }

            public void ManageHighlight()
            {
                switch (highlighted)
                {
                    case true:
                        UnsetHighlight();
                        break;

                    case false:
                        SetHighlight();
                        break;
                }
            }

            public void SetLineRenderer()
            {
                refLineRenderer = GetComponent<LineRenderer>();
                refLineRenderer.sharedMaterial = new Material(edgeGOSettingsModels.edgeShader);
                refLineRenderer.startWidth = edgeGOSettingsModels.startWidthFactor * defaultStartWidth;
                refLineRenderer.endWidth = edgeGOSettingsModels.endWidthFactor * defaultEndWidth;
            }

        }
    }
}
