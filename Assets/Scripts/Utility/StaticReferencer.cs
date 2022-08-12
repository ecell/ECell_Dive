using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using HSVPicker;
using ECellDive.Interfaces;
using ECellDive.Input;
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
        [HideInInspector] public GameObject refAllGuiMenusContainer;
        [HideInInspector] public GameObject refVirtualKeyboard;
        [HideInInspector] public ColorPicker refColorPicker;
        [HideInInspector] public GroupsMenu refGroupsMenu;

        /// <summary>
        /// The list of all gameobjects representing the information tags of 
        /// the controllers' buttons. This is mainly used when an external code
        /// wants to forcefully deactivate/activate them (very relevant when
        /// designing tutorials).
        /// </summary>
        /// <remarks>
        /// In the order of the list we find:
        ///     - 0: Oculus Button IT
        ///     - 1: X Button IT
        ///     - 2: Y Button IT
        ///     - 3: Left Joystick IT
        ///     - 4: Left Trigger Front IT
        ///     - 5: Left Trigger Grip IT
        ///     - 6: A Button IT
        ///     - 7: B Button IT
        ///     - 8: Right Joystik IT
        ///     - 9: Right Trigger Front IT
        ///     - 10: Right Trigger Grip IT
        /// </remarks>
        public List<GameObject> refInfoTags;

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
        public LeftRightData<GameObject> riControllersGO;

        [Header("Other")]
        public InputModeManager inputModeManager;

        // Start is called before the first frame update
        public override void OnNetworkSpawn()
        {
            if (IsLocalPlayer)
            {
                Instance = this;
                GUIManager guiManager = FindObjectOfType<GUIManager>();
                refAllGuiMenusContainer = guiManager.gameObject;

                Instance.refVirtualKeyboard = guiManager.refVirtualKeyboard;
                Instance.refColorPicker = guiManager.refColorPicker;
                Instance.refGroupsMenu = guiManager.refGroupsMenu;

                guiManager.Initialize(GetComponent<PlayerComponents.Player>());
            }
        }
    }
}

