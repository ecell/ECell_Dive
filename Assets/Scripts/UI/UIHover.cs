using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class UIHover : MonoBehaviour
{
    public XRBaseController refXRController;
    public XRRayInteractor refXRRayInteractor;

    public UnityEvent OnHoverEnter;
    public UnityEvent OnHoverExit;

    private GameObject lastHovered = null;
    private AudioSource EffectsAudioSource;

    private void CheckHovering()
    {
        //Check if the RayInteractor is hovering over some UI
        if (refXRRayInteractor.TryGetCurrentUIRaycastResult(out var raycastResultValue, out var uiRaycastHitIndex))
        {
            //Check if the UI element is different from the previous one
            if (lastHovered != raycastResultValue.gameObject)
            {
                HoverEnter();
                lastHovered = raycastResultValue.gameObject;
            }
        }

        //The RayInteractor is hovering over something unrelated
        else
        {
            //Just exiting the UI
            if (lastHovered != null)
            {
                lastHovered = null;
                HoverExit();
            }
        }
    }

    private void HoverEnter()
    {
        if (refXRRayInteractor.playHapticsOnHoverEntered)
        {
            refXRController.SendHapticImpulse(refXRRayInteractor.hapticHoverEnterIntensity,
                                          refXRRayInteractor.hapticHoverEnterDuration);
        }

        if (refXRRayInteractor.playAudioClipOnHoverEntered)
        {
            PlayAudio(refXRRayInteractor.audioClipForOnHoverEntered);
        }

        OnHoverEnter.Invoke();
    }

    private void HoverExit()
    {
        if (refXRRayInteractor.playHapticsOnHoverExited)
        {
            refXRController.SendHapticImpulse(refXRRayInteractor.hapticHoverExitIntensity,
                                          refXRRayInteractor.hapticHoverExitDuration);
        }

        if (refXRRayInteractor.playAudioClipOnHoverExited)
        {
            PlayAudio(refXRRayInteractor.audioClipForOnHoverExited);
        }

        OnHoverExit.Invoke();
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

    void Update()
    {
        CheckHovering();
    }

    
}
