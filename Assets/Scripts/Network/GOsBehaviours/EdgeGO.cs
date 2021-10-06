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
        public class EdgeGO : LivingObject, IEdgeGO, IHighlightable, IModulateFlux
        {
            public IEdge edgeData { get; set; }
            public float defaultStartWidth { get; protected set; }
            public float defaultEndWidth { get; protected set; }
            public LineRenderer refLineRenderer { get; protected set; }

            [SerializeField] private GameObject m_refBoxColliderHolder;
            public GameObject refBoxColliderHolder
            {
                get => refBoxColliderHolder;
                set => refBoxColliderHolder = m_refBoxColliderHolder;
            }
            public bool highlighted { get; protected set; }

            public bool knockedOut { get; protected set; }

            public EdgeGOSettings edgeGOSettingsModels;

            private void Awake()
            {
                highlighted = false;
            }
            #region - IEdgeGO - 
            public void SetDefaultWidth(float _start, float _end)
            {
                defaultStartWidth = _start;
                defaultEndWidth = _end;
            }

            public void SetEdgeData(IEdge _IEdge)
            {
                edgeData = _IEdge;
            }

            public void SetLineCollider(Transform _start, Transform _end)
            {
                m_refBoxColliderHolder.transform.localPosition = 0.5f * (_start.localPosition + _end.localPosition);
                m_refBoxColliderHolder.transform.LookAt(_end);
                m_refBoxColliderHolder.transform.localScale = new Vector3(
                                                                Mathf.Max(refLineRenderer.startWidth, refLineRenderer.endWidth),
                                                                Mathf.Max(refLineRenderer.startWidth, refLineRenderer.endWidth),
                                                                Vector3.Distance(_start.localPosition, _end.localPosition));

            }

            public void SetLineRenderer()
            {
                refLineRenderer = GetComponent<LineRenderer>();
                refLineRenderer.sharedMaterial = new Material(edgeGOSettingsModels.edgeShader);
                refLineRenderer.startWidth = edgeGOSettingsModels.startWidthFactor * defaultStartWidth;
                refLineRenderer.endWidth = edgeGOSettingsModels.endWidthFactor * defaultEndWidth;
            }

            public void SetPosition(Transform _start, Transform _end)
            {
                refLineRenderer.SetPosition(0, _start.localPosition);
                refLineRenderer.SetPosition(1, _end.localPosition);
            }

            #endregion

            #region - IHighlightable -
            public void SetHighlight()
            {
                refLineRenderer.startWidth = edgeGOSettingsModels.startHWidthFactor * defaultStartWidth;
                refLineRenderer.endWidth = edgeGOSettingsModels.endHWidthFactor * defaultEndWidth;
                highlighted = true;
            }

            public void UnsetHighlight()
            {
                refLineRenderer.startWidth = edgeGOSettingsModels.startWidthFactor * defaultStartWidth;
                refLineRenderer.endWidth = edgeGOSettingsModels.endWidthFactor * defaultEndWidth;
                highlighted = false;
            }
            #endregion

            #region - IModulateFlux - 
            public void Activate()
            {
                knockedOut = false;
                refLineRenderer.sharedMaterial.SetInt("Vector1_22F9BCB6", 1);
            }

            public void Knockout()
            {
                knockedOut = true;
                refLineRenderer.sharedMaterial.SetInt("Vector1_22F9BCB6", 0);
            }

            public void SetFlux(float _level)
            {
                refLineRenderer.sharedMaterial.SetFloat("Vector1_A68FF3D0", _level);
            }
            #endregion

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

            public void ManageKnockout()
            {
                switch (knockedOut)
                {
                    case true:
                        Activate();
                        break;

                    case false:
                        Knockout();
                        break;
                }
            }

        }
    }
}
