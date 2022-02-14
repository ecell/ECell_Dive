using UnityEngine;
using UnityEngine.UI;
using HSVPicker;

namespace ECellDive
{
    namespace UI
    {
        [RequireComponent(typeof(Button))]
        public class ButtonColorPicker : MonoBehaviour
        {
            public ColorPicker picker;
            public Button button;
            public int registrationID;

            private void Awake()
            {
                picker.onValueChanged.AddListener(ColorChanged);
            }

            private void OnDestroy()
            {
                picker.onValueChanged.RemoveListener(ColorChanged);
            }

            public void InformColorPickerOfTarget()
            {
                picker.registratedTarget = registrationID;
            }

            public void SetPickerColor()
            {
                picker.AssignColor(button.colors.normalColor);
            }

            private void ColorChanged(Color newColor)
            {
                if (registrationID == picker.registratedTarget)
                {
                    ColorBlock colors = button.colors;
                    colors.normalColor = newColor;
                    colors.highlightedColor = newColor;
                    colors.pressedColor = newColor;
                    button.colors = colors;
                }
            }
        }

    }
}
