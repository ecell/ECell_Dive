using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using ECellDive.Interfaces;
using ECellDive.Modules;
using ECellDive.Utility;
using ECellDive.Portal;

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

        private CyJsonModule refCyJsonModule;
        private bool hasStartedDiving;

        public override bool CheckCondition()
        {
            return hasStartedDiving;
        }

        private void CheckDive(RaycastHit _hit)
        {
            PortalManager gnm;
            if (_hit.transform.TryGetComponent(out gnm))
            {
                hasStartedDiving = true;
            }
        }

        private void CheckDiveLeft(InputAction.CallbackContext _ctx)
        {
            RaycastHit hit;
            if (Physics.Raycast(
                StaticReferencer.Instance.riControllersGO.left.transform.position,
                StaticReferencer.Instance.riControllersGO.left.transform.forward,
                out hit,
                30))
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
                30))
            {
                CheckDive(hit);
            }
        }

        public override void Conclude()
        {
            base.Conclude();

            diveActions.left.action.performed -= CheckDiveLeft;
            diveActions.right.action.performed -= CheckDiveRight;

            StartCoroutine(DelaySearch());
        }

        public override void Initialize()
        {
            base.Initialize();

            diveActions.left.action.performed += CheckDiveLeft;
            diveActions.right.action.performed += CheckDiveRight;

            refCyJsonModule = FindObjectOfType<CyJsonModule>();

            //We collect the CyJson module (created at the previous step) to
            //make sure that it is cleaned up when the user quits the tutorial.
            ModNavTutorialManager.tutorialGarbage.Add(refCyJsonModule.gameObject);

        }

        private IEnumerator DelaySearch()
        {
            //We just wait a bit to make sure the object we will be looking
            //for has been instantiated by the server.
            yield return new WaitForSeconds(0.5f);

            refCyJsonModule = FindObjectOfType<CyJsonModule>();
            //At this stage the user has started diving into the data module.
            //So there should be a rootPathway created that we can capture.
            Debug.Log($"refCyJsonModule.pathwayRoot", refCyJsonModule.pathwayRoot);
            ModNavTutorialManager.tutorialGarbage.Add(refCyJsonModule.pathwayRoot);
        }
    }
}

