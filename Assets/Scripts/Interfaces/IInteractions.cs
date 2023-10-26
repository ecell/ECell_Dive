using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

using ECellDive.Utility.Data;

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
			/// <summary>
			/// A boolean to inform whether a gameobject is currently
			/// being focus.
			/// </summary>
			/// <remarks>
			/// In most cases, this will be set to true when a ray interactor
			/// is pointing at it, and false otherwise.
			/// </remarks>
			bool isFocused { get;}

			/// <summary>
			/// A mutator to set the value of <see cref="isFocused"/> to true.
			/// </summary>
			/// <remarks>
			/// It is mainly used as a subscriber to "UnityEngine.XR.
			/// Interaction.Toolkit.XRBaseInteractable.OnHoverEntered(
			/// UnityEngine.XR.Interaction.Toolkit.HoverEnterEventArgs)"
			/// </remarks>
			void SetFocus();

			/// <summary>
			/// A mutator to set the value of <see cref="isFocused"/> to false.
			/// </summary>
			/// <remarks>
			/// It is mainly used as a subscriber to "UnityEngine.XR.
			/// Interaction.Toolkit.XRBaseInteractable.OnHoverEntered(
			/// UnityEngine.XR.Interaction.Toolkit.HoverEnterEventArgs)"
			/// </remarks>
			void UnsetFocus();
		}

		/// <summary>
		/// An interface to for any objects that may be grouped
		/// manually. It is mainly used as a "marker" for different
		/// FindComponent methods. So, if we create a new Monobehaviour
		/// or NetworkBehaviour and we expect to be able to use the 
		/// manual grouping features, this interface needs to be implemented.
		/// </summary>
		public interface IGroupable
		{
			/// <summary>
			/// The index of the gameobject in the last group he was made
			/// part of.
			/// </summary>
			int grpMemberIndex { get; set; }

			/// <summary>
			/// The actual gameobject that should be grouped.
			/// </summary>
			/// <remarks>This is very practicle to dissociate the gameobject
			/// that has the collider used to detect grouping requests from the
			/// reference of the gameobject that should actually be grouped.</remarks>
			GameObject delegateTarget { get; }
		}

		/// <summary>
		/// An interface to implement highlighting of a gameobject.
		/// </summary>
		public interface IHighlightable
		{
			/// <summary>
			/// A boolean to inform whether the object should stay in an
			/// highlighted state even when it should go back to default.
			/// </summary>
			bool forceHighlight { get; set; }

			/// <summary>
			/// The function to call when the object should enter highlighted
			/// state.
			/// </summary>
			abstract void SetHighlight();

			/// <summary>
			/// The function to call when the object should exit the highlighted
			/// state.
			/// </summary>
			abstract void UnsetHighlight();
		}

		/// <summary>
		/// An interface to change switch between two colors when a gameobject
		/// needs to be highlighted.
		/// </summary>
		public interface IColorHighlightable : IColored, IHighlightable
		{
			/// <summary>
			/// The color the object should be when highlighted.
			/// </summary>
			Color highlightColor { get; set; }
		}

		/// <summary>
		/// An interface to change switch between two colors when a NetworkObject
		/// needs to be highlighted.
		/// </summary>
		public interface IColorHighlightableNet : IColorHighlightable
		{
			/// <summary>
			/// The current color of the object synchronized over the 
			/// network.
			/// </summary>
			NetworkVariable<Color> currentColor { get; }

			/// <summary>
			/// Contacts the server to applies <see cref="IColorHighlightable.defaultColor"/>
			/// to <see cref="currentColor"/>.
			/// </summary>
			/// <remarks>Since <see cref="currentColor"/> is a NetworkVariable,
			/// the value will be synchronized to all clients.</remarks>
			[ServerRpc(RequireOwnership = false)]
			void SetCurrentColorToDefaultServerRpc();

			/// <summary>
			/// Contacts the server to applies <see cref="IColorHighlightable.highlightColor"/>
			/// to <see cref="currentColor"/>.
			/// </summary>
			/// <remarks>Since <see cref="currentColor"/> is a NetworkVariable,
			/// the value will be synchronized to all clients.</remarks>
			[ServerRpc(RequireOwnership = false)]
			abstract void SetCurrentColorToHighlightServerRpc();
		}

		/// <summary>
		/// The interface describing the requirements to implement a handle for a
		/// group of information tags attached to a gameobject.
		/// </summary>
		public interface IInfoTags
		{
			/// <summary>
			/// A boolean to inform whether the infotags are currently visible (i.e. displayed).
			/// </summary>
			bool areVisible { get; }

			/// <summary>
			/// The references to the input actions on the left and right controller
			/// used to trigger the info tags visibility.
			/// </summary>
			LeftRightData<InputActionReference> displayInfoTagsActions { get; set; }

			/// <summary>
			/// The prefab used to instantiate more infotags programatically.
			/// </summary>
			GameObject refInfoTagPrefab { get; set; }

			/// <summary>
			/// The reference to the parent gameobject each info tag will
			/// be made a child upon instantiation.
			/// </summary>
			GameObject refInfoTagsContainer { get; set; }

			/// <summary>
			/// Make the InfoTags visible in the scene
			/// </summary>
			void DisplayInfoTags();

			/// <summary>
			/// Make the InfoTags invisible in the scene
			/// </summary>
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
			/// <summary>
			/// The references to the input actions on the left and right controller
			/// used to trigger the knockout status.
			/// </summary>
			LeftRightData<InputActionReference> triggerKOActions { get; set; }

			/// <summary>
			/// A boolean used to inform whether the gameobject is considered
			/// knocked out.
			/// </summary>
			NetworkVariable<bool> knockedOut { get; }

			/// <summary>
			/// The logic to implement when the gameobject transitions from
			/// <see cref="knockedOut"/>=true to <see cref="knockedOut"/>=false.
			/// </summary>
			void Activate();

			/// <summary>
			/// The logic to implement when the gameobject transitions from
			/// <see cref="knockedOut"/>=false to <see cref="knockedOut"/>=true.
			/// </summary>
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
			/// The real flux value.
			/// </summary>
			NetworkVariable<float> fluxLevel { get; }

			/// <summary>
			/// A clamped flux value to control the visualization of the flux.
			/// </summary>
			NetworkVariable<float> fluxLevelClamped { get; }

			/// <summary>
			/// The logic to apply the effects associated to the value of
			/// <see cref="fluxLevel"/>.
			/// </summary>
			void ApplyFluxLevel();

			/// <summary>
			/// The logic to apply the effects associated to the value of
			/// <see cref="fluxLevelClamped"/>.
			/// </summary>
			void ApplyFluxLevelClamped();

			/// <summary>
			/// A mutator for values of <see cref="fluxLevel"/> and 
			/// <see cref="fluxLevelClamped"/>.
			/// </summary>
			/// <param name="_level">The value to pass on to <see cref="fluxLevel"/>.</param>
			/// <param name="_levelClamped">The value to pass on to <see cref="fluxLevelClamped"/>.</param>
			void SetFlux(float _level, float _levelClamped);
		}

	}
}
