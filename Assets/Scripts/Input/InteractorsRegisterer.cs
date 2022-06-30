using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using ECellDive.Interfaces;


namespace ECellDive.Input
{
    [System.Obsolete("Deprecated: Refactored in a unique  static referencer")]
    public class InteractorsRegisterer : MonoBehaviour
    {
        [Header("XR Interactors References")]
        public LeftRightData<XRRayInteractor> groupsInteractors;
        public LeftRightData<XRRayInteractor> remoteGrabInteractors;
        public LeftRightData<XRRayInteractor> remoteInteractionInteractors;
        public LeftRightData<XRRayInteractor> mainPointerInteractors;

        [Header("XR Action-Based Controller References")]
        public LeftRightData<ActionBasedController> remoteInteractionABC;

        [Header("Controllers GO References")]
        public LeftRightData<GameObject> mvtControllersGO;
        public LeftRightData<GameObject> groupControllersGO;

        private void Awake()
        {
            InteractorsRegister.groupsInteractors.v = groupsInteractors;
            InteractorsRegister.remoteGrabInteractors.v = remoteGrabInteractors;
            InteractorsRegister.remoteInteractionInteractors.v = remoteInteractionInteractors;
            InteractorsRegister.mainPointerInteractors.v = mainPointerInteractors;

            InteractorsRegister.remoteInteractionABC.v = remoteInteractionABC;

            InteractorsRegister.mvtControllersGO.v = mvtControllersGO;
            InteractorsRegister.groupControllersGO.v = groupControllersGO;
        }
    }
}