using System.Collections.Generic;
using UnityEngine;
using ECellDive.Utility.SettingsModels;
using ECellDive.Interfaces;
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

            //public Dictionary<int, GameObject> NodeID_to_NodeGO;
            //public Dictionary<int, GameObject> EdgeID_to_EdgeGO;
            public Dictionary<int, GameObject> DataID_to_DataGO;

            private void Start()
            {
                //NodeID_to_NodeGO = new Dictionary<int, GameObject>();
                //EdgeID_to_EdgeGO = new Dictionary<int, GameObject>();
                DataID_to_DataGO = new Dictionary<int, GameObject>();

                SetNetworkData(CyJsonModulesData.activeData);

                GenerateAssociatedPathway();
            }

            private void GenerateAssociatedPathway()
            {
                //Instantiate Nodes of Layer
                foreach (INode _node in networkData.nodes)
                {
                    ModuleData nodeMD = new ModuleData
                    {
                        typeID = 6,
                    };
                    ModulesData.AddModule(nodeMD);
                    GameObject nodeGO = ScenesData.refSceneManagerMonoBehaviour.InstantiateGOOfModuleDataFromParent(nodeMD,
                                                                                                Vector3.zero,
                                                                                                gameObject.transform);
                    nodeGO.GetComponent<NodeGO>().Initialize(this, _node);
                    DataID_to_DataGO[_node.ID] = nodeGO;
                }

                //Instantiate Edges of Layer
                foreach (IEdge _edge in networkData.edges)
                {
                    ModuleData edgeMD = new ModuleData
                    {
                        typeID = 7,
                    };
                    ModulesData.AddModule(edgeMD);
                    GameObject edgeGO = ScenesData.refSceneManagerMonoBehaviour.InstantiateGOOfModuleDataFromParent(edgeMD,
                                                                                                Vector3.zero,
                                                                                                gameObject.transform);
                    edgeGO.GetComponent<EdgeGO>().Initialize(this, _edge);

                    DataID_to_DataGO[_edge.source].GetComponent<NodeGO>().nodeData.outgoingEdges.Add(_edge.ID);
                    DataID_to_DataGO[_edge.target].GetComponent<NodeGO>().nodeData.incommingEdges.Add(_edge.ID);

                    DataID_to_DataGO[_edge.ID] = edgeGO;
                }
            }

            public void SetNetworkData(INetwork _INetwork)
            {
                networkData = _INetwork;
            }
        }
    }
}
