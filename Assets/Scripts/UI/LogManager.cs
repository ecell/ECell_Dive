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
            public Transform refMessageListContent;
            public GameObject refMessageItemPrefab;
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
                if (LogSystem.recordedMessages != null)
                {
                    for (int i = 0; i<LogSystem.recordedMessages.Count; i++)
                    {
                        RecordMessage(LogSystem.recordedMessages[i].type,
                                      LogSystem.recordedMessages[i].content);
                    }
                }

                if (messages.Count == 0)
                {
                    AddMessage(LogSystem.MessageTypes.Debug, "This is a Debug message");
                    AddMessage(LogSystem.MessageTypes.Trace, "This is a Trace message");
                    AddMessage(LogSystem.MessageTypes.Ping, "This is a ping message");
                    AddMessage(LogSystem.MessageTypes.Errors, "This is an error message");
                }
            }

            /// <summary>
            /// Public interface to add a message to the log.
            /// </summary>
            public void AddMessage(LogSystem.MessageTypes _type, string _content)
            {
                LogSystem.RecordMessage(_type, _content);
                RecordMessage(_type, _content);
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
                    LogSystem.MessageTypes typeCurrentMsg = LogSystem.recordedMessages[i].type;
                    switch (messagesActivity[(int)typeCurrentMsg])
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
            /// <param name="_type"></param>
            /// <param name="_content"></param>
            private void RecordMessage(LogSystem.MessageTypes _type, string _content)
            {
                GameObject newMsg = Instantiate(refMessageItemPrefab, refMessageListContent);
                newMsg.SetActive(true);
                TextMeshProUGUI msgTMP = newMsg.GetComponentInChildren<TextMeshProUGUI>();
                if (msgTMP != null)
                {
                    msgTMP.text = _content;
                    msgTMP.color = messageTypeColors[(int)_type];
                }
                messages.Add(newMsg);
            }

            /// <summary>
            /// Switches the visibility of every message of a specific message type
            /// (see <seealso cref="LogSystem.MessageTypes"/>).
            /// </summary>
            /// <param name="_typeIndex"></param>
            public void UpdateMessageActivity(int _typeIndex)
            {
                messagesActivity[_typeIndex] = toggleIndividuals[_typeIndex].isOn;
                DrawMessageList();
            }
        }
    }
}

