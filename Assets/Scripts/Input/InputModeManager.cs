using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using ECellDive.UI;
using ECellDive.Utility.Data;

namespace ECellDive.Input
{
	/// <summary>
	/// The class to switch between the different input modes of the controllers.
	/// </summary>
	/// <remarks>
	/// This is a network behaviour so the input modes of users are made visible
	/// to every user in the session.
	/// </remarks>
	public class InputModeManager : NetworkBehaviour
	{
		/// <summary>
		/// Reference to the InputActionAsset containing all the action maps that
		/// might be triggered by pressing buttons on the controllers.
		/// </summary>
		public InputActionAsset refInputActionAsset;

		/// <summary>
		/// The Ray Based Controls action map for the Left Hand
		/// </summary>
		/// <remarks>
		/// The ID of Ray Based Controls is 0.
		/// </remarks>
		private InputActionMap refRBCLHMap;

		/// <summary>
		/// The Ray Based Controls action map for the Right Hand
		/// </summary>
		/// <remarks>
		/// The ID or Ray Based Controls is 0.
		/// </remarks>
		private InputActionMap refRBCRHMap;

		/// <summary>
		/// The Movement action map for the Left Hand
		/// </summary>
		/// <remarks>
		/// The ID of Movement controls is 1.
		/// </remarks>
		private InputActionMap refMvtLHMap;

		/// <summary>
		/// The Movement action map for the Right Hand
		/// </summary>
		/// <remarks>
		/// The ID of Movement controls is 1.
		/// </remarks>
		private InputActionMap refMvtRHMap;

		//controller ID: 2
		/// <summary>
		/// The Group Controls action map for the Left Hand
		/// </summary>
		/// <remarks>
		/// The ID of Group Controls is 2.
		/// </remarks>
		private InputActionMap refGCLHMap;

		/// <summary>
		/// The Group Controls action map for the Right Hand
		/// </summary>
		/// <remarks>
		/// The ID of Group Controls is 2.
		/// </remarks>
		private InputActionMap refGCRHMap;

		/// <summary>
		/// The InputActionReference to the action that triggers the switch of the
		/// input mode of the left controller.
		/// </summary>
		public InputActionReference refLeftControlSwitch;

		/// <summary>
		/// The InputActionReference to the action that triggers the switch of the
		/// input mode of the right controller.
		/// </summary>
		public InputActionReference refRightControlSwitch;

		//default is ray mode on left controller
		/// <summary>
		/// The ID of the current input mode of the left controller.
		/// Starting value is 0, which corresponds to Ray Based Controls.
		/// </summary>
		/// <remarks>
		/// Since this is a NetworkVariable, the control mode of the left controller
		/// of a user is made visible to every user in the session.
		/// </remarks>
		private NetworkVariable<int> leftControllerModeID = new NetworkVariable<int>(0);

		/// <summary>
		/// The ID of the current input mode of the right controller.
		/// Starting value is 1, which corresponds to Movement.
		/// </summary>
		/// <remarks>
		/// Since this is a NetworkVariable, the control mode of the right controller
		/// of a user is made visible to every user in the session.
		/// </remarks>
		private NetworkVariable<int> rightControllerModeID = new NetworkVariable<int>(1);

		/// <summary>
		/// The reference to the Unity.XR.Interaction.Toolkit.XRRayInteractor 
		/// of the left and right controllers used to grad objects remotely.
		/// </summary>
		[Header("Ray Based Controls XRRayInteractors")]
		public LeftRightData<XRRayInteractor> remoteGrabInteractors;

		/// <summary>
		/// The reference to the Unity.XR.Interaction.Toolkit.XRRayInteractor
		/// of the left and right controllers used to interact with objects remotely.
		/// </summary>
		public LeftRightData<XRRayInteractor> remoteInteractionInteractors;

		/// <summary>
		/// The reference to the Unity.XR.Interaction.Toolkit.XRRayInteractor
		/// of the left and right controllers used to point at objects.
		/// </summary>
		public LeftRightData<XRRayInteractor> mainPointerInteractors;

		/// <summary>
		/// The reference to theright and left game objects encapsulating the
		/// logic of the movement of the player.
		/// </summary>
		[Header("Controllers GO References")]
		public LeftRightData<GameObject> mvtControllersGO;

		/// <summary>
		/// The reference to the right and left game objects encapsulating the
		/// logic of the group controls.
		/// </summary>
		public LeftRightData<GameObject> groupControllersGO;

		/// <summary>
		/// Array of all the XRRayInteractors of the left controller.
		/// </summary>
		private XRRayInteractor[] leftRBCs;

		/// <summary>
		/// Array of all the XRRayInteractors of the right controller.
		/// </summary>
		private XRRayInteractor[] rightRBCs;

		/// <summary>
		/// The reference to the left and right information telling the 
		/// user which input mode he is switching to.
		/// </summary>
		[Header("UI Input Mode Tags")]
		public LeftRightData<SurgeAndShrinkInfoTag> inputModeTags;

		private void Awake()
		{
			refRBCLHMap = refInputActionAsset.FindActionMap("Ray_Based_Controls_LH");
			refRBCRHMap = refInputActionAsset.FindActionMap("Ray_Based_Controls_RH");

			refMvtLHMap = refInputActionAsset.FindActionMap("Movement_LH");
			refMvtRHMap = refInputActionAsset.FindActionMap("Movement_RH");

			refGCLHMap = refInputActionAsset.FindActionMap("Group_Controls_LH");
			refGCRHMap = refInputActionAsset.FindActionMap("Group_Controls_RH");
		}

		private void Start()
		{
			refLeftControlSwitch.action.performed += LeftControllerModeSwitch;
			refRightControlSwitch.action.performed += RightControllerModeSwitch;

			leftRBCs = new XRRayInteractor[3]
			{
				remoteGrabInteractors.left,
				remoteInteractionInteractors.left,
				mainPointerInteractors.left
			};

			rightRBCs = new XRRayInteractor[3]
			{
				remoteGrabInteractors.right,
				remoteInteractionInteractors.right,
				mainPointerInteractors.right
			};

			//Subscribe the switch of Interactors and Action Maps
			//to a change of value for leftControllerModeID
			leftControllerModeID.OnValueChanged += ApplyLeftControllerInteractorsSwitch;
			leftControllerModeID.OnValueChanged += ApplyLeftControllerActionMapSwitch;

			//Subscribe the switch of Interactors and Action Maps
			//to a change of value for rightControllerModeID
			rightControllerModeID.OnValueChanged += ApplyRightControllerInteractorsSwitch;
			rightControllerModeID.OnValueChanged += ApplyRightControllerActionMapSwitch;

			//Apply default input mode for the left controller
			ApplyLeftControllerInteractorsSwitch(-1, 0);
			ApplyLeftControllerActionMapSwitch(-1, 0);

			//Apply default input mode for the right controller
			ApplyRightControllerInteractorsSwitch(-1, 1);
			ApplyRightControllerActionMapSwitch(-1, 1);
		}

		public override void OnNetworkDespawn()
		{
			//Unsubscride to every event.
			refLeftControlSwitch.action.performed -= LeftControllerModeSwitch;
			refRightControlSwitch.action.performed -= RightControllerModeSwitch;

			leftControllerModeID.OnValueChanged -= ApplyLeftControllerInteractorsSwitch;
			leftControllerModeID.OnValueChanged -= ApplyLeftControllerActionMapSwitch;

			rightControllerModeID.OnValueChanged -= ApplyRightControllerInteractorsSwitch;
			rightControllerModeID.OnValueChanged -= ApplyRightControllerActionMapSwitch;
		}

        /// <summary>
        /// Handles disabling and enabling of interactors of the left controller.
		/// Callback function to call when the OnValueChanged event of the
        /// leftControllerModeID NetworkVariable is triggered.
        /// </summary>
        /// <param name="_previous">
        /// This is the previous value of the leftControllerModeID NetworkVariable.
        /// It is necessary to satisfy the constraint on the signature of the callback
        /// but we are not using it.
        /// </param>
        /// <param name="current">
        /// This is the newly assigned value of the leftControllerModeID NetworkVariable.
        /// It is necessary to satisfy the constraint on the signature of the callback
        /// but we are not using it.
        /// </param>
        private void ApplyLeftControllerInteractorsSwitch(int _previous, int current)
		{
			switch (leftControllerModeID.Value)
			{
				case 0:
					DisableInteractor(groupControllersGO.left);

					DisableInteractor(mvtControllersGO.left);

					EnableInteractors(leftRBCs);

					inputModeTags.left.SurgeAndShrink("Ray Inputs");
					break;

				case 1:
					DisableInteractors(leftRBCs);

					DisableInteractor(groupControllersGO.left);

					EnableInteractor(mvtControllersGO.left);

					inputModeTags.left.SurgeAndShrink("Movement Inputs");
					break;

				case 2:
					DisableInteractors(leftRBCs);

					DisableInteractor(mvtControllersGO.left);

					EnableInteractor(groupControllersGO.left);

					inputModeTags.left.SurgeAndShrink("Group Inputs");
					break;

				default:
					leftControllerModeID.Value = 0;
					goto case 0;
			}

			GetComponent<ContextualHelpManager>().BroadcastControlModeSwitchToLeftController(leftControllerModeID.Value);
		}

		/// <summary>
		/// Handles disabling and enabling of the action maps of the left controller.
		/// Callback function to call when the OnValueChanged event of the
		/// leftControllerModeID NetworkVariable is triggered.
		/// </summary>
		/// <param name="_previous">
		/// This is the previous value of the leftControllerModeID NetworkVariable.
		/// It is necessary to satisfy the constraint on the signature of the callback
		/// but we are not using it.
		/// </param>
		/// <param name="current">
		/// This is the newly assigned value of the leftControllerModeID NetworkVariable.
		/// It is necessary to satisfy the constraint on the signature of the callback
		/// but we are not using it.
		/// </param>
		private void ApplyLeftControllerActionMapSwitch(int _previous, int current)
		{
			switch (leftControllerModeID.Value)
			{
				case 0:
					refGCLHMap.Disable();

					refMvtLHMap.Disable();

					refRBCLHMap.Enable();
					break;

				case 1:
					refRBCLHMap.Disable();

					refGCLHMap.Disable();

					refMvtLHMap.Enable();
					break;

				case 2:
					refRBCLHMap.Disable();

					refMvtLHMap.Disable();

					refGCLHMap.Enable();
					break;

				default:
					leftControllerModeID.Value = 0;
					goto case 0;
			}
		}

        /// <summary>
        /// Handles disabling and enabling of interactors of the right controller.
        /// Callback function to call when the OnValueChanged event of the
        /// rightControllerModeID NetworkVariable is triggered.
        /// </summary>
        /// <param name="_previous">
        /// This is the previous value of the rightControllerModeID NetworkVariable.
        /// It is necessary to satisfy the constraint on the signature of the callback
        /// but we are not using it.
        /// </param>
        /// <param name="current">
        /// This is the newly assigned value of the rightControllerModeID NetworkVariable.
        /// It is necessary to satisfy the constraint on the signature of the callback
        /// but we are not using it.
        /// </param>
        private void ApplyRightControllerInteractorsSwitch(int _previous, int current)
		{
			switch (rightControllerModeID.Value)
			{
				case 0:
					DisableInteractor(groupControllersGO.right);

					DisableInteractor(mvtControllersGO.right);

					EnableInteractors(rightRBCs);

					inputModeTags.right.SurgeAndShrink("Ray Inputs");
					break;

				case 1:
					DisableInteractors(rightRBCs);

					DisableInteractor(groupControllersGO.right);

					EnableInteractor(mvtControllersGO.right);

					inputModeTags.right.SurgeAndShrink("Movement Inputs");
					break;

				case 2:
					DisableInteractors(rightRBCs);

					DisableInteractor(mvtControllersGO.right);

					EnableInteractor(groupControllersGO.right);

					inputModeTags.right.SurgeAndShrink("Group Inputs");
					break;

				default:
					rightControllerModeID.Value = 0;
					goto case 0;
			}
			GetComponent<ContextualHelpManager>().BroadcastControlModeSwitchToRightController(rightControllerModeID.Value);
		}

        /// <summary>
        /// Handles disabling and enabling of the action maps of the right controller.
        /// Callback function to call when the OnValueChanged event of the
        /// rightControllerModeID NetworkVariable is triggered.
        /// </summary>
        /// <param name="_previous">
        /// This is the previous value of the rightControllerModeID NetworkVariable.
        /// It is necessary to satisfy the constraint on the signature of the callback
        /// but we are not using it.
        /// </param>
        /// <param name="current">
        /// This is the newly assigned value of the rightControllerModeID NetworkVariable.
        /// It is necessary to satisfy the constraint on the signature of the callback
        /// but we are not using it.
        /// </param>
        private void ApplyRightControllerActionMapSwitch(int _previous, int current)
		{
			switch (rightControllerModeID.Value)
			{
				case 0:
					refGCRHMap.Disable();

					refMvtRHMap.Disable();

					refRBCRHMap.Enable();
					break;

				case 1:
					refRBCRHMap.Disable();

					refGCRHMap.Disable();

					refMvtRHMap.Enable();
					break;

				case 2:
					refRBCRHMap.Disable();

					refMvtRHMap.Disable();

					refGCRHMap.Enable();
					break;

				default:
					rightControllerModeID.Value = 0;
					goto case 0;
			}
		}

		/// <summary>
		/// Notifies the server that the user wants to switch the input mode of
		/// the left controller.
		/// </summary>
		/// <param name="_modeIdx">
		/// The new value of the leftControllerModeID NetworkVariable.
		/// </param>
		[ServerRpc]
		public void BroadcastLeftControllerModeServerRpc(int _modeIdx)
		{
			leftControllerModeID.Value = _modeIdx;
		}

		/// <summary>
		/// Notifies the server that the user wants to switch the input mode of
		/// the right controller.
		/// </summary>
		/// <param name="_modeIdx">
		/// The new value of the rightControllerModeID NetworkVariable.
		/// </param>
		[ServerRpc]
		public void BroadcastRightControllerModeServerRpc(int _modeIdx)
		{
			rightControllerModeID.Value = _modeIdx;
		}

		/// <summary>
		/// Utility function to disable all the interactors in an array.
		/// </summary>
		/// <param name="_interactors">
		/// The array of interactors to disable.
		/// </param>
		private void DisableInteractors(XRRayInteractor[] _interactors)
		{
			foreach (XRRayInteractor interactor in _interactors)
			{
				interactor.enabled = false;
			}
		}

		/// <summary>
		/// Utility function to disable all interactors in a game object.
		/// It simply sets the game object to inactive.
		/// </summary>
		/// <param name="_selector">
		/// The game object containing the interactors to disable.
		/// </param>
		private void DisableInteractor(GameObject _selector)
		{
			_selector.SetActive(false);
		}
		
		/// <summary>
		/// Utility function to enable all the interactors in an array.
		/// </summary>
		/// <param name="_interactors">
		/// The array of interactors to enable.
		/// </param>
		private void EnableInteractors(XRRayInteractor[] _interactors)
		{
			foreach (XRRayInteractor interactor in _interactors)
			{
				interactor.enabled = true;
			}
		}

		/// <summary>
		/// Utility function to enable all interactors in a game object.
		/// It simply sets the game object to active.
		/// </summary>
		/// <param name="_selector">
		/// The game object containing the interactors to enable.
		/// </param>
		private void EnableInteractor(GameObject _selector)
		{
			_selector.SetActive(true);
		}

		/// <summary>
		/// Callback function to call when the refLeftControlSwitch action is triggered.
		/// It will notify the server that the user wants to switch the input mode of
		/// the left controller if the user (client) is the owner of game object. This
		/// checks avoids the local client to forcefully switch the input mode of the
		/// replicated game object of another user.
		/// </summary>
		/// <param name="_ctx">
		/// Context information of the input action at the moment of the callback.
		/// This is necessary to satisfy the constraint on the signature of the callback
		/// but we are not using it.
		/// </param>
		private void LeftControllerModeSwitch(InputAction.CallbackContext _ctx)
		{
			if (IsOwner)
			{
				BroadcastLeftControllerModeServerRpc(leftControllerModeID.Value + 1);
			}
		}

        /// <summary>
        /// Callback function to call when the refRightControlSwitch action is triggered.
        /// It will notify the server that the user wants to switch the input mode of
        /// the right controller if the user (client) is the owner of game object. This
        /// checks avoids the local client to forcefully switch the input mode of the
        /// replicated game object of another user.
        /// </summary>
        /// <param name="_ctx">
        /// Context information of the input action at the moment of the callback.
        /// This is necessary to satisfy the constraint on the signature of the callback
        /// but we are not using it.
        /// </param>
        private void RightControllerModeSwitch(InputAction.CallbackContext _ctx)
		{
			if (IsOwner)
			{
				BroadcastRightControllerModeServerRpc(rightControllerModeID.Value + 1);
			}
		}

		/// <summary>
		/// Public utility function to subscribe <see cref="ApplyLeftControllerInteractorsSwitch"/>
		/// to the OnValueChanged event of the leftControllerModeID NetworkVariable.
		/// This is usefull only for the tutorials.
		/// </summary>
		public void SubscribeActionMapsSwitch()
		{
			leftControllerModeID.OnValueChanged += ApplyLeftControllerActionMapSwitch;
			rightControllerModeID.OnValueChanged += ApplyRightControllerActionMapSwitch;
		}
		
		/// <summary>
		/// Public utility function to unsubscribe <see cref="ApplyLeftControllerInteractorsSwitch"/>
		/// to the OnValueChanged event of the leftControllerModeID NetworkVariable.
		/// This is usefull only for the tutorials.
		/// </summary>
		public void UnsubscribeActionMapsSwitch()
		{
			leftControllerModeID.OnValueChanged -= ApplyLeftControllerActionMapSwitch;
			rightControllerModeID.OnValueChanged -= ApplyRightControllerActionMapSwitch;
		}
	}
}

