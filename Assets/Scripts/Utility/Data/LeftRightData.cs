namespace ECellDive.Utility.Data
{
	[System.Serializable]
	public struct LeftRightData<T>
	{
		[UnityEngine.SerializeField] private T m_left;
		public T left { get => m_left; set => m_left = value; }

		[UnityEngine.SerializeField] private T m_right;
		public T right { get => m_right; set => m_right = value; }
	}
}

