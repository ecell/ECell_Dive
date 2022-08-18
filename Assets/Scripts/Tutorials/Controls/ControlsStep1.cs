using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using ECellDive.Interfaces;
using ECellDive.Utility;

namespace ECellDive.Tutorials
{
    /// <summary>
    /// The step 1 of the Tutorial on Controls.
    /// Learn how to point at objects and use the front
    /// trigger button of each controller.
    /// </summary>
    public class ControlsStep1 : Step
    {
        [Header("Local Step Members")]
        public GameObject target;
        public int targetRepetitions = 5;
        public LeftRightData<InputActionReference> refInteractionFrontTrigger;

        private int currentRepetitions = 0;

        /// <inheritdoc/>
        public override bool CheckCondition()
        {
            return currentRepetitions >= targetRepetitions;
        }

        public override void Conclude()
        {
            base.Conclude();

            refInteractionFrontTrigger.left.action.performed -= FireRayFromLeftController;
            refInteractionFrontTrigger.right.action.performed -= FireRayFromRightController;
        }

        private void FireRayFromLeftController(InputAction.CallbackContext _ctx)
        {
            if (Physics.Raycast(
                StaticReferencer.Instance.riControllersGO.left.transform.position,
                StaticReferencer.Instance.riControllersGO.left.transform.forward,
                30,
                LayerMask.GetMask(new string[] { "Remote Interaction Raycast" })))
            {
                IncrementRepetitions();
                RelocateTarget();
            }
        }
        
        private void FireRayFromRightController(InputAction.CallbackContext _ctx)
        {
            if (Physics.Raycast(
                StaticReferencer.Instance.riControllersGO.right.transform.position,
                StaticReferencer.Instance.riControllersGO.right.transform.forward,
                30,
                LayerMask.GetMask(new string[] { "Remote Interaction Raycast" })))
            {
                IncrementRepetitions();
                RelocateTarget();
            }
        }

        /// <inheritdoc/>
        public override void Initialize()
        {
            base.Initialize();

            //Force the Ray-based controls for both controllers.
            StaticReferencer.Instance.inputModeManager.BroadcastLeftControllerModeServerRpc(0);
            StaticReferencer.Instance.inputModeManager.BroadcastRightControllerModeServerRpc(0);

            refInteractionFrontTrigger.left.action.performed += FireRayFromLeftController;
            refInteractionFrontTrigger.right.action.performed += FireRayFromRightController;

            //Enable the InfoTags of the front trigger.
            StaticReferencer.Instance.refInfoTags[4].SetActive(true);
            StaticReferencer.Instance.refInfoTags[9].SetActive(true);
        }

        private void IncrementRepetitions()
        {
            currentRepetitions++;
        }

        private void RelocateTarget()
        {
            target.transform.localPosition = new Vector3(Random.Range(-1.5f, 1.5f),
                                                         1.5f,
                                                         0f);
        }
    }
}

