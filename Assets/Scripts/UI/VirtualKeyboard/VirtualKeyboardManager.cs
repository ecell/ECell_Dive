using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ECellDive.Interfaces;
using ECellDive.Utility;

namespace ECellDive
{
    namespace UI
    {
        /// <summary>
        /// A struct to encapsulate the virtual keyboards sub-layouts
        /// </summary>
        [System.Serializable]
        public struct VirtualKeyBoardData
        {
            public Canvas LowerCaseVK;
            public Canvas UpperCaseVK;
            public Canvas NumAndSignsVK;
        }
        
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
                }
            }

            public void Hide()
            {
                UnsetTargetInputField();
                gameObject.SetActive(false);
            }

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
                }
            }

            public void Show()
            {
                gameObject.SetActive(true);
                gameObject.transform.parent.GetComponent<IPopUp>().PopUp();
            }

            /// <summary>
            /// Interfgace to display the Lower Case sub-layout of the
            /// <see cref="VirtualKeyBoardData"> struct.
            /// </summary>
            public void SwitchToLowerCaseVK()
            {
                virtualKeyBoardDatas[activeVKSet].LowerCaseVK.enabled = true;
                virtualKeyBoardDatas[activeVKSet].UpperCaseVK.enabled = false;
                virtualKeyBoardDatas[activeVKSet].NumAndSignsVK.enabled = false;
            }

            /// <summary>
            /// Interfgace to display the Num and Signs sub-layout of the
            /// <see cref="VirtualKeyBoardData"> struct.
            /// </summary>
            public void SwitchToNumAndSignsVK()
            {
                virtualKeyBoardDatas[activeVKSet].LowerCaseVK.enabled = false;
                virtualKeyBoardDatas[activeVKSet].UpperCaseVK.enabled = false;
                virtualKeyBoardDatas[activeVKSet].NumAndSignsVK.enabled = true;
            }

            /// <summary>
            /// Interfgace to display the Upper Case sub-layout of the
            /// <see cref="VirtualKeyBoardData"> struct.
            /// </summary>
            public void SwitchToUpperCaseVK()
            {
                virtualKeyBoardDatas[activeVKSet].LowerCaseVK.enabled = false;
                virtualKeyBoardDatas[activeVKSet].UpperCaseVK.enabled = true;
                virtualKeyBoardDatas[activeVKSet].NumAndSignsVK.enabled = false;
            }

            /// <summary>
            /// Focuses the attention of the virtual keyboard on <paramref name="_targetInputField"/><
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
}

