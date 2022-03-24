using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


namespace ECellDive
{
    namespace UserActions
    {
        [System.Serializable]
        public struct MovementActionData
        {
            public InputActionReference movement;
            public InputActionReference cursorController;
            public InputActionReference switchMovementMode;
        }

        [System.Serializable]
        public struct TeleportationMovementData
        {
            public GameObject teleportationReticle;
            public Vector3 defaultReticlePosition;
            public float reticleMovementSpeed;
            public LineRenderer teleportationLine;
            public float minTeleportationDistance;
            public float maxTeleportationDistance;
        }

        //public struct ContinousMovementData
        //{
        //    public GameObject directionHelper;
        //}

        public class MovementManager : MonoBehaviour
        {
            public GameObject refXRRig;

            public MovementActionData movementActionData;
            public TeleportationMovementData teleportationData;
            private Vector3 reticleVelocity = Vector3.zero;

            private void Awake()
            {
                movementActionData.movement.action.performed += Move;
                movementActionData.cursorController.action.performed += ReticleUpdate;
            }

            private void OnEnable()
            {
                ResetTeleportationTools();
            }

            /// <summary>
            /// Compares if vector <paramref name="_a"/> is less than
            /// <paramref name="_b"/> component-wise.
            /// </summary>
            private bool CompareVec3(Vector3 _a, Vector3 _b)
            {
                return (_a.x < _b.x && _a.y < _b.y && _a.z < _b.z);
            }

            /// <summary>
            /// A hard coded method returning true if Abs(<paramref name="_value"/>)
            /// lower than <paramref name="_threshold"/>. Returns false otherwise.
            /// </summary>
            /// <remarks>Used to clamp Joystick input data since I couldn't
            /// make the built-in deadzone work as intended.</remarks>
            private bool IsInDeadZone(float _value, float _threshold = 0.5f)
            {
                return Mathf.Abs(_value) < _threshold;
            }

            /// <summary>
            /// Moves the selector forward or backward.
            /// </summary>
            /// <param name="_mvtFactor">If greater than 0 then forward movement;
            /// If lower than 0 then backward movement.</param>
            private void ManageDistance(float _mvtFactor)
            {
                Vector3 target = teleportationData.teleportationReticle.transform.localPosition +
                                 _mvtFactor * teleportationData.reticleMovementSpeed * Vector3.forward;
                
                float _d = (target - teleportationData.defaultReticlePosition).z;
                if (_d < teleportationData.minTeleportationDistance)
                {
                    target = teleportationData.teleportationReticle.transform.localPosition;
                }
                if (_d > teleportationData.maxTeleportationDistance)
                {
                    target = teleportationData.teleportationReticle.transform.localPosition;
                }
                teleportationData.teleportationReticle.transform.localPosition = Vector3.SmoothDamp(
                                            teleportationData.teleportationReticle.transform.localPosition,
                                            target,
                                            ref reticleVelocity,
                                            0.1f);
                ResetTeleportationLine();
            }

            private void Move(InputAction.CallbackContext _ctx)
            {
                refXRRig.transform.position = teleportationData.teleportationReticle.transform.position;
            }

            private void ResetTeleportationTools()
            {
                teleportationData.teleportationReticle.transform.localPosition = teleportationData.defaultReticlePosition;
                ResetTeleportationLine();
            }
            
            private void ResetTeleportationLine()
            {
                teleportationData.teleportationLine.SetPosition(1, teleportationData.teleportationReticle.transform.localPosition);
            }

            /// <summary>
            /// Manages the data from the Joystick and processes it to
            /// move the gameobject and scale it.
            /// </summary>
            private void ReticleUpdate(InputAction.CallbackContext _ctx)
            {
                Vector2 _das = _ctx.ReadValue<Vector2>();
                if (!IsInDeadZone(_das.y))
                {
                    ManageDistance(_das.y);
                }

            }
        }
    }
}