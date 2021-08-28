using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using ECellDive.Utility;
using ECellDive.Utility.SettingsModels;
using ECellDive.IInteractions;
using ECellDive.INetworkComponents;
//using CytoscapeUnity.UI;


namespace ECellDive
{
    namespace NetworkComponents
    {
        public class NodeGO : LivingObject,
                                INodeGO, IHighlightable,
                                IFloatingDisplayable, IFixedDisplayable,
                                IPointerEnterHandler, IPointerExitHandler
        {
            public INode nodeData { get; protected set; }
            public string informationString { get; protected set; }

            public bool highlighted { get; protected set; }

            public GameObject refFloatingPlanel { get; protected set; }
            public GameObject refFixedPlanel { get; protected set; }

            public NodeGOSettings nodeGOSettings;

            //private SceneManager refSceneManager;
            private Renderer refRenderer;

            private void Awake()
            {
                highlighted = false;

                //refSceneManager = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<SceneManager>();
                //refFloatingPlanel = refSceneManager.informationPanels.FloatingInformationPanel;
                //refFixedPlanel = refSceneManager.informationPanels.FixedInformationPanel;

                refRenderer = GetComponent<Renderer>();
                refRenderer.sharedMaterial = new Material(nodeGOSettings.nodeShader);
            }
            
            public void SetNodeData(INode _INode)
            {
                nodeData = _INode;
                informationString = $"SUID: {nodeData.ID} \n" +
                                    $"name: {nodeData.NAME}";
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
                //refFloatingPlanel.GetComponent<FloatingPanelDisplay>().ActivateDisplay();
                //refFloatingPlanel.GetComponent<FloatingPanelDisplay>().ManagePosition();
                //refFloatingPlanel.GetComponent<FloatingPanelDisplay>().FillInDisplayContent(informationString);

            }
            public void DeactivateFloatingDisplay()
            {
                //refFloatingPlanel.GetComponent<FloatingPanelDisplay>().ClearDisplayContent();
                //refFloatingPlanel.GetComponent<FloatingPanelDisplay>().DeactivateDisplay();
            }
            #endregion

            #region - IFixedDisplayable -
            public void ActivateFixedDisplay()
            {
                //refFixedPlanel.GetComponent<PanelDisplay>().ClearDisplayContent();
                //refFixedPlanel.GetComponent<PanelDisplay>().FillInDisplayContent(informationString);
            }
            #endregion

            #region - IPointer X Handler -

            public void OnPointerEnter(PointerEventData eventData)
            {
                ManageHighlight();
                ActivateFloatingDisplay();
            }

            public void OnPointerExit(PointerEventData eventData)
            {
                ManageHighlight();
                DeactivateFloatingDisplay();
            }
            #endregion

            private void ManageHighlight()
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
        }
    }
}

