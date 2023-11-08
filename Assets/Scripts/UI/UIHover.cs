using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

using ECellDive.Utility.Data.UI;
using ECellDive.Utility.PlayerComponents;

namespace ECellDive.UI
{
	/// <summary>
	/// Implements Haptic and Sound feedback when hovering on 
	/// interactible UI elements.
	/// </summary>
	/// <remarks>
	/// Adapted from:
	/// https://forum.unity.com/threads/xr-interaction-toolkit-hover-event-on-ui-elements-with-xrrayinteractor.852409/
	/// </remarks>
	public class UIHover : MonoBehaviour
	{
		/// <summary>
		/// A boolean to control whether to add enter end exit hover events to 
		/// every Button, Slider, Toggle and TMP_InputField that are children of
		/// the gameobject this script is attached to.
		/// </summary>
		public bool doTriggerInstanciation = false;

		/// <summary>
		/// The haptic data to use when entering hovering.
		/// </summary>
		public HapticData hoverEnterHapticData;

		/// <summary>
		/// The haptic data to use when exiting hovering.
		/// </summary>
		public HapticData hoverExitHapticData;

		/// <summary>
		/// The audio data to use when entering hovering.
		/// </summary>
		public AudioData hoverEnterAudioData;

		/// <summary>
		/// The audio data to use when exiting hovering.
		/// </summary>
		public AudioData hoverExitAudioData;

		/// <summary>
		/// A buffer reference to the XRcontroller that is currently hovering.
		/// It is used to assign the haptic feedback.
		/// </summary>
		private XRBaseController currentXRController;

		/// <summary>
		/// The audio source to play the audio clips.
		/// </summary>
		/// <remarks>
		/// It is created on demand.
		/// </remarks>
		private AudioSource EffectsAudioSource;

		/// <summary>
		/// A boolean to keep track whether the left interactor is hovering
		/// something.
		/// </summary>
		private bool leftInteractorActive = false;

		/// <summary>
		/// A buffer to the last object the left interactor hovered.
		/// Used to detect when the interactor stops hovering the object.
		/// </summary>
		private GameObject leftLastHovered = null;

		/// <summary>
		/// A boolean to keep track whether the right interactor is hovering
		/// something
		/// </summary>
		private bool rightInteractorActive = false;

		/// <summary>
		/// A buffer to the last object the right interactor hovered.
		/// Used to detect when the interactor stops hovering the object.
		/// </summary>
		private GameObject rightLastHovered = null;

		private void Awake()
		{
			if (doTriggerInstanciation)
			{
				List<GameObject> gameObjects = new List<GameObject>();
				gameObjects.AddRange(gameObject.GetComponentsInChildren<Button>().Select(x => x.gameObject));
				gameObjects.AddRange(gameObject.GetComponentsInChildren<Slider>().Select(x => x.gameObject));
				gameObjects.AddRange(gameObject.GetComponentsInChildren<Toggle>().Select(x => x.gameObject));
				gameObjects.AddRange(gameObject.GetComponentsInChildren<TMP_InputField>().Select(x => x.gameObject));
				foreach (GameObject item in gameObjects)
				{
					AddPointerEvents(item);
				}
			}
		}

		/// <summary>
		/// Adds and EventTrigger component to the given gameobject
		/// <paramref name="_item"/> and adds the HoverEnter and HoverExit
		/// callbacks to it.
		/// </summary>
		/// <param name="_item">
		/// The gameobject to which HoverEnter and HoverExit callbacks subscribe.
		/// </param>
		public void AddPointerEvents(GameObject _item)
		{
			EventTrigger trigger = _item.AddComponent<EventTrigger>();

			EventTrigger.Entry e = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
			e.callback.AddListener(HoverEnter);
			trigger.triggers.Add(e);

			EventTrigger.Entry e2 = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
			e2.callback.AddListener(HoverExit);
			trigger.triggers.Add(e2);
		}

		/// <summary>
		/// Detection of HoverEnter.
		/// </summary>
		/// <param name="eventData">
		/// The event data of the trigger. 
		/// This is necessary to satisfy the EventTrigger callback signature
		/// but we are not using it.
		/// </param>
		private void HoverEnter(BaseEventData eventData)
		{
			//Checks left controller
			if (StaticReferencer.Instance.remoteInteractionInteractors.left.
				TryGetCurrentUIRaycastResult(out var leftRaycastResultValue)
				&& !leftInteractorActive)
			{
				leftInteractorActive = true;
				leftLastHovered = leftRaycastResultValue.gameObject;
				currentXRController = StaticReferencer.Instance.remoteInteractionABC.left;
					
				if (hoverEnterHapticData.play)
				{
					currentXRController.SendHapticImpulse(
						hoverEnterHapticData.intensity,
						hoverEnterHapticData.duration);
				}

				if (hoverEnterAudioData.play)
				{
					PlayAudio(hoverEnterAudioData.clip);
				}
			}

			//Checks right controller
			if (StaticReferencer.Instance.remoteInteractionInteractors.right.
				TryGetCurrentUIRaycastResult(out var rightRaycastResultValue)
				&& !rightInteractorActive)
			{
				rightInteractorActive = true;
				rightLastHovered = rightRaycastResultValue.gameObject;
				currentXRController = StaticReferencer.Instance.remoteInteractionABC.right;

				if (hoverEnterHapticData.play)
				{
					currentXRController.SendHapticImpulse(
						hoverEnterHapticData.intensity,
						hoverEnterHapticData.duration);
				}

				if (hoverEnterAudioData.play)
				{
					PlayAudio(hoverEnterAudioData.clip);
				}
			}
		}

		/// <summary>
		/// Detection of HoverExit.
		/// </summary>
		/// <param name="eventData">
		/// The event data of the trigger. 
		/// This is necessary to satisfy the EventTrigger callback signature
		/// but we are not using it.
		/// </param>
		private void HoverExit(BaseEventData eventData)
		{
			StaticReferencer.Instance.remoteInteractionInteractors.left.
				TryGetCurrentUIRaycastResult(out var leftRaycastResultValue);

			StaticReferencer.Instance.remoteInteractionInteractors.right.
				TryGetCurrentUIRaycastResult(out var rightRaycastResultValue);

			if (leftInteractorActive && leftRaycastResultValue.gameObject != leftLastHovered)
			{
				leftInteractorActive = false;
				if (!leftRaycastResultValue.isValid)
				{
					leftLastHovered = null;
				}
				currentXRController = StaticReferencer.Instance.remoteInteractionABC.left;

				if (hoverExitHapticData.play)
				{
					currentXRController.SendHapticImpulse(
						hoverExitHapticData.intensity,
						hoverExitHapticData.duration);
				}

				if (hoverExitAudioData.play)
				{
					PlayAudio(hoverExitAudioData.clip);
				}
			}

			if (rightInteractorActive && rightRaycastResultValue.gameObject != rightLastHovered)
			{
				rightInteractorActive = false;
				if (!rightRaycastResultValue.isValid)
				{
					rightLastHovered = null;
				}
				currentXRController = StaticReferencer.Instance.remoteInteractionABC.right;

				if (hoverExitHapticData.play)
				{
					currentXRController.SendHapticImpulse(
						hoverExitHapticData.intensity,
						hoverExitHapticData.duration);
				}

				if (hoverExitAudioData.play)
				{
					PlayAudio(hoverExitAudioData.clip);
				}
			}                
		}

		/// <summary>
		/// Play a Unity AudioClip
		/// </summary>
		/// <param name="audioClip">The clip to play.</param>
		/// <remarks>Copied from the XRBaseControllerInteractor class
		/// as a workaround</remarks>
		private void PlayAudio(AudioClip audioClip)
		{
			if (audioClip == null)
				return;

			if (EffectsAudioSource == null)
				CreateEffectsAudioSource();

			EffectsAudioSource.PlayOneShot(audioClip);
		}

		/// <summary>
		/// Creates an UnityEngine.AudioSource component on the gameobject
		/// this script is attached to.
		/// </summary>
		private void CreateEffectsAudioSource()
		{
			EffectsAudioSource = gameObject.AddComponent<AudioSource>();
			EffectsAudioSource.loop = false;
			EffectsAudioSource.playOnAwake = false;
		}
	}
}

