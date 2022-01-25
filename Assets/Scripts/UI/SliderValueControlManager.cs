using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ECellDive
{
    namespace UI
    {
        public class SliderValueControlManager : MonoBehaviour
        {
            public Slider slider;
            public TMP_InputField inputField;

            public void DecrementSliderValue(float _inc = 0.1f)
            {
                slider.value -= _inc;
                UpdateInputFieldValue();
            }
            public void IncrementSliderValue(float _inc = 0.1f)
            {
                slider.value += _inc;
                UpdateInputFieldValue();
            }

            public void UpdateInputFieldValue()
            {
                inputField.text = $"{slider.value:F2}";
            }

            public void UpdateSliderValue()
            {
                slider.value = Convert.ToSingle(inputField.text);
            }
        }
    }
}


