using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

namespace ECellDive.UI
{
	/// <summary>
	/// Manages the synchronisation within a GUI element that links a
	/// slider, with an input field and two buttons (+ and -)
	/// </summary>
	public class SliderValueControlManager : MonoBehaviour
	{
		/// <summary>
		/// The reference to the slider linked to the input field.
		/// </summary>
		public Slider slider;

		/// <summary>
		/// The reference to the input field linked to the slider.
		/// </summary>
		public TMP_InputField inputField;

		/// <summary>
		/// An event to be called back when the slider value changes.
		/// </summary>
		public UnityEvent<float> OnValueChanged;

		/// <summary>
		/// To be called back when clicking on the - button.
		/// </summary>
		/// <param name="_inc">Decrement value</param>
		public void DecrementSliderValue(float _inc = 0.1f)
		{
			slider.value -= _inc;
			UpdateInputFieldValue();
		}

		/// <summary>
		/// To be called back when clicking on the + button.
		/// </summary>
		/// <param name="_inc">Decrement value</param>
		public void IncrementSliderValue(float _inc = 0.1f)
		{
			slider.value += _inc;
			UpdateInputFieldValue();
		}

		/// <summary>
		/// Updates displayed value in the input field.
		/// </summary>
		public void UpdateInputFieldValue()
		{
			inputField.text = $"{slider.value:F2}";
			OnValueChanged?.Invoke(slider.value);
		}

		/// <summary>
		/// Synchronises the slider's with the displayed number
		/// in the input field.
		/// </summary>
		public void UpdateSliderValue()
		{
			slider.value = Convert.ToSingle(inputField.text);
		}
	}
}


