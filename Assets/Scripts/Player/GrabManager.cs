using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
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
            public InputActionReference activate;
        }

        public class GrabManager : MonoBehaviour, IRemoteGrab
        {
            #region - Interface Fields - 

            public XRRayInteractor currentRemoteInteractor { get; set; }
            public bool remoteGrabEnabled { get; set; }

            public bool isGrabed { get; set; }
            public IGrab.XRControllerID controllerID { get; set; }
            public GameObject refCurrentController { get; set; }

            public UnityEvent OnPostGrabMovementUpdate
            {
                get => m_OnPostGrabMovementUpdate;
                set => m_OnPostGrabMovementUpdate = OnPostGrabMovementUpdate;
            }
            #endregion

            #region - GrabManager Fields - 

            public GrabActionData leftGrabActionData;
            public GrabActionData rightGrabActionData;

            [Header("Attraction/Repulsion Parameters")]
            [Range(2f, 50f)] public float objectMaxDistance = 25f;
            [Range(0.05f, 0.5f)] public float attractionThreshold = 0.1f;
            public AnimationCurve attractionSpeedCurve;
            [Range(0.05f, 0.5f)] public float repulsionThreshold = 0.1f;
            public AnimationCurve repulsionSpeedCurve;

            [Header("Additional Processes")]
            [SerializeField] private UnityEvent m_OnPostGrabMovementUpdate;

            private Vector3 controllerStartPosition = Vector3.zero;
            private Vector3 controllerLatePosition = Vector3.zero;
            private Vector3 controllerPosition = Vector3.zero;

            private float objDistance = 0f;

            private Vector3 refVelocity = Vector3.zero;

            private bool attractionRepulsionIsActive = false;
            #endregion

            private void Awake()
            {
                leftGrabActionData.select.action.started += e => SetControllerID(IGrab.XRControllerID.Left);
                rightGrabActionData.select.action.started += e => SetControllerID(IGrab.XRControllerID.Right);

                leftGrabActionData.activate.action.started += e => ActivateAttractionRepulsion(IGrab.XRControllerID.Left);
                rightGrabActionData.activate.action.started += e => ActivateAttractionRepulsion(IGrab.XRControllerID.Right);

                leftGrabActionData.activate.action.canceled += e => DeactivateAttractionRepulsion(IGrab.XRControllerID.Left);
                rightGrabActionData.activate.action.canceled += e => DeactivateAttractionRepulsion(IGrab.XRControllerID.Right);
            }

            /// <summary>
            /// Activates the Attraction/Repulsion mode to bring closer or push
            /// away objects that are being grabed.
            /// Called back after a user input on the XR controller.
            /// The user input is referenced by the ref(Left/Right)Activate fields
            /// of the class.
            /// </summary>
            /// <param name="_controllerID">The controller ID (Left or Right)</param>
            void ActivateAttractionRepulsion(IGrab.XRControllerID _controllerID)
            {
                if (isGrabed)
                {
                    //Checking that the activation comes from the same controller
                    //as the one that grabed the object.
                    if (_controllerID == controllerID)
                    {
                        attractionRepulsionIsActive = true;
                        controllerStartPosition = refCurrentController.transform.position;
                    }
                }
            }

            /// <summary>
            /// Deactivates the Attraction/Repulsion mode to bring closer or push
            /// away objects that are being grabed.
            /// Called back after a user input on the XR controller.
            /// The user input is referenced by the ref(Left/Right)Activate fields
            /// of the class.
            /// </summary>
            /// <param name="_controllerID">The controller ID (Left or Right)</param>
            void DeactivateAttractionRepulsion(IGrab.XRControllerID _controllerID)
            {
                if (isGrabed)
                {
                    if (_controllerID == controllerID)
                    {
                        attractionRepulsionIsActive = false;
                    }
                }
            }

            /// <summary>
            /// Logic to translate an object when grabed from a contact.
            /// </summary>
            void FollowControllerTranslation()
            {
                controllerPosition = refCurrentController.transform.position;
                Vector3 Delta = (controllerPosition - controllerLatePosition);

                transform.localPosition += Delta * 3;

                controllerLatePosition = controllerPosition;
            }

            /// <summary>
            /// Logic to translate the object when grabed from a distance.
            /// </summary>
            void FollowBeamTranslation()
            {
                controllerPosition = refCurrentController.transform.position;
                
                Vector3 target = objDistance * refCurrentController.transform.forward + controllerPosition;

                transform.position = Vector3.SmoothDamp(transform.position,
                                                        target,
                                                        ref refVelocity,
                                                        0.3f);
            }

            /// <summary>
            /// Logic to push away or to bring closer a grabed object
            /// </summary>
            void UndergoTractorBeam()
            {
                controllerPosition = refCurrentController.transform.position;
                Vector3 BeamAxis_n = (transform.position - controllerPosition).normalized;
                Vector3 DirController = (controllerPosition - controllerStartPosition);
                float dot = Vector3.Dot(DirController, BeamAxis_n);

                //repulsion
                if (dot > repulsionThreshold)
                {
                    float repulsionSpeed = repulsionSpeedCurve.Evaluate(objDistance / objectMaxDistance);
                    if (objDistance + repulsionSpeed < objectMaxDistance)
                    {
                        transform.position += BeamAxis_n * repulsionSpeed;
                        objDistance += repulsionSpeed;
                    }
                }

                //attraction
                if (dot < -attractionThreshold)
                {
                    float attractionSpeed = repulsionSpeedCurve.Evaluate(objDistance / objectMaxDistance);
                    transform.position -= BeamAxis_n * attractionSpeed;
                    objDistance -= attractionSpeed;
                }
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

                controllerLatePosition = refCurrentController.transform.position;
                objDistance = Vector3.Distance(transform.position, controllerLatePosition);

            }

            public void OnDirectGrab()
            {
                FollowControllerTranslation();
            }

            public void OnRemoteGrab()
            {
                if (!attractionRepulsionIsActive)
                {
                    FollowBeamTranslation();
                }
                else
                {
                    UndergoTractorBeam();
                }
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
                        currentRemoteInteractor = ScenesData.refSceneManagerMonoBehaviour.remoteGrabData.leftInteractor;
                        break;

                    case IGrab.XRControllerID.Right:
                        currentRemoteInteractor = ScenesData.refSceneManagerMonoBehaviour.remoteGrabData.rightInteractor;
                        break;
                }
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
        }
    }
}
