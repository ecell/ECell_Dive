using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


namespace ECellDive.Interfaces
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
}

