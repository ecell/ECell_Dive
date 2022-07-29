using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Unity.Netcode;
using ECellDive.Interfaces;
using ECellDive.IO;
using ECellDive.GraphComponents;
using ECellDive.Multiplayer;
using ECellDive.SceneManagement;
using ECellDive.Utility;

namespace ECellDive
{
    namespace Modules
    {
        [System.Serializable]
        public struct CyJsonPathwaySettingsData
        {
            [Header("Spawning")]
            public int nodesBatchSize;
            public int edgesBatchSize;
            [Header("Scaling")]
            [Min(1)] public float PositionScaleFactor;
            [Min(1)] public float SizeScaleFactor;
            [Min(0)] public float InterLayersDistance;
        }

        /// <summary>
        /// Derived class with some more specific operations to handle
        /// CyJson modules.
        /// </summary>
        public class CyJsonModule : GameNetModule,
                                    IGraphGO,
                                    IModifiable,
                                    ISaveable
        {
            public int refIndex { get; private set; }
            public NetworkVariable<int> nbActiveDataSet = new NetworkVariable<int>(0);
            public CyJsonPathwaySettingsData cyJsonPathwaySettings;
            public Dictionary<uint, GameObject> DataID_to_DataGO;

            private GameObject pathwayRoot;
            private bool allNodesSpawned = false;
            private Dictionary<uint, Modification<bool>> koModifications = new Dictionary<uint, Modification<bool>>();

            #region  - IGraphGO Members - 
            public IGraph graphData { get; protected set; }

            [SerializeField] private List<GameObject> m_graphPrefabsComponents;
            public List<GameObject> graphPrefabsComponents
            {
                get => m_graphPrefabsComponents;
                private set => m_graphPrefabsComponents = value;
            }
            #endregion

            #region - IModifiable Members -
            public ModificationFile readingModificationFile { get; set; }
            #endregion

            #region - ISaveable Members -
            public ModificationFile writingModificationFile { get; set; }
            #endregion

            public override void OnNetworkSpawn()
            {
                base.OnNetworkSpawn();
                DataID_to_DataGO = new Dictionary<uint, GameObject>();
                GameNetPortal.Instance.modifiables.Add(this);
                GameNetPortal.Instance.saveables.Add(this);
            }
            protected override void ApplyCurrentColorChange(Color _previous, Color _current)
            {
                mpb.SetVector(colorID, _current);
                m_Renderer.SetPropertyBlock(mpb);
            }

            [ClientRpc]
            private void BroadcastKoModificationClientRpc(uint _edgeId)
            {
                if (IsServer) return;

                koModifications[_edgeId] = new Modification<bool>(true, OperationTypes.knockout);
            }

            [ServerRpc(RequireOwnership = false)]
            private void BroadcastKoModificationServerRpc(uint _edgeId)
            {
                koModifications[_edgeId] = new Modification<bool>(true, OperationTypes.knockout);
                BroadcastKoModificationClientRpc(_edgeId);
            }

            [ServerRpc(RequireOwnership = false)]
            public void ConfirmIsActiveDataStatusServerRpc()
            {
                nbActiveDataSet.Value++;
            }

            /// <summary>
            /// Coroutine encapsulating the instantiation of the edges of the pathway.
            /// The coroutine yields until the end of the frame after having instantiated one batch of edges.
            /// </summary>
            private IEnumerator EdgesBatchSpawn(int _sceneId)
            {
                yield return new WaitUntil(() => allNodesSpawned);
                for (int i = 0; i < graphData.edges.Length; i += cyJsonPathwaySettings.edgesBatchSize)
                {
                    for (int j = i; j < Mathf.Min(i + cyJsonPathwaySettings.edgesBatchSize, graphData.edges.Length); j++)
                    {
                        //ModulesData.AddModule(edgeMD);
                        GameObject edgeGO = GameNetScenesManager.Instance.SpawnModuleInScene(_sceneId, graphPrefabsComponents[2], Vector3.zero);
                        edgeGO.transform.parent = pathwayRoot.transform;
                        EdgeSpawnClientRpc(edgeGO, j);
                    }
                    yield return null;
                }
                isReadyForDive.Value = true;
            }

            [ClientRpc]
            public void EdgeSpawnClientRpc(NetworkObjectReference _edgeNetObj, int _edgeIdx)
            {
                GameObject edgeGO = _edgeNetObj;
                edgeGO.GetComponent<EdgeGO>().Initialize(this, graphData.edges[_edgeIdx]);

                DataID_to_DataGO[graphData.edges[_edgeIdx].source].GetComponent<INodeGO>().nodeData.outgoingEdges.Add(graphData.edges[_edgeIdx].ID);
                DataID_to_DataGO[graphData.edges[_edgeIdx].target].GetComponent<INodeGO>().nodeData.incommingEdges.Add(graphData.edges[_edgeIdx].ID);

                DataID_to_DataGO[graphData.edges[_edgeIdx].ID] = edgeGO;
                edgeGO.GetComponent<GameNetModule>().NetHide();
            }

            /// <summary>
            /// Translates the information about knockedout reactions stored
            /// in <see cref="koModifications"/> as a string usable for a request
            /// to the server.
            /// </summary>
            /// <returns>A string listing the knockedout reactions.</returns>
            public string GetKnockoutString()
            {
                string knockouts = "";

                Dictionary<string, ushort> koMatch = new Dictionary<string, ushort>();
                ushort koMatchCount = 0;

                EdgeGO edgeGO;
                foreach (uint _koEdgeId in koModifications.Keys)
                {
                    edgeGO = DataID_to_DataGO[_koEdgeId].GetComponent<EdgeGO>();
                    if (!koMatch.TryGetValue(edgeGO.edgeData.name, out koMatchCount))
                    {
                        koMatch[edgeGO.edgeData.name] = 1;
                        knockouts += "knockout_" + edgeGO.edgeData.ID + ",";
                    }
                }

                if (knockouts.Length > 0)
                {
                    knockouts = knockouts.Substring(0, knockouts.Length - 1);
                }

                return knockouts;
            }

            /// <summary>
            /// Coroutine encapsulating the instantiation of the nodes of the pathway.
            /// The coroutine yields until the end of the frame after having instantiated one batch of nodes.
            /// </summary>
            private IEnumerator NodesBatchSpawn(int _sceneId)
            {
                for (int i = 0; i < graphData.nodes.Length; i += cyJsonPathwaySettings.nodesBatchSize)
                {
                    for (int j = i; j < Mathf.Min(i + cyJsonPathwaySettings.nodesBatchSize, graphData.nodes.Length); j++)
                    {
                        //ModulesData.AddModule(nodeMD);

                        GameObject nodeGO = GameNetScenesManager.Instance.SpawnModuleInScene(_sceneId, graphPrefabsComponents[1], Vector3.zero);
                        nodeGO.transform.parent = pathwayRoot.transform;
                        NodeSpawnClientRpc(nodeGO, j);

                    }
                    yield return null;
                }
                allNodesSpawned = true;
            }

            [ClientRpc]
            public void NodeSpawnClientRpc(NetworkObjectReference _nodeNetObj, int _nodeIdx)
            {
                //Debug.Log($"graphData.nodes[_nodeIdx]: {graphData.nodes[_nodeIdx]}");
                GameObject nodeGO = _nodeNetObj;
                //Debug.Log($"_nodeNetObj: {nodeGO}, DataID_to_DataGO:{DataID_to_DataGO}, graphData: {graphData}");
                
                nodeGO.GetComponent<NodeGO>().Initialize(cyJsonPathwaySettings, graphData.nodes[_nodeIdx]);
                DataID_to_DataGO[graphData.nodes[_nodeIdx].ID] = nodeGO;
                nodeGO.GetComponent<GameNetModule>().NetHide();
            }

            private string ScanForKOModifications()
            {
                string modification = "";
                Modification<bool> koMod;
                foreach(IEdge edge in graphData.edges)
                {
                    if (DataID_to_DataGO[edge.ID].GetComponent<EdgeGO>().knockedOut.Value)
                    {
                        if (!koModifications.TryGetValue(edge.ID, out koMod))
                        {
                            koModifications[edge.ID] = new Modification<bool>(true, OperationTypes.knockout);
                        }
                    }

                    else
                    {
                        if (!koModifications.TryGetValue(edge.ID, out koMod))
                        {
                            koModifications.Remove(edge.ID);
                        }
                    }
                }

                return modification;
            }

            public void SetIndex(int _index)
            {
                refIndex = _index;
            }

            public void StartUpInfo()
            {
                writingModificationFile = new ModificationFile(
                    "",
                    graphData.name,
                    "", "");

                SetIndex(CyJsonModulesData.loadedData.Count - 1);

                SetName(CyJsonModulesData.loadedData[refIndex].name);

                InstantiateInfoTags(new string[] {$"nb edges: {CyJsonModulesData.loadedData[refIndex].edges.Length}\n"+
                                                  $"nb nodes: {CyJsonModulesData.loadedData[refIndex].nodes.Length}"});
            }

            #region - IDive Methods -

            public override IEnumerator GenerativeDiveInC()
            {
                RequestSourceDataGenerationServerRpc(NetworkManager.Singleton.LocalClientId);

                yield return new WaitUntil(()=>isReadyForDive.Value);

                DirectDiveIn();
            }

            [ServerRpc(RequireOwnership = false)]
            public override void RequestSourceDataGenerationServerRpc(ulong _expeditorClientID)
            {
                int rootSceneId = GameNetPortal.Instance.netSessionPlayersDataMap[_expeditorClientID].currentScene;
                
                targetSceneId.Value = GameNetScenesManager.Instance.AddNewDiveScene(rootSceneId);
                LogSystem.refLogManager.AddMessage(LogSystem.MessageTypes.Debug,
                    $"targetSceneId of the new module is: {targetSceneId}");
                RequestGraphGenerationServerRpc(_expeditorClientID, targetSceneId.Value);
            }
            #endregion

            #region - IGraph Methods -

            [ServerRpc]
            public void RequestGraphGenerationServerRpc(ulong _expeditorClientId, int _rootSceneId)
            {
                pathwayRoot = GameNetScenesManager.Instance.SpawnModuleInScene(_rootSceneId,
                                                                                graphPrefabsComponents[0],
                                                                                Vector3.zero);
                pathwayRoot.name = graphData.name;

                //Instantiate Nodes of Layer
                StartCoroutine(NodesBatchSpawn(_rootSceneId));

                //Instantiate Edges of Layer
                StartCoroutine(EdgesBatchSpawn(_rootSceneId));
            }

            public void SetNetworkData(IGraph _IGraph)
            {
                graphData = _IGraph;
            }
            #endregion

            #region - IModifiable Methods -
            public void ApplyFileModifications()
            {
                Debug.Log("Applying modifications");
                string[] operations = readingModificationFile.modification.Split(',');
                foreach(string _op in operations)
                {
                    OperationSwitch(_op);
                }
            }

            public bool CheckName(string _name)
            {
                return _name == graphData.name;
            }

            public void OperationSwitch(string _op)
            {
                string[] opContent = _op.Split('_');

                switch (opContent[0])
                {
                    case "knockout":
                        GameObject edgeGO;
                        uint edgeID = System.Convert.ToUInt32(opContent[1]);
                        if (DataID_to_DataGO.TryGetValue(edgeID, out edgeGO))
                        {
                            edgeGO.GetComponent<EdgeGO>().Knockout();
                            BroadcastKoModificationServerRpc(edgeID);
                        }
                        break;
                }
            }
            #endregion

            #region - IMlprDataExchange Methods -
            public override void AssembleFragmentedData()
            {
                byte[] assembledSourceData = ArrayManipulation.Assemble(fragmentedSourceData);
                string assembledSourceDataName = System.Text.Encoding.UTF8.GetString(sourceDataName);

                JObject requestJObject = JObject.Parse(System.Text.Encoding.UTF8.GetString(assembledSourceData));

                //Loading the file
                CyJsonPathway pathway = CyJsonPathwayLoader.Initiate(requestJObject,
                                                        assembledSourceDataName);
                
                //Instantiating relevant data structures to store the information about
                //the layers, nodes and edges.
                CyJsonPathwayLoader.Populate(pathway);
                CyJsonModulesData.AddData(pathway);

                SetNetworkData(pathway);

                StartUpInfo();
            }
            #endregion

            #region - ISaveable Methods -
            public void CompileModificationFile()
            {
                string author = GameNetPortal.Instance.netSessionPlayersDataMap[NetworkManager.Singleton.LocalClientId].playerName;
                string baseModelPath = graphData.name;
                string description = "";

                ScanForKOModifications();
                string modification = GetKnockoutString();

                writingModificationFile = new ModificationFile(author, baseModelPath, description, modification);
            }

            #endregion
        }
    }
}
