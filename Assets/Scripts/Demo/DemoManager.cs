using Newtonsoft.Json.Linq;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Unity.Netcode;

using ECellDive.CustomEditors;
using ECellDive.GraphComponents;
using ECellDive.IO;
using ECellDive.Interfaces;
using ECellDive.Modules;
using ECellDive.PlayerComponents;
using ECellDive.SceneManagement;
using ECellDive.Utility;
using ECellDive.Utility.Data;

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
        public CyJsonModule cyJsonModule;
        public TextAsset cyJsonGraphData;
        public ColorDataSerializer cyJsonColorData;
        public FluxDataSerializer cyJsonFluxData;

        [Header("References to UI")]
        public GameObject shortDemoUiPanel;

        public UnityEvent initialization;
        public UnityEvent quit;

        private bool previousStateEOC;
        private bool previousStateIOC;

        private void Start()
        {
            Initialization();
        }

        public void Initialization()
        {
            initialization.Invoke();

            //Disable the General GUI
            previousStateEOC = StaticReferencer.Instance.refExternalObjectContainer.activeSelf;
            previousStateIOC = StaticReferencer.Instance.refExternalObjectContainer.activeSelf;
            StaticReferencer.Instance.refExternalObjectContainer.SetActive(false);
            StaticReferencer.Instance.refInternalObjectContainer.SetActive(false);

            //Attach/pin the local gui to the player
            shortDemoUiPanel.transform.SetParent(FindObjectOfType<Player>().transform);
            shortDemoUiPanel.transform.localPosition = new Vector3(-1f, 0.8f, 1.5f);
            shortDemoUiPanel.GetComponent<FaceCamera>().LookAt();

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
            //Allow user to open/close the object menu with the left controller.
            refLeftOpenObjectMenu.action.Enable();
            //Allow user to teleport with right controller
            refRightTeleportation.action.Enable();

            //Hide every InfoTags...
            foreach (GameObject _tag in StaticReferencer.Instance.refInfoTags)
            {
                _tag.SetActive(false);
            }

            //... but still show the InfoTags of the front trigger.
            StaticReferencer.Instance.refInfoTags[4].SetActive(true);//Left Front Trigger
            StaticReferencer.Instance.refInfoTags[5].SetActive(true);//Left Grab Trigger
            StaticReferencer.Instance.refInfoTags[1].SetActive(true);//X Button
            StaticReferencer.Instance.refInfoTags[9].SetActive(true);//Right Front Trigger

            GenerateDemoGraph();
        }

        public void GenerateDemoGraph()
        {
            JObject graphData = JObject.Parse(cyJsonGraphData.text);
            //Loading the file
            CyJsonPathway pathway = CyJsonPathwayLoader.Initiate(graphData,
                                                                cyJsonGraphData.name);

            //Instantiating relevant data structures to store the information about
            //the layers, nodes and edges.
            CyJsonPathwayLoader.Populate(pathway);
            //CyJsonModulesData.AddData(pathway);

            cyJsonModule.SetNetworkData(pathway);
            cyJsonModule.isReadyForGeneration.Value = true;
            cyJsonModule.TryDiveIn();            
        }


        public void Quit()
        {
            //Resubscribe the Action map switch within the InputModeManager
            //in order to restore default behaviour: when the user presses
            //the button binded to switching controls input actions, the 
            //corresponding action maps are also automatically switched
            //on or off entirely and not just the actions we added to the
            //"learnedXXXActions" lists declared in this script.
            StaticReferencer.Instance.inputModeManager.SubscribeActionMapsSwitch();

            //Re-enable the capacity to switch input action modes 
            refInputSwitchMode.left.action.Enable();
            refInputSwitchMode.right.action.Enable();

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

            //Since we pinned the UiPanel to the Player, we need to destroy manually on Quit
            Destroy(shortDemoUiPanel);

            //Change scenes back to the initial one.
            FindObjectOfType<DiveScenesManager>().ResurfaceServerRpc(NetworkManager.Singleton.LocalClientId);

            //Revert the active state of the UI and modules that may have been imported.
            StaticReferencer.Instance.refExternalObjectContainer.SetActive(previousStateEOC);
            StaticReferencer.Instance.refInternalObjectContainer.SetActive(previousStateIOC);

            quit.Invoke();
        }

        public void ResetNetwork()
        {
            IColorHighlightableNet cHN;
            foreach (ColorData _cd in cyJsonColorData.data)
            {
                cHN = cyJsonModule.DataID_to_DataGO[_cd.targetGoID].GetComponent<IColorHighlightableNet>();
                cHN.defaultColor = Color.white;
                cHN.ApplyColor(Color.white);
            }

            EdgeGO edgeGO;
            foreach (FluxData _fd in cyJsonFluxData.data)
            {
                edgeGO = cyJsonModule.DataID_to_DataGO[_fd.targetGoID].GetComponent<EdgeGO>();
                edgeGO.SetFlux(0f, 0f);
            }
        }

        public void ShowGroups()
        {
            IColorHighlightableNet cHN;
            foreach (ColorData _cd in cyJsonColorData.data)
            {
                cHN = cyJsonModule.DataID_to_DataGO[_cd.targetGoID].GetComponent<IColorHighlightableNet>();
                cHN.defaultColor = _cd.color;
                cHN.ApplyColor(_cd.color);
            }
        }

        public void ShowFBA()
        {
            EdgeGO edgeGO;
            foreach (FluxData _fd in cyJsonFluxData.data)
            {
                edgeGO = cyJsonModule.DataID_to_DataGO[_fd.targetGoID].GetComponent<EdgeGO>();
                edgeGO.SetFlux(_fd.fluxLevel, _fd.fluxLevelClamped);
            }
        }
    }
}

