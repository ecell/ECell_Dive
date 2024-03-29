using UnityEngine;
using TMPro;
using ECellDive.Utility.PlayerComponents;

namespace ECellDive.UI
{
	/// <summary>
	/// Links a Text Mesh Pro Input field to the virtual keyboard of the scene.
	/// </summary>
	[RequireComponent(typeof(TMP_InputField))]
	public class VirtualKeyboardLinker : MonoBehaviour
	{
		/// <summary>
		/// The reference of the component defining the virtual keyboard of the scene.
		/// </summary>
		private VirtualKeyboardManager refVKManager;

		private void Start()
		{
			if (StaticReferencer.Instance != null)
			{
				GetSetVKManager();
			}
		}

		/// <summary>
		/// Gets the value of <see cref="ECellDive.Utility.PlayerComponents.StaticReferencer.refVirtualKeyboard"/>
		/// and sets the value of <see cref="refVKManager"/> from it.
		/// </summary>
		public void GetSetVKManager()
		{
			refVKManager = StaticReferencer.Instance.refVirtualKeyboard.GetComponent<VirtualKeyboardManager>();
		}

		/// <summary>
		/// The method to add in the UnityEvent OnSelect of the TMP_InputField.
		/// It focuses the virtual keyboard output on the input field text.
		/// </summary>
		public void OnSelect()
		{
			TMP_InputField targetInputField = GetComponent<TMP_InputField>();
				
			if (!refVKManager.IsAlreadySelected(targetInputField))
			{
				refVKManager.Show();
				refVKManager.SetTargetInputField(GetComponent<TMP_InputField>());
			}
		}
	}
}