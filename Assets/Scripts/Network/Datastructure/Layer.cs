using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json.Linq;
using ECellDive.INetworkComponents;

namespace ECellDive
{
    namespace NetworkComponents
    {
        public class Layer : ILayer
        {
            public int index { get; set; }
            public INode[] nodes { get; protected set; }
            public IEdge[] edges { get; protected set; }

            private IGrouping<string, JToken> jNodes;
            private IGrouping<string, JToken> jEdges;

            public Layer(int _index, IGrouping<string, JToken> _jNodesOfLayer, IGrouping<string, JToken> _jEdgesOfLayer)
            {
                index = _index;
                jNodes =_jNodesOfLayer;
                jEdges = _jEdgesOfLayer;
            }

            public void PopulateNodes()
            {
                int nbNodes = jNodes.Count();
                nodes = new INode[nbNodes];

                for (int i = 0; i< nbNodes; i++)
                {
                    nodes[i] = new Node(jNodes.ElementAt(i)["data"]["SUID"].Value<int>(),
                                        jNodes.ElementAt(i)["data"]["name"].Value<string>(),
                                        new Vector3(jNodes.ElementAt(i)["data"]["KEGG_NODE_X"].Value<float>(),
                                                    jNodes.ElementAt(i)["data"]["KEGG_NODE_Y"].Value<float>(),
                                                    jNodes.ElementAt(i)["data"]["KEGG_NODE_Z"].Value<float>()),
                                        jNodes.ElementAt(i)["data"]["KEGG_NODE_LABEL"].Value<string>());
                }
            }

            public void PopulateEdges()
            {
                int nbEdges = jEdges.Count();
                edges = new IEdge[nbEdges];

                for (int i = 0; i < nbEdges; i++)
                {
                    edges[i] = new Edge(jEdges.ElementAt(i)["data"]["SUID"].Value<int>(),
                                        jEdges.ElementAt(i)["data"]["name"].Value<string>(),
                                        jEdges.ElementAt(i)["data"]["source"].Value<int>(),
                                        jEdges.ElementAt(i)["data"]["target"].Value<int>());
                }
            }
        }
    }
}
