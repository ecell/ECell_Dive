using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using ECellDive.Utility;


namespace ECellDive
{
    namespace UserActions
    {
        public class DiveGrab : MonoBehaviour
        {
            [Header("General References")]
            public DiveGrabHelperManager refDiveGrabHelperManager;

            [Header("Input Action References")]
            public InputActionReference leftControllerPositionInput;
            public InputActionReference refGrabInput;

            [Header("Parameters")]
            public Vector3 deadzoneDistance;
            [Range(0.5f, 5)] public float sensitivity;
            [Range(0.01f, 2f)] public float smoothTime;

            [Header("Additional Processes")]
            [SerializeField] private UnityEvent m_OnPostGrabMovementUpdate;
            public UnityEvent OnPostGrabMovementUpdate
            {
                get => m_OnPostGrabMovementUpdate;
                set => m_OnPostGrabMovementUpdate = OnPostGrabMovementUpdate;
            }

            private bool grabActivated = false;
            private Vector3 leftControllerPosition;
            private Vector3 leftControllerStartPosition;
            private Vector3 refVelocity = Vector3.zero;

            private void Awake()
            {
                leftControllerPositionInput.action.performed += GetLeftPosition;

                refGrabInput.action.started += OnGrab;
                refGrabInput.action.canceled += OnRelease;
            }

            /// <summary>
            /// Logic to translate an object when grabed from a contact.
            /// </summary>
            void FollowControllerTranslation()
            {
                Vector3 currentControllerPosition = leftControllerPosition;
                Vector3 dir = (currentControllerPosition - leftControllerStartPosition);

                //Handling the Helper
                Vector3 dirInHelperSpace = refDiveGrabHelperManager.gameObject.transform.InverseTransformDirection(dir);
                refDiveGrabHelperManager.SetLinesEndPositions(dirInHelperSpace);
                refDiveGrabHelperManager.CheckValidity(dirInHelperSpace, deadzoneDistance);

                Vector3 dir_corr = Vector3.zero;
                
                if (Mathf.Abs(dirInHelperSpace.x) > deadzoneDistance.x)
                {
                    dir_corr.x = dirInHelperSpace.x;
                }

                if (Mathf.Abs(dirInHelperSpace.y) > deadzoneDistance.y)
                {
                    dir_corr.y = dirInHelperSpace.y;
                }

                if (Mathf.Abs(dirInHelperSpace.z) > deadzoneDistance.z)
                {
                    dir_corr.z = dirInHelperSpace.z;
                }

                Vector3 target = transform.position + sensitivity * (dir_corr.x * refDiveGrabHelperManager.gameObject.transform.right +
                                                                     dir_corr.y * refDiveGrabHelperManager.gameObject.transform.up +
                                                                     dir_corr.z * refDiveGrabHelperManager.gameObject.transform.forward);

                transform.position = Vector3.SmoothDamp(transform.position,
                                                        target,
                                                        ref refVelocity,
                                                        smoothTime);
            }

            private void GetLeftPosition(InputAction.CallbackContext _ctx)
            {
                leftControllerPosition = _ctx.ReadValue<Vector3>();
            }

            private void OnDestroy()
            {
                refGrabInput.action.started -= OnGrab;
                refGrabInput.action.canceled -= OnRelease;
            }

            private void OnGrab(InputAction.CallbackContext _ctx)
            {
                grabActivated = true;
                leftControllerStartPosition = leftControllerPosition;

                //Placing the helper
                refDiveGrabHelperManager.gameObject.SetActive(true);
                refDiveGrabHelperManager.FlatPositioning();
                refDiveGrabHelperManager.SetSphereScale(2*deadzoneDistance);
            }

            private void OnRelease(InputAction.CallbackContext _ctx)
            {
                grabActivated = false;
                refDiveGrabHelperManager.gameObject.SetActive(false);
            }

            // Update is called once per frame
            void Update()
            {
                if (grabActivated)
                {
                    FollowControllerTranslation();
                    m_OnPostGrabMovementUpdate.Invoke();
                }
            }
        }
    }
}

