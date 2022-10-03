using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using ECellDive.IO;
using ECellDive.Interfaces;

namespace ECellDive
{
    namespace GraphComponents
    {
        public class CyJsonPathway : ScriptableObject, IGraph
        {
            new public string name { get; protected set; }
            public JObject graphData { get; protected set; }
            public JArray jNodes { get; protected set; }
            public JArray jEdges { get; protected set; }

            [SerializeReference] private INode[] m_nodes;
            public INode[] nodes { get => m_nodes; protected set => m_nodes = value; }

            [SerializeReference] private IEdge[] m_edges;
            public IEdge[] edges { get => m_edges; protected set => m_edges = value; }

            public CyJsonPathway(string _path, string _name)
            {
                graphData = JsonImporter.OpenFile(_path, _name);
                name = _name;
            }

            public CyJsonPathway(JObject _cyJspathway, string _name)
            {
                graphData = _cyJspathway;
                name = _name;
            }

            /// <inheritdoc/>
            public void Copy(IGraph _graph)
            {
                name = _graph.name;
                graphData = _graph.graphData;
                jNodes = _graph.jNodes;
                jEdges = _graph.jEdges;
                nodes = _graph.nodes;
                edges = _graph.edges;
            }

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
        }
    }
}
