using UnityEngine;
using ECellDive.Interfaces;

namespace ECellDive.Utility
{
	/// <summary>
	/// A utility component to make the gameobject it is attached to face the camera
	/// or pop up in front of the camera.
	/// </summary>
	public class FaceCamera : MonoBehaviour,
								ILookAt,
								IPopUp
	{
		/// <summary>
		/// A boolean to determine whether or not to show the gameobject when
		/// it is enabled.
		/// </summary>
		public bool showOnEnable = true;

		#region - ILookAt Members -
		/// <summary>
		/// The field of the <see cref="flip"/> property.
		/// </summary>
		[SerializeField] private bool m_flip = false;

		/// <inheritdoc/>
		public bool flip
		{
			get => m_flip;
			private set => m_flip = value;
		}

		/// <inheritdoc/>
		public Transform lookAtTarget { get; set; }
		#endregion

		#region - IPopUp Members -
		/// <summary>
		/// The field of the <see cref="popupDistance"/> property.
		/// </summary>
		[SerializeField] private float m_popupDistance;

		/// <inheritdoc/>
		public float popupDistance
		{
			get => m_popupDistance;
			private set => m_popupDistance = value;
		}

		/// <summary>
		/// The field of the <see cref="popupHeightOffset"/> property.
		/// </summary>
		[SerializeField] private float m_popupHeightOffset;

		/// <inheritdoc/>
		public float popupHeightOffset
		{
			get => m_popupHeightOffset;
			private set => m_popupHeightOffset = value;
		}

		/// <inheritdoc/>
		public Transform popupTarget { get; set; }
		#endregion

		private void Awake()
		{
			if (Camera.main != null)
			{
				SetTargets(Camera.main.transform);
			}
				
			LookAt();
		}

		private void OnEnable()
		{
			if (showOnEnable)
			{
                LookAt();
			}
		}

		/// <summary>
		/// Sets the value of <see cref="lookAtTarget"/> and <see cref="popupTarget"/>
		/// to <paramref name="target"/>.
		/// </summary>
		/// <param name="target">The target of the <see cref="LookAt"/> and
		/// <see cref="PopUp"/> methods.</param>
		public void SetTargets(Transform target)
		{
			lookAtTarget = target;
			popupTarget = target;
		}

		#region - ILookAt Methods -
		/// <inheritdoc/>
		public void LookAt()
		{
			gameObject.transform.LookAt(lookAtTarget);
			if (flip)
			{
				gameObject.transform.Rotate(new Vector3(0, 180, 0));
			}
		}
		#endregion

		#region - IPopUp Methods -
		/// <inheritdoc/>
		public void PopUp()
		{
			Vector3 pos = Positioning.PlaceInFrontOfTarget(popupTarget, m_popupDistance, m_popupHeightOffset);
			transform.position = pos;
			LookAt();
		}
		#endregion
	}
}