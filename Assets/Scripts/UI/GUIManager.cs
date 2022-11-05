using UnityEngine;
using UnityEngine.InputSystem;
using HSVPicker;
using ECellDive.Interfaces;
using ECellDive.PlayerComponents;
using ECellDive.Utility;

namespace ECellDive
{
    namespace UI
    {
        /// <summary>
        /// Logic to switch On and Off the main menu.
        /// </summary>
        public class GUIManager : MonoBehaviour
        {
            public InputActionReference refMenuOpen;
            
            public GameObject refMainMenu;
            public GameObject refVirtualKeyboard;
            public ColorPicker refColorPicker;
            public ModulesMenuManager refModulesMenuManager;
            public GroupsMakingUIManager refGroupsMakingUIManager;
            public GroupsMenu refGroupsMenu;
            public MultiplayerMenuManager refMultiplayerMenuManager;

            private void Awake()
            {
                refMenuOpen.action.performed += ManageOpenStatus;
            }

            private void OnDestroy()
            {
                refMenuOpen.action.performed -= ManageOpenStatus;
            }

            /// <summary>
            /// Initializes some values of actions associated to some GUI after 
            /// the <paramref name="_player"/> (local client) has been spawned.
            /// </summary>
            /// <param name="_player">The reference to the player class of
            /// the local client spawned in the scene.</param>
            /// <remarks>
            /// Called once in <see cref="Unity.Netcode.NetworkBehaviour.OnNetworkSpawn"/>
            /// of the local client (see code of <see cref="PlayerComponents.Player"/>).
            /// </remarks>
            public void Initialize(Player _player)
            {
                Transform graphicsHolder;
                while (gameObject.transform.childCount > 0)
                {
                    graphicsHolder = gameObject.transform.GetChild(0);
                    //Changing the parent of the UI Menus depending on their pinStatus
                    graphicsHolder.SetParent(StaticReferencer.Instance.refInternalObjectContainer.transform);

                    //Making sure they are looking at the camera from the start.
                    graphicsHolder.GetComponent<FaceCamera>().SetTargets(
                        _player.gameObject.GetComponentInChildren<Camera>().transform);
                }

                refGroupsMenu.Initialize();
                refMultiplayerMenuManager.Initialize();
                _player.GetComponentInChildren<GroupsMakingManager>().SetUIManager(refGroupsMakingUIManager);
            }

            /// <summary>
            /// Shows or hides the UI corresponding to the Main Menu.
            /// </summary>
            private void ManageOpenStatus(InputAction.CallbackContext callbackContext)
            {
                refMainMenu.SetActive(!refMainMenu.activeSelf);
                refMainMenu.transform.parent.GetComponent<IPopUp>().PopUp();
            }
        }
    }
}