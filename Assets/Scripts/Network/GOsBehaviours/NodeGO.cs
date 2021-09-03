using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using ECellDive.Utility;
using ECellDive.Utility.SettingsModels;
using ECellDive.UI;
using ECellDive.IInteractions;
using ECellDive.INetworkComponents;

//using CytoscapeUnity.UI;


namespace ECellDive
{
    namespace NetworkComponents
    {
        public class NodeGO : LivingObject,
                                INodeGO, IHighlightable,
                                IFloatingDisplayable//, IFixedDisplayable,
                                //IPointerEnterHandler, IPointerExitHandler
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

            //public GameObject refFixedPlanel { get; protected set; }

            public NodeGOSettings nodeGOSettings;

            private bool floatingPanelDisplayed = false;
            private Renderer refRenderer;

            private void Awake()
            {
                highlighted = false;

                m_refTriggerFloatingPlanel.action.performed += HandleActivationDisplay;

                refRenderer = GetComponent<Renderer>();
                refRenderer.sharedMaterial = new Material(nodeGOSettings.nodeShader);
            }

            private void HandleActivationDisplay(InputAction.CallbackContext _ctx)
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
                refRenderer.sharedMaterial.SetFloat("Vector1_66D21324", 1);
                highlighted = true;
            }

            public void UnsetHighlight()
            {
                refRenderer.sharedMaterial.SetFloat("Vector1_66D21324", 0);
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

