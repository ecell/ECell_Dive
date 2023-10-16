using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

using ECellDive.Utility.Data;

namespace ECellDive.Interfaces
{
    /// <summary>
    /// Interface to define the behaviour of a remote grab.
    /// </summary>
    public interface IRemoteGrab
    {
        /// <summary>
        /// The boolean struct to inform whether the gameobject
        /// is being grabed by the left or right controllers.
        /// </summary>
        LeftRightData<bool> isGrabed { get; }

        /// <summary>
        /// The references to the input actions on the left and right controller
        /// used to grab objects.
        /// </summary>
        //LeftRightData<InputActionReference> grab { get; }

        /// <summary>
        /// The references to the input actions on the left and right controller
        /// used to modulate the distance of grabed objects.
        /// </summary>
        LeftRightData<InputActionReference> manageDistance { get; }

        /// <summary>
        /// A reference to the gameobject of the controller that is currently
        /// used to grab the gameobject.
        /// </summary>
        GameObject refCurrentController { get; set; }

        /// <summary>
        /// A reference to the XRRayInteractor that was used
        /// as an intermediary to grab the gameobject.
        /// </summary>
        XRRayInteractor currentRemoteInteractor { get; set; }

        /// <summary>
        /// Additional call back to perform right after the object
        /// has been moved (direct or remote grab).
        /// </summary>
        UnityEvent OnPostGrabMovementUpdate { get; set; }

        /// <summary>
        /// Informs whether the gameobject is currently grabed by
        /// either the left or right controller.
        /// </summary>
        /// <returns>
        /// True if the gameobject is grabed by either the left or 
        /// right controller. Flase, otherwise.
        /// </returns>
        bool IsGrabed();

        /// <summary>
        /// Operations to perform when the user is grabing an object
        /// after making a remote contact with it.
        /// </summary>
        void OnGrab();
        
        /// <summary>
        /// Operations to perform when a user realeases the object
        /// it had grabed from a remote contact with it.
        /// </summary>
        void OnReleaseGrab();
    }
}

