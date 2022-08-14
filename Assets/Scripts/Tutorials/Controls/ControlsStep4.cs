using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECellDive.UserActions;

namespace ECellDive.Tutorials
{
    /// <summary>
    /// The step 4 of the tutorial on controls.
    /// Learn that the user can also grab and move UI panels.
    /// </summary>
    public class ControlsStep4 : Step
    {
        [Header("Local Step Members")]
        public GrabManager grabManagerofTarget1;
        public GrabManager grabManagerofTarget2;
        public override bool CheckCondition()
        {
            return grabManagerofTarget1.isGrabed || grabManagerofTarget2.isGrabed;
        }
    }
}
