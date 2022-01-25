using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ECellDive
{
    namespace UI
    {
        public class FbaParametersManager : MonoBehaviour
        {
            public SliderValueControlManager fluxLowerBoundSlider;
            public SliderValueControlManager fluxUpperBoundSlider;

            public void SetFluxeValueControllersBounds(float _minValue, float _maxValue)
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
