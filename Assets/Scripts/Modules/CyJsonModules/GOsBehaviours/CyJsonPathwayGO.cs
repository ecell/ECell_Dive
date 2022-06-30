using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using ECellDive.Utility.SettingsModels;
using ECellDive.Interfaces;
using ECellDive.SceneManagement;

namespace ECellDive
{
    namespace Modules
    {
        [Obsolete("Deprecated since the multiplayer update: it doesn't make sense to use this additional layer of " +
            "instantiation for a CyJson pathway with the refactored scene management system for multiplayers.")]
        public class CyJsonPathwayGO : GameNetModule
                                        //IGraphGO
        {
            public IGraph graphData { get; protected set; }
            public NetworkGOSettings networkGOSettingsModel;
            public Dictionary<uint, GameObject> DataID_to_DataGO;
            private bool allNodesSpawned = false;

            public override void OnNetworkSpawn()
            {
                DataID_to_DataGO = new Dictionary<uint, GameObject>();
                SetNetworkData(CyJsonModulesData.activeData);
                GenerateGraph();
            }

            /// <summary>
            /// Coroutine encapsulating the instantiation of the edges of the pathway.
            /// The coroutine yields until the end of the frame after having instantiated one batch of edges.
            /// </summary>
            private IEnumerator EdgesBatchSpawn()
            {
                yield return new WaitUntil(()=>allNodesSpawned);
                for (int i = 0; i < graphData.edges.Length; i += networkGOSettingsModel.edgesBatchSize)
                {
                    for (int j = i; j < Mathf.Min(i + networkGOSettingsModel.edgesBatchSize, graphData.edges.Length); j++)
                    {
                        //ModulesData.AddModule(edgeMD);
                        //GameObject edgeGO = GameNetScenesManager.Instance.GenerateModuleData(rootSceneId, 4, Vector3.zero);
                        //edgeGO.transform.parent = transform;
                        //EdgeSpawnClientRpc(edgeGO, j);
                    }
                    yield return null;
                }
            }

            [ClientRpc]
            public void EdgeSpawnClientRpc(NetworkObjectReference _edgeNetObj, int _edgeIdx)
            {
                GameObject edgeGO = _edgeNetObj;
                //edgeGO.GetComponent<EdgeGO>().Initialize(this, graphData.edges[_edgeIdx]);

                DataID_to_DataGO[graphData.edges[_edgeIdx].source].GetComponent<INodeGO>().nodeData.outgoingEdges.Add(graphData.edges[_edgeIdx].ID);
                DataID_to_DataGO[graphData.edges[_edgeIdx].target].GetComponent<INodeGO>().nodeData.incommingEdges.Add(graphData.edges[_edgeIdx].ID);

                DataID_to_DataGO[graphData.edges[_edgeIdx].ID] = edgeGO;
            }

            public void GenerateGraph()
            {
                GenerateGraphServerRpc();
            }

            [ServerRpc]
            public void GenerateGraphServerRpc()
            {
                //Instantiate Nodes of Layer
                StartCoroutine(NodesBatchSpawn());

                //Instantiate Edges of Layer
                StartCoroutine(EdgesBatchSpawn());
            }

            /// <summary>
            /// Coroutine encapsulating the instantiation of the nodes of the pathway.
            /// The coroutine yields until the end of the frame after having instantiated one batch of nodes.
            /// </summary>
            private IEnumerator NodesBatchSpawn()
            {
                for (int i = 0; i < graphData.nodes.Length; i += networkGOSettingsModel.nodesBatchSize)
                {
                    for (int j = i; j < Mathf.Min(i + networkGOSettingsModel.nodesBatchSize, graphData.nodes.Length); j++)
                    {
                        //ModulesData.AddModule(nodeMD);

                        //GameObject nodeGO = GameNetScenesManager.Instance.GenerateModuleData(rootSceneId, 3, Vector3.zero); 
                        //nodeGO.transform.parent = transform;
                        //NodeSpawnClientRpc(nodeGO, j);
                        
                    }
                    yield return null;
                }
                allNodesSpawned = true;
            }

            [ClientRpc]
            public void NodeSpawnClientRpc(NetworkObjectReference _nodeNetObj, int _nodeIdx)
            {
                GameObject nodeGO = _nodeNetObj;
                //nodeGO.GetComponent<NodeGO>().Initialize(this, graphData.nodes[_nodeIdx]);
                DataID_to_DataGO[graphData.nodes[_nodeIdx].ID] = nodeGO;
            }

            public void SetNetworkData(IGraph _IGraph)
            {
                graphData = _IGraph;
            }
        }
    }
}
