using System.Collections.Generic;
using UnityEngine;
using ECellDive.Interfaces;

namespace ECellDive.GraphComponents
{
	/// <summary>
	/// The data structure to encode a node for a cyjson graph.
	/// </summary>
	[System.Serializable]
	public class Node : INode
	{
		#region - INode Fields -
		/// <summary>
		/// The field for <see cref="ID"/>.
		/// </summary>
		[SerializeField] private uint m_ID;

		/// <inheritdoc/>
		public uint ID { get => m_ID; set => m_ID = value; }

		/// <summary>
		/// The field for <see cref="position"/>.
		/// </summary>
		[SerializeField] private Vector3 m_position;

		/// <inheritdoc/>
		public Vector3 position { get => m_position; set => m_position = value; }

		/// <summary>
		/// The field for <see cref="name"/>.
		/// </summary>
		[SerializeField] private string m_name;

		/// <inheritdoc/>
		public string name { get => m_name; set => m_name = value; }

		/// <summary>
		/// The field for <see cref="label"/>.
		/// </summary>
		[SerializeField] private string m_label;

		/// <inheritdoc/>
		public string label { get => m_label; set => m_label = value; }

		/// <summary>
		/// The field for <see cref="incommingEdges"/>.
		/// </summary>
		[SerializeField] private List<uint> m_incommingEdges;

		/// <inheritdoc/>
		public List<uint> incommingEdges { get => m_incommingEdges; set => m_incommingEdges = value; }

		/// <summary>
		/// The field for <see cref="outgoingEdges"/>.
		/// </summary>
		[SerializeField] private List<uint> m_outgoingEdges;

		/// <inheritdoc/>
		public List<uint> outgoingEdges { get => m_outgoingEdges; set => m_outgoingEdges = value; }

		/// <summary>
		/// The field for <see cref="isVirtual"/>.
		/// </summary>
		[SerializeField] private bool m_isVirtual;

		/// <inheritdoc/>
		public bool isVirtual { get => m_isVirtual; set => m_isVirtual = value; }
		#endregion

		public Node(uint _ID, string _label, string _name, Vector3 _position, bool _isVirtual)
		{
			ID = _ID;
			position = _position;
			label = _label;
			name = _name;
			incommingEdges = new List<uint>();
			outgoingEdges = new List<uint>();
			isVirtual = _isVirtual;
		}
	}
}

