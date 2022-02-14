﻿using System.Collections.Generic;
using UnityEngine;
using ECellDive.Utility.SettingsModels;
using ECellDive.INetworkComponents;
using ECellDive.SceneManagement;

namespace ECellDive
{
    namespace Modules
    {
        public class NetworkGO : MonoBehaviour,
                                 INetworkGO
        {
            public INetwork networkData { get; protected set; }

            public NetworkGOSettings networkGOSettingsModel;

            public Dictionary<int, GameObject> NodeID_to_NodeGO;
            public Dictionary<int, GameObject> EdgeID_to_EdgeGO;

            private void Start()
            {
                NodeID_to_NodeGO = new Dictionary<int, GameObject>();
                EdgeID_to_EdgeGO = new Dictionary<int, GameObject>();

                SetNetworkData(CyJsonModulesData.activeData);

                GenerateAssociatedPathway();
            }

            private void GenerateAssociatedPathway()
            {
                //Instantiate Layers in Network
                foreach (ILayer _layer in networkData.layers)
                {
                    ModuleData layerMD = new ModuleData
                    {
                        typeID = 6,
                    };
                    ModulesData.AddModule(layerMD);
                    GameObject layerGO = ScenesData.refSceneManagerMonoBehaviour.InstantiateGOOfModuleDataFromParent(layerMD,
                                                                                                Vector3.zero,
                                                                                                gameObject.transform);
                    layerGO.GetComponent<LayerGO>().Initialize(_layer);

                    //Instantiate Nodes of Layer
                    foreach (INode _node in _layer.nodes)
                    {
                        ModuleData nodeMD = new ModuleData
                        {
                            typeID = 7,
                        };
                        ModulesData.AddModule(nodeMD);
                        GameObject nodeGO = ScenesData.refSceneManagerMonoBehaviour.InstantiateGOOfModuleDataFromParent(nodeMD,
                                                                                                    Vector3.zero,
                                                                                                    layerGO.transform);
                        nodeGO.GetComponent<NodeGO>().Initialize(this, _node);
                        NodeID_to_NodeGO[_node.ID] = nodeGO;
                    }

                    //Instantiate Edges of Layer
                    foreach (IEdge _edge in _layer.edges)
                    {
                        ModuleData edgeMD = new ModuleData
                        {
                            typeID = 8,
                        };
                        ModulesData.AddModule(edgeMD);
                        GameObject edgeGO = ScenesData.refSceneManagerMonoBehaviour.InstantiateGOOfModuleDataFromParent(edgeMD,
                                                                                                    Vector3.zero,
                                                                                                    layerGO.transform);
                        edgeGO.GetComponent<EdgeGO>().Initialize(this, _edge);

                        NodeID_to_NodeGO[_edge.source].GetComponent<NodeGO>().nodeData.outgoingEdges.Add(_edge.ID);
                        NodeID_to_NodeGO[_edge.target].GetComponent<NodeGO>().nodeData.incommingEdges.Add(_edge.ID);

                        EdgeID_to_EdgeGO[_edge.ID] = edgeGO;
                    }
                }
            }

            public void SetNetworkData(INetwork _INetwork)
            {
                networkData = _INetwork;
            }
        }
    }
}