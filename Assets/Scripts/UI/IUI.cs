using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECellDive.UI;

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

            //public GameObject itemPrefab { get;}

            /// <summary>
            /// The scroll List prefab to instantiate upon creation of a drop
            /// down object.
            /// </summary>
            public GameObject scrollListPrefab { get; }

            /// <summary>
            /// The content of the drop down. Namely, the objects to hide or display
            /// when the drop down is collapsed or expanded respectively.
            /// </summary>
            OptimizedVertScrollList scrollList { get; set; }

            /// <summary>
            /// The gameobject to activate/deactivate when we wish to show or hide the 
            /// content of the drop down
            /// </summary>
            GameObject content { get; set; }

            /// <summary>
            /// Adds a new gameobject to the dropdown.
            /// </summary>
            public GameObject AddItem();

            /// <summary>
            /// The method to call back when expanding the drop down.
            /// </summary>
            public void DisplayContent();

            /// <summary>
            /// The method to call back when collapsing the drop down.
            /// </summary>
            public void HideContent();

            public void InstantiateContent();
        }
    }
}

