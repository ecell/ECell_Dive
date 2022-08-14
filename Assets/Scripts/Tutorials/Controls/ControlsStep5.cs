using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using ECellDive.Interfaces;
using ECellDive.Utility;

namespace ECellDive.Tutorials
{
    /// <summary>
    /// The step 5 of the tutorial on controls.
    /// Learn about the differnet types of controls input modes
    /// and how to switch from one to another.
    /// </summary>
    public class ControlsStep5 : Step
    {
        [Header("Local Step Members")]
        public LeftRightData<InputActionReference> switchInputModes;

        private bool switchedInputModes;

        public override bool CheckCondition()
        {
            return switchedInputModes;
        }

        public override void Conclude()
        {
            base.Conclude();

            switchInputModes.left.action.performed -= SwitchInputModesConfirmed;
            switchInputModes.right.action.performed -= SwitchInputModesConfirmed;
        }

        public override void Initialize()
        {
            base.Initialize();

            switchInputModes.left.action.Enable();
            switchInputModes.right.action.Enable();

            switchInputModes.left.action.performed += SwitchInputModesConfirmed;
            switchInputModes.right.action.performed += SwitchInputModesConfirmed;

            //Enable the InfoTags of the secondary buttons.
            StaticReferencer.Instance.refInfoTags[2].SetActive(true);
            StaticReferencer.Instance.refInfoTags[7].SetActive(true);

        }

        private void SwitchInputModesConfirmed(InputAction.CallbackContext _ctx)
        {
            switchedInputModes = true;
        }

    }
}