using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using ECellDive.Utility;

namespace ECellDive
{
    namespace UI
    {
        [RequireComponent(typeof(TMP_InputField))]
        public class VirtualKeyboardLinker : MonoBehaviour
        {
            private GameObject refVKGO;

            private void Start()
            {
                refVKGO = GameObject.FindGameObjectWithTag("VirtualKeyboard");
                if (refVKGO == null)
                {
                    LogSystem.refLogManager.AddMessage(LogSystem.MessageTypes.Debug,
                        $"Could not link {gameObject.name} with any virtual keyboard in the scene.");
                }
            }

            public void OnSelect()
            {
                TMP_InputField targetInputField = GetComponent<TMP_InputField>();
                refVKGO.GetComponent<Canvas>().enabled = true;
                refVKGO.GetComponentInChildren<BoxCollider>().enabled = true;
                refVKGO.GetComponent<VirtualKeyboardManager>().SetTargetInputField(GetComponent<TMP_InputField>());
            }
        }
    }
}

