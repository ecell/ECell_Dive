using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using ECellDive.Interfaces;
using ECellDive.SceneManagement;
using ECellDive.Utility;


namespace ECellDive
{
    namespace UserActions
    {
        [System.Serializable]
        public struct ControllersPositionsData
        {
            public InputActionReference leftController;
            public InputActionReference rightController;
        }
        /// <summary>
        /// Gameplay logic to trigger when a user whishes to translate objects
        /// while in the Dive Scene.
        /// </summary>
        public class GroupGrab : MonoBehaviour, IGrab
        {

            #region - IGrab members -
            public bool isGrabed { get; set; }
            public IGrab.XRControllerID controllerID { get; set; }
            public GameObject refCurrentController { get; set; }
            [SerializeField] private UnityEvent m_OnPostGrabMovementUpdate;
            public UnityEvent OnPostGrabMovementUpdate
            {
                get => m_OnPostGrabMovementUpdate;
                set => m_OnPostGrabMovementUpdate = OnPostGrabMovementUpdate;
            }
            #endregion

            [Header("Input Action References")]
            public ControllersPositionsData controllersPositionsData;
            public GrabActionData leftGrabActionData;
            public GrabActionData rightGrabActionData;

            [Header("Parameters")]
            public Vector3 deadzoneDistance;
            [Range(0.5f, 5)] public float sensitivity;
            [Range(0.01f, 2f)] public float smoothTime;

            private DiveGrabHelperManager refDiveGrabHelperManager;
            private Vector3 controllerPosition;
            private Vector3 controllerStartPosition;
            private Vector3 refVelocity = Vector3.zero;

            private void Awake()
            {
                leftGrabActionData.select.action.started += e => SetControllerID(IGrab.XRControllerID.Left);
                rightGrabActionData.select.action.started += e => SetControllerID(IGrab.XRControllerID.Right);

                leftGrabActionData.select.action.performed += OnGrab;
                rightGrabActionData.select.action.performed += OnGrab;

                leftGrabActionData.select.action.canceled += OnRelease;
                rightGrabActionData.select.action.canceled += OnRelease;
            }

            private void OnDestroy()
            {
                leftGrabActionData.select.action.started += e => SetControllerID(IGrab.XRControllerID.Left);
                rightGrabActionData.select.action.started += e => SetControllerID(IGrab.XRControllerID.Right);

                leftGrabActionData.select.action.performed -= OnGrab;
                rightGrabActionData.select.action.performed -= OnGrab;

                leftGrabActionData.select.action.canceled -= OnRelease;
                rightGrabActionData.select.action.canceled -= OnRelease;
            }

            private void Start()
            {
                refDiveGrabHelperManager = ScenesData.refSceneManagerMonoBehaviour.divingData.diveGrabHelper.GetComponent<DiveGrabHelperManager>();
            }

            // Update is called once per frame
            void Update()
            {
                if (isGrabed)
                {
                    GetPosition();
                    FollowControllerTranslation();

                    m_OnPostGrabMovementUpdate.Invoke();
                }
            }

            /// <summary>
            /// Moves the object in the direction where the controller is relatively to its
            /// position at the start of the grabbing.
            /// </summary>
            void FollowControllerTranslation()
            {
                Vector3 dir = (controllerPosition - controllerStartPosition);

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

            private void GetPosition()
            {
                switch (controllerID)
                {
                    case IGrab.XRControllerID.Left:
                        controllerPosition = controllersPositionsData.leftController.action.ReadValue<Vector3>();
                        break;
                    case IGrab.XRControllerID.Right:
                        controllerPosition = controllersPositionsData.rightController.action.ReadValue<Vector3>();
                        break;
                }
            }

            /// <summary>
            /// Called back when the input action corresponding to "Grab" was performed.
            /// </summary>
            private void OnGrab(InputAction.CallbackContext _ctx)
            {
                isGrabed = true;

                GetPosition();

                controllerStartPosition = controllerPosition;

                //Placing the helper
                refDiveGrabHelperManager.gameObject.SetActive(true);
                refDiveGrabHelperManager.FlatPositioning();
                refDiveGrabHelperManager.SetSphereScale(2*deadzoneDistance);
            }

            /// <summary>
            /// Called back when the input action corresponding to "Release Grab" was performed.
            /// </summary>
            private void OnRelease(InputAction.CallbackContext _ctx)
            {
                isGrabed = false;
                refDiveGrabHelperManager.gameObject.SetActive(false);
            }


            #region - IGrab Methods - 
            public void SetControllerID(IGrab.XRControllerID _controllerID)
            {
                if (!isGrabed)
                {
                    controllerID = _controllerID;
                }
            }
            #endregion


        }
    }
}

