using UnityEngine;

namespace ECellDive.Utility.Data.PinSystem
{
    /// <summary>
	/// An enum to inform whether something is pinned to the player or the world.
	/// </summary>
	[System.Serializable]
    public enum PinStatus { ToPlayer, ToWorld }

    /// <summary>
    /// Only serves the purpose to encapsulate the <see cref="PinStatus"/> enum
    /// in a ScriptableObject that can be added as an asset to the project.
    /// </summary>
    [CreateAssetMenu(fileName = "PinData", menuName = "ScriptableObjects/PinData", order = 1)]
    public class PinData : ScriptableObject
    {
        public PinStatus pinTarget;
    }
}

