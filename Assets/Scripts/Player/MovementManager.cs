using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using ECellDive.Utility;


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
            public InputActionReference controllerPosition;
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

        [System.Serializable]
        public struct ContinousMovementData
        {
            public AnchoredContinousMoveHelper directionHelper;
            public Vector3 defaultPosition;
            public Vector3 deadZone;
            public float speed;
        }

        public class MovementManager : NetworkBehaviour
        {
            public GameObject refXRRig;

            public MovementActionData movementActionData;

            public TeleportationMovementData teleportationData;
            private Vector3 reticleVelocity = Vector3.zero;
            private NetworkVariable<Vector3> reticlePosition = new NetworkVariable<Vector3>(default,
                default, NetworkVariableWritePermission.Owner);

            public ContinousMovementData continousMovementData;
            private NetworkVariable<bool>isContinuous = new NetworkVariable<bool>(false);
            private bool doContinousMove = false;
            private Vector3 continousVelocity = Vector3.zero;

            public override void OnNetworkSpawn()
            {
                movementActionData.movement.action.started += TryMoveStart;
                movementActionData.movement.action.canceled += TryMoveEnd;
                movementActionData.switchMovementMode.action.performed += SwitchMovementMode;
                movementActionData.cursorController.action.performed += ReticleUpdate;

                isContinuous.OnValueChanged += ApplyControllerMvtMode;
                reticlePosition.OnValueChanged += ApplyReticlePosition;
            }

            public override void OnNetworkDespawn()
            {
                movementActionData.movement.action.started -= TryMoveStart;
                movementActionData.movement.action.canceled -= TryMoveEnd;
                movementActionData.switchMovementMode.action.performed -= SwitchMovementMode;
                movementActionData.cursorController.action.performed -= ReticleUpdate;

                isContinuous.OnValueChanged -= ApplyControllerMvtMode;
                reticlePosition.OnValueChanged -= ApplyReticlePosition;
            }

            private void OnEnable()
            {
                ResetTeleportationTools();
                ResetContinousMoveHelper();
            }

            private void FixedUpdate()
            {
                if (doContinousMove)
                {
                    ContinousMove();
                }
            }

            private void ApplyControllerMvtMode(bool previous, bool current)
            {
                if (isContinuous.Value)
                {
                    teleportationData.teleportationLine.enabled = false;
                    teleportationData.teleportationReticle.SetActive(false);

                    //Placing the helper
                    continousMovementData.directionHelper.gameObject.SetActive(true);
                    ResetContinousMoveHelper();
                    continousMovementData.directionHelper.SetSphereScale(2 * continousMovementData.deadZone);
                }
                else
                {
                    teleportationData.teleportationLine.enabled = true;
                    teleportationData.teleportationReticle.SetActive(true);
                    ResetTeleportationTools();

                    //Placing the helper
                    continousMovementData.directionHelper.gameObject.SetActive(false);
                }
            }

            private void ApplyReticlePosition(Vector3 previous, Vector3 current)
            {
                if (!IsOwner)
                {
                    teleportationData.teleportationReticle.transform.localPosition = reticlePosition.Value;
                    ResetTeleportationLine();
                }
            }

            [ServerRpc]
            public void BroadcastControllerMvtModeServerRpc()
            {
                isContinuous.Value = !isContinuous.Value;
            }

            /// <summary>
            /// Moves the object in the direction where the controller is relatively to its
            /// position at the start of the grabbing.
            /// </summary>
            private void ContinousMove()
            {
                Vector3 dir = (movementActionData.controllerPosition.action.ReadValue<Vector3>() -
                                continousMovementData.directionHelper.transform.localPosition);

                //Handling the Helper
                Vector3 dirInHelperSpace = continousMovementData.directionHelper.gameObject.transform.InverseTransformDirection(dir);
                continousMovementData.directionHelper.SetLinesEndPositions(dirInHelperSpace);
                continousMovementData.directionHelper.CheckValidity(dirInHelperSpace, continousMovementData.deadZone);

                Vector3 dir_corr = Vector3.zero;

                if (Mathf.Abs(dirInHelperSpace.x) > continousMovementData.deadZone.x)
                {
                    dir_corr.x = dirInHelperSpace.x;
                }

                if (Mathf.Abs(dirInHelperSpace.y) > continousMovementData.deadZone.y)
                {
                    dir_corr.y = dirInHelperSpace.y;
                }

                if (Mathf.Abs(dirInHelperSpace.z) > continousMovementData.deadZone.z)
                {
                    dir_corr.z = dirInHelperSpace.z;
                }

                Vector3 target = refXRRig.transform.position + continousMovementData.speed * (
                                                        dir_corr.x * continousMovementData.directionHelper.gameObject.transform.right +
                                                        dir_corr.y * continousMovementData.directionHelper.gameObject.transform.up +
                                                        dir_corr.z * continousMovementData.directionHelper.gameObject.transform.forward);

                refXRRig.transform.position = Vector3.SmoothDamp(
                                                        refXRRig.transform.position,
                                                        target,
                                                        ref continousVelocity,
                                                        0.1f);
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
                if (IsOwner)
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
                    reticlePosition.Value = teleportationData.teleportationReticle.transform.localPosition;
                    ResetTeleportationLine();
                }
            }

            private void ResetContinousMoveHelper()
            {
                continousMovementData.directionHelper.gameObject.transform.localPosition = Vector3.zero;
                continousMovementData.directionHelper.SetLinesEndPositions(2*continousMovementData.deadZone);
                continousMovementData.directionHelper.gameObject.transform.localRotation = Quaternion.identity;
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

            private void SwitchMovementMode(InputAction.CallbackContext _ctx)
            {
                if (IsOwner)
                {
                    BroadcastControllerMvtModeServerRpc();
                }       
            }

            private void TryMoveEnd(InputAction.CallbackContext _ctx)
            {
                if (isContinuous.Value)
                {
                    doContinousMove = false;
                    continousMovementData.directionHelper.gameObject.transform.parent = gameObject.transform;
                    ResetContinousMoveHelper();
                }
            }
            private void TryMoveStart(InputAction.CallbackContext _ctx)
            {
                if (isContinuous.Value)
                {
                    doContinousMove = true;
                    continousMovementData.directionHelper.gameObject.transform.parent = refXRRig.transform;
                }
                else
                {
                    refXRRig.transform.position = teleportationData.teleportationReticle.transform.position;
                }
            }
        }
    }
}