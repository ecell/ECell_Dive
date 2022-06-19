using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using ECellDive.Utility;

namespace ECellDive
{
    namespace UI
    {
        [System.Serializable]
        public struct HapticData
        {
            public bool play;
            [Range(0f, 1f)] public float intensity;
            public float duration;
        }

        [System.Serializable]
        public struct AudioData
        {
            public bool play;
            public AudioClip clip;
        }

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
            public bool doTriggerInstanciation = false;

            public HapticData hoverEnterHapticData;
            public HapticData hoverExitHapticData;

            public AudioData hoverEnterAudioData;
            public AudioData hoverExitAudioData;

            private XRBaseController currentXRController;

            private AudioSource EffectsAudioSource;

            private bool leftInteractorActive = false;
            private GameObject leftLastHovered = null;
            private bool rightInteractorActive = false;
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

            private void HoverEnter(BaseEventData eventData)
            {
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
            /// Play an <see cref="AudioClip"/>.
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

            private void CreateEffectsAudioSource()
            {
                EffectsAudioSource = gameObject.AddComponent<AudioSource>();
                EffectsAudioSource.loop = false;
                EffectsAudioSource.playOnAwake = false;
            }
        }
    }
}

