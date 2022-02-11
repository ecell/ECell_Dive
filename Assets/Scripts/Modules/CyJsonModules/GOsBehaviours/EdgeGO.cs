﻿using UnityEngine;
using UnityEngine.InputSystem;
using ECellDive.Utility.SettingsModels;
using ECellDive.UI;
using ECellDive.IInteractions;
using ECellDive.INetworkComponents;


namespace ECellDive
{
    namespace Modules
    {
        //[RequireComponent(typeof(LineRenderer))]
        public class EdgeGO : Module,
                              IEdgeGO, IModulateFlux
        {
            #region - IEdgeGO Members -
            public IEdge edgeData { get; set; }
            public string informationString { get; protected set; }
            public float defaultStartWidth { get; protected set; }
            public float defaultEndWidth { get; protected set; }
            public LineRenderer refLineRenderer { get; protected set; }

            [SerializeField] private GameObject m_refBoxColliderHolder;
            public GameObject refBoxColliderHolder
            {
                get => m_refBoxColliderHolder;
                set => refBoxColliderHolder = m_refBoxColliderHolder;
            }
            #endregion

            #region - IModulateFlux Members -
            [SerializeField] private ControllersSymetricAction m_triggerKOActions;
            public ControllersSymetricAction triggerKOActions
            {
                get => m_triggerKOActions;
                set => triggerKOActions = m_triggerKOActions;
            }

            public bool knockedOut { get; protected set; }
            public float fluxLevel { get; protected set; }
            public float fluxLevelClamped { get; protected set; }
            #endregion

            public EdgeGOSettings edgeGOSettingsModels;

            public Color defaultColor;
            public Color highlightColor;

            private MaterialPropertyBlock mpb;
            private int colorID;
            private int activationID;
            private int panningSpeedID;

            private void Start()
            {
                triggerKOActions.leftController.action.performed += ManageKnockout;
                triggerKOActions.rightController.action.performed += ManageKnockout;

                knockedOut = false;
                fluxLevel = 0f;
                fluxLevelClamped = 1f;
            }

            private void OnEnable()
            {
                refLineRenderer = GetComponentInChildren<LineRenderer>();
                mpb = new MaterialPropertyBlock();
                colorID = Shader.PropertyToID("_Color");
                activationID = Shader.PropertyToID("_Activation");
                panningSpeedID = Shader.PropertyToID("_PanningSpeed");
                mpb.SetVector(colorID, defaultColor);
                refLineRenderer.SetPropertyBlock(mpb);
            }

            private void OnDestroy()
            {
                triggerKOActions.leftController.action.performed -= ManageKnockout;
                triggerKOActions.rightController.action.performed -= ManageKnockout;

//#if UNITY_EDITOR
//                refLineRenderer.sharedMaterial.SetFloat("Vector1_A68FF3D0", 0);
//#endif
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
                SetInformationString();
            }

            public void SetCollider(Transform _start, Transform _end)
            {
                m_refBoxColliderHolder.transform.localPosition = 0.5f * (_start.localPosition + _end.localPosition);
                m_refBoxColliderHolder.transform.LookAt(_end);
                m_refBoxColliderHolder.transform.localScale = new Vector3(
                                                                Mathf.Max(refLineRenderer.startWidth, refLineRenderer.endWidth),
                                                                Mathf.Max(refLineRenderer.startWidth, refLineRenderer.endWidth),
                                                                0.95f*Vector3.Distance(_start.localPosition, _end.localPosition));

            }

            public void SetLineRenderer()
            {
                refLineRenderer.startWidth = edgeGOSettingsModels.startWidthFactor * defaultStartWidth;
                refLineRenderer.endWidth = edgeGOSettingsModels.endWidthFactor * defaultEndWidth;
            }

            public void SetPosition(Transform _start, Transform _end)
            {
                refLineRenderer.SetPosition(0, _start.localPosition);
                refLineRenderer.SetPosition(1, _end.localPosition);
            }
            #endregion

            #region - IModulateFlux - 
            public void Activate()
            {
                knockedOut = false;
                SetInformationString();
                mpb.SetFloat(activationID, 1);
                refLineRenderer.SetPropertyBlock(mpb);
            }

            public void Knockout()
            {
                knockedOut = true;
                SetInformationString();
                mpb.SetFloat(activationID, 0);
                refLineRenderer.SetPropertyBlock(mpb);
            }

            public void SetFlux(float _level, float _levelClamped)
            {
                fluxLevel = _level;
                fluxLevelClamped = _levelClamped;
                SetInformationString();
                mpb.SetFloat(panningSpeedID, fluxLevel);
                refLineRenderer.SetPropertyBlock(mpb);
                UnsetHighlight();
            }
            #endregion

            #region - IHighlightable -
            public override void SetHighlight()
            {
                mpb.SetVector(colorID, highlightColor);
                refLineRenderer.SetPropertyBlock(mpb);
            }

            public override void UnsetHighlight()
            {
                mpb.SetVector(colorID, defaultColor);
                refLineRenderer.SetPropertyBlock(mpb);
            }
            #endregion

            public void Initialize(NetworkGO _masterPathway, IEdge _edge)
            {
                InstantiateInfoTags(new string[] { "" });
                SetEdgeData(_edge);
                gameObject.SetActive(true);
                gameObject.name = edgeData.NAME;

                SetDefaultWidth(1 / _masterPathway.networkGOSettingsModel.SizeScaleFactor,
                                1 / _masterPathway.networkGOSettingsModel.SizeScaleFactor);
                SetLineRenderer();

                Transform start = _masterPathway.NodeID_to_NodeGO[edgeData.source].transform;
                Transform target = _masterPathway.NodeID_to_NodeGO[edgeData.target].transform;
                SetPosition(start, target);
                SetCollider(start, target);
            }

            /// <summary>
            /// Public interface to call back on Unity Events to trigger
            /// on or off the highlight of the object.
            /// </summary>
            /// <remarks>Typically the highlight will be turned on when the
            /// pointer enters "hovering" and turned off when it exits
            /// "hovering".</remarks>
            public void ManageHighlight()
            {
                switch (isFocused)
                {
                    case true:
                        UnsetHighlight();
                        break;

                    case false:
                        SetHighlight();
                        break;
                }
            }

            /// <summary>
            /// The public interface to call back on Unity Events to knockout
            /// or activate a reaction represented by the edge.
            /// </summary>
            /// <remarks>Typically called back when the user presses a button
            /// while pointing at the edge.</remarks>
            public void ManageKnockout(InputAction.CallbackContext _ctx)
            {
                if (isFocused)
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

            /// <summary>
            /// The utility function to updates the information string.
            /// </summary>
            private void SetInformationString()
            {
                informationString = $"SUID: {edgeData.ID} \n" +
                                    $"Name: {edgeData.NAME} \n" +
                                    $"Knockedout: {knockedOut} \n" +
                                    $"Flux: {fluxLevel}";
                m_refInfoTags[0].GetComponent<InfoDisplayManager>().SetText(informationString);
            }
        }
    }
}
