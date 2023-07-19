using UnityEngine;

namespace ECellDive.CustomEditors
{
    /// <summary>
    /// The struct to store the flux values of the reaction edge
    ///  of a metabolic network identifiable by its ID in a Dive Scene.
    /// </summary>
    [System.Serializable]
    public struct FluxData
    {
        /// <summary>
        /// ID of the module.
        /// </summary>
        public uint targetGoID;

        /// <summary>
        /// The value of the flux.
        /// </summary>
        public float fluxLevel;

        /// <summary>
        /// The value of the flux after clamping.
        /// </summary>
        public float fluxLevelClamped;
    }

    /// <summary>
    /// A Scriptable object to generate assets (i.e. serialize) data on 
    /// reaction fluxes of a metabolic network during development time.
    /// Usefull when we want to save the results of a FBA to load and 
    /// at run time.
    /// </summary>
    public class FluxDataSerializer : ScriptableObject
    {
        public FluxData[] data;
    }

}
