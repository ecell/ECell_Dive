using UnityEngine;
using ECellDive.Utility.PlayerComponents;
using ECellDive.Utility.Data.PinSystem;

namespace ECellDive.Utility
{
	/// <summary>
	/// A component with the logic to manipulate the parent of the gameobject it is attached to.
	/// </summary>
	public class PinSystem : MonoBehaviour
	{
		/// <summary>
		/// The enum informing about the current parent of the gameobject.
		/// </summary>
		public PinStatus pinStatus;

		/// <summary>
		/// The reference to the gameobject that will be pinned.
		/// </summary>
		public GameObject goToPin;

		/// <summary>
		/// The reference to the button that will be displayed when the gameobject is pinned.
		/// </summary>
		public GameObject pinnedButton;

		/// <summary>
		/// The reference to the button that will be displayed when the gameobject is not pinned.
		/// </summary>
		public GameObject unpinnedButton;

		/// <summary>
		/// Pins <see cref="goToPin"/> to the gameobject
		/// defined by <paramref name="_pinData"/>.
		/// </summary>
		/// <param name="_pinData">The description of the future parent gameobject.</param>
		public void PinTo(PinData _pinData)
		{
			pinStatus = _pinData.pinTarget;
			switch (pinStatus)
			{
				case PinStatus.ToPlayer:
					goToPin.transform.SetParent(StaticReferencer.Instance.refInternalObjectContainer.transform);
					pinnedButton.SetActive(true);
					unpinnedButton.SetActive(false);
					break;

				case PinStatus.ToWorld:
					goToPin.transform.SetParent(StaticReferencer.Instance.refExternalObjectContainer.transform);
					unpinnedButton.SetActive(true);
					pinnedButton.SetActive(false);
					break;
			}
		}
	}
}

