using UnityEngine;

namespace ECellDive.Utility
{
	/// <summary>
	/// This class is used to help the user to understand the AnchoredContinousMove
	/// component. It is used to display the deadzone and the magnitude of the
	/// resulting movement vector.
	/// </summary>
	public class AnchoredContinousMoveHelper : MonoBehaviour
	{
		/// <summary>
		/// The reference sphere that is used to represend the deadzone
		/// elipsoid.
		/// </summary>
		public GameObject refSphere;

		/// <summary>
		/// The line renderer to represent the magnitude of the X component
		/// in the resulting movement vector.
		/// </summary>
		[Header("Line X")]
		public LineRenderer refXLineRenderer;

		/// <summary>
		/// The color gradient of the line when the magnitude of the X component
		/// is in the deadzone.
		/// </summary>
		public Gradient XLineNonValidGradient;

		/// <summary>
		/// The color gradient of the line when the magnitude of the X component
		/// is outside the deadzone.
		/// </summary>
		public Gradient XLineValidGradient;

		/// <summary>
		/// The line renderer to represent the magnitude of the Y component
		/// in the resulting movement vector.
		/// </summary>
		[Header("Line Y")]
		public LineRenderer refYLineRenderer;

		/// <summary>
		/// The color gradient of the line when the magnitude of the Y component
		/// is in the deadzone.
		/// </summary>
		public Gradient YLineNonValidGradient;

		/// <summary>
		/// The color gradient of the line when the magnitude of the Y component
		/// is outside the deadzone.
		/// </summary>
		public Gradient YLineValidGradient;

		/// <summary>
		/// The line renderer to represent the magnitude of the Z component
		/// in the resulting movement vector.
		/// </summary>
		[Header("Line Z")]
		public LineRenderer refZLineRenderer;

		/// <summary>
		/// The color gradient of the line when the magnitude of the Z component
		/// is in the deadzone.
		/// </summary>
		public Gradient ZLineNonValidGradient;

		/// <summary>
		/// The color gradient of the line when the magnitude of the Z component
		/// is outside the deadzone.
		/// </summary>
		public Gradient ZLineValidGradient;

		private void Start()
		{
			refXLineRenderer.colorGradient = XLineNonValidGradient;
			refYLineRenderer.colorGradient = YLineNonValidGradient;
			refZLineRenderer.colorGradient = ZLineNonValidGradient;
		}

		/// <summary>
		/// Changes the color gradients of the lines to the valid ones
		/// if their respective length are above the thresholds.
		/// </summary>
		/// <param name="_linesLength">Length of line X at position _linesLength.x
		/// (same for Y and Z).</param>
		/// <param name="_thresholds">Threshold for line X at position _threshold.x
		/// (same for Y and Z).</param>
		public void CheckValidity(Vector3 _linesLength, Vector3 _thresholds)
		{
			if (Mathf.Abs(_linesLength.x) > _thresholds.x)
			{
				refXLineRenderer.colorGradient = XLineValidGradient;
			}
			else
			{
				refXLineRenderer.colorGradient = XLineNonValidGradient;
			}

			if (Mathf.Abs(_linesLength.y) > _thresholds.y)
			{
				refYLineRenderer.colorGradient = YLineValidGradient;
			}
			else
			{
				refYLineRenderer.colorGradient = YLineNonValidGradient;
			}

			if (Mathf.Abs(_linesLength.z) > _thresholds.z)
			{
				refZLineRenderer.colorGradient = ZLineValidGradient;
			}
			else
			{
				refZLineRenderer.colorGradient = ZLineNonValidGradient;
			}
		}

		/// <summary>
		/// Place the helper in front of the camera with only the rotation on the y axis
		/// </summary>
		public void FlatPositioning()
		{
			transform.position = Positioning.PlaceInFrontOfTargetLocal(Camera.main.transform, 0.5f, -0.2f);
			transform.LookAt(new Vector3(Camera.main.transform.position.x,
											transform.position.y,
											Camera.main.transform.position.z));
		}

		/// <summary>
		/// The lines have only 2 positions. We set the end of the line 
		/// with the positions at index 1 in the LineRenderer.
		/// </summary>
		/// <param name="_endPositions">_endPositions.x is the length of XLine
		/// along the X axis. it's the same for Y and Z.</param>
		public void SetLinesEndPositions(Vector3 _endPositions)
		{
			refXLineRenderer.SetPosition(1, _endPositions.x * Vector3.right);
			refYLineRenderer.SetPosition(1, _endPositions.y * Vector3.up);
			refZLineRenderer.SetPosition(1, _endPositions.z * Vector3.forward);
		}

		/// <summary>
		/// Sets the local scale of the sphere (which can be an ellipsoid).
		/// </summary>
		/// <param name="_scale"></param>
		public void SetSphereScale(Vector3 _scale)
		{
			refSphere.transform.localScale = _scale;
		}
	}
}

