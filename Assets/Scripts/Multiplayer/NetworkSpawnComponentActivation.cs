using UnityEngine;
using UnityEngine.InputSystem.XR;
using Unity.Netcode;

public class NetworkSpawnComponentActivation : NetworkBehaviour
{
    [Header("To ACTIVATE on spawn if IsLocalPlayer")]
    public GameObject XRDeviceSimulator;

    [Header("To DEACTIVATE on spawn if IsLocalPlayer")]
    public GameObject headModel;

    [Header("To DEACTIVATE on spawn if NOT IsLocalPlayer")]
    public Camera refCamera;
    public AudioListener refAudioListener;
    public TrackedPoseDriver trackedPoseDriver;

    public override void OnNetworkSpawn()
    {
        if (IsLocalPlayer)
        {
            XRDeviceSimulator.SetActive(true);

            headModel.SetActive(false);
        }

        if (!IsLocalPlayer)
        {
            refCamera.enabled = false;
            refAudioListener.enabled = false;
            trackedPoseDriver.enabled = false;
        }
    }
}
