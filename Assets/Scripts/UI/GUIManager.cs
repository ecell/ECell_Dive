using UnityEngine;
using UnityEngine.InputSystem;
using HSVPicker;
using ECellDive.Interfaces;
using ECellDive.PlayerComponents;
using ECellDive.Utility;
using ECellDive.Utility.PlayerComponents;

namespace ECellDive.UI
{
	/// <summary>
	/// Manages the initialization of the GUI and the logic to open/close
	/// the Main Menu.
	/// </summary>
	public class GUIManager : MonoBehaviour
	{
		/// <summary>
		/// The reference to the action to open/close the Main Menu.
		/// </summary>
		public InputActionReference refMenuOpen;
		
		/// <summary>
		/// Reference to the gameobject encapsulating the Main Menu.
		/// </summary>
		public GameObject refMainMenu;

		/// <summary>
		/// Reference to the gameobject encapsulating the Virtual Keyboard.
		/// </summary>
		public GameObject refVirtualKeyboard;

		/// <summary>
		/// Reference to the component defining the Color Picker.
		/// </summary>
		public ColorPicker refColorPicker;

		/// <summary>
		/// Reference to the component defining the Modules Menu Manager.
		/// </summary>
		public ModulesMenuManager refModulesMenuManager;

		/// <summary>
		/// Reference to the component defining the Groups Making UI Manager.
		/// </summary>
		public GroupsMakingUIManager refGroupsMakingUIManager;

		/// <summary>
		/// Reference to the component defining the Groups Menu.
		/// </summary>
		public GroupsMenu refGroupsMenu;

		private void Awake()
		{
			refMenuOpen.action.performed += ManageOpenStatus;
		}

		private void OnDestroy()
		{
			refMenuOpen.action.performed -= ManageOpenStatus;
		}

		/// <summary>
		/// Initializes some values of actions associated to some GUI after 
		/// the <paramref name="_player"/> (local client) has been spawned.
		/// </summary>
		/// <param name="_player">The reference to the player class of
		/// the local client spawned in the scene.</param>
		/// <remarks>
		/// Called once in Unity.Netcode.NetworkBehaviour.OnNetworkSpawn
		/// of the local client (see code of <see cref="PlayerComponents.Player"/>).
		/// </remarks>
		public void Initialize(Player _player)
		{
			Transform graphicsHolder;
			while (gameObject.transform.childCount > 0)
			{
				graphicsHolder = gameObject.transform.GetChild(0);
				//Changing the parent of the UI Menus depending on their pinStatus
				graphicsHolder.SetParent(StaticReferencer.Instance.refInternalObjectContainer.transform);

				//Making sure they are looking at the camera from the start.
				graphicsHolder.GetComponent<FaceCamera>().SetTargets(
					_player.gameObject.GetComponentInChildren<Camera>().transform);
			}
			refMainMenu.GetComponent<MainMenuManager>().Initialize();
			refModulesMenuManager.Initialize();
			refGroupsMenu.Initialize();
		}

		/// <summary>
		/// Shows or hides the UI corresponding to the Main Menu.
		/// </summary>
		private void ManageOpenStatus(InputAction.CallbackContext callbackContext)
		{
			refMainMenu.SetActive(!refMainMenu.activeSelf);
			refMainMenu.transform.parent.GetComponent<IPopUp>().PopUp();
		}
	}
}