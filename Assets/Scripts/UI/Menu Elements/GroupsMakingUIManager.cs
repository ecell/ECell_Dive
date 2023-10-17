using UnityEngine;
using TMPro;
using ECellDive.Interfaces;
using ECellDive.Utility.PlayerComponents;

namespace ECellDive.UI
{
	public class GroupsMakingUIManager : MonoBehaviour
	{
		public GameObject refUICanvas;
		public TMP_InputField refGroupNameInputField;

		public void Cancel()
		{
			StaticReferencer.Instance.groupsMakingManager.CancelGroup();

			refUICanvas.SetActive(false);
		}

		public void NewGroupUiElement(IHighlightable[] _highlitables)
		{
			//Create a groupUI component
			StaticReferencer.Instance.refGroupsMenu.AddGroupUI(new GroupData
			{
				value = refGroupNameInputField.text,
				color = Random.ColorHSV(),
				members = _highlitables
			},
				0);
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

		public void Validate()
		{
			StaticReferencer.Instance.groupsMakingManager.ValidateGroup();
			ManageUIConfirmationCanvas(StaticReferencer.Instance.groupsMakingManager.groupMembers.Count);
			refUICanvas.SetActive(false);
		}
	}
}

