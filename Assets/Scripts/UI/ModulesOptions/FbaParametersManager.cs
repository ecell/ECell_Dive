using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ECellDive
{
    namespace UI
    {
        /// <summary>
        /// Manages the GUI elements that are part of the option panel
        /// of the HttpServerFbaAnalysisModule.
        /// </summary>
        public class FbaParametersManager : MonoBehaviour
        {
            public ButtonColorPicker fluxLowerBoundColorPicker;
            public ButtonColorPicker fluxUpperBoundColorPicker;
            public SliderValueControlManager fluxLowerBoundSlider;
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
}
