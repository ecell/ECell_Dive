using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using ECellDive.IO;
using ECellDive.Interfaces;

namespace ECellDive.GraphComponents
{
	/// <summary>
	/// The data structure to retrieve a graph from a Cytoscape Json file.
	/// </summary>
	public class CyJsonPathway : IGraph
	{
		#region - IGraph Fields -
		/// <inheritdoc/>
		public string name { get; protected set; }

		/// <inheritdoc/>
		public JObject graphData { get; protected set; }

		/// <inheritdoc/>
		public JArray jNodes { get; protected set; }

		/// <inheritdoc/>
		public JArray jEdges { get; protected set; }

		/// <inheritdoc/>
		public INode[] nodes { get; protected set; }

		/// <inheritdoc/>
		public IEdge[] edges { get; protected set; }
		#endregion
			
		/// <summary>
		/// Instantiate from a Cytoscape Json file.
		/// </summary>
		/// <param name="_path">
		/// The directory to the file.
		/// </param>
		/// <param name="_name">
		/// The name of the file with the extension.
		/// </param>
		public CyJsonPathway(string _path, string _name)
		{
			graphData = JsonImporter.OpenFile(_path, _name);
			name = _name;
		}

		/// <summary>
		/// Instantiate from a JObject (from Newtonsoft.Json.Linq)
		/// that should encode the content of a Cytoscape Json file.
		/// </summary>
		/// <param name="_cyJspathway">
		/// The JObject containing information about the cyjson graph.
		/// </param>
		/// <param name="_name">
		/// The name of the graph.
		/// </param>
		public CyJsonPathway(JObject _cyJspathway, string _name)
		{
			graphData = _cyJspathway;
			name = _name;
		}

		#region - IGraph Methods -
		/// <inheritdoc/>
		public void MapInOutEdgesIntoNodes()
		{
			Dictionary<uint, uint> nodesMap = new Dictionary<uint, uint>();

			for(uint i = 0; i<nodes.Length; i++)
			{
				nodesMap[nodes[i].ID] = i;
			}

			foreach(IEdge edge in edges)
			{
				nodes[nodesMap[edge.source]].outgoingEdges.Add(edge.ID);
				nodes[nodesMap[edge.target]].incommingEdges.Add(edge.ID);
			}
		}

		/// <inheritdoc/>
		public void PopulateNodes()
		{
			int nbNodes = jNodes.Count();
			nodes = new INode[nbNodes];

			for (int i = 0; i < nbNodes; i++)
			{
				Vector3 nodePos = CyJsonParser.LookForNodePosition(jNodes.ElementAt(i));
				string name = CyJsonParser.LookForName(jNodes.ElementAt(i));
				string label = CyJsonParser.LookForLabel(jNodes.ElementAt(i));
				bool isVirtual = CyJsonParser.LookForNodeType(jNodes.ElementAt(i));
				nodes[i] = new Node(jNodes.ElementAt(i)["data"]["id"].Value<uint>(),
									label,
									name,
									nodePos,
									isVirtual);
			}
		}

		/// <inheritdoc/>
		public void PopulateEdges()
		{
			int nbEdges = jEdges.Count();
			edges = new IEdge[nbEdges];

			for (int i = 0; i < nbEdges; i++)
			{
				edges[i] = new Edge(jEdges.ElementAt(i)["data"]["id"].Value<uint>(),
									jEdges.ElementAt(i)["data"]["reaction_name"].Value<string>(),
									jEdges.ElementAt(i)["data"]["name"].Value<string>(),
									jEdges.ElementAt(i)["data"]["source"].Value<uint>(),
									jEdges.ElementAt(i)["data"]["target"].Value<uint>());
			}
		}

		/// <inheritdoc/>
		public void SetNodes()
		{
			jNodes = CyJsonParser.GetNodes(graphData);
		}

		/// <inheritdoc/>
		public void SetEdges()
		{
			jEdges = CyJsonParser.GetEdges(graphData);
		}
		#endregion
	}
}
