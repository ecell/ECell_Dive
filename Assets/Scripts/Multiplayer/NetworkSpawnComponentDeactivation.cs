using UnityEngine;
using UnityEngine.InputSystem.XR;
using Unity.Netcode;

public class NetworkSpawnComponentDeactivation : NetworkBehaviour
{
    public Camera refCamera;
    public AudioListener refAudioListener;
    public TrackedPoseDriver trackedPoseDriver;

    public override void OnNetworkSpawn()
    {
        if (!IsLocalPlayer)
        {
            refCamera.enabled = false;
            refAudioListener.enabled = false;
            trackedPoseDriver.enabled = false;
        }
    }
}
