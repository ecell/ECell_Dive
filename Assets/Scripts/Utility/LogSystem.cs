using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECellDive.UI;

namespace ECellDive
{
    namespace Utility
    {
        /// <summary>
        /// Class handling the recording of messages to display
        /// in the log.
        /// </summary>
        /// <remarks>
        /// It is static to be accessible from any rooms.
        /// </remarks>
        public static class LogSystem
        {
            public static LogManager refLogManager;

            [Serializable]
            public enum MessageTypes { Trace, Ping, Debug, Errors }

            [Serializable]
            public struct Message
            {
                public MessageTypes type;
                public string content;
            }

            public static List<Message> recordedMessages;

            public static void RecordMessage(Message _msg)
            {
                if (recordedMessages == null)
                {
                    recordedMessages = new List<Message>();
                }
                recordedMessages.Add(_msg);
            }

            public static void RecordMessage(MessageTypes _type, string _content)
            {
                Message msg = new Message{type = _type,
                                          content = _content};
                RecordMessage(msg);
            }
        }
    }
}

