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
	/// An Interface to store a default color and a method on 
	/// how to apply a color to a gameobject.
	/// </summary>
	public interface IColored
	{
		/// <summary>
		/// The color the object should be by default.
		/// </summary>
		Color defaultColor { get; set; }

		/// <summary>
		/// The function to call when we wish to apply a color to the material
		/// of the gameobject.
		/// </summary>
        /// <param name="_color">
        /// The color to apply.
        /// </param>
		abstract void ApplyColor(Color _color);
	}

    /// <summary>
    /// An interface to store a gradient and a method on
    /// how to apply a gradient to a gameobject.
    /// </summary>
    public interface IGradient
    {
        /// <summary>
        /// The array of colors that make up the gradient.
        /// </summary>
        Color[] defaultGradient { get; set; }

        /// <summary>
        /// The function to call when we wish to apply a gradient to the material
        /// of the gameobject.
        /// </summary>
        /// <param name="_gradient">
        /// The gradient to apply.
        /// </param>
        abstract void ApplyGradient(Color[] _gradient);
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

    /// <summary>
    /// An interface to store a scale and a method on
    /// how to apply a scale to a gameobject.
    /// </summary>
    public interface IScaled
    {
        /// <summary>
        /// The scale the object should be by default.
        /// </summary>
        Vector3 defaultScale { get; set; }

        /// <summary>
        /// The function to call when we wish to apply a scale to the gameobject.
        /// </summary>
        /// <param name="_scale">
        /// The scale to apply.
        /// </param>
        abstract void ApplyScale(Vector3 _scale);
    }
}

