using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ECellDive
{
    namespace Interfaces
    {
        [System.Serializable]
        public struct ControllersSymetricAction
        {
            public InputActionReference leftController;
            public InputActionReference rightController;
        }

        /// <summary>
        /// An interface to encode knowledge about whether a gameobject
        /// is pointed at by a ray interactor.
        /// </summary>
        public interface IFocus
        {
            bool isFocused { get;}
            void SetFocus();
            void UnsetFocus();
        }

        /// <summary>
        /// An interface to change switch between two colors when a gameobject
        /// needs to be highlighted.
        /// </summary>
        public interface IHighlightable
        {
            Color defaultColor { get; }
            Color highlightColor { get; }

            abstract void SetDefaultColor(Color _c);
            abstract void SetHighlightColor(Color _c);

            abstract void SetHighlight();
            abstract void UnsetHighlight();
        }

        /// <summary>
        /// The interface describing the requirements to implement a handle for a
        /// group of information tags attached to a gameobject.
        /// </summary>
        public interface IInfoTags
        {
            bool areVisible { get; }

            ControllersSymetricAction displayInfoTagsActions { get; set; }

            GameObject refInfoTagPrefab { get; set; }
            GameObject refInfoTagsContainer { get; set; }
            List<GameObject> refInfoTags { get; set; }

            /// <summary>
            /// Make the InfoTags visible in the scene
            /// </summary>
            void DisplayInfoTags();
            void HideInfoTags();

            /// <summary>
            /// Instantiate one info tag gameobject.
            /// </summary>
            /// <param name="_xyPosition">The X and Y positions.
            /// The Z position will be set to 0.</param>
            /// <param name="_content">The info to display.</param>
            public void InstantiateInfoTag(Vector2 _xyPosition, string _content);

            /// <summary>
            /// Instantiates all info tags of a module based on the
            /// info stored in <paramref name="_content"/>.
            /// </summary>
            /// <param name="_content">The array storing the information
            /// to display within text fields of the tags.</param>
            public void InstantiateInfoTags(string[] _content);

            /// <summary>
            /// Make the InfoTags face in the direction of the player
            /// </summary>
            void ShowInfoTags();
        }

        /// <summary>
        /// An interface to encode knowledge about a chemical reaction being
        /// knockedout.
        /// </summary>
        /// <remarks>Initially used on edges of a metabolic pathway.</remarks>
        public interface IKnockable
        {
            ControllersSymetricAction triggerKOActions { get; set; }
            bool knockedOut { get; }
            void Activate();
            void Knockout();
        }

        /// <summary>
        /// An interface to encode knowledge about fluxes associated to a chemical
        /// reaction.
        /// </summary>
        /// <remarks>Initially used on edges of a metabolic pathway that can
        /// undergo a Flux Balance Analysis.</remarks>
        public interface IModulateFlux: IKnockable
        {
            /// <summary>
            /// The real flux value
            /// </summary>
            float fluxLevel { get; }

            /// <summary>
            /// A clamped flux value to control the visualization of the flux
            /// </summary>
            float fluxLevelClamped { get; }
            void SetFlux(float _level, float _levelClamped);
        }
    }
}
