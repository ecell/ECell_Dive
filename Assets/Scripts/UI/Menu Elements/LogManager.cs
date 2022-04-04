using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ECellDive.Utility;

namespace ECellDive
{
    namespace UI
    {
        /// <summary>
        /// Class handling the UI gameobject used to visualize
        /// the log in app at runtime.
        /// </summary>
        public class LogManager : MonoBehaviour
        {
            [Header("General References")]
            public OptimizedVertScrollList refMessageScrollList;
            //public GameObject refMessageItemPrefab;
            public TextMeshProUGUI refMessageSpace;

            [Header("Mutators")]
            public Toggle toggleAll;
            
            [Tooltip(
                "Must be in the same order as " +
                "LogSystem.MessageTypes")]
            public Toggle[] toggleIndividuals;
            public Color[] messageTypeColors;

            private bool[] messagesActivity;

            private List<GameObject> messages;

            // Start is called before the first frame update
            void Start()
            {
                LogSystem.refLogManager = this;

                messagesActivity = new bool[toggleIndividuals.Length];
                for(int i = 0; i < toggleIndividuals.Length; i++)
                {
                    messagesActivity[i] = true;
                }

                messages = new List<GameObject>();
                if (LogSystem.recordedMessages != null)//initialize with the pre-filled messages
                {
                    foreach (LogSystem.Message _msg in LogSystem.recordedMessages)
                    {
                        RecordMessage(_msg);
                    }
                }

                gameObject.SetActive(false);
            }

            /// <summary>
            /// Public interface to add a message to the log.
            /// </summary>
            public void AddMessage(LogSystem.MessageTypes _type, string _content)
            {
                LogSystem.Message msg = LogSystem.GenerateMessage(_type, _content);
                LogSystem.RecordMessage(msg);
                RecordMessage(msg);
                DrawMessageList();
            }

            /// <summary>
            /// Logic to display the whole content of a message.
            /// </summary>
            /// <remarks>
            /// Called back when interacting with the UI element teasing the message.
            /// </remarks>
            /// <param name="_content">The TextMeshProUGUI component containing the
            /// message to display.</param>
            public void DisplayInMessageSpace(TextMeshProUGUI _content)
            {
                refMessageSpace.text = _content.text;
                refMessageSpace.color = _content.color;
            }

            /// <summary>
            /// Set the visibility of each message teaser depending on the global
            /// visibility of its associated type (see <seealso cref="LogSystem.MessageTypes"/>)
            /// </summary>
            public void DrawMessageList()
            {
                for (int i = 0; i < LogSystem.recordedMessages.Count; i++)
                {
                    switch (messagesActivity[(int)LogSystem.recordedMessages[i].type])
                    {
                        case true:
                            messages[i].SetActive(true);
                            break;

                        case false:
                            messages[i].SetActive(false);
                            break;
                    }
                }
            }

            /// <summary>
            /// Forces to switch the visibility of every message of every type
            /// (see <seealso cref="LogSystem.MessageTypes"/>).
            /// </summary>
            /// <remarks>
            /// Called back when interacting with a UI element
            /// </remarks>
            public void ForceAllMessagesActivity()
            {
                for (int i = 0; i < toggleIndividuals.Length; i++)
                {
                    messagesActivity[i] = toggleAll.isOn;
                }

                foreach (Toggle _msgTypeToggle in toggleIndividuals)
                {
                    _msgTypeToggle.isOn = toggleAll.isOn;
                }
                DrawMessageList();
            }

            /// <summary>
            /// Instantiate the new GUI element that will tease the content of the message.
            /// </summary>
            private void RecordMessage(LogSystem.Message _msg)
            {
                GameObject newMsg = refMessageScrollList.AddItem();
                newMsg.SetActive(true);
                TextMeshProUGUI msgTMP = newMsg.GetComponentInChildren<TextMeshProUGUI>();
                if (msgTMP != null)
                {
                    msgTMP.text = _msg.content;
                    msgTMP.color = messageTypeColors[(int)_msg.type];
                }
                messages.Add(newMsg);

                refMessageScrollList.UpdateScrollList();
            }

            /// <summary>
            /// Switches the visibility of every message of a specific message type
            /// (see <seealso cref="LogSystem.MessageTypes"/>).
            /// </summary>
            public void UpdateMessageActivity(int _typeIndex)
            {
                messagesActivity[_typeIndex] = toggleIndividuals[_typeIndex].isOn;
                DrawMessageList();
            }
        }
    }
}

