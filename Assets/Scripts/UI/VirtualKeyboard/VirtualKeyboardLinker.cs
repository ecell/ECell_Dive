using UnityEngine;
using TMPro;
using ECellDive.SceneManagement;
using ECellDive.Utility;

namespace ECellDive
{
    namespace UI
    {
        /// <summary>
        /// Links a Text Mesh Pro Input field to the virtual keyboard of the scene.
        /// </summary>
        [RequireComponent(typeof(TMP_InputField))]
        public class VirtualKeyboardLinker : MonoBehaviour
        {
            private GameObject refVKGO;

            private void Start()
            {
                //Searches for the Virtual Keyboard in the scene.
                refVKGO = StaticReferencer.Instance.refVirtualKeyboard;
                if (refVKGO == null)
                {
                    LogSystem.refLogManager.AddMessage(LogSystem.MessageTypes.Debug,
                        $"Could not link {gameObject.name} with any virtual keyboard in the scene.");
                }
            }

            /// <summary>
            /// The method to add in the UnityEvent OnSelect of the TMP_InputField.
            /// It focuses the virtual keyboard output on the input field text.
            /// </summary>
            public void OnSelect()
            {
                TMP_InputField targetInputField = GetComponent<TMP_InputField>();
                refVKGO.GetComponent<VirtualKeyboardManager>().Show();
                refVKGO.GetComponent<VirtualKeyboardManager>().SetTargetInputField(GetComponent<TMP_InputField>());
            }
        }
    }
}

