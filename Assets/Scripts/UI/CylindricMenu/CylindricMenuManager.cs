using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using ECellDive.Utility;


namespace ECellDive
{
    namespace UI
    {
        /// <summary>
        /// Logic to switch On and Off the cylindric menu.
        /// </summary>
        public class CylindricMenuManager : MonoBehaviour
        {
            public InputActionReference refMenuOpen;
            public GameObject refOptionMenu;

            private void Awake()
            {
                refMenuOpen.action.performed += OpenCloseOptionMenu;
            }

            private void OnEnable()
            {
                refMenuOpen.action.Enable();
            }

            private void OnDisable()
            {
                refMenuOpen.action.Disable();
            }

            private void OnDestroy()
            {
                refMenuOpen.action.performed -= OpenCloseOptionMenu;
            }

            /// <summary>
            /// Small utility function to set active a specific menu.
            /// </summary>
            /// <param name="_menu">The gameobject of the menu to set
            /// active</param>
            /// <param name="_open">The boolean to control its activity
            /// state</param>
            private void MenuOpenState(GameObject _menu, bool _open)
            {
                _menu.SetActive(_open);
            }

            /// <summary>
            /// Shows or hides the UI corresponding to the Option Menu 
            /// and places it in front of the users camera.
            /// </summary>
            /// <param name="callbackContext">The callback context of
            /// the Input System.</param>
            private void OpenCloseOptionMenu(InputAction.CallbackContext callbackContext)
            {
                MenuOpenState(refOptionMenu, !refOptionMenu.activeSelf);
            }

            private void Start()
            {
                refOptionMenu.SetActive(false);
            }
        }
    }
}

