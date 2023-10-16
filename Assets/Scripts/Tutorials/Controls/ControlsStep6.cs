using UnityEngine;
using UnityEngine.InputSystem;
using ECellDive.Utility.Data;

namespace ECellDive.Tutorials
{
    /// <summary>
    /// The step 6 of the tutorial on controls.
    /// Learn how to move by teleportation.
    /// </summary>
    public class ControlsStep6 : Step
    {
        [Header("Local Step Members")]
        public LeftRightData<InputActionReference> teleport;

        private bool hasTeleported;

        public override bool CheckCondition()
        {
            return hasTeleported;
        }

        public override void Conclude()
        {
            base.Conclude();

            teleport.left.action.performed -= TeleportationConfirmed;
            teleport.right.action.performed -= TeleportationConfirmed;
        }

        public override void Initialize()
        {
            base.Initialize();

            teleport.left.action.performed += TeleportationConfirmed;
            teleport.right.action.performed += TeleportationConfirmed;
        }

        private void TeleportationConfirmed(InputAction.CallbackContext _ctx)
        {
            hasTeleported = true;
        }

    }
}
