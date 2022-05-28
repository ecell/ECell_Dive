using System.Collections;
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

            public Dictionary<int, GameObject> DataID_to_DataGO;

            private bool allNodesSpawned = false;

            private void Start()
            {
                DataID_to_DataGO = new Dictionary<int, GameObject>();

                SetNetworkData(CyJsonModulesData.activeData);

                GenerateAssociatedPathway();
            }

            /// <summary>
            /// Coroutine encapsulating the instantiation of the edges of the network.
            /// The coroutine yields until the end of the frame after having instantiated one batch of edges.
            /// </summary>
            private IEnumerator EdgesBatchSpawn()
            {
                yield return new WaitUntil(areAllNodesSpawned);
                for (int i = 0; i < networkData.edges.Length; i += networkGOSettingsModel.edgesBatchSize)
                {
                    for (int j = i; j < Mathf.Min(i + networkGOSettingsModel.edgesBatchSize, networkData.edges.Length); j++)
                    {
                        ModuleData edgeMD = new ModuleData
                        {
                            typeID = 7,
                        };
                        ModulesData.AddModule(edgeMD);
                        GameObject edgeGO = ScenesData.refSceneManagerMonoBehaviour.InstantiateGOOfModuleDataFromParent(edgeMD,
                                                                                                    Vector3.zero,
                                                                                                    gameObject.transform);
                        edgeGO.GetComponent<EdgeGO>().Initialize(this, networkData.edges[j]);

                        DataID_to_DataGO[networkData.edges[j].source].GetComponent<INodeGO>().nodeData.outgoingEdges.Add(networkData.edges[j].ID);
                        DataID_to_DataGO[networkData.edges[j].target].GetComponent<INodeGO>().nodeData.incommingEdges.Add(networkData.edges[j].ID);

                        DataID_to_DataGO[networkData.edges[j].ID] = edgeGO;
                    }
                    yield return null;
                }
            }

            private void GenerateAssociatedPathway()
            {
                //Instantiate Nodes of Layer
                StartCoroutine(NodesBatchSpawn());
                
                //Instantiate Edges of Layer
                StartCoroutine(EdgesBatchSpawn());
            }

            /// <summary>
            /// Coroutine encapsulating the instantiation of the nodes of the network.
            /// The coroutine yields until the end of the frame after having instantiated one batch of nodes.
            /// </summary>
            private IEnumerator NodesBatchSpawn()
            {
                for (int i = 0; i < networkData.nodes.Length; i += networkGOSettingsModel.nodesBatchSize)
                {
                    for (int j = i; j < Mathf.Min(i + networkGOSettingsModel.nodesBatchSize, networkData.nodes.Length); j++)
                    {
                        ModuleData nodeMD = new ModuleData
                        {
                            typeID = 6,
                        };
                        ModulesData.AddModule(nodeMD);
                        GameObject nodeGO = ScenesData.refSceneManagerMonoBehaviour.InstantiateGOOfModuleDataFromParent(nodeMD,
                                                                                                    Vector3.zero,
                                                                                                    gameObject.transform);
                        nodeGO.GetComponent<NodeGO>().Initialize(this, networkData.nodes[j]);
                        DataID_to_DataGO[networkData.nodes[j].ID] = nodeGO;
                    }
                    yield return null;
                }
                allNodesSpawned = true;
            }

            private bool areAllNodesSpawned()
            {
                return allNodesSpawned;
            }

            public void SetNetworkData(INetwork _INetwork)
            {
                networkData = _INetwork;
            }
        }
    }
}
