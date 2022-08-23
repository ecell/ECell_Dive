using System.Collections.Generic;
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
        /// the built-in <see cref="Transform.LookAt"/>.
        /// </summary>
        /// <remarks>Particularly used for UI elements to be visible from an active
        /// Camera.</remarks>
        public interface ILookAt
        {
            bool isUI { get; }
            Transform lookAtTarget { get; }

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
            /// The height relative to <see cref="popupTarget"/> the gameobject
            /// should appear at.
            /// </summary>
            /// <remarks>A value of 1 means to appear at the same height as <see
            /// cref="popupTarget"/>.</remarks>
            float popupRelativeHeight { get; }

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
            /// An array of references to <see cref="Selectable"/> component.
            /// </summary>
            Selectable[] targetGroup { get; }

            /// <summary>
            /// Switches the value of <see cref="Selectable.interactable"/> to
            /// its opposite for each member of the <see cref="targetGroup"/>.
            /// </summary>
            void SwitchGroupInteractibility();

            /// <summary>
            /// Switches the value of <see cref="Selectable.interactable"/> to
            /// its opposite for the member with index <paramref name="targetIdx"/>
            /// in <see cref="targetGroup"/>.
            /// </summary>
            /// <param name="targetIdx">Index of a <see cref="Selectable"/> in <see
            /// cref="targetGroup"/></param>
            void SwitchSingleInteractibility(int targetIdx);
        }
    }
}

