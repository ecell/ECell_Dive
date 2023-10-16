using UnityEngine;

namespace ECellDive.Utility.Data.UI
{
    /// <summary>
	/// A struct to encapsulate the virtual keyboards sub-layouts
	/// </summary>
	[System.Serializable]
    public struct VirtualKeyBoardData
    {
        /// <summary>
        /// The canvas to store the layout for the lower case virtual keyboard.
        /// </summary>
        public Canvas LowerCaseVK;

        /// <summary>
        /// The canvas to store the layout for the upper case virtual keyboard.
        /// </summary>
        public Canvas UpperCaseVK;

        /// <summary>
        /// The canvas to store the layout for the number and signs virtual keyboard.
        /// </summary>
        public Canvas NumAndSignsVK;
    }
}
