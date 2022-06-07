using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


namespace ECellDive.Input
{
    
    public interface ILeftRightData<T>
    {
        T left { get; set; }
        T right { get; set; }
    }

    [System.Serializable]
    public struct LeftRightData<T> : ILeftRightData<T>
    {
        [SerializeField] private T m_left;
        public T left { get => m_left; set => m_left = value; }

        [SerializeField] private T m_right;
        public T right { get => m_right; set => m_right = value; }
    }

    public struct LeftRightDataPointer<T,U> where T : ILeftRightData<U>
    {
        T m_v;
        public T v
        {
            set
            {
                m_v = value;
                m_v.left = value.left;
                m_v.right = value.right;
            }
        }
        public U left { get => m_v.left; }
        public U right { get => m_v.right; }
    }

    public static class InteractorsRegister
    {        
        public static LeftRightDataPointer<ILeftRightData<XRRayInteractor>, XRRayInteractor> groupsInteractors;
        public static LeftRightDataPointer<ILeftRightData<XRRayInteractor>, XRRayInteractor> remoteGrabInteractors;
        public static LeftRightDataPointer<ILeftRightData<XRRayInteractor>, XRRayInteractor> remoteInteractionInteractors;
        public static LeftRightDataPointer<ILeftRightData<XRRayInteractor>, XRRayInteractor> mainPointerInteractors;

        public static LeftRightDataPointer<ILeftRightData<ActionBasedController>, ActionBasedController> remoteInteractionABC;

        public static LeftRightDataPointer<ILeftRightData<GameObject>, GameObject> mvtControllersGO;
        public static LeftRightDataPointer<ILeftRightData<GameObject>, GameObject> groupControllersGO;
    }
}

