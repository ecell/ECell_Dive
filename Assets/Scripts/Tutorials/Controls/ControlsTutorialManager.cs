using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using ECellDive.Utility;
using ECellDive.Utility.Data;

namespace ECellDive.Tutorials
{
    /// <summary>
    /// The class controlling the chronology and logic of the
    /// tutorial on controls.
    /// </summary>
    public class ControlsTutorialManager : TutorialManager
    {
        public InputActionAsset refInputActionAsset;
        public LeftRightData<InputActionReference> refRBSelect;
        public LeftRightData<InputActionReference> refInputSwitchMode;

        //ControllerModeID = 0
        private List<InputActionReference> learnedLeftRBCActions = new List<InputActionReference>();
        private List<InputActionReference> learnedRightRBCActions = new List<InputActionReference>();

        //ControllerModeID = 1
        private List<InputActionReference> learnedLeftMvtCActions = new List<InputActionReference>();
        private List<InputActionReference> learnedRightMvtCActions = new List<InputActionReference>();

        //ControllerModeID = 2
        private List<InputActionReference> learnedLeftGCActions = new List<InputActionReference>();
        private List<InputActionReference> learnedRightGCActions = new List<InputActionReference>();

        //default is ray mode on left controller
        private int leftControllerModeID = 0;

        //default is movement mode on right controller
        private int rightControllerModeID = 0;

        private List<bool> previousStatesEOC;
        private List<bool> previousStatesIOC;

        public void AddLeftGroupControlAction(InputActionReference _gcAction)
        {
            if (leftControllerModeID == 2)
            {
                _gcAction.action.Enable();
            }
            learnedLeftGCActions.Add(_gcAction);
        }

        public void AddLeftMvtControlAction(InputActionReference _mvtcAction)
        {
            if (leftControllerModeID == 1)
            {
                _mvtcAction.action.Enable();
            }
            learnedLeftMvtCActions.Add(_mvtcAction);
        }

        public void AddLeftRayBasedControlAction(InputActionReference _rbcAction)
        {
            if (leftControllerModeID == 0)
            {
                _rbcAction.action.Enable();
            }
            learnedLeftRBCActions.Add(_rbcAction);
        }

        public void AddRightGroupControlAction(InputActionReference _gcAction)
        {
            if (rightControllerModeID == 2)
            {
                _gcAction.action.Enable();
            }
            learnedRightGCActions.Add(_gcAction);
        }

        public void AddRightMvtControlAction(InputActionReference _mvtcAction)
        {
            if (rightControllerModeID == 1)
            {
                _mvtcAction.action.Enable();
            }
            learnedRightMvtCActions.Add(_mvtcAction);
        }

        public void AddRightRayBasedControlAction(InputActionReference _rbcAction)
        {
            if (rightControllerModeID == 0)
            {
                _rbcAction.action.Enable();
            }
            learnedRightRBCActions.Add(_rbcAction);
        }

        protected override void Conclude()
        {
            base.Conclude();

            //Destroying the group started at the previous step.
            StaticReferencer.Instance.groupsMakingManager.CancelGroup();

            //The tutorial is finished so we unsubscribe the local solution
            //for switching input controls.
            refInputSwitchMode.left.action.performed -= LeftControllerActionMapSwitch;
            refInputSwitchMode.right.action.performed -= RightControllerActionMapSwitch;

            //Resubscribe the Action map switch within the InputModeManager
            //in order to restore default behaviour: when the user presses
            //the button binded to switching controls input actions, the 
            //corresponding action maps are also automatically switched
            //on or off entirely and not just the actions we added to the
            //"learnedXXXActions" lists declared in this script.
            StaticReferencer.Instance.inputModeManager.SubscribeActionMapsSwitch();
        }

        private void DisableLearnedActions(List<InputActionReference> _learnedActions)
        {
            foreach(InputActionReference action in _learnedActions)
            {
                action.action.Disable();
            }
        }

        private void EnableLearnedActions(List<InputActionReference> _learnedActions)
        {
            foreach (InputActionReference action in _learnedActions)
            {
                action.action.Enable();
            }
        }

        protected override void Initialize()
        {
            base.Initialize();


            //Disable the General GUI
            previousStatesEOC = new List<bool>(StaticReferencer.Instance.refExternalObjectContainer.transform.childCount);
            foreach (Transform _guiHolder in StaticReferencer.Instance.refExternalObjectContainer.transform)
            {
                previousStatesEOC.Add(_guiHolder.gameObject.activeSelf);
                _guiHolder.gameObject.SetActive(false);
            }

            previousStatesIOC = new List<bool>(StaticReferencer.Instance.refInternalObjectContainer.transform.childCount);
            foreach (Transform _guiHolder in StaticReferencer.Instance.refInternalObjectContainer.transform)
            {
                previousStatesIOC.Add(_guiHolder.gameObject.activeSelf);
                _guiHolder.gameObject.SetActive(false);
            }

            //Disable the RayBased Action Map
            refInputActionAsset.FindActionMap("Ray_Based_Controls_LH").Disable();
            refInputActionAsset.FindActionMap("Ray_Based_Controls_RH").Disable();

            //Disable the Movement Action Map
            refInputActionAsset.FindActionMap("Movement_LH").Disable();
            refInputActionAsset.FindActionMap("Movement_RH").Disable();

            //Disable the Groups Action Map
            refInputActionAsset.FindActionMap("Group_Controls_LH").Disable();
            refInputActionAsset.FindActionMap("Group_Controls_RH").Disable();

            //Disable the capacity to switch input action modes 
            refInputSwitchMode.left.action.Disable();
            refInputSwitchMode.right.action.Disable();

            //Remove the automatic activation of action map whenever the 
            //input switch mode action will be triggered in the future.
            //That way, we keep full control over progressive activation
            //of input actions during this tutorial.
            StaticReferencer.Instance.inputModeManager.UnsubscribeActionMapsSwitch();

            //Instead of using the controls switch management code in
            //InputModeManager.cs, we subscribe to the local solution
            //that will be valid for the duration of the tutorial.
            refInputSwitchMode.left.action.performed += LeftControllerActionMapSwitch;
            refInputSwitchMode.right.action.performed += RightControllerActionMapSwitch;

            //Force the Ray-based controls for both controllers. This only
            //impacts the visuals this we unsubscribed the Action Map Switch above.
            StaticReferencer.Instance.inputModeManager.BroadcastLeftControllerModeServerRpc(0);
            StaticReferencer.Instance.inputModeManager.BroadcastRightControllerModeServerRpc(0);

            //Allow user to click on UI Buttons at least.
            AddLeftRayBasedControlAction(refRBSelect.left);
            AddRightRayBasedControlAction(refRBSelect.right);

            //Hide every InfoTags...
            foreach (GameObject _tag in StaticReferencer.Instance.refInfoTags)
            {
                _tag.SetActive(false);
            }

            //... but still show the InfoTags of the front trigger.
            StaticReferencer.Instance.refInfoTags[4].SetActive(true);
            StaticReferencer.Instance.refInfoTags[9].SetActive(true);
        }

        private void LeftControllerActionMapSwitch(InputAction.CallbackContext _ctx)
        {
            leftControllerModeID++;

            switch (leftControllerModeID)
            {
                case 0:
                    DisableLearnedActions(learnedLeftGCActions);
                    DisableLearnedActions(learnedLeftMvtCActions);
                    EnableLearnedActions(learnedLeftRBCActions);
                    break;

                case 1:
                    DisableLearnedActions(learnedLeftRBCActions);
                    DisableLearnedActions(learnedLeftGCActions);
                    EnableLearnedActions(learnedLeftMvtCActions);
                    break;

                case 2:
                    DisableLearnedActions(learnedLeftRBCActions);
                    DisableLearnedActions(learnedLeftMvtCActions);
                    EnableLearnedActions(learnedLeftGCActions);
                    break;

                default:
                    leftControllerModeID = 0;
                    goto case 0;
            }
        }

        public override void Quit()
        {
            base.Quit();

            //Destroying the group that may have been started if the
            //user reached the group making step.
            StaticReferencer.Instance.groupsMakingManager.CancelGroup();

            //The tutorial is finished so we unsubscribe the local solution
            //for switching input controls.
            refInputSwitchMode.left.action.performed -= LeftControllerActionMapSwitch;
            refInputSwitchMode.right.action.performed -= RightControllerActionMapSwitch;

            //Resubscribe the Action map switch within the InputModeManager
            //in order to restore default behaviour: when the user presses
            //the button binded to switching controls input actions, the 
            //corresponding action maps are also automatically switched
            //on or off entirely and not just the actions we added to the
            //"learnedXXXActions" lists declared in this script.
            StaticReferencer.Instance.inputModeManager.SubscribeActionMapsSwitch();

            //Force the Ray-based controls for both controllers. This will
            //impact the visuals and the actions since we subscribed back to
            //the Action Map Switch above. Using the value -1 makes sure that
            //we get back to 0 with a change of value which will call the event
            //.OnValueChanged for leftControllerModeID and right ControllerModeID
            //in the InputModeManager.cs.
            StaticReferencer.Instance.inputModeManager.BroadcastLeftControllerModeServerRpc(-1);
            StaticReferencer.Instance.inputModeManager.BroadcastRightControllerModeServerRpc(-1);

            //Force back activation of every InfoTags
            foreach (GameObject _tag in StaticReferencer.Instance.refInfoTags)
            {
                _tag.SetActive(true);
            }

            //Revert the active state of the UI and modules that may have been imported.
            ushort _i = 0;
            foreach (Transform _guiHolder in StaticReferencer.Instance.refExternalObjectContainer.transform)
            {
                _guiHolder.gameObject.SetActive(previousStatesEOC[_i]);
                _i++;
            }

            _i = 0;
            foreach (Transform _guiHolder in StaticReferencer.Instance.refInternalObjectContainer.transform)
            {
                _guiHolder.gameObject.SetActive(previousStatesIOC[_i]);
                _i++;
            }
        }

        private void RightControllerActionMapSwitch(InputAction.CallbackContext _ctx)
        {
            rightControllerModeID++;

            switch (rightControllerModeID)
            {
                case 0:
                    DisableLearnedActions(learnedRightGCActions);
                    DisableLearnedActions(learnedRightMvtCActions);
                    EnableLearnedActions(learnedRightRBCActions);
                    break;

                case 1:
                    DisableLearnedActions(learnedRightRBCActions);
                    DisableLearnedActions(learnedRightGCActions);
                    EnableLearnedActions(learnedRightMvtCActions);
                    break;

                case 2:
                    DisableLearnedActions(learnedRightRBCActions);
                    DisableLearnedActions(learnedRightMvtCActions);
                    EnableLearnedActions(learnedRightGCActions);
                    break;

                default:
                    rightControllerModeID = 0;
                    goto case 0;
            }
        }
    }
}