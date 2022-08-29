using System.Collections;
using UnityEngine.Events;

namespace ECellDive.Interfaces
{
    /// <summary>
    /// The interface used to declare the logic characterizing a
    /// step of a tutorial.
    /// </summary>
    public interface ITutorialStep 
    {
        /// <summary>
        /// A string used to descrie the goal of this tutorial step
        /// </summary>
        string goal { get; }

        /// <summary>
        /// A string used to describe the task the user should accomplish
        /// to reach the goal and be able to move on to the next step.
        /// </summary>
        string task { get; }

        /// <summary>
        /// A string used to give a more detailed description of the task.
        /// </summary>
        string details { get; }

        /// <summary>
        /// A unity event invoked during <see cref="Initialize"/>.
        /// </summary>
        UnityEvent initializationInstructions { get; }

        /// <summary>
        /// A unity event invoked during <see cref="Conclude"/>.
        /// </summary>
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

