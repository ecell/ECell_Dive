using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;


namespace ECellDive
{
    namespace UI
    {
        public class InfoTagManager : InfoDisplayManager
        {
            public enum InputActionTime { started, performed, canceled }//, onEnable, onDisable }
            [Serializable]
            public struct TagMutator
            {
                public InputActionTime actionTime;
                public InputActionReference refInputAction;
                public UnityEvent refCallback;
            }
            
            [Serializable]
            public struct Tag
            {
                [TextArea] public string[] content;
                public int currentContentIndex;

                public TagMutator[] mutators;
            }

            public Tag tagGC;
            public Tag tagMvt;
            public Tag tagRBC;

            private Tag currentTag;

            private void Awake()
            {
                AwakeCallBackActions(tagGC);
                AwakeCallBackActions(tagMvt);
                AwakeCallBackActions(tagRBC);
            }

            private void Start()
            {
                if (hideOnStart)
                {
                    Hide();
                }
                target = NetworkManager.Singleton.LocalClient.PlayerObject.gameObject.
                            GetComponentInChildren<Camera>().transform;
            }

            private void AwakeCallBackActions(Tag _tag)
            {
                foreach (TagMutator _tagMut in _tag.mutators)
                {
                    switch (_tagMut.actionTime)
                    {
                        case (InputActionTime.started):
                            _tagMut.refInputAction.action.started += e => ButtonTagCallBack(_tagMut.refCallback);
                            break;
                        case (InputActionTime.performed):
                            _tagMut.refInputAction.action.performed += e => ButtonTagCallBack(_tagMut.refCallback);
                            break;
                        case (InputActionTime.canceled):
                            _tagMut.refInputAction.action.canceled += e => ButtonTagCallBack(_tagMut.refCallback);
                            break;
                    }
                }
            }

            private void ButtonTagCallBack(UnityEvent _callbackEvent)
            {
                _callbackEvent.Invoke();
            }

            protected override void Hide()
            {
                GetComponentInChildren<CanvasGroup>().alpha = 0f;
                refConnectionLineHandler.gameObject.GetComponent<LineRenderer>().enabled = false;
            }

            public void SetText(int _contentIndex)
            {
                currentTag.currentContentIndex = _contentIndex;
                refInfoTextMesh.text = currentTag.content[currentTag.currentContentIndex];
            }

            protected override void Show()
            {
                GetComponentInChildren<CanvasGroup>().alpha = 1f;
                refConnectionLineHandler.gameObject.GetComponent<LineRenderer>().enabled = true;
            }

            public void SwitchControlMode(int _controlModeID)
            {
                switch (_controlModeID)
                {
                    case 0:
                        currentTag = tagGC;
                        break;
                    case 1:
                        currentTag = tagMvt;
                        break;
                    case 2:
                        currentTag = tagRBC;
                        break;
                }
                updateTagText();
            }

            /// <summary>
            /// Changes the content of the Text UI by rotating the indeces
            /// over the content array.
            /// </summary>
            public void TextRotation()
            {
                if (++currentTag.currentContentIndex >= currentTag.content.Length)
                {
                    currentTag.currentContentIndex = 0;
                }
                refInfoTextMesh.text = currentTag.content[currentTag.currentContentIndex];

                updateTagText();
            }

            private void updateTagText()
            {
                if (currentTag.content.Length > 0)
                {              
                    refInfoTextMesh.text = currentTag.content[currentTag.currentContentIndex];
                    Show();

                    if (refInfoTextMesh.text.Length == 0)
                    {
                        Hide();
                    }
                }
                else
                {
                    Hide();
                }
            }
        }
    }
}

