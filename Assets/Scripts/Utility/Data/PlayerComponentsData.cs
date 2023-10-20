using UnityEngine;
using UnityEngine.InputSystem;

namespace ECellDive.Utility.Data.PlayerComponents
{
	/// <summary>
	/// A simple struct to hold all input actions related to movement
	/// for the player.
	/// </summary>
	[System.Serializable]
	public struct MovementActionData
	{
		/// <summary>
		/// The action to move the player.
		/// </summary>
		public InputActionReference movement;

		/// <summary>
		/// The action to retrieve the position of the cursor.
		/// </summary>
		public InputActionReference cursorController;

		/// <summary>
		/// The action to switch between movement modes.
		/// </summary>
		public InputActionReference switchMovementMode;

		/// <summary>
		/// The action to retrieve the position of the controller.
		/// </summary>
		public InputActionReference controllerPosition;
	}

	/// <summary>
	/// A simple struct to hold all the data related to the teleportation
	/// of the player.
	/// </summary>
	[System.Serializable]
	public struct TeleportationMovementData
	{
		/// <summary>
		/// The gameobject encapsulating the reticle indicating
		/// teleportation tagets.
		/// </summary>
		public GameObject teleportationReticle;

		/// <summary>
		/// The default position of the reticle.
		/// </summary>
		public Vector3 defaultReticlePosition;

		/// <summary>
		/// The speed at which the reticle can be moved to adapt
		/// the teleportation target.
		/// </summary>
		public float reticleMovementSpeed;

		/// <summary>
		/// The line renderer used to indicate the teleportation
		/// direction.
		/// </summary>
		public LineRenderer teleportationLine;

		/// <summary>
		/// The minimal teleportation distance.
		/// </summary>
		public float minTeleportationDistance;

		/// <summary>
		/// The maximal teleportation distance.
		/// </summary>
		public float maxTeleportationDistance;
	}

	/// <summary>
	/// A simple struct to hold all the data related to the
	/// continuous movement of the player.
	/// </summary>
	[System.Serializable]
	public struct ContinousMovementData
	{
		/// <summary>
		/// Reference to the component defining the behaviour of the
		/// continuous movement.
		/// </summary>
		public AnchoredContinousMoveHelper directionHelper;

		/// <summary>
		/// The default position of the direction helper.
		/// </summary>
		public Vector3 defaultPosition;

		/// <summary>
		/// The size of the elipsoid used to define the dead zone
		/// in which the user's input is ignored.
		/// </summary>
		public Vector3 deadZone;

		/// <summary>
		/// The movement speed of the player.
		/// </summary>
		public float speed;
	}
}
