using UnityEngine;
using UnityEngine.UI;

namespace ECellDive.Utility
{
    [System.Serializable]
    public enum PinStatus { ToPlayer, ToWorld }

    /// <summary>
    /// A component with the logic to manipulate the parent of the gameobject it is attached to.
    /// </summary>
    public class PinSystem : MonoBehaviour
    {
        public PinStatus pinStatus;
        public GameObject goToPin;

        public GameObject pinnedButton;
        public GameObject unpinnedButton;

        /// <summary>
        /// Updates the parent of the gamobject depending on the value of <see cref="pinStatus"/>.
        /// </summary>
        public void Pin()
        {
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

        /// <summary>
        /// Pins <see cref="goToPin"/> to the gameobject
        /// defined by <paramref name="_pinData"/>.
        /// </summary>
        /// <param name="_pinData">The description of the future parent gameobject.</param>
        public void PinTo(PinData _pinData)
        {
            pinStatus = _pinData.pinTarget;
            Pin();
        }

    }
}
