using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using ECellDive.Interfaces;

namespace ECellDive
{
    namespace SceneManagement
    {
        /// <summary>
        /// Implements the logic to let a diver resurface from an Input.
        /// </summary>
        /// <remarks>
        /// While diving is implemented at the level of a module, resurfacing
        /// is at the level of the scene.</remarks>
        public class ResurfaceManager : MonoBehaviour
        {
            public LeftRightData<InputActionReference> resurfaceAction;

            private void Awake()
            {
                resurfaceAction.left.action.performed += ResurfaceLeft;
                resurfaceAction.right.action.performed += ResurfaceRight;
            }

            private void OnEnable()
            {
                resurfaceAction.left.action.Enable();
                resurfaceAction.right.action.Enable();
            }

            private void OnDisable()
            {
                resurfaceAction.left.action.Disable();
                resurfaceAction.right.action.Disable();
            }

            private void OnDestroy()
            {
                resurfaceAction.left.action.performed -= ResurfaceLeft;
                resurfaceAction.right.action.performed -= ResurfaceRight;
            }

            /// <summary>
            /// Buffer logic to callback <see cref="Resurface"/> when the
            /// input on the left controller is triggered.
            /// </summary>
            /// <remarks>Same as <see cref="ResurfaceRight(InputAction.CallbackContext)"/>
            /// but for the left controller.</remarks>
            private void ResurfaceLeft(InputAction.CallbackContext _ctx)
            {
                Resurface();
            }

            /// <summary>
            /// Buffer logic to callback <see cref="Resurface"/> when the
            /// input on the left controller is triggered.
            /// </summary>
            /// <remarks>Same as <see cref="ResurfaceLeft(InputAction.CallbackContext)"/>
            /// but for the right controller.</remarks>
            private void ResurfaceRight(InputAction.CallbackContext _ctx)
            {
                Resurface();
            }

            /// <summary>
            /// Start point of the logic allowing a user to resurface from a child
            /// dive scene to the parent dive scene.
            /// </summary>
            public void Resurface()
            {
                DiveScenesManager.Instance.ResurfaceServerRpc(NetworkManager.Singleton.LocalClientId);
            }
        }
    }
}

