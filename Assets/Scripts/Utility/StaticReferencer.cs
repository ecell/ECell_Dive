using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using HSVPicker;
using ECellDive.Input;
using ECellDive.PlayerComponents;
using ECellDive.UI;
using ECellDive.Utility.Data;

namespace ECellDive.Utility
{
	/// <summary>
	/// Singleton class referencing elements (GameObjects, Components,...) of the player
	/// that are needed by gameobjects outside it. Typically, an instanced module needs 
	/// access to the controller's interactors to correctly compute the movements when grabed.
	/// This class therefore exposes all the interactors to the ouside.
	/// </summary>
	/// <remarks>WARNING: This class is enabled only for the local player. Do not use this class
	/// to access elements that should be synchronized across the network.</remarks>
	[RequireComponent(typeof(Player))]
	public class StaticReferencer : NetworkBehaviour
	{
		public static StaticReferencer Instance;

		[Header("UI Elements")]
		/// <summary>
		/// The anchor for the modules and GUI that will be pinned to the player
		/// </summary>
		public GameObject refInternalObjectContainer;

		/// <summary>
		/// The anchor for the modules and GUI that will be unpinned from the player
		/// </summary>
		[HideInInspector] public GameObject refExternalObjectContainer;
		[HideInInspector] public GameObject refVirtualKeyboard;
		[HideInInspector] public ColorPicker refColorPicker;
		[HideInInspector] public GroupsMenu refGroupsMenu;
		[HideInInspector] public GroupsMakingUIManager refGroupsMakingUIManager;

		/// <summary>
		/// The list of all gameobjects representing the information tags of 
		/// the controllers' buttons. This is mainly used when an external code
		/// wants to forcefully deactivate/activate them (very relevant when
		/// designing tutorials).
		/// </summary>
		/// <remarks>
		/// In the order of the list we find:
		///     - 0: Oculus Button IT
		///     - 1: X Button IT
		///     - 2: Y Button IT
		///     - 3: Left Joystick IT
		///     - 4: Left Trigger Front IT
		///     - 5: Left Trigger Grip IT
		///     - 6: A Button IT
		///     - 7: B Button IT
		///     - 8: Right Joystik IT
		///     - 9: Right Trigger Front IT
		///     - 10: Right Trigger Grip IT
		/// </remarks>
		public List<GameObject> refInfoTags;

		[Header("XR Interactors References")]
		public LeftRightData<XRRayInteractor> groupsInteractors;
		public LeftRightData<XRRayInteractor> remoteGrabInteractors;
		public LeftRightData<XRRayInteractor> remoteInteractionInteractors;
		public LeftRightData<XRRayInteractor> mainPointerInteractors;

		[Header("XR Action-Based Controller References")]
		public LeftRightData<ActionBasedController> remoteInteractionABC;

		[Header("Controllers GO References")]
		public LeftRightData<GameObject> mvtControllersGO;
		public LeftRightData<GameObject> groupControllersGO;
		public LeftRightData<GameObject> riControllersGO;

		[Header("Controllers Other References")]
		public LeftRightData<VolumetricSelectorManager> volumetricSelectorManagers;

		[Header("Other")]
		public InputModeManager inputModeManager;
		public GroupsMakingManager groupsMakingManager;

		// This OnNetworkSpawn is called when the player is spawned on the network
		public override void OnNetworkSpawn()
		{
			if (IsLocalPlayer)
			{
				Instance = this;
				refExternalObjectContainer = GameObject.FindGameObjectWithTag("ExternalObjectContainer");

				GUIManager guiManager = refExternalObjectContainer.GetComponent<GUIManager>();
				Instance.refVirtualKeyboard = guiManager.refVirtualKeyboard;
				Instance.refColorPicker = guiManager.refColorPicker;
				Instance.refGroupsMenu = guiManager.refGroupsMenu;
				Instance.refGroupsMakingUIManager = guiManager.refGroupsMakingUIManager;

				guiManager.Initialize(GetComponent<Player>());
			}
		}

		// This OnNetworkDespawn is called when the player is despawned from the network
		public override void OnNetworkDespawn()
		{
			if (IsLocalPlayer)
			{
				//We clean the internal object container from any object or UI that
				//may be pinned to the player. This avoids destroying them when the
				//player is despawned.
				ushort childCount = (ushort)refInternalObjectContainer.transform.childCount;
				for (ushort i = 0; i < childCount; i++)
				{
					refInternalObjectContainer.transform.GetChild(0).SetParent(refExternalObjectContainer.transform);
				}
			}
		}
	}
}

