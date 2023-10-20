using UnityEngine;

using ECellDive.Utility;

namespace ECellDive.Modules
{
	/// <summary>
	/// The Module to control multiplayer connection.
	/// </summary>
	[RequireComponent(typeof(AnimationLoopWrapper), typeof(ColorFlash))]
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

		/// <summary>
		/// The array of renderers of this module
		/// </summary>
		/// <remarks>
		/// Used to change the color of the module when highlighted and flashed.
		/// </remarks>
		[SerializeField] private Renderer[] renderers;
		
		/// <summary>
		/// The material property block used to change the color of the module while 
		/// avoiding to create a new material instance.
		/// </summary>
		private MaterialPropertyBlock mpb;

		/// <summary>
		/// The ID of the color property in the shader of the module to change its color
		/// in the material property block.
		/// </summary>
		private int colorID;

		private void Start()
		{
			Instance = this;
		}

		private void OnEnable()
		{
			mpb = new MaterialPropertyBlock();
			colorID = Shader.PropertyToID("_Color");
			mpb.SetVector(colorID, defaultColor);
			foreach (Renderer _renderer in renderers)
			{
				_renderer.SetPropertyBlock(mpb);
			}
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

		#region - IHighlightable -
		/// <inheritdoc/>
		public override void ApplyColor(Color _color)
		{
			mpb.SetVector(colorID, _color);
			foreach (Renderer _renderer in renderers)
			{
				_renderer.SetPropertyBlock(mpb);
			}
		}

		/// <inheritdoc/>
		public override void SetHighlight()
		{
			ApplyColor(highlightColor);
		}

		/// <inheritdoc/>
		public override void UnsetHighlight()
		{
			if (!forceHighlight)
			{
				ApplyColor(defaultColor);
			}
		}
		#endregion
	}
}