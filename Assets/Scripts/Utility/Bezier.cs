using UnityEngine;

namespace ECellDive.Utility.Maths
{
	/// <summary>
	/// A collection of Bezier curve functions.
	/// </summary>
	public static class Bezier
	{
		/// <summary>
		/// Quadratic Bezier curve in the Bernstein form.
		/// </summary>
		/// <param name="p0">
		/// The starting point.
		/// </param>
		/// <param name="p1">
		/// The control point.
		/// </param>
		/// <param name="p2">
		/// The end point.
		/// </param>
		/// <param name="t">
		/// The time parameter.
		/// </param>
		/// <returns>
		/// The point on the curve at time t.
		/// </returns>
		public static Vector3 Quadratic(Vector3 p0, Vector3 p1, Vector3 p2, float t)
		{
			t = Mathf.Clamp01(t);
			float oneMinusT = 1f - t;
			return
				oneMinusT * oneMinusT * p0 +
				2f * oneMinusT * t * p1 +
				t * t * p2;
		}

		/// <summary>
		/// First derivative of the quadratic Bezier curve in the Bernstein form.
		/// </summary>
		/// <param name="p0">
		/// The starting point.
		/// </param>
		/// <param name="p1">
		/// The control point.
		/// </param>
		/// <param name="p2">
		/// The end point.
		/// </param>
		/// <param name="t">
		/// The time parameter.
		/// </param>
		/// <returns>
		/// The first derivative of the curve at time t.
		/// </returns>
		public static Vector3 QuadraticFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, float t)
		{
			return
				2f * (1f - t) * (p1 - p0) +
				2f * t * (p2 - p1);
		}

		/// <summary>
		/// Cubic Bezier curve in the Bernstein form.
		/// </summary>
		/// <param name="p0">
		/// The starting point.
		/// </param>
		/// <param name="p1">
		/// The first control point.
		/// </param>
		/// <param name="p2">
		/// The second control point.
		/// </param>
		/// <param name="p3">
		/// The end point.
		/// </param>
		/// <param name="t">
		/// The time parameter.
		/// </param>
		/// <returns>
		/// The point on the curve at time t.
		/// </returns>
		public static Vector3 Cubic(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
		{
			t = Mathf.Clamp01(t);
			float OneMinusT = 1f - t;
			return
				OneMinusT * OneMinusT * OneMinusT * p0 +
				3f * OneMinusT * OneMinusT * t * p1 +
				3f * OneMinusT * t * t * p2 +
				t * t * t * p3;
		}

		/// <summary>
		/// First derivative of the cubic Bezier curve in the Bernstein form.
		/// </summary>
		/// <param name="p0">
		/// The starting point.
		/// </param>
		/// <param name="p1">
		/// The first control point.
		/// </param>
		/// <param name="p2">
		/// The second control point.
		/// </param>
		/// <param name="p3">
		/// The end point.
		/// </param>
		/// <param name="t">
		/// The time parameter.
		/// </param>
		/// <returns>
		/// The first derivative of the curve at time t.
		/// </returns>
		public static Vector3 CubicFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
		{
			t = Mathf.Clamp01(t);
			float OneMinusT = 1f - t;
			return
				3f * OneMinusT * OneMinusT * (p1 - p0) +
				6f * OneMinusT * t * (p2 - p1) +
				3f * t * t * (p3 - p2);
		}

		/// <summary>
		/// Get the interpolation on a Bezier curve defined by an arbitrary
		/// number of points.
		/// 
		/// If the number of points is 3 or 4, the curve is evaluated directly
		/// thanks to <see cref="Quadratic"/> or <see cref="Cubic"/>. Otherwise,
		/// the curve is recursively subdivided until the number of points is 4.
		/// </summary>
		/// <param name="points">
		/// The points that define the curve.
		/// </param>
		/// <param name="t">
		/// The time parameter.
		/// </param>
		/// <returns>
		/// The point on the curve at time t.
		/// </returns>
		public static Vector3 GetPoint(Vector3[] points, float t)
		{
			int i;
			if (points.Length == 3)
			{
				return Quadratic(points[0], points[1], points[2], t);
			}
			else if (points.Length == 4)
			{
				return Cubic(points[0], points[1], points[2], points[3], t);
			}
			else
			{
				Vector3[] newPoints = new Vector3[points.Length - 1];
				for (i = 0; i < newPoints.Length; i++)
				{
					newPoints[i] = Vector3.Lerp(points[i], points[i + 1], t);
				}
				return GetPoint(newPoints, t);
			}
		}

		/// <summary>
		/// Get the first derivative of a Bezier curve defined by an arbitrary
		/// number of points.
		/// 
		/// If the number of points is 3 or 4, the curve is evaluated directly
		/// thanks to <see cref="QuadraticFirstDerivative"/> or
		/// <see cref="CubicFirstDerivative"/>. Otherwise, the curve is
		/// recursively subdivided until the number of points is 4.
		/// </summary>
		/// <param name="points">
		/// The points that define the curve.
		/// </param>
		/// <param name="t">
		/// The time parameter.
		/// </param>
		/// <returns>
		/// The first derivative of the curve at time t.
		/// </returns>
		public static Vector3 GetFirstDerivative(Vector3[] points, float t)
		{
			int i;
			if (points.Length == 3)
			{
				return QuadraticFirstDerivative(points[0], points[1], points[2], t);
			}
			else if (points.Length == 4)
			{
				return CubicFirstDerivative(points[0], points[1], points[2], points[3], t);
			}
			else
			{
				Vector3[] newPoints = new Vector3[points.Length - 1];
				for (i = 0; i < newPoints.Length; i++)
				{
					newPoints[i] = Vector3.Lerp(points[i], points[i + 1], t);
				}
				return GetFirstDerivative(newPoints, t);
			}
		}

	}

}
