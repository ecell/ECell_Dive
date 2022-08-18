using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ECellDive.Utility
{
    /// <summary>
    /// A component allowing external scripts to subscribe to events
    /// that are invoked following Trigger events associated with the
    /// gameobject this component is attached to.
    /// </summary>
    public class TriggerBroadcaster : MonoBehaviour
    {
        public event Action<Collider> onTriggerEnter;
        private void OnTriggerEnter(Collider other)
        {
            onTriggerEnter?.Invoke(other);
        }
    }

}
