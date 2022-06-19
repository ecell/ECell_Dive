using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using ECellDive.Utility.SettingsModels;
using ECellDive.UI;
using ECellDive.Interfaces;
using ECellDive.SceneManagement;


namespace ECellDive
{
    namespace Modules
    {
        public class EdgeGO : GameNetModule,
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

            [Range(0, 1)] public float startWidthFactor = 0.25f;
            [Range(0, 1)] public float endWidthFactor = 0.75f;

            private MaterialPropertyBlock mpb;
            private int colorID;
            private int activationID;
            private int panningSpeedID;

            private CyJsonModule refMasterPathway;

            protected override void Awake()
            {
                base.Awake();
                triggerKOActions.leftController.action.performed += ManageKnockout;
                triggerKOActions.rightController.action.performed += ManageKnockout;
            }

            private void Start()
            {
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

            public override void OnDestroy()
            {
                triggerKOActions.leftController.action.performed -= ManageKnockout;
                triggerKOActions.rightController.action.performed -= ManageKnockout;
            }

            public void Initialize(CyJsonModule _masterPathway, IEdge _edge)
            {
                refMasterPathway = _masterPathway;
                InstantiateInfoTags(new string[] { "" });
                SetEdgeData(_edge);
                gameObject.SetActive(true);
                gameObject.name = edgeData.NAME;

                SetDefaultWidth(1 / refMasterPathway.cyJsonPathwaySettings.SizeScaleFactor,
                                1 / refMasterPathway.cyJsonPathwaySettings.SizeScaleFactor);
                SetLineRendererWidth();

                Transform start = refMasterPathway.DataID_to_DataGO[edgeData.source].transform;
                Transform target = refMasterPathway.DataID_to_DataGO[edgeData.target].transform;
                SetLineRendererPosition(start, target);
                SetCollider(start, target);
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
                    if (knockedOut)
                    {
                        SpreadActivationDownward();
                        SpreadActivationUpward();
                    }

                    else
                    {
                        SpreadKODownward();
                        SpreadKOUpward();
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

            /// <summary>
            /// Spreads the activation state to every contiguous downstream edge that are part of the same reaction.
            /// </summary>
            public void SpreadActivationDownward()
            {
                Activate();

                GameObject targetNode = refMasterPathway.DataID_to_DataGO[edgeData.target];
                foreach (int edgeID in targetNode.GetComponent<NodeGO>().nodeData.outgoingEdges)
                {
                    EdgeGO neighbourEdgeGo = refMasterPathway.DataID_to_DataGO[edgeID].GetComponent<EdgeGO>();
                    if (neighbourEdgeGo.edgeData.NAME == edgeData.NAME)
                    {
                        neighbourEdgeGo.SpreadActivationDownward();
                    }
                }

                foreach (int edgeID in targetNode.GetComponent<NodeGO>().nodeData.incommingEdges)
                {
                    EdgeGO neighbourEdgeGo = refMasterPathway.DataID_to_DataGO[edgeID].GetComponent<EdgeGO>();
                    if (neighbourEdgeGo.edgeData.ID != edgeData.ID &&
                        neighbourEdgeGo.edgeData.NAME == edgeData.NAME)
                    {
                        neighbourEdgeGo.SpreadActivationUpward();
                    }
                }
            }

            /// <summary>
            /// Spreads the activation state to every contiguous upstream edge that are part of the same reaction.
            /// </summary>
            public void SpreadActivationUpward()
            {
                Activate();

                GameObject sourceNode = refMasterPathway.DataID_to_DataGO[edgeData.source];
                foreach (int edgeID in sourceNode.GetComponent<NodeGO>().nodeData.incommingEdges)
                {
                    EdgeGO neighbourEdgeGo = refMasterPathway.DataID_to_DataGO[edgeID].GetComponent<EdgeGO>();
                    if (neighbourEdgeGo.edgeData.NAME == edgeData.NAME)
                    {
                        neighbourEdgeGo.SpreadActivationUpward();
                    }
                }

                foreach (int edgeID in sourceNode.GetComponent<NodeGO>().nodeData.outgoingEdges)
                {
                    EdgeGO neighbourEdgeGo = refMasterPathway.DataID_to_DataGO[edgeID].GetComponent<EdgeGO>();
                    if (neighbourEdgeGo.edgeData.ID != edgeData.ID &&
                        neighbourEdgeGo.edgeData.NAME == edgeData.NAME)
                    {
                        neighbourEdgeGo.SpreadActivationDownward();
                    }
                }
            }

            /// <summary>
            /// Spreads the Knockout state to every contiguous downstream edge that are part of the same reaction.
            /// </summary>
            public void SpreadKODownward()
            {
                Knockout();

                GameObject targetNode = refMasterPathway.DataID_to_DataGO[edgeData.target];
                foreach (int edgeID in targetNode.GetComponent<NodeGO>().nodeData.outgoingEdges)
                {
                    EdgeGO neighbourEdgeGo = refMasterPathway.DataID_to_DataGO[edgeID].GetComponent<EdgeGO>();
                    if (neighbourEdgeGo.edgeData.NAME == edgeData.NAME)
                    {
                        neighbourEdgeGo.SpreadKODownward();
                    }
                }

                foreach (int edgeID in targetNode.GetComponent<NodeGO>().nodeData.incommingEdges)
                {
                    EdgeGO neighbourEdgeGo = refMasterPathway.DataID_to_DataGO[edgeID].GetComponent<EdgeGO>();
                    if (neighbourEdgeGo.edgeData.ID != edgeData.ID &&
                        neighbourEdgeGo.edgeData.NAME == edgeData.NAME)
                    {
                        neighbourEdgeGo.SpreadKOUpward();
                    }
                }
            }

            /// <summary>
            /// Spreads the Knockout state to every contiguous upstream edge that are part of the same reaction.
            /// </summary>
            public void SpreadKOUpward()
            {
                Knockout();

                GameObject sourceNode = refMasterPathway.DataID_to_DataGO[edgeData.source];
                foreach (int edgeID in sourceNode.GetComponent<NodeGO>().nodeData.incommingEdges)
                {
                    EdgeGO neighbourEdgeGo = refMasterPathway.DataID_to_DataGO[edgeID].GetComponent<EdgeGO>();
                    if (neighbourEdgeGo.edgeData.NAME == edgeData.NAME)
                    {
                        neighbourEdgeGo.SpreadKOUpward();
                    }
                }

                foreach (int edgeID in sourceNode.GetComponent<NodeGO>().nodeData.outgoingEdges)
                {
                    EdgeGO neighbourEdgeGo = refMasterPathway.DataID_to_DataGO[edgeID].GetComponent<EdgeGO>();
                    if (neighbourEdgeGo.edgeData.ID != edgeData.ID &&
                        neighbourEdgeGo.edgeData.NAME == edgeData.NAME)
                    {
                        neighbourEdgeGo.SpreadKODownward();
                    }
                }
            }

            /// <summary>
            /// Spreads the highlighted state to every contiguous downstream edge that are part of the same reaction.
            /// </summary>
            public void SpreadHighlightDownward()
            {
                SetHighlight();

                GameObject targetNode = refMasterPathway.DataID_to_DataGO[edgeData.target];
                foreach (int edgeID in targetNode.GetComponent<NodeGO>().nodeData.outgoingEdges)
                {
                    EdgeGO neighbourEdgeGo = refMasterPathway.DataID_to_DataGO[edgeID].GetComponent<EdgeGO>();
                    if (neighbourEdgeGo.edgeData.NAME == edgeData.NAME)
                    {
                        neighbourEdgeGo.SpreadHighlightDownward();
                    }
                }

                foreach (int edgeID in targetNode.GetComponent<NodeGO>().nodeData.incommingEdges)
                {
                    EdgeGO neighbourEdgeGo = refMasterPathway.DataID_to_DataGO[edgeID].GetComponent<EdgeGO>();
                    if (neighbourEdgeGo.edgeData.ID != edgeData.ID &&
                        neighbourEdgeGo.edgeData.NAME == edgeData.NAME)
                    {
                        neighbourEdgeGo.SpreadHighlightUpward();
                    }
                }
            }

            /// <summary>
            /// Spreads the highlighted state to every contiguous upstream edge that are part of the same reaction.
            /// </summary>
            public void SpreadHighlightUpward()
            {
                SetHighlight();

                GameObject sourceNode = refMasterPathway.DataID_to_DataGO[edgeData.source];
                foreach (int edgeID in sourceNode.GetComponent<NodeGO>().nodeData.incommingEdges)
                {
                    EdgeGO neighbourEdgeGo = refMasterPathway.DataID_to_DataGO[edgeID].GetComponent<EdgeGO>();
                    if (neighbourEdgeGo.edgeData.NAME == edgeData.NAME)
                    {
                        neighbourEdgeGo.SpreadHighlightUpward();
                    }
                }

                foreach (int edgeID in sourceNode.GetComponent<NodeGO>().nodeData.outgoingEdges)
                {
                    EdgeGO neighbourEdgeGo = refMasterPathway.DataID_to_DataGO[edgeID].GetComponent<EdgeGO>();
                    if (neighbourEdgeGo.edgeData.ID != edgeData.ID &&
                        neighbourEdgeGo.edgeData.NAME == edgeData.NAME)
                    {
                        neighbourEdgeGo.SpreadHighlightDownward();
                    }
                }
            }

            /// <summary>
            /// Spreads the unhighlighted state to every contiguous upstream edge that are part of the same reaction.
            /// </summary>
            public void SpreadUnsetHighlightDownward()
            {
                UnsetHighlight();

                GameObject targetNode = refMasterPathway.DataID_to_DataGO[edgeData.target];
                foreach (int edgeID in targetNode.GetComponent<NodeGO>().nodeData.outgoingEdges)
                {
                    EdgeGO neighbourEdgeGo = refMasterPathway.DataID_to_DataGO[edgeID].GetComponent<EdgeGO>();
                    if (neighbourEdgeGo.edgeData.NAME == edgeData.NAME)
                    {
                        neighbourEdgeGo.SpreadUnsetHighlightDownward();
                    }
                }

                foreach (int edgeID in targetNode.GetComponent<NodeGO>().nodeData.incommingEdges)
                {
                    EdgeGO neighbourEdgeGo = refMasterPathway.DataID_to_DataGO[edgeID].GetComponent<EdgeGO>();
                    if (neighbourEdgeGo.edgeData.ID != edgeData.ID &&
                        neighbourEdgeGo.edgeData.NAME == edgeData.NAME)
                    {
                        neighbourEdgeGo.SpreadUnsetHighlightUpward();
                    }
                }
            }

            /// <summary>
            /// Spreads the unhighlighted state to every contiguous downstream edge that are part of the same reaction.
            /// </summary>
            public void SpreadUnsetHighlightUpward()
            {
                UnsetHighlight();

                GameObject sourceNode = refMasterPathway.DataID_to_DataGO[edgeData.source];
                foreach (int edgeID in sourceNode.GetComponent<NodeGO>().nodeData.incommingEdges)
                {
                    EdgeGO neighbourEdgeGo = refMasterPathway.DataID_to_DataGO[edgeID].GetComponent<EdgeGO>();
                    if (neighbourEdgeGo.edgeData.NAME == edgeData.NAME)
                    {
                        neighbourEdgeGo.SpreadUnsetHighlightUpward();
                    }
                }

                foreach (int edgeID in sourceNode.GetComponent<NodeGO>().nodeData.outgoingEdges)
                {
                    EdgeGO neighbourEdgeGo = refMasterPathway.DataID_to_DataGO[edgeID].GetComponent<EdgeGO>();
                    if (neighbourEdgeGo.edgeData.ID != edgeData.ID &&
                        neighbourEdgeGo.edgeData.NAME == edgeData.NAME)
                    {
                        neighbourEdgeGo.SpreadUnsetHighlightDownward();
                    }
                }
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

            public void SetLineRendererWidth()
            {
                refLineRenderer.startWidth = startWidthFactor * defaultStartWidth;
                refLineRenderer.endWidth = endWidthFactor * defaultEndWidth;
            }

            public void SetLineRendererPosition(Transform _start, Transform _end)
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
                //mpb.SetFloat(panningSpeedID, _levelClamped);
                //refLineRenderer.SetPropertyBlock(mpb);
                defaultStartWidth = Mathf.Max(1 / refMasterPathway.cyJsonPathwaySettings.SizeScaleFactor, _levelClamped);
                endWidthFactor = Mathf.Max(1 / refMasterPathway.cyJsonPathwaySettings.SizeScaleFactor, _levelClamped);
                SetLineRendererWidth();
                UnsetHighlight();
            }
            #endregion

            #region - IHighlightable -

            public override void SetDefaultColor(Color _c)
            {
                base.SetDefaultColor(_c);
                mpb.SetVector(colorID, defaultColor);
                refLineRenderer.SetPropertyBlock(mpb);
            }

            public override void SetHighlightColor(Color _c)
            {
                base.SetDefaultColor(_c);
                mpb.SetVector(colorID, highlightColor);
                refLineRenderer.SetPropertyBlock(mpb);
            }

            public override void SetHighlight()
            {
                mpb.SetVector(colorID, highlightColor);
                refLineRenderer.SetPropertyBlock(mpb);
            }

            public override void UnsetHighlight()
            {
                if (!forceHighlight)
                {
                    mpb.SetVector(colorID, defaultColor);
                    refLineRenderer.SetPropertyBlock(mpb);
                }
            }
            #endregion

            
        }
    }
}
