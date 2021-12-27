using System;
using System.Collections.Generic;
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

            public static List<Message> recordedMessages = new List<Message>
            {
                new Message()
                {
                    type = MessageTypes.Trace,
                    content = "This is a Trace message"
                },
                new Message()
                {
                    type = MessageTypes.Ping,
                    content = "This is a Ping message"
                },
                new Message()
                {
                    type= MessageTypes.Debug,
                    content = "This is a Debug message"
                },
                new Message()
                {
                    type= MessageTypes.Errors,
                    content = "This is an Error message"
                }
            };

            public static Message GenerateMessage(MessageTypes _type, string _content)
            {
                Message msg = new Message();
                msg.type = _type;

                if (_content.Length > 450)
                {
                    msg.content = _content.Substring(0, 450)+ 
                                    "..... \n" +
                                    "-- TRUNCATED message to avoid frame drops --";
                }
                else
                {
                    msg.content= _content;
                }

                return msg;
            }

            /// <summary>
            /// Add a message info to the list of messages
            /// </summary>
            /// <param name="_msg"></param>
            public static void RecordMessage(Message _msg)
            {
                recordedMessages.Add(_msg);
            }

            /// <summary>
            /// Adds a message info to the list of messages.
            /// The message will be truncated to 450 characters in the
            /// case that it is longer in order to avoid frame drops.
            /// </summary>
            /// <param name="_type"></param>
            /// <param name="_content"></param>
            //public static void RecordMessage(MessageTypes _type, string _content)
            //{
            //    Message msg = new Message{type = _type,
            //                              content = tr_content
            //    };
            //    RecordMessage(msg);
            //}
        }
    }
}

