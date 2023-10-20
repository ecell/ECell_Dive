using UnityEngine;
using TMPro;
using ECellDive.Interfaces;
using ECellDive.Utility.PlayerComponents;

namespace ECellDive.UI
{
	/// <summary>
	/// The high level manager of the UI used to validate or cancel the creation
	/// of new groups.
	/// </summary>
	public class GroupsMakingUIManager : MonoBehaviour
	{
		/// <summary>
		/// The reference to the canvas containing the buttons to validate or cancel
		/// the creation of the group.
		/// </summary>
		public GameObject refUICanvas;

		/// <summary>
		/// The reference to the input field used to name the group.
		/// </summary>
		public TMP_InputField refGroupNameInputField;

		/// <summary>
		/// Cancel the creation of the group.
		/// </summary>
		/// <seealso cref="ECellDive.PlayerComponents.GroupsMakingManager.CancelGroup"/>
		public void Cancel()
		{
			StaticReferencer.Instance.groupsMakingManager.CancelGroup();

			refUICanvas.SetActive(false);
		}

		/// <summary>
		/// Manages the visibility of the gameobject <see cref="refUICanvas"/>
		/// containing the canvas of the UI used to validate or cancel the
		/// creation of the group.
		/// </summary>
		public void ManageUIConfirmationCanvas(int _nbMembers)
		{
			if (_nbMembers == 0)
			{
				refUICanvas.SetActive(false);
			}
			else
			{
				refUICanvas.SetActive(true);
				GetComponent<IPopUp>().PopUp();
			}
		}

		/// <summary>
		/// Validate the creation of the group.
		/// Called back when the user presses the validate button.
		/// </summary>
		public void Validate()
		{
			StaticReferencer.Instance.groupsMakingManager.ValidateGroup(refGroupNameInputField.text);
			ManageUIConfirmationCanvas(StaticReferencer.Instance.groupsMakingManager.groupMembers.Count);
			refUICanvas.SetActive(false);
		}
	}
}

