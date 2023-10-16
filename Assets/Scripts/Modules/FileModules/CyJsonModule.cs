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
using ECellDive.Utility.Data.Graph;
using ECellDive.Utility.Data.Modification;

#if UNITY_EDITOR
using UnityEditor;
using ECellDive.CustomEditors;
#endif

namespace ECellDive
{
	namespace Modules
	{
		/// <summary>
		/// Derived class with some more specific operations to handle
		/// CyJson modules.
		/// </summary>
		public class CyJsonModule : GameNetModule,
									IGraphGONet,
									IModifiable,
									ISaveable
		{
			[System.Serializable]
			public struct EdgeParametersOverride
			{
				public bool forceDefaultColor;
				public bool forceDefaultWidth;
				[Range(0, 1)] public float startWidthFactor;
				[Range(0, 1)] public float endWidthFactor;
			}

			[Header("GraphGONet")]
			public NetworkVariable<int> nbActiveDataSet = new NetworkVariable<int>(0);
			public int refIndex { get; private set; }
			
			public GameObject pathwayRoot;

			private bool allNodesSpawned = false;
			[SerializeField] private EdgeParametersOverride edgeParametersOverride = new EdgeParametersOverride
			{
				forceDefaultColor = false,
				forceDefaultWidth = false,
				startWidthFactor = 1,
				endWidthFactor = 1,
			};

			#region  - IGraphGONet Members - 
			[SerializeField] private CyJsonPathway m_graphData;
			public IGraph graphData { get => m_graphData; protected set => m_graphData = (CyJsonPathway)value; }

			[SerializeField] private GraphScalingData m_graphScalingData;
			public GraphScalingData graphScalingData
			{
				get => m_graphScalingData;
			}
			
			[SerializeField] private GraphBatchSpawning m_graphBatchSpawning;
			public GraphBatchSpawning graphBatchSpawning
			{
				get => m_graphBatchSpawning;
			}

			public Dictionary<uint, GameObject> DataID_to_DataGO { get; set; }

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

			public void ApplyEdgeWidthFactorOverride()
			{
				StartCoroutine(ApplyEdgeWidthFactorOverrideC());
			}

			public IEnumerator ApplyEdgeWidthFactorOverrideC()
			{
				Debug.Log($"ApplyEdgeStartWidthFactorOverrideC: startWidthFactor =" +
					$" {edgeParametersOverride.startWidthFactor}, endWidthFactor = " +
					$" {edgeParametersOverride.endWidthFactor}", gameObject);
				EdgeGO edgeGO;
				foreach (IEdge _edge in m_graphData.edges)
				{
					edgeGO = DataID_to_DataGO[_edge.ID].GetComponent<EdgeGO>();
					edgeGO.forceDefaultColor = edgeParametersOverride.forceDefaultColor;
					edgeGO.forceDefaultWidth = edgeParametersOverride.forceDefaultWidth;
					edgeGO.startWidthFactor = edgeParametersOverride.startWidthFactor;
					edgeGO.endWidthFactor = edgeParametersOverride.endWidthFactor;
					edgeGO.ApplyFluxLevelClamped();
					edgeGO.SetCurrentColorToDefaultServerRpc();

				}

				yield return null;
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
				for (int i = 0; i < graphData.edges.Length; i += m_graphBatchSpawning.edgesBatchSize)
				{
					for (int j = i; j < Mathf.Min(i + m_graphBatchSpawning.edgesBatchSize, graphData.edges.Length); j++)
					{
						//ModulesData.AddModule(edgeMD);
						GameObject edgeGO = DiveScenesManager.Instance.SpawnModuleInScene(_sceneId, graphPrefabsComponents[2], Vector3.zero);
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

				DataID_to_DataGO[graphData.edges[_edgeIdx].ID] = edgeGO;
				edgeGO.GetComponent<GameNetModule>().NetHide();
			}

#if UNITY_EDITOR

			/// <summary>
			/// USED ONLY FOR DEVELOPMENT: generates the structure of the graph outside
			/// of runtime. Use <see cref="EdgesBatchSpawn(int)"/> instead if you want to
			/// spawn the edges at runtime.
			/// </summary>
			private void EdgesSpawn()
			{
				for (int i = 0; i < graphData.edges.Length; i++)
				{
					//ModulesData.AddModule(edgeMD);
					GameObject edgeGO = Instantiate(graphPrefabsComponents[2]);
					edgeGO.transform.parent = pathwayRoot.transform;
					edgeGO.GetComponent<EdgeGO>().Initialize(this, graphData.edges[i]);

					DataID_to_DataGO[graphData.edges[i].ID] = edgeGO;
				}
			}

			/// <summary>
			/// USED ONLY FOR DEVELOPMENT: generates the structure of the graph outside
			/// of runtime. Use <see cref="RequestGraphGenerationServerRpc(ulong, int)"/>
			/// instead if you want to spawn the graph at runtime.
			/// </summary>
			public void GenerateGraph()
			{
				pathwayRoot = Instantiate(graphPrefabsComponents[0]);
				pathwayRoot.name = graphData.name;
				DataID_to_DataGO = new Dictionary<uint, GameObject>();
				//Instantiate Nodes of Layer
				NodesSpawn();

				//Instantiate Edges of Layer
				EdgesSpawn();
			}

			/// <summary>
			/// USED ONLY FOR DEVELOPMENT: Creates an asset for <see cref="graphData"/>.
			/// </summary>
			public void GenerateGraphAsset()
			{
				TextAsset graphDataAsset = new TextAsset(graphData.graphData.ToString());
				AssetDatabase.CreateAsset(graphDataAsset, "Assets/Resources/Prefabs/Modules/Demo_iJO1366/"+graphData.name+"_GraphData.asset");
				
			}
#endif

			/// <summary>
			/// Translates the information about knockedout reactions stored
			/// as a string usable for a request to the server.
			/// </summary>
			/// <returns>The list of all the individual knockout commands.
			/// It is to be paired with the "command=" kayword for every entry.</returns>
			public List<string> GetKnockouts()
			{
				List<string> knockouts = new List<string>();
				Dictionary<string, ushort> reactionMatch = new Dictionary<string, ushort>();
				ushort reactionMatchCount = 0;
				foreach (IEdge _edgeData in graphData.edges)
				{
					if (DataID_to_DataGO[_edgeData.ID].GetComponent<EdgeGO>().knockedOut.Value)
					{
						if (!reactionMatch.TryGetValue(_edgeData.name, out reactionMatchCount))
						{
							reactionMatch[_edgeData.name] = 1;
							knockouts.Add("knockout-" + _edgeData.ID);
						}
					}
				}
				return knockouts;
			}

			/// <summary>
			/// Coroutine encapsulating the instantiation of the nodes of the pathway.
			/// The coroutine yields until the end of the frame after having instantiated
			/// one batch of nodes.
			/// </summary>
			private IEnumerator NodesBatchSpawn(int _sceneId)
			{
				for (int i = 0; i < graphData.nodes.Length; i += m_graphBatchSpawning.nodesBatchSize)
				{
					for (int j = i; j < Mathf.Min(i + m_graphBatchSpawning.nodesBatchSize, graphData.nodes.Length); j++)
					{
						GameObject nodeGO = DiveScenesManager.Instance.SpawnModuleInScene(_sceneId, graphPrefabsComponents[1], Vector3.zero);
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
				GameObject nodeGO = _nodeNetObj;
				
				nodeGO.GetComponent<NodeGO>().Initialize(m_graphScalingData, graphData.nodes[_nodeIdx]);
				DataID_to_DataGO[graphData.nodes[_nodeIdx].ID] = nodeGO;
				nodeGO.GetComponent<GameNetModule>().NetHide();
			}

#if UNITY_EDITOR

			/// <summary>
			/// USED ONLY FOR DEVELOPMENT: generates the structure of the graph outside
			/// of runtime. Use <see cref="NodesBatchSpawn(int)"/> instead if you want to
			/// spawn the nodes at runtime.
			/// </summary>
			private void NodesSpawn()
			{
				for (int i = 0; i < graphData.nodes.Length; i++)
				{
					GameObject nodeGO = Instantiate(graphPrefabsComponents[1]);
					nodeGO.transform.parent = pathwayRoot.transform;
					nodeGO.GetComponent<NodeGO>().Initialize(m_graphScalingData, graphData.nodes[i]);
					DataID_to_DataGO[graphData.nodes[i].ID] = nodeGO;
				}
			}
#endif

			public void SetIndex(int _index)
			{
				refIndex = _index;
			}

			public void SwitchEdgeForceDefaultColorOverride()
			{
				edgeParametersOverride.forceDefaultColor = !edgeParametersOverride.forceDefaultColor;
			}

			public void SwitchEdgeForceDefaultWidthOverride()
			{
				edgeParametersOverride.forceDefaultWidth = !edgeParametersOverride.forceDefaultWidth;
			}
			
			public void SetEdgeStartWidthFactorOverride(float _startWidthFactor)
			{
				edgeParametersOverride.startWidthFactor = _startWidthFactor;
			}

			public void SetEdgeEndWidthFactorOverride(float _endWidthFactor)
			{
				edgeParametersOverride.endWidthFactor = _endWidthFactor;
			}

			#region - IDive Methods -
			/// <inheritdoc/>
			public override IEnumerator GenerativeDiveInC()
			{
				RequestSourceDataGenerationServerRpc(NetworkManager.Singleton.LocalClientId);

				yield return new WaitUntil(()=>isReadyForDive.Value);

				DirectDiveIn();
			}
			#endregion

			#region - IGraphGONet Methods -

			/// <inheritdoc/>
			[ServerRpc(RequireOwnership = false)]
			public void RequestGraphGenerationServerRpc(ulong _expeditorClientId, int _rootSceneId)
			{
				pathwayRoot = DiveScenesManager.Instance.SpawnModuleInScene(_rootSceneId,
																			graphPrefabsComponents[0],
																			Vector3.zero);
				pathwayRoot.name = graphData.name;

				//Instantiate Nodes of Layer
				StartCoroutine(NodesBatchSpawn(_rootSceneId));

				//Instantiate Edges of Layer
				StartCoroutine(EdgesBatchSpawn(_rootSceneId));
			}

			/// <inheritdoc/>
			public void SetNetworkData(IGraph _IGraph)
			{
				graphData = _IGraph;
				SetName(_IGraph.name);
			}
			#endregion

			#region - IModifiable Methods -
			/// <inheritdoc/>
			public void ApplyFileModifications()
			{
				List<string[]> allModifications = readingModificationFile.GetAllCommands();
				foreach (string[] _mod in allModifications)
				{
					foreach (string _command in _mod)
					{
						OperationSwitch(_command);
					}
				}
			}

			/// <inheritdoc/>
			public bool CheckName(string _name)
			{
				return _name == graphData.name;
			}

			/// <inheritdoc/>
			public void OperationSwitch(string _op)
			{
				string[] opContent = _op.Split('-');

				switch (opContent[0])
				{
					case "knockout":
						GameObject edgeGO;
						uint edgeID = System.Convert.ToUInt32(opContent[1]);
						if (DataID_to_DataGO.TryGetValue(edgeID, out edgeGO))
						{
							Debug.Log("KnockingOut:" + opContent[1]);
							edgeGO.GetComponent<EdgeGO>().Knockout();
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
				CyJsonModulesData.AddData(this);
				SetIndex(CyJsonModulesData.loadedData.Count - 1);

				SetNetworkData(pathway);
			}

			/// <inheritdoc/>
			[ServerRpc(RequireOwnership = false)]
			public override void RequestSourceDataGenerationServerRpc(ulong _expeditorClientID)
			{
				int rootSceneId = GameNetPortal.Instance.netSessionPlayersDataMap[_expeditorClientID].currentScene;

				targetSceneId.Value = DiveScenesManager.Instance.AddNewDiveScene(rootSceneId);
				LogSystem.AddMessage(LogMessageTypes.Debug,
					$"targetSceneId of the new module is: {targetSceneId}");
				RequestGraphGenerationServerRpc(_expeditorClientID, targetSceneId.Value);
			}
			#endregion

			#region - ISaveable Methods -
			/// <inheritdoc/>
			public void CompileModificationFile()
			{
				string author = GameNetPortal.Instance.netSessionPlayersDataMap[NetworkManager.Singleton.LocalClientId].playerName;
				string baseModelName = graphData.name;

				//ScanForKOModifications();
				List<string> KOCommand = GetKnockouts();
				Modification modification = new Modification(author,
					System.DateTime.Now.ToString("yyyyMMddTHHmmssZ"),
					KOCommand);

				writingModificationFile = new ModificationFile(baseModelName, modification);
			}

#endregion
		}
	}
}
