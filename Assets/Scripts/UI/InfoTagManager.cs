using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;


namespace ECellDive
{
    namespace UI
    {
        public class InfoTagManager : InfoDisplayManager
        {
            [Serializable]
            public struct ButtonTag
            {
                public enum InputActionTime { started, performed, canceled, onEnable, onDisable}
                public InputActionTime actionTime;
                public InputActionReference refInputAction;
                public UnityEvent refCallback;
            }
            
            [TextArea] public string[] content;
            public int currentContentIndex;

            public ButtonTag[] ButtonTagEffects;

            private void Awake()
            {
                foreach (ButtonTag _buttonTag in ButtonTagEffects)
                {
                    switch (_buttonTag.actionTime)
                    {
                        case (ButtonTag.InputActionTime.started):
                            _buttonTag.refInputAction.action.started += e => ButtonTagCallBack(_buttonTag.refCallback);
                            break;
                        case (ButtonTag.InputActionTime.performed):
                            _buttonTag.refInputAction.action.performed += e => ButtonTagCallBack(_buttonTag.refCallback);
                            break;
                        case (ButtonTag.InputActionTime.canceled):
                            _buttonTag.refInputAction.action.canceled += e => ButtonTagCallBack(_buttonTag.refCallback);
                            break;
                    }
                }
            }

            private void ButtonTagCallBack(UnityEvent _callbackEvent)
            {
                _callbackEvent.Invoke();
            }

            private void OnEnable()
            {
                foreach (ButtonTag _buttonTag in ButtonTagEffects)
                {
                    _buttonTag.refInputAction.action.Enable();
                    if (_buttonTag.actionTime == ButtonTag.InputActionTime.onEnable)
                    {
                        ButtonTagCallBack(_buttonTag.refCallback);
                    }
                }
            }

            private void OnDisable()
            {
                foreach (ButtonTag _buttonTag in ButtonTagEffects)
                {
                    _buttonTag.refInputAction.action.Disable();
                    if (_buttonTag.actionTime == ButtonTag.InputActionTime.onDisable)
                    {
                        ButtonTagCallBack(_buttonTag.refCallback);
                    }
                }
            }

            public void SetText(int _contentIndex)
            {
                currentContentIndex = _contentIndex;
                refInfoTextMesh.text = content[currentContentIndex];
            }

            private void Start()
            {
                if (content.Length == 0)
                {
                    content = new string[] { "Information" };
                }
                refInfoTextMesh.text = content[currentContentIndex];

                if (hideOnStart)
                {
                    Hide();
                }
            }

            /// <summary>
            /// Changes the content of the Text UI by rotating the indeces
            /// over the content array.
            /// </summary>
            public void TextRotation()
            {
                if (++currentContentIndex >= content.Length)
                {
                    currentContentIndex = 0;
                }
                refInfoTextMesh.text = content[currentContentIndex];
            }
        }
    }
}

