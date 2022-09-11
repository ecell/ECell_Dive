using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using ECellDive.UI;
using ECellDive.Interfaces;

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

            private CyJsonModule refMasterPathway;

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
                //control direction of the edge if the flux is reversed
                if ((_previous < 0 && _current > 0) || (_previous > 0 && _current < 0))
                {
                    Vector3 startBuffer = m_LineRenderer.GetPosition(0);
                    m_LineRenderer.SetPosition(0, m_LineRenderer.GetPosition(1));
                    m_LineRenderer.SetPosition(1, startBuffer);
                    refParticleSystem.transform.localPosition = m_LineRenderer.GetPosition(0);
                    refParticleSystem.transform.LookAt(m_LineRenderer.GetPosition(1));
                }

                //Update emission rate
                emissionModule.rateOverTime = _current;
            }

            private void ApplyFLCChanges(float _previous, float _current)
            {
                SetInformationString();

                SetLineRendererWidth();

                UnsetHighlightServerRpc();
            }

            private void ApplyKOChanges(bool _previous, bool _current)
            {
                SetInformationString();
                mpb.SetFloat(activationID, knockedOut.Value? 0 : 1);
                m_LineRenderer.SetPropertyBlock(mpb);
            }

            public void Initialize(CyJsonModule _masterPathway, IEdge _edge)
            {
                refMasterPathway = _masterPathway;
                InstantiateInfoTags(new string[] { "" });
                SetEdgeData(_edge);
                gameObject.SetActive(true);
                gameObject.name = edgeData.name;
                SetName(edgeData.reaction_name);
                HideName();
                SetDefaultWidth(1 / refMasterPathway.cyJsonPathwaySettings.SizeScaleFactor,
                                1 / refMasterPathway.cyJsonPathwaySettings.SizeScaleFactor);

                SetLineRendererWidth();

                Transform start = refMasterPathway.DataID_to_DataGO[edgeData.source].transform;
                Transform target = refMasterPathway.DataID_to_DataGO[edgeData.target].transform;
                SetLineRendererPosition(start, target);
                SetCollider(start, target);

                //Particle system parameters
                shapeModule.scale = m_refBoxColliderHolder.transform.localScale;
                refParticleSystem.transform.position = start.position;
                refParticleSystem.transform.LookAt(target.position);
                mainModule.startLifetime = Vector3.Distance(start.position, target.position);
                

                m_nameTextFieldContainer.transform.position = 0.5f * (start.position + target.position) +
                                                                1 / refMasterPathway.cyJsonPathwaySettings.SizeScaleFactor * 1.5f * Vector3.up;
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
                fluxLevel.Value = _fluxValue;
                fluxLevelClamped.Value = _fluxClampedValue;
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
                UnsetHighlightServerRpc();
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
                UnsetHighlightServerRpc();
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
            public override void SetHighlight()
            {
                SpreadHighlightUpward();
                SpreadHighlightDownward();
            }
            
            public override void UnsetHighlight()
            {
                SpreadUnsetHighlightUpward();
                SpreadUnsetHighlightDownward();
            }
            #endregion

            #region - IEdgeGO Methods- 
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
                                                                Mathf.Max(m_LineRenderer.startWidth, m_LineRenderer.endWidth),
                                                                Mathf.Max(m_LineRenderer.startWidth, m_LineRenderer.endWidth),
                                                                0.95f*Vector3.Distance(_start.localPosition, _end.localPosition));
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
            public void SetFlux(float _level, float _levelClamped)
            {
                SetFluxValuesServerRpc(_level, _levelClamped);
            }
            #endregion            
        }
    }
}
