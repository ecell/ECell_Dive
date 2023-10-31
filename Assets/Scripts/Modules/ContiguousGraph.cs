using System.Collections.Generic;
using UnityEngine;
using ECellDive.Interfaces;
using ECellDive.Utility.Data.Graph;

namespace ECellDive.GraphComponents
{
	/// <summary>
	/// Encodes the data for a graph made from reading the contiguous pairs of
	/// nodes in a list. Nodes are represented by integers.
	/// </summary>
	/// <example>
	/// [node1, node2, node3, ...., nodeN] yields the following graph:
	/// node1 -> node2 -> node3 -> ... -> nodeN
	/// If the graph may be cyclic with nodes repeating. [node1, node2, node3, node1, node4]
	/// yields the following graph:
	///   |-----------------------|
	///   |                       v
	/// node1 -> node2 -> node3 node4
	///   ^                 |
	///   |-----------------|
	/// </example>
	public class ContiguousGraph : IGraph<Edge, Node>
	{
		#region - IGraph Members -
		/// <summary>
		/// The field of the property <see cref="name"/>.
		/// </summary>
		private string m_name;

		/// <inheritdoc/>
		public string name
		{
			get => m_name;
			set => m_name = value;
		}

		/// <summary>
		/// The field of the property <see cref="nodes"/>.
		/// </summary>
		private Node[] m_nodes;

		/// <inheritdoc/>
		public Node[] nodes
		{
			get => m_nodes;
			set => m_nodes = value;
		}

		/// <summary>
		/// The field of the property <see cref="edges"/>.
		/// </summary>
		private Edge[] m_edges;

		/// <inheritdoc/>
		public Edge[] edges
		{
			get => m_edges;
			set => m_edges = value;
		}
		#endregion

		public ContiguousGraph(string _name)
		{
			name = _name;
		}

		/// <summary>
		/// Populate the nodes and edges of the graph.
		/// Overrides the previous data.
		/// </summary>
		public void Populate(List<int> contiguousNodes)
		{
			Dictionary<int, int> nodesMap = new Dictionary<int, int>();
			List<Node> nodesList = new List<Node>();
			List<Edge> edgesList = new List<Edge>();

			if (contiguousNodes.Count == 0)
				return;

			if (contiguousNodes.Count == 1)
			{
				string nodeName = $"{contiguousNodes[0]}";
				nodesList.Add(new Node((uint)nodeName.GetHashCode(), nodeName));
				nodes = nodesList.ToArray();
				edges = new Edge[0];
				return;
			}

			for(int i = 0; i<contiguousNodes.Count-1; i++)
			{
				string nodeName = $"{contiguousNodes[i]}";
				uint nodeID = (uint)nodeName.GetHashCode();
				string nodeP1Name = $"{contiguousNodes[i+1]}";
				uint nodeP1ID = (uint)nodeP1Name.GetHashCode();

				if (!nodesMap.ContainsKey(contiguousNodes[i]))
				{
					nodesList.Add(new Node(nodeID, nodeName));
					nodesMap.Add(contiguousNodes[i], nodesList.Count - 1);
				}
				string edgeName = $"{contiguousNodes[i]}-{contiguousNodes[i + 1]}";	
				edgesList.Add(new Edge((uint)edgeName.GetHashCode(), edgeName, nodeID, nodeP1ID));
			}

			if (!nodesMap.ContainsKey(contiguousNodes[contiguousNodes.Count-1]))
			{
				string nodeName = $"{contiguousNodes[contiguousNodes.Count - 1]}";
				nodesList.Add(new Node((uint)nodeName.GetHashCode(), nodeName));
			}

			nodes = nodesList.ToArray();
			edges = edgesList.ToArray();
		}
	}
}
