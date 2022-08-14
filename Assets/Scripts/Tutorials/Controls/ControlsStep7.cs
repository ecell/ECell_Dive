using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using ECellDive.Interfaces;

namespace ECellDive.Tutorials
{
    /// <summary>
    /// The step 7 of the tutorial on controls.
    /// Learn how to move continuously.
    /// </summary>
    public class ControlsStep7 : Step
    {
        [Header("Local Step Members")]
        public LeftRightData<InputActionReference> switchMovementMode;

        public override void Initialize()
        {
            base.Initialize();

            switchMovementMode.left.action.Enable();
            switchMovementMode.right.action.Enable();
        }
    }
}

