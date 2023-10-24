using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using ECellDive.IO;
using ECellDive.Interfaces;
using ECellDive.Utility.Data.Graph;

namespace ECellDive.GraphComponents
{
	/// <summary>
	/// The data structure to retrieve a graph from a Cytoscape Json file.
	/// </summary>
	public class CyJsonPathway : IGraph<CyJsonEdge, CyJsonNode>
	{
		#region - IGraph Fields -
		/// <inheritdoc/>
		public string name { get; protected set; }

		/// <inheritdoc/>
		public CyJsonNode[] nodes { get; protected set; }

		/// <inheritdoc/>
		public CyJsonEdge[] edges { get; protected set; }
		#endregion

		public GraphData<JObject, JArray, JArray> cyJsonGraphData;
			
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
			name = _name;
			JObject graphDataTemp = JsonImporter.OpenFile(_path, _name);
			cyJsonGraphData = new GraphData<JObject, JArray, JArray>
			{
				graphData = graphDataTemp,
				nodesData = CyJsonParser.GetNodes(graphDataTemp),
				edgesData = CyJsonParser.GetEdges(graphDataTemp)
			};

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
			name = _name;
			cyJsonGraphData = new GraphData<JObject, JArray, JArray>
			{
				graphData = _cyJspathway,
				nodesData = CyJsonParser.GetNodes(_cyJspathway),
				edgesData = CyJsonParser.GetEdges(_cyJspathway)
			};
		}

		/// <summary>
		/// Uses the information stored in the <see cref="edges"/> to fill the 
		/// <see cref="INode.incommingEdges"/> and <see cref="INode.outgoingEdges"/> lists.
		/// </summary>
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

		/// <summary>
		/// Populates the <see cref="nodes"/> array.
		/// </summary>
		public void PopulateNodes()
		{
			int nbNodes = cyJsonGraphData.nodesData.Count();
			nodes = new CyJsonNode[nbNodes];

			for (int i = 0; i < nbNodes; i++)
			{
				Vector3 nodePos = CyJsonParser.LookForNodePosition(cyJsonGraphData.nodesData.ElementAt(i));
				string name = CyJsonParser.LookForName(cyJsonGraphData.nodesData.ElementAt(i));
				string label = CyJsonParser.LookForLabel(cyJsonGraphData.nodesData.ElementAt(i));
				bool isVirtual = CyJsonParser.LookForNodeType(cyJsonGraphData.nodesData.ElementAt(i));
				nodes[i] = new CyJsonNode(cyJsonGraphData.nodesData.ElementAt(i)["data"]["id"].Value<uint>(),
										label,
										name,
										nodePos,
										isVirtual);
			}
		}

		/// <summary>
		/// Populates the <see cref="edges"/> array.
		/// </summary>
		public void PopulateEdges()
		{
			int nbEdges = cyJsonGraphData.edgesData.Count();
			edges = new CyJsonEdge[nbEdges];

			for (int i = 0; i < nbEdges; i++)
			{
				edges[i] = new CyJsonEdge(cyJsonGraphData.edgesData.ElementAt(i)["data"]["id"].Value<uint>(),
									cyJsonGraphData.edgesData.ElementAt(i)["data"]["name"].Value<string>(),
									cyJsonGraphData.edgesData.ElementAt(i)["data"]["reaction_name"].Value<string>(),
									cyJsonGraphData.edgesData.ElementAt(i)["data"]["source"].Value<uint>(),
									cyJsonGraphData.edgesData.ElementAt(i)["data"]["target"].Value<uint>());
			}
		}
	}
}
