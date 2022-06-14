using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using ECellDive.UI;


namespace ECellDive
{
    namespace Input
    {
        public class InputModeManager : NetworkBehaviour
        {
            public InputActionAsset refInputActionAsset;
            public InteractorsRegisterer localInteractorsRegisterer;

            //controller ID: 0
            private InputActionMap refGCLHMap;
            private InputActionMap refGCRHMap;

            //controller ID: 1
            private InputActionMap refMvtLHMap;
            private InputActionMap refMvtRHMap;

            //controller ID: 2
            private InputActionMap refRBCLHMap;
            private InputActionMap refRBCRHMap;

            public InputActionReference refLeftControlSwitch;
            public InputActionReference refRightControlSwitch;

            //default is ray mode on left controller
            private NetworkVariable<int> leftControllerModeID = new NetworkVariable<int>(2);

            //default is movement mode on right controller
            private NetworkVariable<int> rightControllerModeID = new NetworkVariable<int>(1);

            [Header("Ray Based Controls XRRayInteractors")]
            private XRRayInteractor[] leftRBCs;

            private XRRayInteractor[] rightRBCs;


            private void Awake()
            {
                refGCLHMap = refInputActionAsset.FindActionMap("Group_Controls_LH");
                refGCRHMap = refInputActionAsset.FindActionMap("Group_Controls_RH");

                refMvtLHMap = refInputActionAsset.FindActionMap("Movement_LH");
                refMvtRHMap = refInputActionAsset.FindActionMap("Movement_RH");

                refRBCLHMap = refInputActionAsset.FindActionMap("Ray_Based_Controls_LH");
                refRBCRHMap = refInputActionAsset.FindActionMap("Ray_Based_Controls_RH");
            }

            public override void OnNetworkSpawn()
            {
                refLeftControlSwitch.action.performed += LeftControllerModeSwitch;
                refRightControlSwitch.action.performed += RightControllerModeSwitch;

                leftRBCs = new XRRayInteractor[3]
                {
                    localInteractorsRegisterer.remoteGrabInteractors.left,
                    localInteractorsRegisterer.remoteInteractionInteractors.left,
                    localInteractorsRegisterer.mainPointerInteractors.left
                };

                rightRBCs = new XRRayInteractor[3]
                {
                    localInteractorsRegisterer.remoteGrabInteractors.right,
                    localInteractorsRegisterer.remoteInteractionInteractors.right,
                    localInteractorsRegisterer.mainPointerInteractors.right
                };

                leftControllerModeID.OnValueChanged += ApplyLeftControllerModeSwitch;
                rightControllerModeID.OnValueChanged += ApplyRightControllerModeSwitch;

                ApplyLeftControllerModeSwitch(-1, 2);
                ApplyRightControllerModeSwitch(-1, 1);
            }

            public override void OnNetworkDespawn()
            {
                refLeftControlSwitch.action.performed -= LeftControllerModeSwitch;
                refRightControlSwitch.action.performed -= RightControllerModeSwitch;

                leftControllerModeID.OnValueChanged -= ApplyLeftControllerModeSwitch;
                rightControllerModeID.OnValueChanged -= ApplyRightControllerModeSwitch;
            }

            private void ApplyLeftControllerModeSwitch(int _previous, int current)
            {
                switch (leftControllerModeID.Value)
                {
                    case 0:
                        refRBCLHMap.Disable();
                        DisableInteractors(leftRBCs);

                        refMvtLHMap.Disable();
                        DisableInteractor(localInteractorsRegisterer.mvtControllersGO.left);

                        refGCLHMap.Enable();
                        EnableInteractor(localInteractorsRegisterer.groupControllersGO.left);
                        break;

                    case 1:
                        refRBCLHMap.Disable();
                        DisableInteractors(leftRBCs);

                        refGCLHMap.Disable();
                        DisableInteractor(localInteractorsRegisterer.groupControllersGO.left);

                        refMvtLHMap.Enable();
                        EnableInteractor(localInteractorsRegisterer.mvtControllersGO.left);
                        break;

                    case 2:
                        refGCLHMap.Disable();
                        DisableInteractor(localInteractorsRegisterer.groupControllersGO.left);

                        refMvtLHMap.Disable();
                        DisableInteractor(localInteractorsRegisterer.mvtControllersGO.left);

                        refRBCLHMap.Enable();
                        EnableInteractors(leftRBCs);
                        break;

                    default:
                        leftControllerModeID.Value = 0;
                        goto case 0;
                }

                GetComponent<ContextualHelpManager>().BroadcastControlModeSwitchToLeftController(leftControllerModeID.Value);
            }

            private void ApplyRightControllerModeSwitch(int _previous, int current)
            {
                switch (rightControllerModeID.Value)
                {
                    case 0:
                        refRBCRHMap.Disable();
                        DisableInteractors(rightRBCs);

                        refMvtRHMap.Disable();
                        DisableInteractor(localInteractorsRegisterer.mvtControllersGO.right);

                        refGCRHMap.Enable();
                        EnableInteractor(localInteractorsRegisterer.groupControllersGO.right);
                        break;

                    case 1:
                        refRBCRHMap.Disable();
                        DisableInteractors(rightRBCs);

                        refGCRHMap.Disable();
                        DisableInteractor(localInteractorsRegisterer.groupControllersGO.right);

                        refMvtRHMap.Enable();
                        EnableInteractor(localInteractorsRegisterer.mvtControllersGO.right);
                        break;

                    case 2:
                        refGCRHMap.Disable();
                        DisableInteractor(localInteractorsRegisterer.groupControllersGO.right);

                        refMvtRHMap.Disable();
                        DisableInteractor(localInteractorsRegisterer.mvtControllersGO.right);

                        refRBCRHMap.Enable();
                        EnableInteractors(rightRBCs);
                        break;

                    default:
                        rightControllerModeID.Value = 0;
                        goto case 0;
                }
                GetComponent<ContextualHelpManager>().BroadcastControlModeSwitchToRightController(rightControllerModeID.Value);
            }

            [ServerRpc]
            public void BroadcastLeftControllerModeServerRpc()
            {
                leftControllerModeID.Value++;
            }

            [ServerRpc]
            public void BroadcastRightControllerModeServerRpc()
            {
                rightControllerModeID.Value++;
            }

            private void DisableInteractors(XRRayInteractor[] _interactors)
            {
                foreach (XRRayInteractor interactor in _interactors)
                {
                    interactor.enabled = false;
                }
            }

            //private void DisableInteractor(XRRayInteractor _interactor)
            //{
            //    _interactor.enabled = false;
            //}

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

            private void EnableInteractor(XRRayInteractor _interactor)
            {
                _interactor.enabled = true;
            }

            private void EnableInteractor(GameObject _selector)
            {
                _selector.SetActive(true);
            }

            private void LeftControllerModeSwitch(InputAction.CallbackContext _ctx)
            {
                if (IsOwner)
                {
                    BroadcastLeftControllerModeServerRpc();
                }
            }

            private void RightControllerModeSwitch(InputAction.CallbackContext _ctx)
            {
                if (IsOwner)
                {
                    BroadcastRightControllerModeServerRpc();
                }
            }
        }
    }
}

