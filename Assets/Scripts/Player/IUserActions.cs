using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;


namespace ECellDive
{
    namespace Interfaces
    {
        /// <summary>
        /// Base interaface for the Grab event.
        /// </summary>
        public interface IGrab
        {
            public bool isGrabed { get; set; }
            [Serializable] public enum XRControllerID { Left, Right }
            public XRControllerID controllerID { get; set; }
            public GameObject refCurrentController { get; set; }

            /// <summary>
            /// Additional call back to perform right after the object
            /// has been moved (direct or remote grab).
            /// </summary>
            public UnityEvent OnPostGrabMovementUpdate { get; set; }

            /// <summary>
            /// Assigns a value to controllerID. Used to make the
            /// distinction between the Left and Right controller.
            /// </summary>
            /// <param name="_controllerID">Left or Right</param>
            public void SetControllerID(XRControllerID _controllerID);
        }

        /// <summary>
        /// Interface to define the behaviour of a direct contact grab.
        /// </summary>
        public interface IDirectGrab: IGrab
        {
            public XRDirectInteractor currentDirectInteractor { get; set; }
            public bool directGrabEnabled { get; set; }

            /// <summary>
            /// Operations to perform when the user is grabing an object
            /// after making a direct contact with it.
            /// </summary>
            public void OnDirectGrab();

            /// <summary>
            /// Operations to perform when a user realeases the object
            /// it had grabed from a direct contact with it.
            /// </summary>
            public void OnDirectRelease();
        }

        /// <summary>
        /// Interface to define the behaviour of a remote contact grab.
        /// </summary>
        public interface IRemoteGrab : IGrab
        {
            public XRRayInteractor currentRemoteInteractor { get; set; }
            public bool remoteGrabEnabled { get; set; }

            /// <summary>
            /// Operations to perform when the user is grabing an object
            /// after making a remote contact with it.
            /// </summary>
            public void OnRemoteGrab();

            /// <summary>
            /// Operations to perform when a user realeases the object
            /// it had grabed from a remote contact with it.
            /// </summary>
            public void OnRemoteRelease();
        }

        /// <summary>
        /// Interface to define the behaviour of an agent implementing
        /// both direct and remote grab.
        /// </summary>
        public interface IDualGrab: IDirectGrab, IRemoteGrab
        {
            /// <summary>
            /// Handles whether the user is doing a direct or remote grab
            /// on an object.
            /// </summary>
            public void HandleInteractorsSorting();

            /// <summary>
            /// Puts everything back to a no-grab situation.
            /// </summary>
            public void ResetLogic();
        }

        public interface IPing
        {

        }
    }
}

