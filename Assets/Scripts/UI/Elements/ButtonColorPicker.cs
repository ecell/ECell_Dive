using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using HSVPicker;
using ECellDive.Interfaces;
using ECellDive.Utility;
using ECellDive.Utility.PlayerComponents;

namespace ECellDive.UI
{
	/// <summary>
	/// Associates a color picker to a button. This way
	/// clicking on the button will open the color picker.
	/// </summary>
	[RequireComponent(typeof(Button))]
	public class ButtonColorPicker : MonoBehaviour
	{
		/// <summary>
		/// The button that will open the color picker.
		/// </summary>
		public Button button;

		/// <summary>
		/// The event that will be called when the color of the color picker changes.
		/// </summary>
		public UnityEvent OnColorChange;

		/// <summary>
		/// The ID of the button. This is used to know if the color picker should
		/// update the color of the button or not. The ID is simply the number of
		/// targets registered in the color picker when the button is created.
		/// </summary>
		private int registrationID;

		/// <summary>
		/// Reference to the color picker.
		/// </summary>
		private ColorPicker picker;

		private void Awake()
		{
			picker = StaticReferencer.Instance.refColorPicker;
			picker.onValueChanged.AddListener(ColorChanged);
			registrationID = picker.nbTargetsRegistered;
			picker.nbTargetsRegistered++;
		}

		private void OnDestroy()
		{
			picker.onValueChanged.RemoveListener(ColorChanged);
			picker.nbTargetsRegistered--;
		}

		/// <summary>
		/// Updates the color of the button and invokes the
		/// <see cref="OnColorChange"/> event.
		/// It is called back by the color picker when its color changes.
		/// </summary>
		/// <param name="newColor">
		/// The new color of the button.
		/// </param>
		private void ColorChanged(Color newColor)
		{
			if (registrationID == picker.registratedTarget)
			{
				ColorBlock colors = button.colors;
				colors.normalColor = newColor;
				colors.highlightedColor = newColor;
				colors.pressedColor = newColor;
				button.colors = colors;

				OnColorChange.Invoke();
			}
		}

		/// <summary>
		/// Opens the color picker in front of the camera.
		/// It is called back when <see cref="button"/> is pressed.
		/// </summary>
		public void OpenColorPicker()
		{
			picker.transform.parent.gameObject.SetActive(true);
			Vector3 pos = Positioning.PlaceInFrontOfTarget(Camera.main.transform, 1.5f, 0.2f);
			picker.transform.parent.transform.position = pos;
			picker.transform.parent.gameObject.GetComponent<ILookAt>().LookAt();
			InformColorPickerOfTarget();
			SetPickerColor();
		}

		/// <summary>
		/// Informs ths color picker that its target is <see cref="registrationID"/>.
		/// </summary>
		private void InformColorPickerOfTarget()
		{
			picker.registratedTarget = registrationID;
		}

		/// <summary>
		/// Assigns the normal color of the button to the color picker.
		/// </summary>
		private void SetPickerColor()
		{
			picker.AssignColor(button.colors.normalColor);
		}
	}
}
