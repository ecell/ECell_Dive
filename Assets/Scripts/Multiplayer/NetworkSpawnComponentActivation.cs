using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.Netcode;


namespace ECellDive.Multiplayer
{
    public class NetworkSpawnComponentActivation : NetworkBehaviour
    {
        [Header("To ACTIVATE on spawn if IsLocalPlayer")]
        public GameObject XRDeviceSimulator;

        [Header("To DEACTIVATE on spawn if IsLocalPlayer")]
        public GameObject headModel;
        public GameObject playerNameRoot;

        [Header("To DEACTIVATE on spawn if NOT IsLocalPlayer")]
        public Camera refCamera;
        public AudioListener refAudioListener;
        public TrackedPoseDriver trackedPoseDriver;
        public List<ActionBasedController> actionBasedControllers;
        public GameObject UIRoot;
        public GameObject XRLocomotionSystemHolder;
        public GameObject XRTeleportationproviderHolder;

        public override void OnNetworkSpawn()
        {
            if (IsLocalPlayer)
            {
                XRDeviceSimulator.SetActive(true);

                headModel.SetActive(false);
                playerNameRoot.SetActive(false);
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

                UIRoot.SetActive(false);
                XRLocomotionSystemHolder.SetActive(false);
                XRTeleportationproviderHolder.SetActive(false);
            }
        }
    }
}

