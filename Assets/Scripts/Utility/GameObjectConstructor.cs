using UnityEngine;
using ECellDive.Interfaces;

namespace ECellDive.Utility
{
	/// <summary>
	/// Spawns a prefab at a given distance in front of the camera.
	/// </summary>
	public class GameObjectConstructor : MonoBehaviour, IPopUp
	{
		/// <summary>
		/// The reference prefab to spawn.
		/// </summary>
		public GameObject refPrefab;

        #region - IPopUp Members -
        /// <summary>
        /// The field of the<see cref="popupDistance"/> property.
        /// </summary>
        [SerializeField] private float m_popupDistance;

		/// <inheritdoc/>
		public float popupDistance
		{
			get => m_popupDistance;
			private set => m_popupDistance = value;
		}

		/// <summary>
		/// The field of the<see cref="popupHeightOffset"/> property.
		/// </summary>
		[SerializeField] private float m_popupHeightOffset;

		/// <inheritdoc/>
		public float popupHeightOffset
		{
			get => m_popupHeightOffset;
			private set => m_popupHeightOffset = value;
		}

		/// <summary>
		/// The field of the<see cref="popupTarget"/> property.
		/// </summary>
		[Tooltip("If popup target is left to null, then the Main Camera's transform is used.")]
		[SerializeField] private Transform m_popupTarget;

		/// <inheritdoc/>
		public Transform popupTarget
		{
			get => m_popupTarget;
			set => m_popupTarget = value;
		}
		#endregion

		/// <summary>
		/// Instantiates the reference prefab at the given distance in front of the camera.
		/// </summary>
		public void Constructor()
		{
			PopUp();
		}

		#region - IPopUp Methods -
		/// <inheritdoc/>
		public void PopUp()
		{
			if (popupTarget == null)
			{
				popupTarget = Camera.main.transform;
			}
			Vector3 pos = Positioning.PlaceInFrontOfTarget(popupTarget, m_popupDistance, m_popupHeightOffset);
			Instantiate(refPrefab, pos, Quaternion.identity);
		}
		#endregion
	}
}

