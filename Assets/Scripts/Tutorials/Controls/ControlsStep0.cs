using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using ECellDive.Interfaces;
using ECellDive.Utility;

namespace ECellDive.Tutorials
{
    /// <summary>
    /// The step 0 of the tutorial on Controls.
    /// Used to start up the tutorial: the user will now "see" this step.
    /// </summary>
    public class ControlsStep0 : Step
    {
        [Header("Local Step Members")]
        public InputActionAsset refInputActionAsset;

        public LeftRightData<InputActionReference> refInputSwitchMode;

        private InputActionMap refRBCLHMap;
        private InputActionMap refRBCRHMap;

        private InputActionMap refMvtLHMap;
        private InputActionMap refMvtRHMap;

        private InputActionMap refGCLHMap;
        private InputActionMap refGCRHMap;

        public override void Initialize()
        {
            base.Initialize();

            //Disable the General GUI
            StaticReferencer.Instance.refAllGuiMenusContainer.SetActive(false);

            //Disable the RayBased Action Map
            refRBCLHMap = refInputActionAsset.FindActionMap("Ray_Based_Controls_LH");
            refRBCRHMap = refInputActionAsset.FindActionMap("Ray_Based_Controls_RH");
            refRBCLHMap.Disable();
            refRBCRHMap.Disable();

            //Disable the Movement Action Map
            refMvtLHMap = refInputActionAsset.FindActionMap("Movement_LH");
            refMvtRHMap = refInputActionAsset.FindActionMap("Movement_RH");
            refMvtLHMap.Disable();
            refMvtRHMap.Disable();

            //Disable the Groups Action Map
            refGCLHMap = refInputActionAsset.FindActionMap("Group_Controls_LH");
            refGCRHMap = refInputActionAsset.FindActionMap("Group_Controls_RH");
            refGCLHMap.Disable();
            refGCRHMap.Disable();

            //Disable the capacity to switch input action modes 
            refInputSwitchMode.left.action.Disable();
            refInputSwitchMode.right.action.Disable();

            //Hide every information tags
            foreach (GameObject _tag in StaticReferencer.Instance.refInfoTags)
            {
                _tag.SetActive(false);
            }
        }
    }
}
