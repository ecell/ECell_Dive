using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;


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

            //Group controls interactors are not used yet (2022-01-19)
            //public XRRayInteractor[] leftGCs;
            //public XRRayInteractor[] rightGCs;

            //Movement related interactors
            [Header("Movement XRRayInteractors")]
            public XRRayInteractor[] leftMvts;
            public XRRayInteractor[] rightMvts;

            //General interactors*
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
                //DisableInteractors(leftGCs);
                //DisableInteractors(rightGCs);

                refMvtLHMap.Disable();
                refMvtRHMap.Enable();//default is movement mode on right controller
                DisableInteractors(leftMvts);
                EnableInteractors(rightMvts);

                refRBCLHMap.Enable();//default is ray mode on left controller
                refRBCRHMap.Disable();
                EnableInteractors(leftRBCs);
                DisableInteractors(rightRBCs);
            }

            private void DisableInteractors(XRRayInteractor[] _interactors)
            {
                foreach (XRRayInteractor interactor in _interactors)
                {
                    interactor.enabled = false;
                }
            }
            
            private void EnableInteractors(XRRayInteractor[] _interactors)
            {
                foreach (XRRayInteractor interactor in _interactors)
                {
                    interactor.enabled = true;
                }
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
                        //EnableInteractors(leftGCs);
                        break;

                    case 1:
                        refGCLHMap.Disable();
                        //DisableInteractors(leftGCs);

                        refMvtLHMap.Enable();
                        EnableInteractors(leftMvts);
                        break;

                    case 2:
                        refMvtLHMap.Disable();
                        DisableInteractors(leftMvts);

                        refRBCLHMap.Enable();
                        EnableInteractors(leftRBCs);
                        break;

                    default:
                        leftControllerModeID = 0;
                        goto case 0;
                }
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
                        //EnableInteractors(rightGCs);
                        break;

                    case 1:
                        refGCRHMap.Disable();
                        //DisableInteractors(leftGCs);

                        refMvtRHMap.Enable();
                        EnableInteractors(rightMvts);
                        break;

                    case 2:
                        refMvtRHMap.Disable();
                        DisableInteractors(rightMvts);

                        refRBCRHMap.Enable();
                        EnableInteractors(rightRBCs);
                        break;

                    default:
                        rightControllerModeID = 0;
                        goto case 0;
                }
            }
        }
    }
}

