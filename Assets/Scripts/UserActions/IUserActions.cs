using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


namespace ECellDive
{
    namespace Interfaces
    {
        public interface IGrab
        {
            public bool isGrabed { get; set; }
            [Serializable] public enum XRControllerID { Left, Right }
            public XRControllerID controllerID { get; set; }
            public GameObject refCurrentController { get; set; }
            public void SetControllerID(XRControllerID _controllerID);
        }

        public interface IDirectGrab: IGrab
        {
            public XRDirectInteractor currentDirectInteractor { get; set; }
            public bool directGrabEnabled { get; set; }
            public void OnDirectGrab();
            public void OnDirectRelease();
        }

        public interface IRemoteGrab : IGrab
        {
            public XRRayInteractor currentRemoteInteractor { get; set; }
            public bool remoteGrabEnabled { get; set; }
            public void OnRemoteGrab();
            public void OnRemoteRelease();
        }

        public interface IDualGrab: IDirectGrab, IRemoteGrab
        {
            public void HandleInteractorsSorting();

            public void ResetLogic();
        }

        public interface IPing
        {

        }
    }
}

