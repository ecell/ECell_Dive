using UnityEngine;

namespace ECellDive.UI
{
	/// <summary>
	/// Class to link the input mode of the controllers to the content of the information
	/// tags. When the input mode changes, the information tags are updated accordingly.
	/// It also provides a method to hide all information tags at once.
	/// </summary>
	public class ContextualHelpManager : MonoBehaviour
	{
		/// <summary>
		/// The reference to the gameobject encapsulating the information tags on
		/// the left controller.
		/// </summary>
		public GameObject leftControllerModel;

		/// <summary>
		/// The reference to the gameobject encapsulating the information tags on
		/// the right controller.
		/// </summary>
		public GameObject rightControllerModel;

		/// <summary>
		/// The array of information tag managers on the left controller.
		/// </summary>
		private InfoTagManager[] leftInfoTagManagers;

		/// <summary>
		/// The array of information tag managers on the right controller.
		/// </summary>
		private InfoTagManager[] rightInfoTagManagers;

		private void Awake()
		{
			leftInfoTagManagers = leftControllerModel.GetComponentsInChildren<InfoTagManager>();
			rightInfoTagManagers = rightControllerModel.GetComponentsInChildren<InfoTagManager>();
		}

		/// <summary>
		/// Changes the information tags on the left controller to the input mode
		/// indicated by <paramref name="_controlModeID"/>.
		/// </summary>
		/// <param name="_controlModeID">
		/// The ID of the input mode to switch to.
		/// </param>
		public void BroadcastControlModeSwitchToLeftController(int _controlModeID)
		{
			foreach (InfoTagManager _infoTag in leftInfoTagManagers)
			{
				_infoTag.SwitchControlMode(_controlModeID);
			}
		}

		/// <summary>
		/// Changes the information tags on the right controller to the input mode
		/// indicated by <paramref name="_controlModeID"/>.
		/// </summary>
		/// <param name="_controlModeID">
		/// The ID of the input mode to switch to.
		/// </param>
		public void BroadcastControlModeSwitchToRightController(int _controlModeID)
		{
			foreach (InfoTagManager _infoTag in rightInfoTagManagers)
			{
				_infoTag.SwitchControlMode(_controlModeID);
			}
		}

		/// <summary>
		/// Hides all information tags on both controllers.
		/// </summary>
		public void ContextualHelpGlobalHide()
		{
			foreach (InfoTagManager _infoTag in leftInfoTagManagers)
			{
				_infoTag.GlobalHide();
			}
			foreach (InfoTagManager _infoTag in rightInfoTagManagers)
			{
				_infoTag.GlobalHide();
			}
		}
	}
}

