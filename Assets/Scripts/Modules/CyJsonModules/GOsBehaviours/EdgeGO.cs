using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using ECellDive.UI;
using ECellDive.Interfaces;
using TMPro;

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

            [SerializeField] private GameObject m_refBoxColliderHolder;
            public GameObject refBoxColliderHolder
            {
                get => m_refBoxColliderHolder;
                set => refBoxColliderHolder = m_refBoxColliderHolder;
            }
            #endregion

            #region - IModulateFlux Members -
            [SerializeField] private LeftRightData<InputActionReference> m_triggerKOActions;
            public LeftRightData<InputActionReference> triggerKOActions
            {
                get => m_triggerKOActions;
                set => m_triggerKOActions = value;
            }

            private NetworkVariable<bool> m_knockedOut = new NetworkVariable<bool>(false);
            public NetworkVariable<bool> knockedOut { get => m_knockedOut; protected set => m_knockedOut = value; }

            private NetworkVariable<float> m_fluxLevel = new NetworkVariable<float>();
            public NetworkVariable<float> fluxLevel { get => m_fluxLevel; protected set => m_fluxLevel = value; }

            private NetworkVariable<float> m_fluxLevelClamped = new NetworkVariable<float>();
            public NetworkVariable<float> fluxLevelClamped { get => m_fluxLevelClamped; protected set => m_fluxLevelClamped = value; }
            #endregion

            [Range(0, 1)] public float startWidthFactor = 0.25f;
            [Range(0, 1)] public float endWidthFactor = 0.75f;

            private int activationID;
            private ParticleSystem refParticleSystem;
            private ParticleSystem.MainModule mainModule;
            private ParticleSystem.EmissionModule emissionModule;
            private ParticleSystem.ShapeModule shapeModule;

            private IGraphGO refMasterPathway;

            protected override void Awake()
            {
                base.Awake();
                triggerKOActions.left.action.performed += ManageKnockout;
                triggerKOActions.right.action.performed += ManageKnockout;

                activationID = Shader.PropertyToID("_Activation");
                refParticleSystem = GetComponentInChildren<ParticleSystem>();
                mainModule = refParticleSystem.main;
                emissionModule = refParticleSystem.emission;
                shapeModule = refParticleSystem.shape;
            }

            public override void OnDestroy()
            {
                triggerKOActions.left.action.performed -= ManageKnockout;
                triggerKOActions.right.action.performed -= ManageKnockout;
            }

            public override void OnNetworkSpawn()
            {
                base.OnNetworkSpawn();

                fluxLevel.OnValueChanged += ApplyFLChanges;
                fluxLevelClamped.OnValueChanged += ApplyFLCChanges;
                knockedOut.OnValueChanged += ApplyKOChanges;
            }

            public override void OnNetworkDespawn()
            {
                base.OnNetworkDespawn();

                fluxLevel.OnValueChanged -= ApplyFLChanges;
                fluxLevelClamped.OnValueChanged -= ApplyFLCChanges;
                knockedOut.OnValueChanged -= ApplyKOChanges;
            }

            [ServerRpc(RequireOwnership = false)]
            private void ActivateServerRpc()
            {
                knockedOut.Value = false;
            }

            protected override void ApplyCurrentColorChange(Color _previous, Color _current)
            {
                mpb.SetVector(colorID, _current);
                m_LineRenderer.SetPropertyBlock(mpb);
            }

            private void ApplyFLChanges(float _previous, float _current)
            {
                ApplyFluxLevel();
            }

            private void ApplyFLCChanges(float _previous, float _current)
            {
                ApplyFluxLevelClamped();
            }

            private void ApplyKOChanges(bool _previous, bool _current)
            {
                SetInformationString();
                mpb.SetFloat(activationID, knockedOut.Value? 0 : 1);
                m_LineRenderer.SetPropertyBlock(mpb);
            }

            public void Initialize(CyJsonModule _masterPathway, IEdge _edge)
            {
#if UNITY_EDITOR
                m_LineRenderer = GetComponent<LineRenderer>();
                if (nameTextFieldContainer != null)
                {
                    nameField = nameTextFieldContainer?.GetComponentInChildren<TextMeshProUGUI>();
                }

                mpb = new MaterialPropertyBlock();
                colorID = Shader.PropertyToID("_Color");
                mpb.SetVector(colorID, defaultColor);
                m_LineRenderer.SetPropertyBlock(mpb);

                activationID = Shader.PropertyToID("_Activation");
                refParticleSystem = GetComponentInChildren<ParticleSystem>();
                mainModule = refParticleSystem.main;
                emissionModule = refParticleSystem.emission;
                shapeModule = refParticleSystem.shape;
#endif
                refMasterPathway = _masterPathway;
                InstantiateInfoTags(new string[] { "" });
                SetEdgeData(_edge);
                gameObject.SetActive(true);
                gameObject.name = $"{edgeData.ID}";
                SetName(edgeData.reaction_name);
                HideName();
                SetDefaultWidth(1 / refMasterPathway.graphScalingData.sizeScaleFactor,
                                1 / refMasterPathway.graphScalingData.sizeScaleFactor);

                SetLineRendererWidth();

                Transform start = refMasterPathway.DataID_to_DataGO[edgeData.source].transform;
                Transform target = refMasterPathway.DataID_to_DataGO[edgeData.target].transform;
                SetLineRendererPosition(start, target);
                SetCollider(start, target);

                //Particle system parameters
                emissionModule.rateOverTime = 0;
                shapeModule.scale = m_refBoxColliderHolder.transform.localScale;
                refParticleSystem.transform.position = start.position;
                refParticleSystem.transform.LookAt(target.position);
                mainModule.startLifetime = Vector3.Distance(start.position, target.position);
                

                m_nameTextFieldContainer.transform.position = 0.5f * (start.position + target.position) +
                                                                1 / refMasterPathway.graphScalingData.sizeScaleFactor * 1.5f * Vector3.up;
            }

            [ServerRpc(RequireOwnership = false)]
            private void KnockoutServerRpc()
            {
                knockedOut.Value = true;
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
                    if (knockedOut.Value)
                    {
                        Activate();
                    }

                    else
                    {
                        Knockout();
                    }
                }
            }

            /// <summary>
            /// The utility function to update the information string.
            /// </summary>
            private void SetInformationString()
            {
                informationString = $"SUID: {edgeData.ID} \n" +
                                    $"Name: {edgeData.name} \n" +
                                    $"Reaction: {edgeData.reaction_name} \n" +
                                    $"Knockedout: {knockedOut.Value} \n" +
                                    $"Flux: {fluxLevel.Value}";
                m_refInfoTagsContainer.transform.GetChild(0).GetComponent<InfoDisplayManager>().SetText(informationString);
            }

            [ServerRpc(RequireOwnership = false)]
            private void SetFluxValuesServerRpc(float _fluxValue, float _fluxClampedValue)
            {
                Debug.Log("SetFlux");
                fluxLevel.Value = _fluxValue;
                fluxLevelClamped.Value = _fluxClampedValue;
            }

            /// <summary>
            /// Sets the X and Y scale of the box collider relatively to the line renderer's width.
            /// </summary>
            private void SetColliderHeightWidth()
            {
                m_refBoxColliderHolder.transform.localScale = new Vector3(
                                                                0.33f * Mathf.Max(m_LineRenderer.startWidth, m_LineRenderer.endWidth),
                                                                0.33f * Mathf.Max(m_LineRenderer.startWidth, m_LineRenderer.endWidth),
                                                                m_refBoxColliderHolder.transform.localScale.z);
            }

            /// <summary>
            /// Sets the value for <see cref="refMasterPathway"/>.
            /// </summary>
            /// <param name="_masterPathway">The value for <see cref="refMasterPathway"/>.</param>
            public void SetRefMasterPathway(IGraphGO _masterPathway)
            {
                refMasterPathway = _masterPathway;
            }

            /// <summary>
            /// Spreads the activation state to every contiguous downstream edge that are part of the same reaction.
            /// </summary>
            public void SpreadActivationDownward()
            {
                ActivateServerRpc();

                GameObject targetNode = refMasterPathway.DataID_to_DataGO[edgeData.target];
                foreach (uint edgeID in targetNode.GetComponent<NodeGO>().nodeData.outgoingEdges)
                {
                    EdgeGO neighbourEdgeGo = refMasterPathway.DataID_to_DataGO[edgeID].GetComponent<EdgeGO>();
                    if (neighbourEdgeGo.edgeData.name == edgeData.name)
                    {
                        neighbourEdgeGo.SpreadActivationDownward();
                    }
                }

                foreach (uint edgeID in targetNode.GetComponent<NodeGO>().nodeData.incommingEdges)
                {
                    EdgeGO neighbourEdgeGo = refMasterPathway.DataID_to_DataGO[edgeID].GetComponent<EdgeGO>();
                    if (neighbourEdgeGo.edgeData.ID != edgeData.ID &&
                        neighbourEdgeGo.edgeData.name == edgeData.name)
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
                ActivateServerRpc();

                GameObject sourceNode = refMasterPathway.DataID_to_DataGO[edgeData.source];
                foreach (uint edgeID in sourceNode.GetComponent<NodeGO>().nodeData.incommingEdges)
                {
                    EdgeGO neighbourEdgeGo = refMasterPathway.DataID_to_DataGO[edgeID].GetComponent<EdgeGO>();
                    if (neighbourEdgeGo.edgeData.name == edgeData.name)
                    {
                        neighbourEdgeGo.SpreadActivationUpward();
                    }
                }

                foreach (uint edgeID in sourceNode.GetComponent<NodeGO>().nodeData.outgoingEdges)
                {
                    EdgeGO neighbourEdgeGo = refMasterPathway.DataID_to_DataGO[edgeID].GetComponent<EdgeGO>();
                    if (neighbourEdgeGo.edgeData.ID != edgeData.ID &&
                        neighbourEdgeGo.edgeData.name == edgeData.name)
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
                KnockoutServerRpc();

                GameObject targetNode = refMasterPathway.DataID_to_DataGO[edgeData.target];
                foreach (uint edgeID in targetNode.GetComponent<NodeGO>().nodeData.outgoingEdges)
                {
                    EdgeGO neighbourEdgeGo = refMasterPathway.DataID_to_DataGO[edgeID].GetComponent<EdgeGO>();
                    if (neighbourEdgeGo.edgeData.name == edgeData.name)
                    {
                        neighbourEdgeGo.SpreadKODownward();
                    }
                }

                foreach (uint edgeID in targetNode.GetComponent<NodeGO>().nodeData.incommingEdges)
                {
                    EdgeGO neighbourEdgeGo = refMasterPathway.DataID_to_DataGO[edgeID].GetComponent<EdgeGO>();
                    if (neighbourEdgeGo.edgeData.ID != edgeData.ID &&
                        neighbourEdgeGo.edgeData.name == edgeData.name)
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
                KnockoutServerRpc();

                GameObject sourceNode = refMasterPathway.DataID_to_DataGO[edgeData.source];
                foreach (uint edgeID in sourceNode.GetComponent<NodeGO>().nodeData.incommingEdges)
                {
                    EdgeGO neighbourEdgeGo = refMasterPathway.DataID_to_DataGO[edgeID].GetComponent<EdgeGO>();
                    if (neighbourEdgeGo.edgeData.name == edgeData.name)
                    {
                        neighbourEdgeGo.SpreadKOUpward();
                    }
                }

                foreach (uint edgeID in sourceNode.GetComponent<NodeGO>().nodeData.outgoingEdges)
                {
                    EdgeGO neighbourEdgeGo = refMasterPathway.DataID_to_DataGO[edgeID].GetComponent<EdgeGO>();
                    if (neighbourEdgeGo.edgeData.ID != edgeData.ID &&
                        neighbourEdgeGo.edgeData.name == edgeData.name)
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
                SetCurrentColorToHighlightServerRpc();
                refParticleSystem.gameObject.SetActive(true);
                refParticleSystem.Play();

                GameObject targetNode = refMasterPathway.DataID_to_DataGO[edgeData.target];
                foreach (uint edgeID in targetNode.GetComponent<NodeGO>().nodeData.outgoingEdges)
                {
                    EdgeGO neighbourEdgeGo = refMasterPathway.DataID_to_DataGO[edgeID].GetComponent<EdgeGO>();
                    if (neighbourEdgeGo.edgeData.name == edgeData.name)
                    {
                        neighbourEdgeGo.SpreadHighlightDownward();
                    }
                }

                foreach (uint edgeID in targetNode.GetComponent<NodeGO>().nodeData.incommingEdges)
                {
                    EdgeGO neighbourEdgeGo = refMasterPathway.DataID_to_DataGO[edgeID].GetComponent<EdgeGO>();
                    if (neighbourEdgeGo.edgeData.ID != edgeData.ID &&
                        neighbourEdgeGo.edgeData.name == edgeData.name)
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
                SetCurrentColorToHighlightServerRpc();
                refParticleSystem.gameObject.SetActive(true);
                refParticleSystem.Play();

                GameObject sourceNode = refMasterPathway.DataID_to_DataGO[edgeData.source];
                foreach (uint edgeID in sourceNode.GetComponent<NodeGO>().nodeData.incommingEdges)
                {
                    EdgeGO neighbourEdgeGo = refMasterPathway.DataID_to_DataGO[edgeID].GetComponent<EdgeGO>();
                    if (neighbourEdgeGo.edgeData.name == edgeData.name)
                    {
                        neighbourEdgeGo.SpreadHighlightUpward();
                    }
                }

                foreach (uint edgeID in sourceNode.GetComponent<NodeGO>().nodeData.outgoingEdges)
                {
                    EdgeGO neighbourEdgeGo = refMasterPathway.DataID_to_DataGO[edgeID].GetComponent<EdgeGO>();
                    if (neighbourEdgeGo.edgeData.ID != edgeData.ID &&
                        neighbourEdgeGo.edgeData.name == edgeData.name)
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
                SetCurrentColorToDefaultServerRpc();
                refParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);

                GameObject targetNode = refMasterPathway.DataID_to_DataGO[edgeData.target];
                foreach (uint edgeID in targetNode.GetComponent<NodeGO>().nodeData.outgoingEdges)
                {
                    EdgeGO neighbourEdgeGo = refMasterPathway.DataID_to_DataGO[edgeID].GetComponent<EdgeGO>();
                    if (neighbourEdgeGo.edgeData.name == edgeData.name)
                    {
                        neighbourEdgeGo.SpreadUnsetHighlightDownward();
                    }
                }

                foreach (uint edgeID in targetNode.GetComponent<NodeGO>().nodeData.incommingEdges)
                {
                    EdgeGO neighbourEdgeGo = refMasterPathway.DataID_to_DataGO[edgeID].GetComponent<EdgeGO>();
                    if (neighbourEdgeGo.edgeData.ID != edgeData.ID &&
                        neighbourEdgeGo.edgeData.name == edgeData.name)
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
                SetCurrentColorToDefaultServerRpc();
                refParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);

                GameObject sourceNode = refMasterPathway.DataID_to_DataGO[edgeData.source];
                foreach (uint edgeID in sourceNode.GetComponent<NodeGO>().nodeData.incommingEdges)
                {
                    EdgeGO neighbourEdgeGo = refMasterPathway.DataID_to_DataGO[edgeID].GetComponent<EdgeGO>();
                    if (neighbourEdgeGo.edgeData.name == edgeData.name)
                    {
                        neighbourEdgeGo.SpreadUnsetHighlightUpward();
                    }
                }

                foreach (uint edgeID in sourceNode.GetComponent<NodeGO>().nodeData.outgoingEdges)
                {
                    EdgeGO neighbourEdgeGo = refMasterPathway.DataID_to_DataGO[edgeID].GetComponent<EdgeGO>();
                    if (neighbourEdgeGo.edgeData.ID != edgeData.ID &&
                        neighbourEdgeGo.edgeData.name == edgeData.name)
                    {
                        neighbourEdgeGo.SpreadUnsetHighlightDownward();
                    }
                }
            }

            #region - IColorHighlightable Methods -

            public override void ApplyColor(Color _color)
            {
                mpb.SetVector(colorID, _color);
                if (m_LineRenderer != null)
                {
                    m_LineRenderer.SetPropertyBlock(mpb);
                }
            }

            public override void SetHighlight()
            {
                SpreadHighlightUpward();
                SpreadHighlightDownward();
            }
            
            public override void UnsetHighlight()
            {
                if (!forceHighlight)
                {
                    SpreadUnsetHighlightUpward();
                    SpreadUnsetHighlightDownward();
                }
            }
            #endregion

            #region - IEdgeGO Methods- 
            /// <inheritdoc/>
            public void ReverseOrientation()
            {
                Vector3 startBuffer = m_LineRenderer.GetPosition(0);
                m_LineRenderer.SetPosition(0, m_LineRenderer.GetPosition(1));
                m_LineRenderer.SetPosition(1, startBuffer);
                refParticleSystem.transform.localPosition = m_LineRenderer.GetPosition(0);
                refParticleSystem.transform.LookAt(m_LineRenderer.GetPosition(1));
            }

            /// <inheritdoc/>
            public void SetDefaultWidth(float _start, float _end)
            {
                defaultStartWidth = _start;
                defaultEndWidth = _end;
            }

            /// <inheritdoc/>
            public void SetEdgeData(IEdge _IEdge)
            {
                edgeData = _IEdge;
                SetInformationString();
            }

            /// <inheritdoc/>
            public void SetCollider(Transform _start, Transform _end)
            {
                m_refBoxColliderHolder.transform.localPosition = 0.5f * (_start.localPosition + _end.localPosition);
                m_refBoxColliderHolder.transform.LookAt(_end);
                m_refBoxColliderHolder.transform.localScale = new Vector3(
                                                                0.33f * Mathf.Max(m_LineRenderer.startWidth, m_LineRenderer.endWidth),//0.33f is custom for the inner size of the arrow texture
                                                                0.33f * Mathf.Max(m_LineRenderer.startWidth, m_LineRenderer.endWidth),//0.33f is custom for the inner size of the arrow texture
                                                                0.95f * Vector3.Distance(_start.localPosition, _end.localPosition));//0.95f is custom to avoid overlapping of the edge box collider with the nodes colliders
            }

            /// <inheritdoc/>
            public void SetLineRendererWidth()
            {
                m_LineRenderer.startWidth = startWidthFactor * Mathf.Max(defaultStartWidth, defaultStartWidth*fluxLevelClamped.Value);
                m_LineRenderer.endWidth = endWidthFactor * Mathf.Max(defaultEndWidth, defaultEndWidth*fluxLevelClamped.Value);
            }

            /// <inheritdoc/>
            public void SetLineRendererPosition(Transform _start, Transform _end)
            {
                m_LineRenderer.SetPosition(0, _start.localPosition);
                m_LineRenderer.SetPosition(1, _end.localPosition);
            }
            #endregion

            #region - IModulateFlux Methods- 

            /// <inheritdoc/>
            public void Activate()
            {
                SpreadActivationDownward();
                SpreadActivationUpward();
            }

            /// <inheritdoc/>
            public void Knockout()
            {
                SpreadKODownward();
                SpreadKOUpward();
            }

            /// <inheritdoc/>
            public void ApplyFluxLevel()
            {
                //Update emission rate
                emissionModule.rateOverTime = fluxLevel.Value;
            }

            /// <inheritdoc/>
            public void ApplyFluxLevelClamped()
            {
                SetInformationString();

                SetLineRendererWidth();

                SetColliderHeightWidth();
                shapeModule.scale = m_refBoxColliderHolder.transform.localScale;
            }

            /// <inheritdoc/>
            public void SetFlux(float _level, float _levelClamped)
            {
#if UNITY_EDITOR
                fluxLevel.Value = _level;
                fluxLevelClamped.Value = _levelClamped;
#else
                SetFluxValuesServerRpc(_level, _levelClamped);
#endif
            }
            #endregion
        }
    }
}
