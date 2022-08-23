using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using ECellDive.Interfaces;
using ECellDive.Modules;
using ECellDive.Utility;


namespace ECellDive.Tutorials
{
    /// <summary>
    /// The step 3 of the Tutorial on Modules Navigation.
    /// DIVE!
    /// </summary>
    public class ModNavStep3 : Step
    {
        [Header("Local Step Members")]
        public LeftRightData<InputActionReference> diveActions;
        
        private bool hasStartedDiving;

        public override bool CheckCondition()
        {
            return hasStartedDiving;
        }

        private void CheckDive(RaycastHit _hit)
        {
            GameNetModule gnm;
            if (_hit.transform.TryGetComponent(out gnm) && _hit.transform.name == "iJO1366")
            {
                hasStartedDiving = gnm.isReadyForDive.Value;
            }
        }

        private void CheckDiveLeft(InputAction.CallbackContext _ctx)
        {
            RaycastHit hit;
            if (Physics.Raycast(
                StaticReferencer.Instance.riControllersGO.left.transform.position,
                StaticReferencer.Instance.riControllersGO.left.transform.forward,
                out hit,
                30,
                LayerMask.GetMask(new string[] { "Remote Grab Raycast" })))
            {
                CheckDive(hit);
            }
        }

        private void CheckDiveRight(InputAction.CallbackContext _ctx)
        {
            RaycastHit hit;
            if (Physics.Raycast(
                StaticReferencer.Instance.riControllersGO.right.transform.position,
                StaticReferencer.Instance.riControllersGO.right.transform.forward,
                out hit,
                30,
                LayerMask.GetMask(new string[] { "Remote Grab Raycast" })))
            {
                CheckDive(hit);
            }
        }

        public override void Conclude()
        {
            base.Conclude();

            diveActions.left.action.performed -= CheckDiveLeft;
            diveActions.right.action.performed -= CheckDiveRight;
        }

        public override void Initialize()
        {
            base.Initialize();

            diveActions.left.action.performed += CheckDiveLeft;
            diveActions.right.action.performed += CheckDiveRight;
        }

        
    }
}

