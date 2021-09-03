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
        public class OptionMenuManager : MonoBehaviour
        {
            public GameObject refCamera;
            public InputActionReference refMenuOpen;
            public GameObject refOptionMenu;

            public GameObject[] refMenus;

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
                if (!refOptionMenu.activeSelf)
                {
                    //Reset position to be in front of the user's camera
                    //and correctly rotated.
                    refOptionMenu.transform.position = Positioning.PlaceInFrontOfTarget(refCamera.transform, 1f, 0.5f);
                    Positioning.UIFaceTarget(refOptionMenu, refCamera.transform);
                }
                MenuOpenState(refOptionMenu, !refOptionMenu.activeSelf);
            }

            /// <summary>
            /// One method to handle the deactivation of all menus and activation
            /// of a target menu. Tnis effectively hides all UI except form the
            /// one of the target menu.
            /// </summary>
            /// <param name="_menuType">The index of the target menu in the 
            /// refMenus list defined as a field of teh OptionMenuManager class</param>
            public void SwitchMenus(int _menuType)
            {
                for (int i=0; i<refMenus.Length; i++)
                {
                    if (i != _menuType)
                    {
                        MenuOpenState(refMenus[i], false);
                    }
                }
                MenuOpenState(refMenus[_menuType], !refMenus[_menuType].activeSelf);
            }

            private void Start()
            {
                refOptionMenu.SetActive(false);
            }
        }
    }
}

