using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using ECellDive.Utility.SettingsModels;
using ECellDive.UI;
using ECellDive.IInteractions;
using ECellDive.INetworkComponents;

namespace ECellDive
{
    namespace Modules
    {
        public class NodeGO : MonoBehaviour,
                                INodeGO, IHighlightable,
                                IFloatingDisplayable
        {
            public INode nodeData { get; protected set; }
            public string informationString { get; protected set; }
            public bool highlighted { get; protected set; }

            [SerializeField] private GameObject m_refFloatingPanel;
            public GameObject refFloatingPlanel {
                get => m_refFloatingPanel;
                set => refFloatingPlanel = m_refFloatingPanel; }

            [SerializeField] private InputActionReference m_refTriggerFloatingPlanel;
            public InputActionReference refTriggerFloatingPlanel {
                get => m_refTriggerFloatingPlanel;
                set => refTriggerFloatingPlanel = m_refTriggerFloatingPlanel;
            }
            public bool floatingPanelDisplayed { get; protected set; }


            private Renderer refRenderer;
            private Material refSharedMaterial;

            private void Awake()
            {
                highlighted = false;

                floatingPanelDisplayed = false;
                m_refTriggerFloatingPlanel.action.performed += ManageFloatingDisplay;

                refRenderer = GetComponent<Renderer>();
                refSharedMaterial = refRenderer.sharedMaterial;
            }

            private void OnEnable()
            {
                m_refTriggerFloatingPlanel.action.Enable();
            }

            private void OnDisable()
            {
                m_refTriggerFloatingPlanel.action.Disable();
            }

            public void Initialize(NetworkGO _masterPathway, in INode _node)
            {
                SetNodeData(_node);
                Vector3 nodePos = new Vector3(nodeData.position.x,
                                              nodeData.position.z,
                                              nodeData.position.y) / _masterPathway.networkGOSettingsModel.PositionScaleFactor;
                gameObject.SetActive(true);
                gameObject.transform.position = nodePos;
                gameObject.transform.localScale /= _masterPathway.networkGOSettingsModel.SizeScaleFactor;
                gameObject.name = $"{nodeData.ID}";
                
                refFloatingPlanel.transform.localScale *= _masterPathway.networkGOSettingsModel.SizeScaleFactor;
            }

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

            public void SetNodeData(INode _INode)
            {
                nodeData = _INode;
                informationString = $"SUID: {nodeData.ID} \n" +
                                    $"name: {nodeData.NAME}";
                m_refFloatingPanel.GetComponent<InfoDisplayManager>().SetText(informationString);
            }

            #region - IHighlightable -
            public void SetHighlight()
            {
                refRenderer.material.SetFloat("Vector1_66D21324", 1);
                highlighted = true;
            }

            public void UnsetHighlight()
            {
                refRenderer.sharedMaterial = refSharedMaterial;
                highlighted = false;
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

        }
    }
}

