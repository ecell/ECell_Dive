using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using ECellDive.Interfaces;
using ECellDive.Utility.Data;
using ECellDive.Utility.PlayerComponents;

namespace ECellDive.PlayerComponents
{
	/// <summary>
	/// The logic behind the remote grab feature.
	/// </summary>
	[RequireComponent(typeof(XRBaseInteractable))]
	public class GrabManager : MonoBehaviour, IRemoteGrab
	{
		#region - IRemoteGrab Members - 
		/// <summary>
		/// The field of the <see cref="isGrabed"/> property.
		/// </summary>
		private LeftRightData<bool> m_isGrabed;

		/// <inheritdoc/>
		public LeftRightData<bool> isGrabed
		{
			get => m_isGrabed;
		}

		/// <summary>
		/// The field of the <see cref="manageDistance"/> property.
		/// </summary>
		[SerializeField] private LeftRightData<InputActionReference> m_manageDistance;

		/// <inheritdoc/>
		public LeftRightData<InputActionReference> manageDistance
		{
			get => m_manageDistance;
		}

		/// <inheritdoc/>
		public XRRayInteractor currentRemoteInteractor { get; set; }

		/// <inheritdoc/>
		public GameObject refCurrentController { get; set; }

		/// <summary>
		/// The field of the <see cref="OnPostGrabMovementUpdate"/> property.
		/// </summary>
		[SerializeField] private UnityEvent m_OnPostGrabMovementUpdate;

		/// <inheritdoc/>
		public UnityEvent OnPostGrabMovementUpdate
		{
			get => m_OnPostGrabMovementUpdate;
			set => m_OnPostGrabMovementUpdate = value;
		}
		#endregion

		/// <summary>
		/// The maximum distance an object can be moved away from the controller.
		/// </summary>
		[Header("Attraction/Repulsion Parameters")]
		[Range(2f, 50f)] public float objectMaxDistance = 25f;

		/// <summary>
		/// The minimum distance an object can be moved away from the controller.
		/// </summary>
		[Range(0f, 2f)] public float objectMinDistance = 1f;

		/// <summary>
		/// The curve controlling the speed of attraction.
		/// The value of the curve is evaluated at the linerar interpolation
		/// of the current distance between the minimum and maximum distances.
		/// </summary>
		public AnimationCurve attractionSpeedCurve;

		/// <summary>
		/// The curve controlling the speed of repulsion.
		/// The value of the curve is evaluated at the linerar interpolation
		/// of the current distance between the minimum and maximum distances.
		/// </summary>
		public AnimationCurve repulsionSpeedCurve;

		/// <summary>
		/// Reference to the XRBaseInteractable of this gameobject.
		/// </summary>
		/// <remarks>
		/// It is automatically set in the Start() method.
		/// </remarks>
		private XRBaseInteractable refInteractable;

		/// <summary>
		/// A boolean to inform whether grabing is in progress.
		/// </summary>
		private bool isReady;

		/// <summary>
		/// The distance between the object and the controller.
		/// </summary>
		private float objDistance = 0f;

		/// <summary>
		/// Reference to the velocity of the movement of the object used for
		/// a smooth damp movement.
		/// </summary>
		private Vector3 refVelocity = Vector3.zero;

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

        /// <summary>
        /// Manages the distance between the object and the left controller.
        /// Calledback when the action <see cref="manageDistance"/>.left is performed.
        /// </summary>
        /// <param name="_ctx">
        /// The input context at the time of the callback.
        /// It is necessary to statisfy the constraint on the callback signature.
        /// Used to retrieve a Vector2.
        /// </param>
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

        /// <summary>
        /// Manages the distance between the object and the right controller.
        /// Calledback when the action <see cref="manageDistance"/>.right is performed.
        /// </summary>
        /// <param name="_ctx">
        /// The input context at the time of the callback.
        /// It is necessary to statisfy the constraint on the callback signature.
        /// Used to retrieve a Vector2.
        /// </param>
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
