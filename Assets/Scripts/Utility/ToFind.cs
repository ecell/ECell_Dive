using UnityEngine;

namespace ECellDive.Utility
{
	/// <summary>
	/// A utility class that provides a function to find a component in itself, its children
	/// gameobject and its parent (if it exists).
	/// </summary>
	public static class ToFind
	{
		/// <summary>
		/// A utility function that tries to find a component in itself, its children
		/// gameobject and its parent (if it exists)
		/// </summary>
		/// <typeparam name="T">The type of the component to look for.</typeparam>
		/// <param name="_go">The source gameobject of the search.</param>
		/// <returns>The component if it found one or null if it didn't.</returns>
		/// <remarks>Implemented to handle the case when the gameobject that has
		/// the collider is not the gameobject that has the graphics renderer or any
		/// other component of interest.</remarks>
		public static T FindComponent<T>(GameObject _go)
		{
			T component = _go.GetComponentInChildren<T>();
			if (component == null)
			{
				if (_go.transform.parent != null)
				{
					component = _go.transform.parent.gameObject.GetComponent<T>();
				}
			}
			return component;
		}
	}
}

