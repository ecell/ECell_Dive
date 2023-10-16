using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ECellDive.Utility.Data.UI
{
    /// <summary>
	/// A struct to encapsulate the virtual keyboards sub-layouts
	/// </summary>
	[System.Serializable]
    public struct VirtualKeyBoardData
    {
        public Canvas LowerCaseVK;
        public Canvas UpperCaseVK;
        public Canvas NumAndSignsVK;
    }
}
