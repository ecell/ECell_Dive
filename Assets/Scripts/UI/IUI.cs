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
            bool isExpanded { get; }

            /// <summary>
            /// The 3D holder of the UI scroll list that will be instantiated
            /// to allow manipulation of the scroll list in the 3D space.
            /// </summary>
            public GameObject scrollListHolderPrefab { get; }

            /// <summary>
            /// The scroll List prefab to instantiate upon creation of a drop
            /// down object.
            /// </summary>
            public GameObject scrollListPrefab { get; }

            /// <summary>
            /// The reference to the 3D position container of the scroll list.
            /// </summary>
            public GameObject scrollListHolder {get;}

            /// <summary>
            /// The content of the drop down. Namely, the objects to hide or display
            /// when the drop down is collapsed or expanded respectively.
            /// </summary>
            OptimizedVertScrollList scrollList { get; }

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

            /// <summary>
            /// The method to instantiate the scroll list that will contain
            /// the items of the drop down.
            /// </summary>
            public void InstantiateContent();
        }
    }
}

