using UnityEngine;


namespace ECellDive.CustomEditors
{
    /// <summary>
    /// The struct to store the color of a module identifiable 
    /// by its ID in a Dive Scene.
    /// </summary>
    [System.Serializable]
    public struct ColorData
    {
        /// <summary>
        /// ID of the module.
        /// </summary>
        public uint targetGoID;

        /// <summary>
        /// Color of the module.
        /// </summary>
        public Color color;
    }

    /// <summary>
    /// A Scriptable object to generate assets (i.e. serialize) data on 
    /// about the color of data modules during development time.
    /// Usefull when we want to save visualization setups and load them
    /// at runtime.
    /// </summary>
    public class ColorDataSerializer : ScriptableObject
    {
        public ColorData[] data;
    }

}
