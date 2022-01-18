using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using ECellDive.Utility;

namespace ECellDive
{
    namespace SceneManagement
    {
        /// <summary>
        /// Resurfacing is at the level of the scene while
        /// diving is at the level of the module.
        /// </summary>
        public class ResurfaceManager : MonoBehaviour
        {

            public InputActionReference refResurfaceAction;

            private void Awake()
            {
                refResurfaceAction.action.performed += Resurface;
            }

            private void OnEnable()
            {
                refResurfaceAction.action.Enable();
            }

            private void OnDisable()
            {
                refResurfaceAction.action.Disable();
            }

            private void Resurface(InputAction.CallbackContext _ctx)
            {
                if (ScenesData.activeScene.sceneID > 0)
                {
                    ScenesData.Resurface();
                }
                else
                {
                    LogSystem.refLogManager.AddMessage(LogSystem.MessageTypes.Errors,
                        "You tried to resurface while you are at the root scene");
                }
            }
        }
    }
}

