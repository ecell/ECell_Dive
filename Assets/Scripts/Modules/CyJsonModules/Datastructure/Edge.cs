using UnityEngine;
using ECellDive.Interfaces;

namespace ECellDive.GraphComponents
{
	/// <summary>
	/// The data structure to encode an edge for a cyjson graph.
	/// </summary>
	[System.Serializable]
	public struct Edge : IEdge
	{
		#region - IEdge Fields -
		/// <summary>
		/// The field for <see cref="ID"/>.
		/// </summary>
		[SerializeField] private uint m_ID;

		/// <inheritdoc/>
		public uint ID { get => m_ID; set => m_ID = value; }

		/// <summary>
		/// The field for <see cref="source"/>.
		/// </summary>
		[SerializeField] private uint m_source;

		/// <inheritdoc/>
		public uint source { get => m_source; set => m_source = value; }

		/// <summary>
		/// The field for <see cref="target"/>.
		/// </summary>
		[SerializeField] private uint m_target;

		/// <inheritdoc/>
		public uint target { get => m_target; set => m_target = value; }

		/// <summary>
		/// The field for <see cref="name"/>.
		/// </summary>
		[SerializeField] private string m_name;

		/// <inheritdoc/>
		public string name { get => m_name; set => m_name = value; }
		#endregion

		public Edge(uint _ID, string _name, uint _source, uint _target)
		{
			m_ID = _ID;
			m_source = _source;
			m_target = _target;
			m_name = _name;
		}
	}
}
