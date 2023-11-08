using UnityEngine;
using UnityEngine.Events;

using ECellDive.Utility.Data.UI;

namespace ECellDive.UI
{
	/// <summary>
	/// A specialized InfoDisplayManager that manages the display of the
	/// input controls of the player. Typically handles what it should display
	/// when the player is in a certain control mode (i.e. Interaction, Movement or Groups).
	/// </summary>
	/// <seealso cref="ECellDive.Input.InputModeManager"/>
	public class InfoTagManager : InfoDisplayManager
	{
		/// <summary>
		/// The tag associated with the Group Control mode.
		/// </summary>
		public Tag tagGC;

		/// <summary>
		/// The tag associated with the Movement mode.
		/// </summary>
		public Tag tagMvt;

		/// <summary>
		/// The tag associated with the Interaction mode.
		/// </summary>
		public Tag tagRBC;

		/// <summary>
		/// A reference to the current active tag.
		/// It shouldn't be different from <see cref="tagGC"/>,
		/// <see cref="tagMvt"/> or <see cref="tagRBC"/>."/>
		/// </summary>
		private Tag currentTag;

		private void Awake()
		{
			AwakeCallBackActions(tagGC);
			AwakeCallBackActions(tagMvt);
			AwakeCallBackActions(tagRBC);
		}

		private void Start()
		{
			if (hideOnStart)
			{
				Hide();
			}
			//lookAtTarget = NetworkManager.Singleton.LocalClient.PlayerObject.gameObject.
			//                GetComponentInChildren<Camera>().transform;
			lookAtTarget = Camera.main.transform;
		}

		/// <summary>
		/// Subscribe the mutators callback of a tag to the corresponding input action timings
		/// according to <see cref="TagMutator.actionTime"/>.
		/// </summary>
		/// <param name="_tag">
		/// The tag which mutators callback subscription must be initialized.
		/// </param>
		private void AwakeCallBackActions(Tag _tag)
		{
			foreach (TagMutator _tagMut in _tag.mutators)
			{
				switch (_tagMut.actionTime)
				{
					case (InputActionTime.started):
						_tagMut.refInputAction.action.started += e => ButtonTagCallBack(_tagMut.refCallback);
						break;
					case (InputActionTime.performed):
						_tagMut.refInputAction.action.performed += e => ButtonTagCallBack(_tagMut.refCallback);
						break;
					case (InputActionTime.canceled):
						_tagMut.refInputAction.action.canceled += e => ButtonTagCallBack(_tagMut.refCallback);
						break;
				}
			}
		}

		/// <summary>
		/// The link function between the subscription of a tag mutator input action
		/// callback and the invocation of the corresponding UnityEvent.
		/// </summary>
		/// <param name="_callbackEvent"></param>
		private void ButtonTagCallBack(UnityEvent _callbackEvent)
		{
			_callbackEvent.Invoke();
		}

		/// <summary>
		/// Hides the tag by setting its alpha to 0.
		/// </summary>
		protected override void Hide()
		{
			GetComponentInChildren<CanvasGroup>().alpha = 0f;
			refConnectionLineHandler.gameObject.GetComponent<LineRenderer>().enabled = false;
		}

		/// <summary>
		/// Forcefully sets the text in the <see cref="currentTag"/>'s content array at index
		/// <paramref name="_contentIndex"/> into the inherited text mesh
		/// (<see cref="ECellDive.UI.InfoDisplayManager.refInfoTextMesh"/>).
		/// </summary>
		/// <param name="_contentIndex">
		/// The index of the content to display.
		/// </param>
		public void SetText(int _contentIndex)
		{
			currentTag.currentContentIndex = _contentIndex;
			refInfoTextMesh.text = currentTag.content[currentTag.currentContentIndex];
		}

		/// <summary>
		/// Shows the tag by setting its alpha to 1.
		/// </summary>
		protected override void Show()
		{
			GetComponentInChildren<CanvasGroup>().alpha = 1f;
			refConnectionLineHandler.gameObject.GetComponent<LineRenderer>().enabled = true;
		}

		/// <summary>
		/// Switch the value of <see cref="currentTag"/> according to the the
		/// <paramref name="_controlModeID"/> value.
		/// 0 is for the Interaction mode (<see cref="tagRBC"/>), 1 for the
		/// Movement mode (<see cref="tagMvt"/>) and 2 for the Group Control
		/// mode (<see cref="tagGC"/>).
		/// </summary>
		/// <param name="_controlModeID">
		/// The ID of the control mode to switch to.
		/// </param>
		public void SwitchControlMode(int _controlModeID)
		{
			switch (_controlModeID)
			{
				case 0:
					currentTag = tagRBC;
					break;
				case 1:
					currentTag = tagMvt;
					break;
				case 2:
					currentTag = tagGC;
					break;
			}
			updateTagText();
		}

		/// <summary>
		/// Changes the content of the Text UI by rotating the indeces
		/// over the content array.
		/// </summary>
		public void TextRotation()
		{
			if (++currentTag.currentContentIndex >= currentTag.content.Length)
			{
				currentTag.currentContentIndex = 0;
			}
			refInfoTextMesh.text = currentTag.content[currentTag.currentContentIndex];

			updateTagText();
		}

        /// <summary>
        /// Sets the text in the <see cref="currentTag"/>'s content array at index
        /// <paramref name="_contentIndex"/> into the inherited text mesh
        /// (<see cref="ECellDive.UI.InfoDisplayManager.refInfoTextMesh"/>).
        /// but first checks if the content is empty. If the string is empty, the tag is hidden.
        /// </summary>
        private void updateTagText()
		{
			if (currentTag.content.Length > 0)
			{              
				refInfoTextMesh.text = currentTag.content[currentTag.currentContentIndex];
				Show();

				if (refInfoTextMesh.text.Length == 0)
				{
					Hide();
				}
			}
			else
			{
				Hide();
			}
		}
	}
}

