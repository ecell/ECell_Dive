using Unity.Netcode;
using UnityEngine;
using TMPro;
using ECellDive.Interfaces;
using ECellDive.Utility;

namespace ECellDive.UI
{
	/// <summary>
	/// A class to manage an information 2D UI panel that is attached to a
	/// gameobject by a connection line. The info tag can be displayed/hidden,
	/// can look at a target, and can change its text.
	/// </summary>
	public class InfoDisplayManager : MonoBehaviour,
										ILookAt
	{
		/// <summary>
		/// The master gameobject that the information tag is attached to.
		/// </summary>
		public Transform refMaster;

		/// <summary>
		/// The default offset of the information tag from the master gameobject.
		/// </summary>
		public Vector3 defaultPositionOffset;

		/// <summary>
		/// The text mesh to display the information.
		/// </summary>
		public TextMeshProUGUI refInfoTextMesh;

		/// <summary>
		/// The reference to the component that handles the update of the anchor of
		/// the connection line between the information tag and the master gameobject.
		/// </summary>
		public ConnectionAnchorPosition refConnectionAnchorHandler;

		/// <summary>
		/// The reference to the component that handles the update of the positions of
		/// the connection line between the information tag's anchor and the master gameobject.
		/// </summary>
		public LinePositionHandler refConnectionLineHandler;

		/// <summary>
		/// A boolean to control whether the information tag should always look at the player.
		/// </summary>
		public bool alwaysShowInfoToPlayer = false;

		/// <summary>
		/// A boolean to control whether the information tag should be forcefully hidden or not.
		/// </summary>
		[HideInInspector] public bool globalHide = false;

		/// <summary>
		/// A boolean to control whether the information tag should be hidden on start or not.
		/// </summary>
		public bool hideOnStart = false;

		#region - ILookAt Members-
		/// <summary>
		/// The field to the <see cref="flip"/> property.
		/// </summary>
		[SerializeField] private bool m_flip = false;

		/// <inheritdoc/>
		public bool flip
		{
			get => m_flip;
			private set => m_flip = value;
		}

		/// <inheritdoc/>
		public Transform lookAtTarget{ get; set; }
		#endregion


		private void Start()
		{
			if (refMaster == null)
			{
				refMaster = transform.parent;
			}
			transform.position = refMaster.transform.position + defaultPositionOffset;

			if (hideOnStart)
			{
				Hide();
			}

			lookAtTarget = NetworkManager.Singleton.LocalClient.PlayerObject.gameObject.
						GetComponentInChildren<Camera>().transform;
			LookAt();
		}

		private void Update()
		{
			if (alwaysShowInfoToPlayer)
			{
				LookAt();
			}
		}

		/// <summary>
		/// Switches the value of the globalHide field on every call.
		/// If set to false, it resets the visibility of the gameobject
		/// based on the value of hideOnStart.
		/// </summary>
		public void GlobalHide()
		{
			globalHide = !globalHide;
			if (globalHide)
			{
				Hide();
			}
			else
			{
				if (!hideOnStart)
				{
					Show();
				}
			}
		}

		/// <summary>
		/// Hides the Info tag without setting it inactive
		/// </summary>
		protected virtual void Hide()
		{
			gameObject.SetActive(false);
		}

		/// <summary>
		/// Sets the value for <see cref="refInfoTextMesh"/> to the given string.
		/// </summary>
		/// <param name="_text">
		/// The new string to be displayed by <see cref="refInfoTextMesh"/>.
		/// </param>
		public void SetText(string _text)
		{
			refInfoTextMesh.text = _text;
		}

		/// <summary>
		/// Controls the visibility of the gameobject only when
		/// globalHide is set to false.
		/// </summary>
		/// <param name="_show">True => Show, False => Hide</param>
		public void SetVisibility(bool _show)
		{
			if (!globalHide)
			{
				if (_show)
				{
					Show();
				}

				else
				{
					Hide();
				}
			}
		}

		/// <summary>
		/// Shows the info tag without setting it active
		/// </summary>
		protected virtual void Show()
		{
			gameObject.SetActive(true);
		}

		#region - ILookAt Methods-
		/// <inheritdoc/>
		public void LookAt()
		{
			Positioning.UIFaceTarget(gameObject, lookAtTarget);
			refConnectionAnchorHandler.SetClosestCorner();
			refConnectionLineHandler.RefreshLinePositions();
		}
		#endregion
	}
}