using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ECellDive.Utility
{
    /// <summary>
    /// Only serves the purpose to have access to a 
    /// </summary>
    [CreateAssetMenu(fileName = "PinData", menuName = "ScriptableObjects/PinData", order = 1)]
    public class PinData : ScriptableObject
    {
        public PinStatus pinTarget;
    }
}

