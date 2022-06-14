using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.Netcode;
using ECellDive.UserActions;


namespace ECellDive.Multiplayer
{
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
        public List<ActionBasedController> actionBasedControllers;

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
                

                foreach (ActionBasedController _abc in actionBasedControllers)
                {
                    _abc.enabled = false;
                }
            }
        }
    }
}

