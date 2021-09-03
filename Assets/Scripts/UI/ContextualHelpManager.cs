using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ECellDive
{
    namespace UI
    {
        /// <summary>
        /// Class to control the visibility of all contextual info tags
        /// that could be attached to children gameobjects.
        /// </summary>
        public class ContextualHelpManager : MonoBehaviour
        {
            public InputActionReference refGlobalHideInput;
            private InfoTagManager[] infoTagManagers;

            private void Awake()
            {
                refGlobalHideInput.action.performed += ContextualHelpGlobalHide;
            }

            private void OnEnable()
            {
                refGlobalHideInput.action.Enable();
            }

            private void OnDisable()
            {
                refGlobalHideInput.action.Disable();
            }

            private void OnDestroy()
            {
                refGlobalHideInput.action.performed -= ContextualHelpGlobalHide;
            }

            private void ContextualHelpGlobalHide(InputAction.CallbackContext _ctx)
            {
                foreach (InfoTagManager _infoTag in infoTagManagers)
                {
                    _infoTag.GlobalHide();
                }
            }

            private void Start()
            {
                infoTagManagers = GetComponentsInChildren<InfoTagManager>();
            }
        }
    }
}

