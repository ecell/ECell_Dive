namespace ECellDive.Utility.Data
{
	/// <summary>
	/// A struct to encapsulate a pair of data usually associated
	/// with the left or right XR controllers or hands.
	/// </summary>
	/// <typeparam name="T">
	/// The type of data that can be used for both the left and right
	/// controllers or hands.
	/// </typeparam>
	[System.Serializable]
	public struct LeftRightData<T>
	{
		/// <summary>
		/// The field of the property <see cref="left"/>
		/// </summary>
		[UnityEngine.SerializeField] private T m_left;

		/// <summary>
		/// The public property for accessing and setting the left data.
		/// </summary>
		public T left { get => m_left; set => m_left = value; }

		/// <summary>
		/// The field of the property <see cref="right"/>
		/// </summary>
		[UnityEngine.SerializeField] private T m_right;

		/// <summary>
		/// The public property for accessing and setting the right data.
		/// </summary>
		public T right { get => m_right; set => m_right = value; }
	}
}

