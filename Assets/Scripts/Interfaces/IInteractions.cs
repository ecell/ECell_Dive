using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ECellDive
{
    namespace Interfaces
    {
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

        public interface IGroupable
        {
            int grpMemberIndex { get; set; }
        }

        /// <summary>
        /// An interface to change switch between two colors when a gameobject
        /// needs to be highlighted.
        /// </summary>
        public interface IHighlightable
        {
            NetworkVariable<Color> currentColor { get; }
            Color defaultColor { get; }
            Color highlightColor { get; }

            bool forceHighlight { get; set; }

            /// <summary>
            /// Contacts the server to applies <see cref="defaultColor"/>
            /// to <see cref="currentColor"/>.
            /// </summary>
            /// <remarks>Since <see cref="currentColor"/> is a <see cref=
            /// "NetworkVariable{T}"/>, the value will be synchronized to all
            /// clients.</remarks>
            [ServerRpc(RequireOwnership = false)]
            void SetDefaultServerRpc();

            /// <summary>
            /// Sets the value of <see cref="defaultColor"/> to <paramref name="_c"/>.
            /// </summary>
            /// <param name="_c">The new value for <see cref="defaultColor"/></param>
            void SetDefaultColor(Color _c);

            /// <summary>
            /// Contacts the server to applies <see cref="highlightColor"/>
            /// to <see cref="currentColor"/>.
            /// </summary>
            /// <remarks>Since <see cref="currentColor"/> is a <see cref=
            /// "NetworkVariable{T}"/>, the value will be synchronized to all
            /// clients.</remarks>
            [ServerRpc(RequireOwnership = false)]
            abstract void SetHighlightServerRpc();

            /// <summary>
            /// Sets the value of <see cref="highlightColor"/> to <paramref name="_c"/>.
            /// </summary>
            /// <param name="_c">The new value for <see cref="highlightColor"/></param>
            void SetHighlightColor(Color _c);

            /// <summary>
            /// Checks for <see cref="forceHighlight"/>.
            /// If false, applies <see cref="defaultColor"/> to <see cref="currentColor"/>.
            /// If true, nothing happens.
            /// </summary>
            /// <remarks>Since <see cref="currentColor"/> is a <see cref=
            /// "NetworkVariable{T}"/>, the value will be synchronized to all
            /// clients.</remarks>
            [ServerRpc(RequireOwnership = false)]
            abstract void UnsetHighlightServerRpc();
        }

        /// <summary>
        /// The interface describing the requirements to implement a handle for a
        /// group of information tags attached to a gameobject.
        /// </summary>
        public interface IInfoTags
        {
            bool areVisible { get; }

            LeftRightData<InputActionReference> displayInfoTagsActions { get; set; }

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
            LeftRightData<InputActionReference> triggerKOActions { get; set; }
            NetworkVariable<bool> knockedOut { get; }
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
            NetworkVariable<float> fluxLevel { get; }

            /// <summary>
            /// A clamped flux value to control the visualization of the flux
            /// </summary>
            NetworkVariable<float> fluxLevelClamped { get; }
            void SetFlux(float _level, float _levelClamped);
        }

    }
}
