using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using ECellDive.Interfaces;

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

            private void ResurfaceLeft(InputAction.CallbackContext _ctx)
            {
                Resurface();
            }
            
            private void ResurfaceRight(InputAction.CallbackContext _ctx)
            {
                Resurface();
            }
            private void Resurface()
            {
                GameNetScenesManager.Instance.ResurfaceServerRpc(NetworkManager.Singleton.LocalClientId);
            }
        }
    }
}

