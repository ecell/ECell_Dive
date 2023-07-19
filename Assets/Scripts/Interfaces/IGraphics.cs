using UnityEngine;
using TMPro;


namespace ECellDive.Interfaces
{
    /// <summary>
    /// Interface for <see cref="GameObject"/> in the scene that may
    /// display a name.
    /// </summary>
    public interface INamed
    {
        /// <summary>
        /// The <see cref="GameObject"/> that contains the <see cref="nameField"/>.
        /// </summary>
        /// <remarks>
        /// Usefull when we wish to display/hide the name by manipulating the
        /// <see cref="GameObject"/> activity instead of disabling/enabling
        /// the <see cref="nameField"/>.
        /// </remarks>
        GameObject nameTextFieldContainer { get; }

        /// <summary>
        /// A reference to the component used to display a string corresponding
        /// to the name.
        /// </summary>
        TextMeshProUGUI nameField { get; }

        /// <summary>
        /// Makes the name field visible (using GameObject.SetActive and the like)
        /// </summary>
        void DisplayName();

        /// <summary>
        /// Gets the value stored in <see cref="nameField"/>.
        /// </summary>
        /// <returns>The text value of <see cref="nameField"/></returns>
        string GetName();

        /// <summary>
        /// Makes the name field invisible.
        /// </summary>
        void HideName();

        /// <summary>
        /// Sets the value to display through <see cref="nameField"/>.
        /// </summary>
        /// <param name="_name">The name to display</param>
        void SetName(string _name);

        /// <summary>
        /// Make the name readable from the POV of the camera
        /// </summary>
        void ShowName();
    }
}

