using System;
using System.Collections;
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

            public void AddMessage(LogSystem.MessageTypes _type, string _content)
            {
                LogSystem.RecordMessage(_type, _content);
                RecordMessage(_type, _content);
            }

            public void DisplayInMessageSpace(TextMeshProUGUI _content)
            {
                refMessageSpace.text = _content.text;
                refMessageSpace.color = _content.color;
            }

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

            public void ForceAllMessagesActivity()
            {
                for (int i = 0; i < toggleIndividuals.Length; i++)
                {
                    messagesActivity[i] = true;
                }

                foreach (Toggle _msgTypeToggle in toggleIndividuals)
                {
                    _msgTypeToggle.isOn = toggleAll.isOn;
                }
                DrawMessageList();
            }

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

            public void UpdateMessageActivity(int _typeIndex)
            {
                Debug.Log($"messagesActivity[_typeIndex]: {messagesActivity[_typeIndex]}");
                Debug.Log($"toggleIndividuals[_typeIndex].isOn: {toggleIndividuals[_typeIndex].isOn}");
                Debug.Log($"_typeIndex: {_typeIndex}");
                messagesActivity[_typeIndex] = toggleIndividuals[_typeIndex].isOn;
                DrawMessageList();
            }
        }
    }
}

