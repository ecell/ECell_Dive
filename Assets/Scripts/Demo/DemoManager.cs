using ECellDive.Interfaces;
using ECellDive.Modules;
using ECellDive.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

using ECellDive.PlayerComponents;

namespace ECellDive.Tutorials
{
    public class DemoManager : MonoBehaviour
    {
        [Header("References to actions")]
        [Tooltip("The action map will be globally disabled on initialization" +
            "and enabled on quit.")]
        public InputActionAsset refInputActionAsset;
        public InputActionReference refLeftSelect;
        public InputActionReference refLeftGrab;
        public InputActionReference refLeftOpenObjectMenu;
        public InputActionReference refRightTeleportation;


        [Tooltip("The action map will be globally disabled on initialization" +
            "and enabled on quit.")]
        public LeftRightData<InputActionReference> refInputSwitchMode;


        [Header("References to CyJsonModules")]
        public CyJsonModule cyJsModulePlain;
        public GameObject cyJsGoPlain;
        
        public CyJsonModule cyJsModuleGrouped;
        public GameObject cyJsGoGrouped;
        
        public CyJsonModule cyJsModuleGroupedFBA;
        public GameObject cyJsGoGroupedFBA;

        [Header("References to UI")]
        public GameObject shortDemoUiPanel;

        public UnityEvent initialization;
        public UnityEvent quit;

        private void Start()
        {
            Initialization();
        }

        public void Initialization()
        {
            initialization.Invoke();

            //Disable the General GUI
            StaticReferencer.Instance.refAllGuiMenusContainer.SetActive(false);

            //Attach/pin the local gui to the player
            shortDemoUiPanel.transform.SetParent(FindObjectOfType<Player>().transform);

            //Disable the RayBased Action Map
            refInputActionAsset.FindActionMap("Ray_Based_Controls_LH").Disable();
            refInputActionAsset.FindActionMap("Ray_Based_Controls_RH").Disable();

            //Disable the Movement Action Map
            refInputActionAsset.FindActionMap("Movement_LH").Disable();
            refInputActionAsset.FindActionMap("Movement_RH").Disable();

            //Disable the Groups Action Map
            refInputActionAsset.FindActionMap("Group_Controls_LH").Disable();
            refInputActionAsset.FindActionMap("Group_Controls_RH").Disable();

            //Disable the capacity to switch input action modes 
            refInputSwitchMode.left.action.Disable();
            refInputSwitchMode.right.action.Disable();

            //Remove the automatic activation of action map whenever the 
            //input switch mode action will be triggered in the future.
            //That way, we keep full control over progressive activation
            //of input actions during this tutorial.
            StaticReferencer.Instance.inputModeManager.UnsubscribeActionMapsSwitch();

            //Force the Ray-based controls for the left controller and the movement
            //controls for the right controller. This only impacts the visuals this
            //we unsubscribed the Action Map Switch above.
            StaticReferencer.Instance.inputModeManager.BroadcastLeftControllerModeServerRpc(0);
            StaticReferencer.Instance.inputModeManager.BroadcastRightControllerModeServerRpc(1);

            //Allow user to click on UI Buttons with the left controller.
            refLeftSelect.action.Enable();
            //Allow user to grab objects (the UI menu) with the left controller.
            refLeftGrab.action.Enable();
            //Allow user to open objects' menu with left controller.
            refLeftOpenObjectMenu.action.Enable();
            //Allow user to teleport with right controller
            refRightTeleportation.action.Enable();

            //Hide every InfoTags...
            foreach (GameObject _tag in StaticReferencer.Instance.refInfoTags)
            {
                _tag.SetActive(false);
            }

            //... but still show the InfoTags of the front trigger.
            StaticReferencer.Instance.refInfoTags[1].SetActive(true);//X
            StaticReferencer.Instance.refInfoTags[4].SetActive(true);//Left Front Trigger
            StaticReferencer.Instance.refInfoTags[5].SetActive(true);//Left Grab Trigger
            StaticReferencer.Instance.refInfoTags[9].SetActive(true);//Right Front Trigger

            //We make sure to synchronize the cyJsonModules with the gameobjects.
            //Supposedly we should have been able to better deal with this by 
            //correctly handling serialization aof the cyJsonModule and the graph.
            foreach(Transform _graphComponent in cyJsGoPlain.transform)
            {
                cyJsModulePlain.DataID_to_DataGO[System.Convert.ToUInt32(_graphComponent.name)] = _graphComponent.gameObject;
            }

            foreach(Transform _graphComponent in cyJsGoGrouped.transform)
            {
                cyJsModuleGrouped.DataID_to_DataGO[System.Convert.ToUInt32(_graphComponent.name)] = _graphComponent.gameObject;
            }

            foreach(Transform _graphComponent in cyJsGoGroupedFBA.transform)
            {
                cyJsModuleGroupedFBA.DataID_to_DataGO[System.Convert.ToUInt32(_graphComponent.name)] = _graphComponent.gameObject;
            }

            SetReferencing();

        }


        public void Quit()
        {
            quit.Invoke();

            //Resubscribe the Action map switch within the InputModeManager
            //in order to restore default behaviour: when the user presses
            //the button binded to switching controls input actions, the 
            //corresponding action maps are also automatically switched
            //on or off entirely and not just the actions we added to the
            //"learnedXXXActions" lists declared in this script.
            StaticReferencer.Instance.inputModeManager.SubscribeActionMapsSwitch();

            //Force the Ray-based controls for both controllers. This will
            //impact the visuals and the actions since we subscribed back to
            //the Action Map Switch above. Using the value -1 makes sure that
            //we get back to 0 with a change of value which will call the event
            //.OnValueChanged for leftControllerModeID and right ControllerModeID
            //in the InputModeManager.cs.
            StaticReferencer.Instance.inputModeManager.BroadcastLeftControllerModeServerRpc(-1);
            StaticReferencer.Instance.inputModeManager.BroadcastRightControllerModeServerRpc(-1);

            //Force back activation of every InfoTags
            foreach (GameObject _tag in StaticReferencer.Instance.refInfoTags)
            {
                _tag.SetActive(true);
            }
        }

        public void SetReferencing()
        {
            EdgeGO edgeGO;
            //Since the graph data will be the same for the 3 variations of the graph,
            //we can use the same loops to set the referencing for all.
            foreach (IEdge edge in cyJsModulePlain.graphData.edges)
            {
                edgeGO = cyJsModulePlain.DataID_to_DataGO[edge.ID].GetComponent<EdgeGO>();
                edgeGO.SetEdgeData(edge);
                edgeGO.SetRefMasterPathway(cyJsModulePlain);
                
                edgeGO = cyJsModuleGrouped.DataID_to_DataGO[edge.ID].GetComponent<EdgeGO>();
                edgeGO.SetEdgeData(edge);
                edgeGO.SetRefMasterPathway(cyJsModuleGrouped);
                
                edgeGO = cyJsModuleGroupedFBA.DataID_to_DataGO[edge.ID].GetComponent<EdgeGO>();
                edgeGO.SetEdgeData(edge);
                edgeGO.SetRefMasterPathway(cyJsModuleGroupedFBA);
            }

            foreach (INode node in cyJsModulePlain.graphData.nodes)
            {
                cyJsModulePlain.DataID_to_DataGO[node.ID].GetComponent<NodeGO>().SetNodeData(node);

                cyJsModuleGrouped.DataID_to_DataGO[node.ID].GetComponent<NodeGO>().SetNodeData(node);

                cyJsModuleGroupedFBA.DataID_to_DataGO[node.ID].GetComponent<NodeGO>().SetNodeData(node);
            }
        }
    }
}

