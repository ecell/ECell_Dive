using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace ECellDive
{
    namespace UI
    {
        [System.Serializable]
        public struct VirtualKeyBoardData
        {
            public Canvas LowerCaseVK;
            public Canvas UpperCaseVK;
            public Canvas NumAndSignsVK;
        }

        public class VirtualKeyboardManager : MonoBehaviour
        {
            private TMP_InputField refTargetInputField;

            private int activeVKSet = 0;
            public List<VirtualKeyBoardData> virtualKeyBoardDatas;

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

            public void SwitchToLowerCaseVK()
            {
                virtualKeyBoardDatas[activeVKSet].LowerCaseVK.enabled = true;
                virtualKeyBoardDatas[activeVKSet].UpperCaseVK.enabled = false;
                virtualKeyBoardDatas[activeVKSet].NumAndSignsVK.enabled = false;
            }

            public void SwitchToNumAndSignsVK()
            {
                virtualKeyBoardDatas[activeVKSet].LowerCaseVK.enabled = false;
                virtualKeyBoardDatas[activeVKSet].UpperCaseVK.enabled = false;
                virtualKeyBoardDatas[activeVKSet].NumAndSignsVK.enabled = true;
            }

            public void SwitchToUpperCaseVK()
            {
                virtualKeyBoardDatas[activeVKSet].LowerCaseVK.enabled = false;
                virtualKeyBoardDatas[activeVKSet].UpperCaseVK.enabled = true;
                virtualKeyBoardDatas[activeVKSet].NumAndSignsVK.enabled = false;
            }

            public void SetTargetInputField(TMP_InputField _targetInputField)
            {
                refTargetInputField = _targetInputField;
            }

            public void UnsetTargetInputField()
            {
                refTargetInputField = null;
            }
        }
    }
}

