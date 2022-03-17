using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ECellDive
{
    namespace Interfaces
    {
        /// <summary>
        /// Describes the requirements to implement a simple drop down.
        /// </summary>
        public interface IDropDown
        {
            /// <summary>
            /// The GameObject encapsulating the image to display when
            /// the drop down is collapsed.
            /// </summary>
            GameObject refDropDownImageCollapsed { get; }

            /// <summary>
            /// The GameObject encapsulating the image to display when
            /// the drop down is expanded.
            /// </summary>
            GameObject refDropDownImageExpanded { get; }

            /// <summary>
            /// A boolean representing the state of the dropdown.
            /// </summary>
            bool isExpanded {get;}

            /// <summary>
            /// The content of the drop down. Namely, the objects to hide or display
            /// when the drop down is collapsed or expanded respectively.
            /// </summary>
            List<GameObject> content { get; }

            /// <summary>
            /// Adds a new gameobject to the dropdown.
            /// </summary>
            /// <param name="_item">The gameobject to add to the drop down.</param>
            public void AddItem(GameObject _item);

            /// <summary>
            /// The method to call back when expanding the drop down.
            /// </summary>
            public void DisplayContent();

            /// <summary>
            /// The method to call back when collapsing the drop down.
            /// </summary>
            public void HideContent();
        }
    }
}

