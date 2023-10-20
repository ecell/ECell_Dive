using System;
using UnityEngine;

namespace ECellDive.Utility
{
    /// <summary>
    /// A component allowing external scripts to subscribe to "major"
    /// Unity events (e.g. OnTriggerEnter) that are invoked on by this
    /// gameobject.
    /// </summary>
    public class TriggerBroadcaster : MonoBehaviour
    {
        /// <summary>
        /// The event that is invoked when the gameobject this component is attached to
        /// has its OnTriggerEnter method called.
        /// </summary>
        public event Action<Collider> onTriggerEnter;

        private void OnTriggerEnter(Collider other)
        {
            onTriggerEnter?.Invoke(other);
        }
    }

}
