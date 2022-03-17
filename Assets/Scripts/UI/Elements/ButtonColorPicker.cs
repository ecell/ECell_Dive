using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using HSVPicker;
using ECellDive.SceneManagement;


namespace ECellDive
{
    namespace UI
    {
        [RequireComponent(typeof(Button))]
        public class ButtonColorPicker : MonoBehaviour
        {
            public Button button;
            public UnityEvent OnColorChange;
            private int registrationID;
            private ColorPicker picker;

            private void Awake()
            {
                picker = ScenesData.refSceneManagerMonoBehaviour.refColorPicker;
                picker.onValueChanged.AddListener(ColorChanged);
                registrationID = picker.nbTargetsRegistered;
                picker.nbTargetsRegistered++;
            }

            private void OnDestroy()
            {
                picker.onValueChanged.RemoveListener(ColorChanged);
                picker.nbTargetsRegistered--;
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

                    OnColorChange.Invoke();
                }
            }

            public void OpenColorPicker()
            {
                picker.transform.parent.gameObject.SetActive(true);
                InformColorPickerOfTarget();
                SetPickerColor();
            }

            private void InformColorPickerOfTarget()
            {
                picker.registratedTarget = registrationID;
            }

            private void SetPickerColor()
            {
                picker.AssignColor(button.colors.normalColor);
            }
        }
    }
}
