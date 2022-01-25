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

            private IEnumerable<JToken> jNodes;
            private IEnumerable<JToken> jEdges;

            public Layer(int _index, IEnumerable<JToken> _jNodesOfLayer, IEnumerable<JToken> _jEdgesOfLayer)
            {
                index = _index;
                jNodes = _jNodesOfLayer;
                jEdges = _jEdgesOfLayer;
            }

            /// <summary>
            /// Looks for either a "SUID" or "id" field
            /// </summary>
            private int LookForID(JToken _jToken)
            {
                int id = -1;
                JObject jObj = (JObject)_jToken;

                if (jObj.ContainsKey("SUID"))
                {
                    id = jObj["data"]["SUID"].Value<int>();
                }
                else if (jObj.ContainsKey("id"))
                {
                    Debug.Log($"id stored:{jObj["data"]["id"]}");
                    
                    id = jObj["data"]["id"].Value<int>();
                    Debug.Log($"id extracted: {id}");
                }

                return id;
            }

            /// <summary>
            /// Looks for a "name" field in a "data" field in a _JToken.
            /// </summary>
            private string LookForName(JToken _jToken)
            {
                string name = "No Name";
                JObject jObj = (JObject)_jToken;
                if (jObj.ContainsKey("data"))
                {
                    if (((JObject)jObj["data"]).ContainsKey("name"))
                    {
                        name = jObj["data"]["name"].Value<string>();
                    }
                }
                return name;
            }

            private Vector3 LookForNodePosition(JToken _node)
            {
                Vector3 nodePos = Vector3.zero;
                JObject jObjNode = (JObject)_node;

                if (jObjNode.ContainsKey("KEGG_NODE_X"))
                {
                    nodePos.x = jObjNode["data"]["KEGG_NODE_X"].Value<float>();
                }
                else
                {
                    nodePos.x = jObjNode["position"]["x"].Value<float>();
                }

                if (jObjNode.ContainsKey("KEGG_NODE_Y"))
                {
                    nodePos.y = jObjNode["data"]["KEGG_NODE_Y"].Value<float>();
                }
                else
                {
                    nodePos.y = jObjNode["position"]["y"].Value<float>();
                }

                if (jObjNode.ContainsKey("KEGG_NODE_Z"))
                {
                    nodePos.z = jObjNode["data"]["KEGG_NODE_Z"].Value<float>();
                }
                else
                {
                    nodePos.z = 0f;
                }

                return nodePos;
            }

            public void PopulateNodes()
            {
                int nbNodes = jNodes.Count();
                nodes = new INode[nbNodes];

                for (int i = 0; i< nbNodes; i++)
                {
                    //int id = LookForID(jNodes.ElementAt(i));
                    Vector3 nodePos = LookForNodePosition(jNodes.ElementAt(i));
                    string name = LookForName(jNodes.ElementAt(i));
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
                    //int id = LookForID(jEdges.ElementAt(i));
                    edges[i] = new Edge(jEdges.ElementAt(i)["data"]["id"].Value<int>(),
                                        jEdges.ElementAt(i)["data"]["name"].Value<string>(),
                                        jEdges.ElementAt(i)["data"]["source"].Value<int>(),
                                        jEdges.ElementAt(i)["data"]["target"].Value<int>());
                }
            }
        }
    }
}
