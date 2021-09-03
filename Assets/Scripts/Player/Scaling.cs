using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ECellDive
{
    namespace UserActions
    {
        /// <summary>
        /// Class controlling the scaling of all the objects loaded in the diving room.
        /// </summary>
        public class Scaling : MonoBehaviour
        {
            public InputActionReference leftControllerPositionInput;
            public InputActionReference rightControllerPositionInput;
            public InputActionReference refScalingInput;

            [Min(0.0001f)] public Vector3 minScale = 0.1f * Vector3.one;
            [Min(0.0001f)] public Vector3 maxScale = 10f * Vector3.one;

            private bool scalingActivated = false;
            private Vector3 leftControllerPosition;
            private Vector3 rightControllerPosition;
            private float controllersStartDistance;
            private Vector3 startScale;

            private void Awake()
            {
                leftControllerPositionInput.action.performed += GetLeftPosition;
                rightControllerPositionInput.action.performed += GetRightPosition;

                refScalingInput.action.started += GetStartInfo;
                refScalingInput.action.performed += e => scalingActivated = true;
                refScalingInput.action.canceled += e => scalingActivated = false;
            }

            private void GetLeftPosition(InputAction.CallbackContext _ctx)
            {
                leftControllerPosition = _ctx.ReadValue<Vector3>();
            }

            private void GetRightPosition(InputAction.CallbackContext _ctx)
            {
                rightControllerPosition = _ctx.ReadValue<Vector3>();
            }

            private void GetStartInfo(InputAction.CallbackContext _ctx)
            {
                controllersStartDistance = Vector3.Distance(leftControllerPosition, rightControllerPosition);
                startScale = transform.localScale;
            }

            private void OnEnable()
            {
                refScalingInput.action.Enable();
            }

            private void OnDisable()
            {
                refScalingInput.action.Disable();
            }

            private void OnDestroy()
            {
                leftControllerPositionInput.action.performed -= GetLeftPosition;
                rightControllerPositionInput.action.performed -= GetRightPosition;

                refScalingInput.action.started -= GetStartInfo;
            }

            private void ScalingObject()
            {
                float currentControllerDistance = Vector3.Distance(leftControllerPosition, rightControllerPosition);
                float ratio = currentControllerDistance / controllersStartDistance;
                Vector3 newScale = ratio * startScale;
                transform.localScale = new Vector3(Mathf.Clamp(newScale.x, minScale.x, maxScale.x),
                                                   Mathf.Clamp(newScale.y, minScale.y, maxScale.y),
                                                   Mathf.Clamp(newScale.z, minScale.z, maxScale.z));
            }

            private void Update()
            {
                if (scalingActivated)
                {
                    ScalingObject();
                }
            }
        }
    }
}

