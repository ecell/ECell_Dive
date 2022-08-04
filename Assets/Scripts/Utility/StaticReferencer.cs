using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using HSVPicker;
using ECellDive.Interfaces;
using ECellDive.UI;


namespace ECellDive.Utility
{
    /// <summary>
    /// Singleton class referencing elements (GameObjects, Components,...) of the player
    /// that are needed by gameobjects outside it. Typically, an instanced module needs 
    /// access to the controller's interactors to correctly compute the movements when grabed.
    /// This class therefore exposes all the interactors to the ouside.
    /// </summary>
    /// <remarks>WARNING: This class is enabled only for the local player. Do not use this class
    /// to access elements that should be synchronized across the network.</remarks>
    public class StaticReferencer : NetworkBehaviour
    {
        public static StaticReferencer Instance;

        [Header("UI Elements")]
        public GameObject refVirtualKeyboard;
        public ColorPicker refColorPicker;
        public GroupsMenu refGroupsMenu;

        [Header("XR Interactors References")]
        public LeftRightData<XRRayInteractor> groupsInteractors;
        public LeftRightData<XRRayInteractor> remoteGrabInteractors;
        public LeftRightData<XRRayInteractor> remoteInteractionInteractors;
        public LeftRightData<XRRayInteractor> mainPointerInteractors;

        [Header("XR Action-Based Controller References")]
        public LeftRightData<ActionBasedController> remoteInteractionABC;

        [Header("Controllers GO References")]
        public LeftRightData<GameObject> mvtControllersGO;
        public LeftRightData<GameObject> groupControllersGO;

        // Start is called before the first frame update
        public override void OnNetworkSpawn()
        {
            if (IsLocalPlayer)
            {
                Instance = this;
                GUIManager guiManager = FindObjectOfType<GUIManager>();

                Instance.refVirtualKeyboard = guiManager.refVirtualKeyboard;
                Instance.refColorPicker = guiManager.refColorPicker;
                Instance.refGroupsMenu = guiManager.refGroupsMenu;

                guiManager.Initialize(GetComponent<PlayerComponents.Player>());
            }
        }
    }
}

