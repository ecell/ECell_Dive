using System.Linq;
using Newtonsoft.Json.Linq;
using Unity.Netcode;
using UnityEngine;
using ECellDive.IO;
using ECellDive.Interfaces;

namespace ECellDive
{
    namespace GraphComponents
    {
        public class CyJsonPathway : IGraph
        {
            public string name { get; protected set; }
            public JObject graphData { get;}
            public JArray jNodes { get; protected set; }
            public JArray jEdges { get; protected set; }
            public INode[] nodes { get; protected set; }
            public IEdge[] edges { get; protected set; }

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

            public void PopulateNodes()
            {
                int nbNodes = jNodes.Count();
                nodes = new INode[nbNodes];

                for (int i = 0; i < nbNodes; i++)
                {
                    Vector3 nodePos = CyJsonParser.LookForNodePosition(jNodes.ElementAt(i));
                    string name = CyJsonParser.LookForName(jNodes.ElementAt(i));
                    bool isVirtual = CyJsonParser.LookForNodeType(jNodes.ElementAt(i));
                    nodes[i] = new Node(jNodes.ElementAt(i)["data"]["id"].Value<uint>(),
                                        name,
                                        nodePos,
                                        isVirtual);
                }
            }

            public void PopulateEdges()
            {
                int nbEdges = jEdges.Count();
                edges = new IEdge[nbEdges];

                for (int i = 0; i < nbEdges; i++)
                {
                    edges[i] = new Edge(jEdges.ElementAt(i)["data"]["id"].Value<uint>(),
                                        jEdges.ElementAt(i)["data"]["name"].Value<string>(),
                                        jEdges.ElementAt(i)["data"]["source"].Value<uint>(),
                                        jEdges.ElementAt(i)["data"]["target"].Value<uint>());
                }
            }

            public void SetNodes()
            {
                jNodes = CyJsonParser.GetNodes(graphData);
            }

            public void SetEdges()
            {
                jEdges = CyJsonParser.GetEdges(graphData);
            }
        }
    }
}
