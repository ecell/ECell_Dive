using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using ECellDive.Input;
using ECellDive.Interfaces;
using ECellDive.SceneManagement;

namespace ECellDive
{
    namespace UserActions
    {
        [System.Serializable]
        public struct GrabActionData
        {
            public InputActionReference select;
            public InputActionReference attractRepulse;
        }

        public class GrabManager : MonoBehaviour, IRemoteGrab
        {
            #region - Interface Fields - 

            public XRRayInteractor currentRemoteInteractor { get; set; }
            public bool remoteGrabEnabled { get; set; }

            public bool isGrabed { get; set; }

            private IGrab.XRControllerID m_controllerID = IGrab.XRControllerID.Unassigned;
            public IGrab.XRControllerID controllerID
            {
                get => m_controllerID;
                set => m_controllerID = value;
            }
            public GameObject refCurrentController { get; set; }

            public UnityEvent OnPostGrabMovementUpdate
            {
                get => m_OnPostGrabMovementUpdate;
                set => m_OnPostGrabMovementUpdate = value;
            }
            #endregion

            #region - GrabManager Fields - 

            public GrabActionData leftGrabActionData;
            public GrabActionData rightGrabActionData;

            [Header("Attraction/Repulsion Parameters")]
            [Range(2f, 50f)] public float objectMaxDistance = 25f;
            [Range(0f, 2f)] public float objectMinDistance = 1f;
            public AnimationCurve attractionSpeedCurve;
            public AnimationCurve repulsionSpeedCurve;

            [Header("Additional Processes")]
            [SerializeField] private UnityEvent m_OnPostGrabMovementUpdate;

            private float objDistance = 0f;

            private Vector3 refVelocity = Vector3.zero;
            #endregion

            private void Awake()
            {
                leftGrabActionData.select.action.started += e => SetControllerID(IGrab.XRControllerID.Left);
                rightGrabActionData.select.action.started += e => SetControllerID(IGrab.XRControllerID.Right);

                leftGrabActionData.attractRepulse.action.performed += AttractionRepulsionLeft;
                rightGrabActionData.attractRepulse.action.performed += AttractionRepulsionRight;
            }

            private void OnDestroy()
            {
                leftGrabActionData.select.action.started -= e => SetControllerID(IGrab.XRControllerID.Left);
                rightGrabActionData.select.action.started -= e => SetControllerID(IGrab.XRControllerID.Right);

                leftGrabActionData.attractRepulse.action.performed -= AttractionRepulsionLeft;
                rightGrabActionData.attractRepulse.action.performed -= AttractionRepulsionRight;
            }

            private void Update()
            {
                if (isGrabed)
                {
                    if (remoteGrabEnabled)
                    {
                        OnRemoteGrab();
                    }

                    m_OnPostGrabMovementUpdate.Invoke();
                }
            }

            private void AttractionRepulsionLeft(InputAction.CallbackContext _ctx)
            {
                if (remoteGrabEnabled && m_controllerID == IGrab.XRControllerID.Left)
                {
                    Vector2 _das = _ctx.ReadValue<Vector2>();
                    if (!IsInDeadZone(_das.y))
                    {
                        ManageDistance(_das.y);
                    }
                }
            }

            private void AttractionRepulsionRight(InputAction.CallbackContext _ctx)
            {
                if (remoteGrabEnabled && m_controllerID == IGrab.XRControllerID.Right)
                {
                    Vector2 _das = _ctx.ReadValue<Vector2>();
                    if (!IsInDeadZone(_das.y))
                    {
                        ManageDistance(_das.y);
                    }
                }
            }

            /// <summary>
            /// Logic to translate the object when grabed from a distance.
            /// </summary>
            void FollowBeamTranslation()
            {                
                Vector3 target = objDistance * refCurrentController.transform.forward +
                                 refCurrentController.transform.position;

                transform.position = Vector3.SmoothDamp(transform.position,
                                                        target,
                                                        ref refVelocity,
                                                        0.3f);
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
                float speed;
                if (_mvtFactor > 0)
                {
                    speed = repulsionSpeedCurve.Evaluate(objDistance / objectMaxDistance);
                }
                else
                {
                    speed = attractionSpeedCurve.Evaluate(objDistance / objectMaxDistance);
                }
                Vector3 target = transform.position +
                                 _mvtFactor * speed * (
                                 transform.position - refCurrentController.transform.position);

                objDistance = Vector3.Distance(target, refCurrentController.transform.position);
                if (objDistance < objectMinDistance || objDistance > objectMaxDistance)
                {
                    target = transform.position;
                }
                transform.localPosition = Vector3.SmoothDamp(
                                            transform.localPosition,
                                            target,
                                            ref refVelocity,
                                            0.1f);
            }

            /// <summary>
            /// Logic to determine which controller (left or right) has grabbed an object.
            /// </summary>
            public void HandleInteractorsSorting()
            {
                isGrabed = true;
                SortControllers();

                if (currentRemoteInteractor.isActiveAndEnabled)
                {
                    remoteGrabEnabled = true;
                    refCurrentController = currentRemoteInteractor.gameObject;
                }

                objDistance = Vector3.Distance(transform.position,
                                               refCurrentController.transform.position);

            }

            public void OnRemoteGrab()
            {
                FollowBeamTranslation();
            }

            public void OnRemoteRelease()
            {
                remoteGrabEnabled = false;
            }

            public void ResetLogic()
            {
                isGrabed = false;
                OnRemoteRelease();
            }

            /// <summary>
            /// Assigns a value to the field "controllerID"
            /// </summary>
            /// <param name="_controllerID">Left or Right</param>
            public void SetControllerID(IGrab.XRControllerID _controllerID)
            {
                if (!isGrabed)
                {
                    controllerID = _controllerID;
                }
            }

            /// <summary>
            /// Assigns the current interactors to consider based on
            /// the value of the controllerID.
            /// </summary>
            private void SortControllers()
            {
                switch (controllerID)
                {
                    case IGrab.XRControllerID.Left:
                        currentRemoteInteractor = InteractorsRegister.remoteGrabInteractors.left;
                        break;

                    case IGrab.XRControllerID.Right:
                        currentRemoteInteractor = InteractorsRegister.remoteGrabInteractors.right;
                        break;
                }
            }
        }
    }
}
