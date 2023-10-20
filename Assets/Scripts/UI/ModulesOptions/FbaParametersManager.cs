using UnityEngine;

namespace ECellDive.UI
{
	/// <summary>
	/// Manages the GUI elements that are part of the option panel
	/// of the HttpServerFbaAnalysisModule.
	/// </summary>
	public class FbaParametersManager : MonoBehaviour
	{
		/// <summary>
		/// The reference to the color picker to select the color
		/// for the low fluxes.
		/// </summary>
		public ButtonColorPicker fluxLowerBoundColorPicker;

		/// <summary>
		/// The reference to the color picker to select the color
		/// for the high fluxes.
		/// </summary>
		public ButtonColorPicker fluxUpperBoundColorPicker;

		/// <summary>
		/// The reference to the slider to select the lower bound
		/// clamp value for the fluxes.
		/// </summary>
		public SliderValueControlManager fluxLowerBoundSlider;

		/// <summary>
		/// The reference to the slider to select the upper bound
		/// clamp value for the fluxes.
		/// </summary>
		public SliderValueControlManager fluxUpperBoundSlider;

		/// <summary>
		/// Synchronises the GUIs with the min and max values
		/// </summary>
		/// <param name="_minValue">Min flux value in the whole network.</param>
		/// <param name="_maxValue">Max flux value in the whole network.</param>
		public void SetFluxValueControllersBounds(float _minValue, float _maxValue)
		{
			fluxLowerBoundSlider.slider.minValue = _minValue;
			fluxLowerBoundSlider.slider.maxValue = _maxValue;

			//by default we set the lower bound to min value
			fluxLowerBoundSlider.slider.value = _minValue;
			fluxLowerBoundSlider.UpdateInputFieldValue();

			fluxUpperBoundSlider.slider.minValue = _minValue;
			fluxUpperBoundSlider.slider.maxValue = _maxValue;

			//by default we set the upper bound to max value
			fluxUpperBoundSlider.slider.value = _maxValue;
			fluxUpperBoundSlider.UpdateInputFieldValue();
		}
	}
}
