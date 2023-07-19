using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using ECellDive.Interfaces;
using ECellDive.Utility;

namespace ECellDive.PlayerComponents
{
    public class GrabManager : MonoBehaviour, IRemoteGrab
    {
        #region - IRemoteGrab Members - 
        private LeftRightData<bool> m_isGrabed;
        public LeftRightData<bool> isGrabed
        {
            get => m_isGrabed;
        }

        [SerializeField] private LeftRightData<InputActionReference> m_manageDistance;
        public LeftRightData<InputActionReference> manageDistance
        {
            get => m_manageDistance;
        }

        public XRRayInteractor currentRemoteInteractor { get; set; }

        public GameObject refCurrentController { get; set; }

        [SerializeField] private UnityEvent m_OnPostGrabMovementUpdate;
        public UnityEvent OnPostGrabMovementUpdate
        {
            get => m_OnPostGrabMovementUpdate;
            set => m_OnPostGrabMovementUpdate = value;
        }
        #endregion

        #region - GrabManager Fields - 
        [Header("Attraction/Repulsion Parameters")]
        [Range(2f, 50f)] public float objectMaxDistance = 25f;
        [Range(0f, 2f)] public float objectMinDistance = 1f;
        public AnimationCurve attractionSpeedCurve;
        public AnimationCurve repulsionSpeedCurve;
        
        private XRBaseInteractable refInteractable;
        private bool isReady;
        private float objDistance = 0f;
        private Vector3 refVelocity = Vector3.zero;
        #endregion

        private void Awake()
        {
            manageDistance.left.action.performed += ManageDistanceLeft;
            manageDistance.right.action.performed += ManageDistanceRight;
        }

        private void OnDestroy()
        {
            manageDistance.left.action.performed -= ManageDistanceLeft;
            manageDistance.right.action.performed -= ManageDistanceRight;
        }

        private void Start()
        {
            refInteractable = GetComponent<XRBaseInteractable>();
        }

        private void Update()
        {
            if (isReady)
            {
                FollowBeamTranslation();
                m_OnPostGrabMovementUpdate.Invoke();
            }
        }

        private void ManageDistanceLeft(InputAction.CallbackContext _ctx)
        {
            if (m_isGrabed.left)
            {
                Vector2 _das = _ctx.ReadValue<Vector2>();
                if (!IsInDeadZone(_das.y))
                {
                    ManageDistance(_das.y);
                }
            }
        }

        private void ManageDistanceRight(InputAction.CallbackContext _ctx)
        {
            if (m_isGrabed.right)
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

        #region - IRemoteGrab Methods -
        /// <inheritdoc/>
        public bool IsGrabed()
        {
            return m_isGrabed.left || m_isGrabed.right;
        }

        /// <inheritdoc/>
        public void OnGrab()
        {
            refCurrentController = refInteractable.selectingInteractor.gameObject;
            objDistance = Vector3.Distance(transform.position,
                                            refCurrentController.transform.position);

            if (refInteractable.selectingInteractor == StaticReferencer.Instance.remoteGrabInteractors.left)
            {
                m_isGrabed.left = true;
            }

            if (refInteractable.selectingInteractor == StaticReferencer.Instance.remoteGrabInteractors.right)
            {
                m_isGrabed.right = true;
            }

            isReady = true;
        }

        /// <inheritdoc/>
        public void OnReleaseGrab()
        {
            isReady = false;
            m_isGrabed.left = false;
            m_isGrabed.right = false;
        }

        #endregion
    }
}
