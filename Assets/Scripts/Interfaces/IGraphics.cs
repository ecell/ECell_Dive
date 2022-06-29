using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


namespace ECellDive.Interfaces
{
    public interface INamed
    {
        GameObject nameTextFieldContainer { get; }
        TextMeshProUGUI nameField { get; }

        /// <summary>
        /// Makes the name field visible (using GameObject.SetActive and the like)
        /// </summary>
        void DisplayName();

        string GetName();

        /// <summary>
        /// Makes the name field invisible.
        /// </summary>
        void HideName();

        void SetName(string _name);

        /// <summary>
        /// Make the name readable from the POV of the camera
        /// </summary>
        void ShowName();
    }
}

