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

            [Header("Movement XRRayInteractors")]
            public GameObject leftMvt;
            public GameObject rightMvt;

            [Header("Group Controls Interactors")]
            public GameObject leftGC;
            public GameObject rightGC;

            [Header("Ray Based Controls XRRayInteractors")]
            public XRRayInteractor[] leftRBCs;
            public XRRayInteractor[] rightRBCs;


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
                refGCLHMap.Disable();
                refGCRHMap.Disable();
                DisableInteractor(leftGC);
                DisableInteractor(rightGC);

                refMvtLHMap.Disable();
                refMvtRHMap.Enable();//default is movement mode on right controller
                DisableInteractor(leftMvt);
                EnableInteractor(rightMvt);
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
                        EnableInteractor(leftGC);
                        break;

                    case 1:
                        refGCLHMap.Disable();
                        DisableInteractor(leftGC);

                        refMvtLHMap.Enable();
                        EnableInteractor(leftMvt);
                        break;

                    case 2:
                        refMvtLHMap.Disable();
                        DisableInteractor(leftMvt);

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
                        EnableInteractor(rightGC);
                        break;

                    case 1:
                        refGCRHMap.Disable();
                        DisableInteractor(rightGC);

                        refMvtRHMap.Enable();
                        EnableInteractor(rightMvt);
                        break;

                    case 2:
                        refMvtRHMap.Disable();
                        DisableInteractor(rightMvt);

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

