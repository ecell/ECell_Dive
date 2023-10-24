using UnityEngine;

using ECellDive.Utility;

namespace ECellDive.Modules
{
	/// <summary>
	/// The Module to control multiplayer connection.
	/// </summary>
	public class MultiplayerModule : Module
	{
		/// <summary>
		/// The singleton instance of this class.
		/// </summary>
		[Header("Multiplayer Module")]
		static public MultiplayerModule Instance;

		/// <summary>
		/// The animation loop controller to control the visual feedback
		/// of the module in case of request.
		/// </summary>
		[SerializeField] private AnimationLoopWrapper alw;

		/// <summary>
		/// The color flash component to alter the visual feedback
		/// of the module in case of request.
		/// </summary>s
		[SerializeField] private ColorFlash colorFlash;

		private void Start()
		{
			Instance = this;
		}

		/// <summary>
		/// The logic for this module after starting to try to establish a connection.
		/// </summary>
		public void OnConnectionStart()
		{
			alw.PlayLoop("MultiplayerModule");
		}

		/// <summary>
		/// The logic for this module after the connection has failed.
		/// </summary>
		public void OnConnectionFails()
		{
			alw.StopLoop();
			colorFlash.Flash(0);//red fail flash
		}

		/// <summary>
		/// The logic for this module after the connection was successful.
		/// </summary>
		public void OnConnectionSuccess()
		{
			alw.StopLoop();
			colorFlash.Flash(1);//Green fail flash
		}
	}
}