using UnityEngine;
using UnityEngine.UI;
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

        /// <summary>
        /// Defines the interface for an object to LookAt (Z-Axis) a target.
        /// This allows us to wrap or build more complex LookAt behaviour than
        /// the built-in Transform.LookAt.
        /// </summary>
        /// <remarks>Particularly used for UI elements to be visible from an active
        /// Camera.</remarks>
        public interface ILookAt
        {
            /// <summary>
            /// A boolean to know whether the gameobject should be flipped after it has
            /// looked at its target. It basically means looking at the opposite direction.
            /// </summary>
            bool flip { get; }

            /// <summary>
            /// The target to look at.
            /// </summary>
            Transform lookAtTarget { get; }

            /// <summary>
            /// The method to call to make the gameobject look at its target.
            /// </summary>
            void LookAt();
        }

        /// <summary>
        /// An interface to have the gameobject appear in front of the user's camera.
        /// </summary>
        public interface IPopUp
        {
            /// <summary>
            /// The distance to <see cref="popupTarget"/> the the gameobject should
            /// appear at.
            /// </summary>
            float popupDistance { get; }

            /// <summary>
            /// The offset to add to <paramref name="popupTarget"/>'s height.
            /// </summary>
            float popupHeightOffset { get; }

            /// <summary>
            /// The gameobject used as reference for the positioning of the
            /// gameobject that pops up.
            /// </summary>
            Transform popupTarget { get; }

            void PopUp();
        }

        /// <summary>
        /// An interface to allow control of the "interactibility" of
        /// UI elements.
        /// </summary>
        public interface IInteractibility
        {
            /// <summary>
            /// An array of references to UnityEngine.UI.Selectable component.
            /// </summary>
            Selectable[] targetGroup { get; }

            /// <summary>
            /// Forces the value <paramref name="_interactibility"/> upon 
            /// UnityEngine.UI.Selectable.interactable for every member of
            /// <see cref="targetGroup"/>.
            /// </summary>
            /// <param name="_interactibility">The value to apply to 
            /// UnityEngine.UI.Selectable.interactable.</param>
            void ForceGroupInteractibility(bool _interactibility);

            /// <summary>
            /// Switches the value of UnityEngine.UI.Selectable.interactable to
            /// its opposite for each member of the <see cref="targetGroup"/>.
            /// </summary>
            void SwitchGroupInteractibility();

            /// <summary>
            /// Switches the value of UnityEngine.UI.Selectable.interactable to
            /// its opposite for the member with index <paramref name="targetIdx"/>
            /// in <see cref="targetGroup"/>.
            /// </summary>
            /// <param name="targetIdx">Index of a UnityEngine.UI.Selectable in <see
            /// cref="targetGroup"/></param>
            void SwitchSingleInteractibility(int targetIdx);
        }
    }
}

