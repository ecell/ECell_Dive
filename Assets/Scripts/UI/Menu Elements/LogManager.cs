using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ECellDive.Utility;

namespace ECellDive.UI
{
	/// <summary>
	/// Class handling the UI gameobject used to visualize
	/// the log in app at runtime.
	/// </summary>
	public class LogManager : MonoBehaviour
	{
		/// <summary>
		/// The reference to the scroll list used to display the head of the messages.
		/// </summary>
		[Header("General References")]
		public OptimizedVertScrollList refMessageScrollList;

		/// <summary>
		/// The reference to the rect transform of a deactivated UI gameobject
		/// to which we will store the messages that are not currently displayed.
		/// </summary>
		public RectTransform refHiddenMessageStorage;

		/// <summary>
		/// The reference to the larger text mesh where a longer version of the
		/// message will be displayed.
		/// </summary>
		public TextMeshProUGUI refMessageSpace;

		/// <summary>
		/// A reference to the toggle that controls the value of all the other toggles.
		/// </summary>
		[Header("Mutators")]
		public Toggle toggleAll;
		
		/// <summary>
		/// An array of toggles that controls the visibility of the messages in the list
		/// depending on their types.
		/// </summary>
		[Tooltip(
			"Must be in the same order as " +
			"LogSystem.LogMessageTypes")]
		public Toggle[] toggleIndividuals;

		/// <summary>
		/// The array of colors to assign to the messages depending on their types.
		/// </summary>
		public Color[] messageTypeColors;

		// Start is called before the first frame update
		void Start()
		{
			LogSystem.refLogManager = this;

			LogSystem.traceMessages = new List<LogMessage>();
			LogSystem.pingMessages = new List<LogMessage>();
			LogSystem.debugMessages = new List<LogMessage>();
			LogSystem.errorMessages = new List<LogMessage>();

			LogSystem.AddMessage(LogMessageTypes.Trace,
				"This is a Trace Message");
			LogSystem.AddMessage(LogMessageTypes.Ping,
				"This is a Ping Message");
			LogSystem.AddMessage(LogMessageTypes.Debug,
				"This is a Debug Message");
			LogSystem.AddMessage(LogMessageTypes.Errors,
				"This is a Errors Message");

			DrawMessageLists();
			gameObject.SetActive(false);
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
		/// The public interface to callback when we wish to only update the 
		/// drawing of one message list
		/// </summary>
		/// <param name="_toggleIdx">The index of the toggle that is used to 
		/// control the visibility of an inidividual message list. 0 is for the
		/// trace toggle, 1 for the ping toggle, 2 for the debug toggle and
		/// 3 for the errors toggle.</param>
		public void DrawMessageList(int _toggleIdx)
		{
			switch (_toggleIdx)
			{
				case 0:
					DrawMessageList(toggleIndividuals[0].isOn, LogSystem.traceMessages);
					break;
				case 1:
					DrawMessageList(toggleIndividuals[1].isOn, LogSystem.pingMessages);
					break;
				case 2:
					DrawMessageList(toggleIndividuals[2].isOn, LogSystem.debugMessages);
					break;
				case 3:
					DrawMessageList(toggleIndividuals[3].isOn, LogSystem.errorMessages);
					break;
			}
			refMessageScrollList.UpdateAllChildrenPositions();
			refMessageScrollList.UpdateScrollList();
		}

		/// <summary>
		/// The private implementation of the operations necessary to draw an individual
		/// message list.
		/// </summary>
		/// <param name="_visibility">Whether the list should be drawn.</param>
		/// <param name="_msgList">The list to draw or hide.</param>
		private void DrawMessageList(bool _visibility, List<LogMessage> _msgList)
		{
			if (_visibility)
			{
				foreach (LogMessage _msg in _msgList)
				{
					_msg.refUI.SetParent(refMessageScrollList.refContent.transform);
					_msg.refUI.SetSiblingIndex(_msg.id);
				}
			}
			else
			{
				foreach (LogMessage _msg in _msgList)
				{
					_msg.refUI.SetParent(refHiddenMessageStorage.transform);
				}
			}
		}

		/// <summary>
		/// Updates the drawing status of every list. 
		/// </summary>
		public void DrawMessageLists()
		{
			DrawMessageList(toggleIndividuals[0].isOn, LogSystem.traceMessages);
			DrawMessageList(toggleIndividuals[1].isOn, LogSystem.pingMessages);
			DrawMessageList(toggleIndividuals[2].isOn, LogSystem.debugMessages);
			DrawMessageList(toggleIndividuals[3].isOn, LogSystem.errorMessages);

			refMessageScrollList.UpdateAllChildrenPositions();
			refMessageScrollList.UpdateScrollList();
		}

		/// <summary>
		/// Forces to switch the visibility of every message of every type.
		/// </summary>
		/// <remarks>
		/// Called back when interacting with a UI element
		/// </remarks>
		public void ForceAllMessagesActivity()
		{
			foreach (Toggle _msgTypeToggle in toggleIndividuals)
			{
				_msgTypeToggle.isOn = toggleAll.isOn;
			}

			DrawMessageLists();
		}

		/// <summary>
		/// Instantiate the new GUI element that will tease the content of the message.
		/// </summary>
		public RectTransform GenerateMessageUI(string _content, LogMessageTypes _type)
		{
			GameObject newMsg = refMessageScrollList.AddItem();
			newMsg.SetActive(true);
			TextMeshProUGUI msgTMP = newMsg.GetComponentInChildren<TextMeshProUGUI>();
			if (msgTMP != null)
			{
				msgTMP.text = _content;
				msgTMP.color = messageTypeColors[(int)_type];
			}

			return newMsg.GetComponent<RectTransform>();
		}
	}
}

