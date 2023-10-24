using UnityEngine;
using TMPro;


namespace ECellDive.Interfaces
{
    /// <summary>
    /// Interface to manage a Bezier curve.
    /// </summary>
    public interface IBezierCurve
    {
        /// <summary>
        /// The control points of the curve.
        /// With 2 points it will just be a Lerp. With 3 points to
        /// we have a quadratic curve and with 4 points, the curve
        /// is cubic. And so on.
        /// </summary>
        Vector3[] controlPoints { get; }

        /// <summary>
        /// The number of control points.
        /// </summary>
        uint controlPointsCount { get; }

        /// <summary>
        /// The interpolated points of the curve.
        /// </summary>
        Vector3[] curvePoints { get; }

        /// <summary>
        /// The number of interpolated points.
        /// </summary>
        uint curvePointsCount { get; set; }

        /// <summary>
        /// Adds a control point at the back.
        /// </summary>
        /// <param name="_point">
        /// The new control point to add.
        /// </param>
        void AddControlPoint(Vector3 _point);

		/// <summary>
		/// Computes <see name="curvePointsCount"/> - 2 between
		/// the first and last control points.
		/// </summary>
		/// <returns>
		/// <see cref="curvePoints"/> after interpolation.
		/// </returns>
		Vector3[] Interpolate();

		/// <summary>
		/// Computes <paramref name="_curvePointsCount"/> - 2 between
        /// the first and last control points. Also sets the value of
        /// <see cref="curvePointsCount"/> to <paramref name="_curvePointsCount"/>.
		/// </summary>
		/// <param name="_curvePointsCount">
        /// The number of points to interpolate.
        /// </param>
		/// <returns>
        /// <see cref="curvePoints"/> after interpolation.
        /// </returns>
		Vector3[] Interpolate(uint _curvePointsCount);

        /// <summary>
        /// Sets the control point at the given index.
        /// Guards against out of range index but nothing happens
        /// in that case.
        /// </summary>
        /// <param name="_index">
        /// The index of the control point to set.
        /// </param>
        /// <param name="_point">
        /// The new value of the control point.
        /// </param>
        void SetControlPoint(int _index, Vector3 _point);

        /// <summary>
        /// Sets the control points of the curve.
        /// </summary>
        /// <param name="_points">
        /// The new control points.
        /// </param>
        void SetControlPoints(Vector3[] _points);
    }

    /// <summary>
    /// Interface for gameobject in the scene that may
    /// display a name.
    /// </summary>
    public interface INamed
    {
        /// <summary>
        /// The gameobject that contains the <see cref="nameField"/>.
        /// </summary>
        /// <remarks>
        /// Usefull when we wish to display/hide the name by manipulating the
        /// gameobject activity instead of disabling/enabling
        /// the <see cref="nameField"/>.
        /// </remarks>
        GameObject nameTextFieldContainer { get; }

        /// <summary>
        /// A reference to the component used to display a string corresponding
        /// to the name.
        /// </summary>
        TextMeshProUGUI nameField { get; }

        /// <summary>
        /// Makes the name field visible (using GameObject.SetActive and the like)
        /// </summary>
        void DisplayName();

        /// <summary>
        /// Gets the value stored in <see cref="nameField"/>.
        /// </summary>
        /// <returns>The text value of <see cref="nameField"/></returns>
        string GetName();

        /// <summary>
        /// Makes the name field invisible.
        /// </summary>
        void HideName();

        /// <summary>
        /// Sets the value to display through <see cref="nameField"/>.
        /// </summary>
        /// <param name="_name">The name to display</param>
        void SetName(string _name);

        /// <summary>
        /// Make the name readable from the POV of the camera
        /// </summary>
        void ShowName();
    }
}

