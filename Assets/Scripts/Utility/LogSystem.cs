using System.Collections.Generic;
using UnityEngine;
using ECellDive.UI;

namespace ECellDive.Utility
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
		/// <summary>
		/// An id to identify the message in the log.
		/// </summary>
		public int id;

		/// <summary>
		/// The content of the message.
		/// </summary>
		public string content;

		/// <summary>
		/// The reference to the UI element displaying the message.
		/// </summary>
		public RectTransform refUI;
	}

	/// <summary>
	/// Class handling the recording of messages to display
	/// in the log.
	/// </summary>
	public static class LogSystem
	{
		/// <summary>
		/// The reference to the LogManager component.
		/// </summary>
		public static LogManager refLogManager;

		/// <summary>
		/// The list of message to treat as trace messages.
		/// </summary>
		public static List<LogMessage> traceMessages;

		/// <summary>
		/// The list of message to treat as ping messages.
		/// </summary>
		public static List<LogMessage> pingMessages;

		/// <summary>
		/// The list of message to treat as debug messages.
		/// </summary>
		public static List<LogMessage> debugMessages;

		/// <summary>
		/// The list of message to treat as error messages.
		/// </summary>
		public static List<LogMessage> errorMessages;

		/// <summary>
		/// Public interface to add a message to the log.
		/// </summary>
		public static void AddMessage(LogMessageTypes _type, string _content)
		{
#if !UNITY_EDITOR
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
#endif
		}

		/// <summary>
		/// Counts the total number of messages in the log.
		/// </summary>
		/// <returns>
		/// The sum of size of all the lists of messages.
		/// </returns>
		private static int GetMessageCount()
		{
			return traceMessages.Count +
					pingMessages.Count +
					debugMessages.Count +
					errorMessages.Count;
		}
	}
}

