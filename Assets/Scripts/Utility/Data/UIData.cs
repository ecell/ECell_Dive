using ECellDive.Interfaces;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace ECellDive.Utility.Data.UI
{
    #region - Enums -
    /// <summary>
    /// An enum to specify the time when an action should be triggered.
    /// It effectively modulates wo which event of the action the 
    /// callback should subscribe to.
    /// </summary>
    /// <seealso cref="TagMutator"/>
    public enum InputActionTime { started, performed, canceled }
    #endregion

    #region - Structs -

	/// <summary>
	/// A simple struct to encapsulate the data of an audio clip 
	/// and whether it should be played or not.
	/// </summary>
    [System.Serializable]
    public struct AudioData
    {
		/// <summary>
		/// A boolean to specify whether the audio clip should be played or not.
		/// </summary>
        public bool play;

		/// <summary>
		/// The reference to the audio clip to play.
		/// </summary>
        public AudioClip clip;
    }

    /// <summary>
    /// A struct to encapsulate the data of a group visible via the
    /// GUI (<see cref="ECellDive.UI.GroupUIManager"/>) of the <see cref="ECellDive.Modules.GroupByModule"/>
    /// </summary>
    [System.Serializable]
	public struct GroupData
	{
		/// <summary>
		/// The value this group is representing.
		/// </summary>
		public string value;

		/// <summary>
		/// The color to represent this group.
		/// </summary>
		public Color color;

		/// <summary>
		/// The members of this group as IHighlightable objects since
		/// a group is visually identified by its color.
		/// </summary>
		public IHighlightable[] members;

		/// <summary>
		/// The IDs of the members of this group.
		/// </summary>
		public uint[] membersIds;
	}

	/// <summary>
	/// A simple struct to encapsulate the data of a haptic feedback.
	/// </summary>
	[System.Serializable]
	public struct HapticData
	{
		/// <summary>
		/// A boolean to specify whether the haptic feedback should be played or not.
		/// </summary>
		public bool play;

		/// <summary>
		/// The intensity of the haptic feedback.
		/// </summary>
		[Range(0f, 1f)] public float intensity;

		/// <summary>
		/// The duration of the haptic feedback.
		/// </summary>
		public float duration;
	}

	/// <summary>
	/// A simple struct to encapsulate four float values to represent
	/// padding values on the left, right, top and bottom of a UI element.
	/// </summary>
	[System.Serializable]
	public struct Padding
	{
		/// <summary>
		/// The left padding value.
		/// </summary>
		public float left;

		/// <summary>
		/// The right padding value.
		/// </summary>
		public float right;

		/// <summary>
		/// The top padding value.
		/// </summary>
		public float top;

		/// <summary>
		/// The bottom padding value.
		/// </summary>
		public float bottom;
	}

	/// <summary>
	/// A simple struct to encapsulate the callback data necessary to 
	/// control when a <see cref="Tag"/> should change its content.
	/// </summary>
	[Serializable]
	public struct TagMutator
	{
		/// <summary>
		/// The action callback event type to subscribe to.
		/// </summary>
		public InputActionTime actionTime;

		/// <summary>
		/// The input action to subscribe to and which triggers the
		/// invocation of <see cref="refCallback"/>.
		/// </summary>
		public InputActionReference refInputAction;

		/// <summary>
		/// The callback to invoke to mutate a <see cref="Tag"/>.
		/// </summary>
		public UnityEvent refCallback;
	}

	/// <summary>
	/// A struct to hold basic data and logic to switch between
	/// strings to display in a <see cref="ECellDive.UI.InfoTagManager"/>.
	/// </summary>
	[Serializable]
	public struct Tag
	{
		/// <summary>
		/// Array of alternative strings the tag can display.
		/// </summary>
		[TextArea] public string[] content;

		/// <summary>
		/// The index of the current content displayed by the tag.
		/// </summary>
		public int currentContentIndex;

		/// <summary>
		/// The mutators that will trigger the change of the content of the tag.
		/// </summary>
		public TagMutator[] mutators;
	}

	/// <summary>
	/// A struct to encapsulate the virtual keyboards sub-layouts
	/// </summary>
	[System.Serializable]
	public struct VirtualKeyBoardData
	{
		/// <summary>
		/// The canvas to store the layout for the lower case virtual keyboard.
		/// </summary>
		public Canvas LowerCaseVK;

		/// <summary>
		/// The canvas to store the layout for the upper case virtual keyboard.
		/// </summary>
		public Canvas UpperCaseVK;

		/// <summary>
		/// The canvas to store the layout for the number and signs virtual keyboard.
		/// </summary>
		public Canvas NumAndSignsVK;
	}
	#endregion
}