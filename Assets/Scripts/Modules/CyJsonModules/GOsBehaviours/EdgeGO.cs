using UnityEngine;
using UnityEngine.InputSystem;
using ECellDive.Utility.SettingsModels;
using ECellDive.UI;
using ECellDive.IInteractions;
using ECellDive.INetworkComponents;


namespace ECellDive
{
    namespace Modules
    {
        [RequireComponent(typeof(LineRenderer))]
        public class EdgeGO : MonoBehaviour,
                                IEdgeGO, IHighlightable,
                                IFloatingDisplayable, IModulateFlux
        {
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
            public bool highlighted { get; protected set; }

            public bool floatingPanelDisplayed { get; protected set; }
            [SerializeField] private GameObject m_refFloatingPanel;
            public GameObject refFloatingPlanel
            {
                get => m_refFloatingPanel;
                set => refFloatingPlanel = m_refFloatingPanel;
            }
            [SerializeField] private InputActionReference m_refTriggerFloatingPlanel;
            public InputActionReference refTriggerFloatingPlanel
            {
                get => m_refTriggerFloatingPlanel;
                set => refTriggerFloatingPlanel = m_refTriggerFloatingPlanel;
            }

            [SerializeField] private InputActionReference m_refTriggerKO;
            public InputActionReference refTriggerKO
            {
                get => m_refTriggerKO;
                set => refTriggerKO = m_refTriggerKO;
            }

            public bool knockedOut { get; protected set; }
            public float fluxLevel { get; protected set; }

            public EdgeGOSettings edgeGOSettingsModels;

            private void Awake()
            {
                highlighted = false;

                floatingPanelDisplayed = false;
                m_refTriggerFloatingPlanel.action.performed += ManageFloatingDisplay;
                m_refTriggerKO.action.performed += ManageKnockout;

                knockedOut = false;
                fluxLevel = 0f;
            }

            public void Initialize(NetworkGO _masterPathway, IEdge _edge)
            {
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

            #region - IFloatingDisplayable -
            public void ActivateFloatingDisplay()
            {
                refFloatingPlanel.GetComponent<InfoDisplayManager>().SetVisibility(true);

            }
            public void DeactivateFloatingDisplay()
            {
                refFloatingPlanel.GetComponent<InfoDisplayManager>().SetVisibility(false);
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
                SetInformationString();
                refLineRenderer.sharedMaterial.SetInt("Vector1_22F9BCB6", 1);
            }

            public void Knockout()
            {
                knockedOut = true;
                SetInformationString();
                refLineRenderer.sharedMaterial.SetInt("Vector1_22F9BCB6", 0);
            }

            public void SetFlux(float _level)
            {
                fluxLevel = _level;
                SetInformationString();
                refLineRenderer.sharedMaterial.SetFloat("Vector1_A68FF3D0", _level);
            }
            #endregion

            /// <summary>
            /// Input call back action on which the floating display
            /// is turned on or off.
            /// </summary>
            /// <param name="_ctx">The input action callback context</param>
            private void ManageFloatingDisplay(InputAction.CallbackContext _ctx)
            {
                if (highlighted)
                {
                    floatingPanelDisplayed = !floatingPanelDisplayed;
                    if (floatingPanelDisplayed)
                    {
                        ActivateFloatingDisplay();
                    }
                    else
                    {
                        DeactivateFloatingDisplay();
                    }
                }
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

            /// <summary>
            /// The public interface to call back on Unity Events to knockout
            /// or activate a reaction represented by the edge.
            /// </summary>
            /// <remarks>Typically called back when the user presses a button
            /// while pointing at the edge.</remarks>
            public void ManageKnockout(InputAction.CallbackContext _ctx)
            {
                Debug.Log("Manage Knockout");
                if (highlighted)
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
                m_refFloatingPanel.GetComponent<InfoDisplayManager>().SetText(informationString);
            }

        }
    }
}
