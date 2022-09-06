using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using ECellDive.Interfaces;
using ECellDive.UI;

namespace ECellDive
{
    namespace Input
    {
        public class InputModeManager : NetworkBehaviour
        {
            public InputActionAsset refInputActionAsset;

            //controller ID: 0
            private InputActionMap refRBCLHMap;
            private InputActionMap refRBCRHMap;

            //controller ID: 1
            private InputActionMap refMvtLHMap;
            private InputActionMap refMvtRHMap;

            //controller ID: 2
            private InputActionMap refGCLHMap;
            private InputActionMap refGCRHMap;

            public InputActionReference refLeftControlSwitch;
            public InputActionReference refRightControlSwitch;

            //default is ray mode on left controller
            private NetworkVariable<int> leftControllerModeID = new NetworkVariable<int>(0);

            //default is movement mode on right controller
            private NetworkVariable<int> rightControllerModeID = new NetworkVariable<int>(1);

            [Header("Ray Based Controls XRRayInteractors")]
            public LeftRightData<XRRayInteractor> remoteGrabInteractors;
            public LeftRightData<XRRayInteractor> remoteInteractionInteractors;
            public LeftRightData<XRRayInteractor> mainPointerInteractors;

            [Header("Controllers GO References")]
            public LeftRightData<GameObject> mvtControllersGO;
            public LeftRightData<GameObject> groupControllersGO;

            private XRRayInteractor[] leftRBCs;
            private XRRayInteractor[] rightRBCs;

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

            private void ApplyLeftControllerInteractorsSwitch(int _previous, int current)
            {
                switch (leftControllerModeID.Value)
                {
                    case 0:
                        DisableInteractor(groupControllersGO.left);

                        DisableInteractor(mvtControllersGO.left);

                        EnableInteractors(leftRBCs);
                        break;

                    case 1:
                        DisableInteractors(leftRBCs);

                        DisableInteractor(groupControllersGO.left);

                        EnableInteractor(mvtControllersGO.left);
                        break;

                    case 2:
                        DisableInteractors(leftRBCs);

                        DisableInteractor(mvtControllersGO.left);

                        EnableInteractor(groupControllersGO.left);
                        break;

                    default:
                        leftControllerModeID.Value = 0;
                        goto case 0;
                }

                GetComponent<ContextualHelpManager>().BroadcastControlModeSwitchToLeftController(leftControllerModeID.Value);
            }

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

            private void ApplyRightControllerInteractorsSwitch(int _previous, int current)
            {
                switch (rightControllerModeID.Value)
                {
                    case 0:
                        DisableInteractor(groupControllersGO.right);

                        DisableInteractor(mvtControllersGO.right);

                        EnableInteractors(rightRBCs);
                        break;

                    case 1:
                        DisableInteractors(rightRBCs);

                        DisableInteractor(groupControllersGO.right);

                        EnableInteractor(mvtControllersGO.right);
                        break;

                    case 2:
                        DisableInteractors(rightRBCs);

                        DisableInteractor(mvtControllersGO.right);

                        EnableInteractor(groupControllersGO.right);
                        break;

                    default:
                        rightControllerModeID.Value = 0;
                        goto case 0;
                }
                GetComponent<ContextualHelpManager>().BroadcastControlModeSwitchToRightController(rightControllerModeID.Value);
            }

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

            [ServerRpc]
            public void BroadcastLeftControllerModeServerRpc(int _modeIdx)
            {
                leftControllerModeID.Value = _modeIdx;
            }

            [ServerRpc]
            public void BroadcastRightControllerModeServerRpc(int _modeIdx)
            {
                rightControllerModeID.Value = _modeIdx;
            }

            private void DisableInteractors(XRRayInteractor[] _interactors)
            {
                foreach (XRRayInteractor interactor in _interactors)
                {
                    interactor.enabled = false;
                }
            }

            private void DisableInteractor(GameObject _selector)
            {
                _selector.SetActive(false);
            }
            
            private void EnableInteractors(XRRayInteractor[] _interactors)
            {
                foreach (XRRayInteractor interactor in _interactors)
                {
                    interactor.enabled = true;
                }
            }

            private void EnableInteractor(GameObject _selector)
            {
                _selector.SetActive(true);
            }

            private void LeftControllerModeSwitch(InputAction.CallbackContext _ctx)
            {
                if (IsOwner)
                {
                    BroadcastLeftControllerModeServerRpc(leftControllerModeID.Value + 1);
                }
            }

            private void RightControllerModeSwitch(InputAction.CallbackContext _ctx)
            {
                if (IsOwner)
                {
                    BroadcastRightControllerModeServerRpc(rightControllerModeID.Value + 1);
                }
            }

            public void SubscribeActionMapsSwitch()
            {
                leftControllerModeID.OnValueChanged += ApplyLeftControllerActionMapSwitch;
                rightControllerModeID.OnValueChanged += ApplyRightControllerActionMapSwitch;
            }
            
            public void UnsubscribeActionMapsSwitch()
            {
                leftControllerModeID.OnValueChanged -= ApplyLeftControllerActionMapSwitch;
                rightControllerModeID.OnValueChanged -= ApplyRightControllerActionMapSwitch;
            }
        }
    }
}

