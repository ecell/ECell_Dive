using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using ECellDive.IO;


using ECellDive.NetworkComponents;

namespace ECellDive
{
    namespace SceneManagement
    {
        /// <summary>
        /// Base manager class of the Diving room.
        /// Stores the references to the XR rig and the Dive Container.
        /// Also handles the action to exit the dive room and come back
        /// to the main room.
        /// </summary>
        public class DiveRoomManager : MonoBehaviour
        {
            public GameObject refXRRig;
            public GameObject DiveContainer;

            public Animator refDivingAnimator;
            public InputActionReference refBackToMainRoom;

            private void Awake()
            {
                refBackToMainRoom.action.performed += BackToMainRoom;
            }

            private void BackToMainRoom(InputAction.CallbackContext ctx)
            {
                refDivingAnimator.SetTrigger("DiveStart");
                StartCoroutine(Loading.SwitchScene(0, 1.5f));
            }

            private void OnEnable()
            {
                refBackToMainRoom.action.Enable();
            }

            private void OnDisable()
            {
                refBackToMainRoom.action.Disable();
            }

            private void OnDestroy()
            {
                refBackToMainRoom.action.performed -= BackToMainRoom;
            }
        }
    }
}

