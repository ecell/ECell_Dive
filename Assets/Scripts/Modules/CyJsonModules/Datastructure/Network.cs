using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using ECellDive.IO;
using ECellDive.INetworkComponents;

namespace ECellDive
{
    namespace NetworkComponents
    {
        public class Network : INetwork
        {
            public string name { get; protected set; }
            public JObject networkData { get;}
            public JArray jNodes { get; protected set; }
            public JArray jEdges { get; protected set; }
            public INode[] nodes { get; protected set; }
            public IEdge[] edges { get; protected set; }

            public Network(string _path, string _name)
            {
                networkData = JsonImporter.OpenFile(_path, _name);
                name = _name;
            }

            public Network(JObject _networkCyJs, string _name)
            {
                networkData = _networkCyJs;
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
                    nodes[i] = new Node(jNodes.ElementAt(i)["data"]["id"].Value<int>(),
                                        name,
                                        nodePos);
                }
            }

            public void PopulateEdges()
            {
                int nbEdges = jEdges.Count();
                edges = new IEdge[nbEdges];

                for (int i = 0; i < nbEdges; i++)
                {
                    edges[i] = new Edge(jEdges.ElementAt(i)["data"]["id"].Value<int>(),
                                        jEdges.ElementAt(i)["data"]["name"].Value<string>(),
                                        jEdges.ElementAt(i)["data"]["source"].Value<int>(),
                                        jEdges.ElementAt(i)["data"]["target"].Value<int>());
                }
            }

            public void SetNodes()
            {
                jNodes = CyJsonParser.GetNodes(networkData);
            }

            public void SetEdges()
            {
                jEdges = CyJsonParser.GetEdges(networkData);
            }
        }
    }
}
