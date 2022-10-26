using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ECellDive.CustomEditors
{
    [System.Serializable]
    public struct FluxData
    {
        public uint targetGoID;
        public float fluxLevel;
        public float fluxLevelClamped;
    }

    public class FluxDataSerializer : ScriptableObject
    {
        public FluxData[] data;
    }

}
