using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace ECellDive.Interfaces
{
    public interface ITutorialStep 
    {
        string goal { get; }
        string task { get; }
        string details { get; }

        UnityEvent initializationInstructions { get; }
        UnityEvent conclusionInstructions { get; }

        /// <summary>
        /// Checks the success condition of the tutorial.
        /// </summary>
        /// <returns>
        /// True if the condition is satisfied. False, otherwise.
        /// </returns>
        bool CheckCondition();

        /// <summary>
        /// Makes every call stored in <see cref="conclusionInstructions"/>
        /// by the designer of the step.
        /// </summary>
        void Conclude();

        /// <summary>
        /// Makes every call stored in <see cref="initializationInstructions"/>
        /// by the designer of the step.
        /// </summary>
        void Initialize();
    }
}

