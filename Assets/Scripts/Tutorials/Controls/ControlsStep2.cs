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
    /// The step 2 of the tutorial on controls.
    /// Learn how to open the menus attached to modules.
    /// </summary>
    public class ControlsStep2 : Step
    {
        [Header("Local Step Members")]
        public GameObject target;

        /// <inheritdoc/>
        public override bool CheckCondition()
        {
            return target.GetComponent<GameNetModule>().areVisible;
        }

        /// <inheritdoc/>
        public override void Initialize()
        {
            base.Initialize();

            //Enable the InfoTags of the front trigger.
            StaticReferencer.Instance.refInfoTags[1].SetActive(true);
            StaticReferencer.Instance.refInfoTags[6].SetActive(true);
        }
    }

}