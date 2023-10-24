using System.Collections.Generic;
using UnityEngine;
using ECellDive.Interfaces;

namespace ECellDive.Utility.Data.Graph
{
	/// <summary>
	/// The data structure to implement a <see cref="INode"/> and nothing more.
	/// </summary>
	[System.Serializable]
	public struct Node : INode
	{
		#region - INode Fields -
		/// <summary>
		/// The field for the property <see cref="ID"/>.
		/// </summary>
		[SerializeField] private uint m_ID;

		/// <inheritdoc/>
		public uint ID { get => m_ID; set => m_ID = value; }

		/// <summary>
		/// The field for the property <see cref="name"/>.
		/// </summary>
		[SerializeField] private string m_name;

		/// <inheritdoc/>
		public string name { get => m_name; set => m_name = value; }

		/// <summary>
		/// The field for the property <see cref="incommingEdges"/>.
		/// </summary>
		[SerializeField] private List<uint> m_incommingEdges;

		/// <inheritdoc/>
		public List<uint> incommingEdges { get => m_incommingEdges; set => m_incommingEdges = value; }

		/// <summary>
		/// The field for the property <see cref="outgoingEdges"/>.
		/// </summary>
		[SerializeField] private List<uint> m_outgoingEdges;

		/// <inheritdoc/>
		public List<uint> outgoingEdges { get => m_outgoingEdges; set => m_outgoingEdges = value; }
		#endregion

		public Node(uint _ID, string _name)
		{
			m_ID = _ID;
			m_name = _name;
			m_incommingEdges = new List<uint>();
			m_outgoingEdges = new List<uint>();
		}
	}
	
	/// <summary>
	/// The data structure to encode a node for a cyjson graph.
	/// </summary>
	[System.Serializable]
	public struct CyJsonNode : INode
	{
		#region - INode Fields -
		/// <summary>
		/// The field for the property <see cref="ID"/>.
		/// </summary>
		[SerializeField] private uint m_ID;

		/// <inheritdoc/>
		public uint ID { get => m_ID; set => m_ID = value; }

		/// <summary>
		/// The field for the property <see cref="name"/>.
		/// </summary>
		[SerializeField] private string m_name;

		/// <inheritdoc/>
		public string name { get => m_name; set => m_name = value; }

		/// <summary>
		/// The field for the property <see cref="incommingEdges"/>.
		/// </summary>
		[SerializeField] private List<uint> m_incommingEdges;

		/// <inheritdoc/>
		public List<uint> incommingEdges { get => m_incommingEdges; set => m_incommingEdges = value; }

		/// <summary>
		/// The field for the property <see cref="outgoingEdges"/>.
		/// </summary>
		[SerializeField] private List<uint> m_outgoingEdges;

		/// <inheritdoc/>
		public List<uint> outgoingEdges { get => m_outgoingEdges; set => m_outgoingEdges = value; }
		#endregion

		/// <summary>
		/// The field for the property <see cref="position"/>.
		/// </summary>
		[SerializeField] private Vector3 m_position;

		/// <summary>
		/// Position in the 3D space of the node
		/// </summary>
		public Vector3 position { get => m_position; set => m_position = value; }
		
		/// <summary>
		/// The field for the property <see cref="label"/>.
		/// </summary>
		[SerializeField] private string m_label;

		/// <summary>
		/// A string to store additional textual information about the 
		/// node.
		/// </summary>
		/// <remarks>
		/// In CyJson graphs, the user-readable name for nodes is
		/// actually encoded the Label while the Name is shorter
		/// and less explicit.
		/// </remarks>
		public string label { get => m_label; set => m_label = value; }

		/// <summary>
		/// The field for the property <see cref="isVirtual"/>.
		/// </summary>
		private bool m_isVirtual;

		/// <summary>
		/// A utility state variable to describe whether the node is
		/// simply there to structure the network or if it's a node
		/// representing important data for the user.
		/// </summary>
		public bool isVirtual { get => m_isVirtual ; set => m_isVirtual = value ; }

		public CyJsonNode(uint _ID, string _label, string _name, Vector3 _position, bool _isVirtual)
		{
			m_ID = _ID;
			m_position = _position;
			m_label = _label;
			m_name = _name;
			m_incommingEdges = new List<uint>();
			m_outgoingEdges = new List<uint>();
			m_isVirtual = _isVirtual;
		}
	}
}

