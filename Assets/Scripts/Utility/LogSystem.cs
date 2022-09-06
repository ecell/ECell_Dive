using System;
using System.Collections.Generic;
using UnityEngine;
using ECellDive.UI;

namespace ECellDive
{
    namespace Utility
    {
        /// <summary>
        /// The different types of Log messages we handle.
        /// </summary>
        public enum LogMessageTypes { Trace, Ping, Debug, Errors }

        /// <summary>
        /// A struct to hold the data nessecary to handle Log messages
        /// </summary>
        public struct LogMessage
        {
            public int id;
            public string content;
            public RectTransform refUI;
        }

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

            public static List<LogMessage> traceMessages;
            public static List<LogMessage> pingMessages;
            public static List<LogMessage> debugMessages;
            public static List<LogMessage> errorMessages;

            /// <summary>
            /// Public interface to add a message to the log.
            /// </summary>
            public static void AddMessage(LogMessageTypes _type, string _content)
            {
                //Truncating the message string if it's too long to 
                //avoid performance drop on very big messages.
                if (_content.Length > 450)
                {
                    _content = _content.Substring(0, 450) +
                                    "..... \n" +
                                    "-- TRUNCATED message to avoid frame drops --";
                }

                LogMessage msg = new LogMessage
                {
                    id = GetMessageCount(),
                    content = _content,
                    refUI = refLogManager.GenerateMessageUI(_content, _type)
                };

                switch (_type)
                {
                    case LogMessageTypes.Trace:
                        traceMessages.Add(msg);
                        break;

                    case LogMessageTypes.Ping:
                        pingMessages.Add(msg);
                        break;

                    case LogMessageTypes.Debug:
                        debugMessages.Add(msg);
                        break;

                    case LogMessageTypes.Errors:
                        errorMessages.Add(msg);
                        break;
                }
            }

            private static int GetMessageCount()
            {
                return traceMessages.Count +
                       pingMessages.Count +
                       debugMessages.Count +
                       errorMessages.Count;
            }
        }
    }
}

