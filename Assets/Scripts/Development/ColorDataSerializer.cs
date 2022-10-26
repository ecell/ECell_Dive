using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ECellDive.CustomEditors
{
    [System.Serializable]
    public struct ColorData
    {
        public uint targetGoID;
        public Color color;
    }

    public class ColorDataSerializer : ScriptableObject
    {
        public ColorData[] data;
    }

}
