using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.Netcode;
using ECellDive.Utility.Data;

namespace ECellDive.Utility.Multiplayer
{
	/// <summary>
	/// Utility class to add to player gameobjects to activate/deactivate components
	/// depending on whether the player is local or not. Any player gameobject that
	/// is not local (i.e. it is the gamobject representing a remote player) will
	/// require slightly different components to be active than the local player.
	/// </summary>
	public class NetworkSpawnComponentActivation : NetworkBehaviour
	{
		/// <summary>
		/// Reference to the gameobject with the XRDeviceSimulator. This is only
		/// used for debugging purposes (i.e. the XRDeviceSimulator allows to emulate
		/// XR controls with a mouse and keyboard).
		/// </summary>
		/// <remarks>
		/// ACTIVATED on spawn if IsLocalPlayer.
		/// </remarks>
		[Header("To ACTIVATE on spawn if IsLocalPlayer")]
		public GameObject XRDeviceSimulator;

		/// <summary>
		/// Reference to the gameobject with the head model.
		/// </summary>
		/// <remarks>
		/// DEACTIVATED on spawn if IsLocalPlayer.
		/// </remarks>
		[Header("To DEACTIVATE on spawn if IsLocalPlayer")]
		public GameObject headModel;

		/// <summary>
		/// Reference to the gameobject with the canvas displaying player name.
		/// </summary>
		/// <remarks>
		/// DEACTIVATED on spawn if IsLocalPlayer.
		/// </remarks>
		public GameObject playerNameRoot;

		/// <summary>
		/// Reference to the left and right gameobjects encapsulating the models
		/// to use for the left and right controllers of replicated clients.
		/// </summary>
		/// <remarks>
		/// DEACTIVATED on spawn if IsLocalPlayer.
		/// </remarks>
		public LeftRightData<GameObject> imgrReplicatedClient;

		/// <summary>
		/// Reference to the camera of the player.
		/// </summary>
		/// <remarks>
		/// DEACTIVATED on spawn if NOT IsLocalPlayer.
		/// </remarks>
		[Header("To DEACTIVATE on spawn if NOT IsLocalPlayer")]
		public Camera refCamera;

		/// <summary>
		/// Reference to the audio listener of the player.
		/// </summary>
		/// <remarks>
		/// DEACTIVATED on spawn if NOT IsLocalPlayer.
		/// </remarks>
		public AudioListener refAudioListener;

		/// <summary>
		/// Reference to the component tracking the position and rotation
		/// of the player's head.
		/// </summary>
		/// <remarks>
		/// DEACTIVATED on spawn if NOT IsLocalPlayer.
		/// </remarks>
		public TrackedPoseDriver trackedPoseDriver;

        /// <summary>
        /// Reference to the left and right gameobjects encapsulating the models
        /// to use for the left and right controllers of local clients.
        /// </summary>
        /// <remarks>
        /// DEACTIVATED on spawn if NOT IsLocalPlayer.
        /// </remarks>
        public LeftRightData<GameObject> imgrLocalClient;

        /// <summary>
        /// Reference to the left and right components controlling the basic action
        /// callbacks when buttons are pressed or controllers are moved.
        /// </summary>
        /// <remarks>
        /// DEACTIVATED on spawn if NOT IsLocalPlayer.
        /// </remarks>
        public LeftRightData<ActionBasedController> actionBasedControllers;

		/// <summary>
		/// The list of gameobjects encapsulating the action info tags (the labels
		/// that inform the user of the action associated with each button).
		/// </summary>
		/// <remarks>
        /// DEACTIVATED on spawn if NOT IsLocalPlayer.
        /// </remarks> 
		public List<GameObject> actionInfoTags;

		/// <summary>
		/// Reference to the gameobject encapsulating the XRLocomotionSystem.
		/// </summary>
		/// <remarks>
        /// DEACTIVATED on spawn if NOT IsLocalPlayer.
        /// </remarks> 
		public GameObject XRLocomotionSystemHolder;

		/// <summary>
		/// Reference to the gameobject encapsulating the XRTeleportationProvider.
		/// </summary>
		/// <remarks>
        /// DEACTIVATED on spawn if NOT IsLocalPlayer.
        /// </remarks> 
		public GameObject XRTeleportationproviderHolder;

		/// <summary>
		/// Reference to the component that allows global access to some objects
		/// in the scene.
		/// </summary>
		/// <remarks>
        /// DEACTIVATED on spawn if NOT IsLocalPlayer.
        /// </remarks> 
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

