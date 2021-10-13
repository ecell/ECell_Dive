using Newtonsoft.Json.Linq;
using UnityEngine;
using ECellDive.INetworkComponents;
using ECellDive.NetworkComponents;


namespace ECellDive
{
    namespace IO
    {
        public static class NetworkLoader
        {
            /// <summary>
            /// Instantiate a <see cref="Network"/> data structure
            /// from the Cystoscape network Json file.
            /// </summary>
            /// <param name="_path">Path of the Cytoscape network Json file</param>
            /// <param name="_name">Name of the Cytoscape network Json file</param>
            /// <returns></returns>
            public static NetworkComponents.Network Initiate(string _path, string _name)
            {
                return new NetworkComponents.Network(_path, _name);
            }

            /// <summary>
            /// Instantiate a Network data structure from the content of a Cystoscape
            /// network Json file.
            /// </summary>
            /// <param name="_networkCyJs">Content of the Cytoscape network Json file
            /// serialized as a JObject</param>
            /// <param name="_name">Name of the Cytoscape network Json file</param>
            /// <returns></returns>
            public static NetworkComponents.Network Initiate(JObject _networkCyJs, string _name)
            {
                return new NetworkComponents.Network(_networkCyJs, _name);
            }

            /// <summary>
            /// Parse the Cytoscape network Json file and fill in the info about the
            /// layers, nodes and edges.
            /// </summary>
            /// <param name="_refNetwork">The network object previously instantiated
            /// for the Initiate function.</param>
            public static void Populate( NetworkComponents.Network _refNetwork)
            {
                //get all nodes
                _refNetwork.SetNodes();
                
                //get all edges
                _refNetwork.SetEdges();

                //Organize layers, edges and nodes information
                _refNetwork.PopulateLayers();
            }

            /// <summary>
            /// Instantiates the NetworkGO corresponding to the informlation loaded
            /// in the Network data structure
            /// </summary>
            /// <param name="_refNetwork">A loaded network</param>
            /// <param name="_networkGO">NetworkGO prefabs</param>
            /// <param name="_layerGO">LayerGO prefabs</param>
            /// <param name="_nodeGO">NodeGO prefabs</param>
            /// <param name="_edgeGO">EdgeGO prefabs</param>
            /// <returns>Returns the instantiated Network GO with all layers, nodes and 
            /// edges as his children objects.</returns>
            public static GameObject Generate(NetworkComponents.Network _refNetwork,
                                              GameObject _networkGO,
                                              GameObject _layerGO,
                                              GameObject _nodeGO,
                                              GameObject _edgeGO)
            {
                //Instantiate Network Base
                GameObject networkGO = _networkGO.GetComponent<NetworkGO>().SelfInstance();
                NetworkGO refNetworkGO = networkGO.GetComponent<NetworkGO>();
                refNetworkGO.SetNetworkData(_refNetwork);

                //Instantiate Layers in Network
                foreach (ILayer _layer in _refNetwork.layers)
                {
                    GameObject layerGO = Object.Instantiate(_layerGO,
                                                            Vector3.zero,
                                                            Quaternion.identity,
                                                            refNetworkGO.transform);
                    layerGO.GetComponent<LayerGO>().SetLayerData(_layer);
                    layerGO.name = $"Layer {_layer.index}";

                    //Instantiate Nodes of Layer
                    foreach(INode _node in _layer.nodes)
                    {
                        Vector3 nodePos = new Vector3(_node.position.x,
                                                      _node.position.z,
                                                      _node.position.y) / refNetworkGO.networkGOSettingsModel.PositionScaleFactor;
                        GameObject nodeGO = Object.Instantiate(_nodeGO,
                                                               Vector3.zero,
                                                               Quaternion.identity,
                                                               layerGO.transform);
                        nodeGO.SetActive(true);
                        nodeGO.transform.position = nodePos;
                        nodeGO.transform.localScale /= refNetworkGO.networkGOSettingsModel.SizeScaleFactor;
                        nodeGO.name = $"{_node.ID}";

                        NodeGO refNodeGO = nodeGO.GetComponent<NodeGO>();
                        refNodeGO.SetNodeData(_node);
                        refNodeGO.refFloatingPlanel.transform.localScale *= refNetworkGO.networkGOSettingsModel.SizeScaleFactor;

                        refNetworkGO.NodeID_to_NodeGO[_node.ID] = nodeGO;
                    }

                    //Instantiate Edges of Layer
                    foreach (IEdge _edge in _layer.edges)
                    {
                        Transform start = refNetworkGO.NodeID_to_NodeGO[_edge.source].transform;
                        Transform target = refNetworkGO.NodeID_to_NodeGO[_edge.target].transform;

                        refNetworkGO.NodeID_to_NodeGO[_edge.source].GetComponent<NodeGO>().nodeData.outgoingEdges.Add(_edge.ID);
                        refNetworkGO.NodeID_to_NodeGO[_edge.target].GetComponent<NodeGO>().nodeData.incommingEdges.Add(_edge.ID);

                        GameObject edgeGO = _edgeGO.GetComponent<EdgeGO>().SelfInstance();
                        edgeGO.SetActive(true);
                        edgeGO.transform.parent = layerGO.transform;
                        edgeGO.name = _edge.NAME;

                        EdgeGO refEdgeGO = edgeGO.GetComponent<EdgeGO>();
                        refEdgeGO.SetEdgeData(_edge);
                        refEdgeGO.SetDefaultWidth(1 / refNetworkGO.networkGOSettingsModel.SizeScaleFactor,
                                                  1 / refNetworkGO.networkGOSettingsModel.SizeScaleFactor);
                        refEdgeGO.SetLineRenderer();
                        refEdgeGO.SetPosition(start, target);
                        refEdgeGO.SetCollider(start, target);

                        refNetworkGO.EdgeID_to_EdgeGO[_edge.ID] = edgeGO;
                    }
                }
                return networkGO;
            }
        }
    }
}

