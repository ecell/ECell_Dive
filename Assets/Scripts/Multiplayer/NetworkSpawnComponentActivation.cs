using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.Netcode;
using ECellDive.Utility;
using ECellDive.Utility.Data;

namespace ECellDive.Multiplayer
{
	public class NetworkSpawnComponentActivation : NetworkBehaviour
	{
		[Header("To ACTIVATE on spawn if IsLocalPlayer")]
		public GameObject XRDeviceSimulator;

		[Header("To DEACTIVATE on spawn if IsLocalPlayer")]
		public GameObject headModel;
		public GameObject playerNameRoot;
		public LeftRightData<GameObject> imgrReplicatedClient;

		[Header("To DEACTIVATE on spawn if NOT IsLocalPlayer")]
		public Camera refCamera;
		public AudioListener refAudioListener;
		
		public TrackedPoseDriver trackedPoseDriver;

		public LeftRightData<GameObject> imgrLocalClient;
		public LeftRightData<ActionBasedController> actionBasedControllers;

		public List<GameObject> actionInfoTags;

		public GameObject XRLocomotionSystemHolder;
		public GameObject XRTeleportationproviderHolder;

		public StaticReferencer staticReferencer;

		public override void OnNetworkSpawn()
		{
			if (IsLocalPlayer)
			{
				if (XRDeviceSimulator != null)
				{
					XRDeviceSimulator.SetActive(true);
				}

				headModel.SetActive(false);
				playerNameRoot.SetActive(false);
				imgrReplicatedClient.left.SetActive(false);
				imgrReplicatedClient.right.SetActive(false);
			}

			if (!IsLocalPlayer)
			{
				imgrLocalClient.left.SetActive(false);
				imgrLocalClient.right.SetActive(false);

				refCamera.enabled = false;
				refAudioListener.enabled = false;
				trackedPoseDriver.enabled = false;

				actionBasedControllers.left.enabled = false;
				actionBasedControllers.right.enabled = false;

				foreach (GameObject _goIT in actionInfoTags)
				{
					_goIT.SetActive(false);
				}

				XRLocomotionSystemHolder.SetActive(false);
				XRTeleportationproviderHolder.SetActive(false);
				staticReferencer.enabled = false;
			}
		}

	}
}

