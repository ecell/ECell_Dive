using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using ECellDive.Interfaces;
using ECellDive.Utility.Data;
using ECellDive.Utility.Data.UI;
using ECellDive.Utility.PlayerComponents;

namespace ECellDive.UI
{	
	/// <summary>
	/// Simple virtual keyboard manager for QUERTY layout, with 
	/// symbols, numbers and capital letters.
	/// </summary>
	public class VirtualKeyboardManager : MonoBehaviour
	{
		/// <summary>
		/// Reference to the TMP_InputField currently focused.
		/// </summary>
		private TMP_InputField refTargetInputField;

		/// <summary>
		/// The index of the set of virtual keyboard layout that should be used.
		/// </summary>
		/// <remarks> TO DO: build layouts for AZERTY, or any language
		/// that requires accents.</remarks>
		private int activeVKSet = 0;

		/// <summary>
		/// The list of sub-layouts for QWERTY, AZERTY or "language with accents"
		/// </summary>
		public List<VirtualKeyBoardData> virtualKeyBoardDatas;

		/// <summary>
		/// The press key actions associated with the left or right controllers.
		/// </summary>
		public LeftRightData<InputActionReference> uiPressActions;

		/// <summary>
		/// Booleans to keep track whether the left or right press
		/// actions were performed.
		/// </summary>
		/// <remarks>
		/// Somehow, the Input System API to retrieve this exact
		/// information did not seem to work, so we implemented
		/// our own.
		/// </remarks>
		private LeftRightData<bool> uiPressActionsPressed;

		private void Awake()
		{
			uiPressActionsPressed.right = false;
			uiPressActionsPressed.left = false;

			uiPressActions.right.action.performed += ctx => uiPressActionsPressed.right = true;
			uiPressActions.left.action.performed += ctx => uiPressActionsPressed.left = true;
		}

		private void OnDestroy()
		{
			uiPressActions.right.action.performed -= ctx => uiPressActionsPressed.right = true;
			uiPressActions.left.action.performed -= ctx => uiPressActionsPressed.left = true;
		}

		/// <summary>
		/// Adds the character of the key the VK that was just pressed at the position
		/// of the caret.
		/// </summary>
		/// <param name="_char">The text component containing the string that
		/// should be added.</param>
		public void AddCharToTargetInputField(TMP_Text _char)
		{
			if (refTargetInputField != null)
			{
				string start = refTargetInputField.text.Substring(0,refTargetInputField.caretPosition);
				string end = refTargetInputField.text.Substring(refTargetInputField.caretPosition);
				refTargetInputField.text = start + _char.text + end;
				refTargetInputField.caretPosition = start.Length + _char.text.Length;

				SendHapticImpulse();
			}
		}

		/// <summary>
		/// Hides the virtual keyboard by deactivating the game object.
		/// </summary>
		public void Hide()
		{
			UnsetTargetInputField();
			gameObject.SetActive(false);
		}

		/// <summary>
		/// A utility check to see if an input field (<paramref name="_targetInputField"/>)
		/// is already the target of the virtual keyboard.
		/// </summary>
		/// <param name="_targetInputField">
		/// The input field to check.
		/// </param>
		/// <returns>
		/// True if <paramref name="_targetInputField"/> is equal to <see cref="refTargetInputField"/>, false otherwise.
		/// </returns>
		public bool IsAlreadySelected(TMP_InputField _targetInputField)
		{
			return _targetInputField == refTargetInputField;
		}
			
		/// <summary>
		/// Deletes the character on the left of the position of the caret
		/// </summary>
		public void RemoveCharInTargetInputField()
		{
			if (refTargetInputField != null && refTargetInputField.caretPosition > 0)
			{
				string start = refTargetInputField.text.Substring(0, refTargetInputField.caretPosition-1);
				string end = refTargetInputField.text.Substring(refTargetInputField.caretPosition);
				refTargetInputField.text = start + end;
				refTargetInputField.caretPosition = start.Length;

				SendHapticImpulse();
			}
		}

		/// <summary>
		/// Send a haptic impulse to the left or right controllers.
		/// </summary>
		private void SendHapticImpulse()
		{
			ActionBasedController left = StaticReferencer.Instance.riControllersGO.left.GetComponent<ActionBasedController>();
			ActionBasedController right = StaticReferencer.Instance.riControllersGO.right.GetComponent<ActionBasedController>();

			if (uiPressActionsPressed.left)
			{
				left.SendHapticImpulse(0.25f, 0.1f);
			}
				
			if (uiPressActionsPressed.right)
			{
				right.SendHapticImpulse(0.25f, 0.1f);
			}

			uiPressActionsPressed.right = false;
			uiPressActionsPressed.left = false;
		}

		/// <summary>
		/// Displays the virtual keyboard by activating the game object and
		/// poping it up in front of the user.
		/// </summary>
		public void Show()
		{
			gameObject.SetActive(true);
			gameObject.transform.parent.GetComponent<IPopUp>().PopUp();
		}

		/// <summary>
		/// Interfgace to display the Lower Case sub-layout of the
		/// <see cref="VirtualKeyBoardData"/> struct.
		/// </summary>
		public void SwitchToLowerCaseVK()
		{
			virtualKeyBoardDatas[activeVKSet].LowerCaseVK.enabled = true;
			virtualKeyBoardDatas[activeVKSet].UpperCaseVK.enabled = false;
			virtualKeyBoardDatas[activeVKSet].NumAndSignsVK.enabled = false;

			SendHapticImpulse();
		}

		/// <summary>
		/// Interfgace to display the Num and Signs sub-layout of the
		/// <see cref="VirtualKeyBoardData"/> struct.
		/// </summary>
		public void SwitchToNumAndSignsVK()
		{
			virtualKeyBoardDatas[activeVKSet].LowerCaseVK.enabled = false;
			virtualKeyBoardDatas[activeVKSet].UpperCaseVK.enabled = false;
			virtualKeyBoardDatas[activeVKSet].NumAndSignsVK.enabled = true;

			SendHapticImpulse();
		}

		/// <summary>
		/// Interfgace to display the Upper Case sub-layout of the
		/// <see cref="VirtualKeyBoardData"/> struct.
		/// </summary>
		public void SwitchToUpperCaseVK()
		{
			virtualKeyBoardDatas[activeVKSet].LowerCaseVK.enabled = false;
			virtualKeyBoardDatas[activeVKSet].UpperCaseVK.enabled = true;
			virtualKeyBoardDatas[activeVKSet].NumAndSignsVK.enabled = false;

			SendHapticImpulse();
		}

		/// <summary>
		/// Focuses the attention of the virtual keyboard on <paramref name="_targetInputField"/>
		/// </summary>
		public void SetTargetInputField(TMP_InputField _targetInputField)
		{
			refTargetInputField = _targetInputField;
		}

		/// <summary>
		/// Resets the virtual keyboard focus.
		/// </summary>
		public void UnsetTargetInputField()
		{
			refTargetInputField = null;
		}
	}
}

