using UnityEngine;
using UnityEngine.InputSystem;
using ECellDive.Utility;


namespace ECellDive
{
    namespace UI
    {
        /// <summary>
        /// Logic to switch On and Off the main menu.
        /// </summary>
        public class MainMenuManager : MonoBehaviour
        {
            public InputActionReference refMenuOpen;
            public GameObject refMainMenu;

            private void Awake()
            {
                refMenuOpen.action.performed += ManageOpenStatus;
            }

            private void OnDestroy()
            {
                refMenuOpen.action.performed -= ManageOpenStatus;
            }

            /// <summary>
            /// Shows or hides the UI corresponding to the Main Menu.
            /// </summary>
            private void ManageOpenStatus(InputAction.CallbackContext callbackContext)
            {
                refMainMenu.SetActive(!refMainMenu.activeSelf);
            }
        }
    }
}

