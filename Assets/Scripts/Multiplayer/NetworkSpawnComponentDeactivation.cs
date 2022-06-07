using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkSpawnComponentDeactivation : NetworkBehaviour
{
    public Camera refCamera;
    public AudioListener refAudioListener;

    public override void OnNetworkSpawn()
    {
        if (!IsLocalPlayer)
        {
            refCamera.enabled = false;
            refAudioListener.enabled = false;
        }
    }
}
