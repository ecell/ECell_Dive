using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using HSVPicker;
using ECellDive.Interfaces;
using ECellDive.Utility;


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
                Vector3 pos = Positioning.PlaceInFrontOfTarget(Camera.main.transform, 1.5f, 0.2f);
                picker.transform.parent.transform.position = pos;
                picker.transform.parent.gameObject.GetComponent<ILookAt>().LookAt();
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
