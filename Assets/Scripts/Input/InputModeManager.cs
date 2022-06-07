using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using ECellDive.UI;


namespace ECellDive
{
    namespace Input
    {
        public class InputModeManager : MonoBehaviour
        {
            public InputActionAsset refInputActionAsset;

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

            private int leftControllerModeID = 2;//default is ray mode in left controller
            private int rightControllerModeID = 1;//default is movement mode on right controller

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

                refLeftControlSwitch.action.performed += LeftControllerModeSwitch;
                refRightControlSwitch.action.performed += RightControllerModeSwitch;
            }

            private void Start()
            {
                leftRBCs = new XRRayInteractor[3]
                {
                    InteractorsRegister.groupsInteractors.left,
                    InteractorsRegister.remoteInteractionInteractors.left,
                    InteractorsRegister.mainPointerInteractors.left
                };

                rightRBCs = new XRRayInteractor[3]
                {
                    InteractorsRegister.groupsInteractors.right,
                    InteractorsRegister.remoteInteractionInteractors.right,
                    InteractorsRegister.mainPointerInteractors.right
                };

                refGCLHMap.Disable();
                refGCRHMap.Disable();
                DisableInteractor(InteractorsRegister.groupControllersGO.left);
                DisableInteractor(InteractorsRegister.groupControllersGO.right);

                refMvtLHMap.Disable();
                refMvtRHMap.Enable();//default is movement mode on right controller
                DisableInteractor(InteractorsRegister.mvtControllersGO.left);
                EnableInteractor(InteractorsRegister.mvtControllersGO.right);
                GetComponent<ContextualHelpManager>().BroadcastControlModeSwitchToRightController(rightControllerModeID);

                refRBCLHMap.Enable();//default is ray mode on left controller
                refRBCRHMap.Disable();
                EnableInteractors(leftRBCs);
                DisableInteractors(rightRBCs);
                GetComponent<ContextualHelpManager>().BroadcastControlModeSwitchToLeftController(leftControllerModeID);
            }

            private void DisableInteractors(XRRayInteractor[] _interactors)
            {
                foreach (XRRayInteractor interactor in _interactors)
                {
                    interactor.enabled = false;
                }
            }

            private void DisableInteractor(XRRayInteractor _interactor)
            {
                _interactor.enabled = false;
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
                leftControllerModeID++;

                switch (leftControllerModeID)
                {
                    case 0:
                        refRBCLHMap.Disable();
                        DisableInteractors(leftRBCs);

                        refGCLHMap.Enable();
                        EnableInteractor(InteractorsRegister.groupControllersGO.left);
                        break;

                    case 1:
                        refGCLHMap.Disable();
                        DisableInteractor(InteractorsRegister.groupControllersGO.left);

                        refMvtLHMap.Enable();
                        EnableInteractor(InteractorsRegister.mvtControllersGO.left);
                        break;

                    case 2:
                        refMvtLHMap.Disable();
                        DisableInteractor(InteractorsRegister.mvtControllersGO.left);

                        refRBCLHMap.Enable();
                        EnableInteractors(leftRBCs);
                        break;

                    default:
                        leftControllerModeID = 0;
                        goto case 0;
                }

                GetComponent<ContextualHelpManager>().BroadcastControlModeSwitchToLeftController(leftControllerModeID);
            }

            private void RightControllerModeSwitch(InputAction.CallbackContext _ctx)
            {
                rightControllerModeID++;

                switch (rightControllerModeID)
                {
                    case 0:
                        refRBCRHMap.Disable();
                        DisableInteractors(rightRBCs);

                        refGCRHMap.Enable();
                        EnableInteractor(InteractorsRegister.groupControllersGO.right);
                        break;

                    case 1:
                        refGCRHMap.Disable();
                        DisableInteractor(InteractorsRegister.groupControllersGO.right);

                        refMvtRHMap.Enable();
                        EnableInteractor(InteractorsRegister.mvtControllersGO.right);
                        break;

                    case 2:
                        refMvtRHMap.Disable();
                        DisableInteractor(InteractorsRegister.mvtControllersGO.right);

                        refRBCRHMap.Enable();
                        EnableInteractors(rightRBCs);
                        break;

                    default:
                        rightControllerModeID = 0;
                        goto case 0;
                }
                GetComponent<ContextualHelpManager>().BroadcastControlModeSwitchToRightController(rightControllerModeID);
            }
        }
    }
}

