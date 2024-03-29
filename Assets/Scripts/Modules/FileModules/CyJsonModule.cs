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
									IGraphGONet<CyJsonEdge, CyJsonNode>,
									IModifiable,
									ISaveable
		{			
			/// <summary>
			/// The reference to the root that will contain the nodes and edges
			/// once the graph is generated.
			/// </summary>
			[Header("GraphGONet")]
			[HideInInspector] public GameObject pathwayRoot;

			/// <summary>
			/// A boolean to check if all the nodes have been spawned.
			/// </summary>
			private bool allNodesSpawned = false;

			/// <summary>
			/// The structure to store the parameters to override the default
			/// visual parameters of the edges of this graph.
			/// </summary>
			[SerializeField] private CyJsonEdgeGOParametersOverrideData edgeParametersOverride = new CyJsonEdgeGOParametersOverrideData
            {
				forceDefaultColor = false,
				forceDefaultWidth = false,
				startWidthFactor = 1,
				endWidthFactor = 1,
			};

			#region  - IGraphGONet Members - 
			/// <summary>
			/// The field of the property <see cref="graphData"/>.
			/// </summary>
			[SerializeField] private CyJsonPathway m_graphData;

			/// <inheritdoc/>
			public IGraph<CyJsonEdge, CyJsonNode> graphData { get => m_graphData; protected set => m_graphData = (CyJsonPathway)value; }

			/// <summary>
			/// The field of the property <see cref="graphScalingData"/>.
			/// </summary>
			[SerializeField] private GraphScalingData m_graphScalingData;

			/// <inheritdoc/>
			public GraphScalingData graphScalingData
			{
				get => m_graphScalingData;
			}
			
			/// <summary>
			/// The field of the property <see cref="graphBatchSpawning"/>.
			/// </summary>
			[SerializeField] private GraphBatchSpawning m_graphBatchSpawning;

			/// <inheritdoc/>
			public GraphBatchSpawning graphBatchSpawning
			{
				get => m_graphBatchSpawning;
			}

			/// <inheritdoc/>
			public Dictionary<uint, GameObject> DataID_to_DataGO { get; set; }

			/// <summary>
			/// The field of the property <see cref="graphPrefabsComponents"/>.
			/// </summary>
			[SerializeField] private List<GameObject> m_graphPrefabsComponents;

			/// <inheritdoc/>
			public List<GameObject> graphPrefabsComponents
			{
				get => m_graphPrefabsComponents;
				private set => m_graphPrefabsComponents = value;
			}
			#endregion

			#region - IModifiable Members -
			/// <inheritdoc/>
			public ModificationFile readingModificationFile { get; set; }
			#endregion

			#region - ISaveable Members -
			/// <inheritdoc/>
			public ModificationFile writingModificationFile { get; set; }
			#endregion

			public override void OnNetworkSpawn()
			{
				base.OnNetworkSpawn();

				DataID_to_DataGO = new Dictionary<uint, GameObject>();
				GameNetDataManager.Instance.modifiables.Add(this);
				GameNetDataManager.Instance.saveables.Add(this);
			}

			/// <inheritdoc/>
			protected override void ApplyCurrentColorChange(Color _previous, Color _current)
			{
				mpb.SetVector(colorID, _current);
				m_Renderer.SetPropertyBlock(mpb);
			}

			/// <summary>
			/// The public interface to apply the new visual parameters for the
			/// edges and update the display.
			/// </summary>
			public void ApplyEdgeWidthFactorOverride()
			{
				StartCoroutine(ApplyEdgeWidthFactorOverrideC());
			}

			/// <summary>
			/// The coroutine to apply the new visual parameters for the
			/// edges and update the display by batches.
			/// </summary>
			public IEnumerator ApplyEdgeWidthFactorOverrideC()
			{
				Debug.Log($"ApplyEdgeStartWidthFactorOverrideC: startWidthFactor =" +
					$" {edgeParametersOverride.startWidthFactor}, endWidthFactor = " +
					$" {edgeParametersOverride.endWidthFactor}", gameObject);
				CyJsonEdgeGO edgeGO;
				foreach (IEdge _edge in m_graphData.edges)
				{
					edgeGO = DataID_to_DataGO[_edge.ID].GetComponent<CyJsonEdgeGO>();
					edgeGO.forceDefaultColor = edgeParametersOverride.forceDefaultColor;
					edgeGO.forceDefaultWidth = edgeParametersOverride.forceDefaultWidth;
					edgeGO.startWidthFactor = edgeParametersOverride.startWidthFactor;
					edgeGO.endWidthFactor = edgeParametersOverride.endWidthFactor;
					edgeGO.ApplyFluxLevelClamped();
					edgeGO.SetCurrentColorToDefaultServerRpc();

				}

				yield return null;
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

			/// <summary>
			/// The server send the reference of the newly spawned edge to the clients
			/// to locally assign data to the edge and update <see cref="DataID_to_DataGO"/>.
			/// </summary>
			/// <param name="_edgeNetObj">
			/// The network reference to the edge gameobject.
			/// </param>
			/// <param name="_edgeIdx">
			/// The index of the edge data in the <see cref="IGraph{EdgeType, NodeType}.edges"/>
			/// array of <see cref="IGraphGO{EdgeType, NodeType}.graphData"/>.
			/// </param>
			[ClientRpc]
			public void EdgeSpawnClientRpc(NetworkObjectReference _edgeNetObj, int _edgeIdx)
			{
				GameObject edgeGO = _edgeNetObj;
				edgeGO.GetComponent<CyJsonEdgeGO>().Initialize(this, graphData.edges[_edgeIdx]);

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
					edgeGO.GetComponent<CyJsonEdgeGO>().Initialize(this, graphData.edges[i]);

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
				TextAsset graphDataAsset = new TextAsset(m_graphData.cyJsonGraphData.graphData.ToString());
				AssetDatabase.CreateAsset(graphDataAsset, "Assets/Resources/Prefabs/Modules/Demo_iJO1366/"+graphData.name+"_GraphData.asset");
				
			}
#endif

			/// <summary>
			/// Gets the raw Json data used to generate the CyJsonPathway.
			/// </summary>
			/// <returns>
			/// The Json data used to generate the CyJsonPathway.
			/// </returns>
			public GraphData<JObject, JArray, JArray> GetJsonGraphData()
			{
				return m_graphData.cyJsonGraphData;
			}

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
					if (DataID_to_DataGO[_edgeData.ID].GetComponent<CyJsonEdgeGO>().knockedOut.Value)
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

			/// <summary>
			/// The server send the reference of the newly spawned node to the clients
			/// to locally assign data to the node and update <see cref="DataID_to_DataGO"/>.
			/// </summary>
			/// <param name="_nodeNetObj">
			/// The network reference to the node gameobject.
			/// </param>
			/// <param name="_nodeIdx">
			/// The index of the node data in the <see cref="IGraph{EdgeType, NodeType}.nodes"/>
			/// array of <see cref="IGraphGO{EdgeType, NodeType}.graphData"/>.
			/// </param>
			[ClientRpc]
			public void NodeSpawnClientRpc(NetworkObjectReference _nodeNetObj, int _nodeIdx)
			{
				GameObject nodeGO = _nodeNetObj;
				
				nodeGO.GetComponent<CyJsonNodeGO>().Initialize(m_graphScalingData, graphData.nodes[_nodeIdx]);
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
					nodeGO.GetComponent<CyJsonNodeGO>().Initialize(m_graphScalingData, graphData.nodes[i]);
					DataID_to_DataGO[graphData.nodes[i].ID] = nodeGO;
				}
			}
#endif
			/// <summary>
			/// Switches the value of <see cref="ECellDive.Utility.Data.Graph.CyJsonEdgeGOParametersOverrideData.forceDefaultColor"/>
			/// in <see cref="edgeParametersOverride"/>.
			/// </summary>
			public void SwitchEdgeForceDefaultColorOverride()
			{
				edgeParametersOverride.forceDefaultColor = !edgeParametersOverride.forceDefaultColor;
			}

			/// <summary>
			/// Switches the value of <see cref="ECellDive.Utility.Data.Graph.CyJsonEdgeGOParametersOverrideData.forceDefaultWidth"/>
			/// in <see cref="edgeParametersOverride"/>.
			/// </summary>
			public void SwitchEdgeForceDefaultWidthOverride()
			{
				edgeParametersOverride.forceDefaultWidth = !edgeParametersOverride.forceDefaultWidth;
			}

            /// <summary>
            /// Sets the value of <see cref="ECellDive.Utility.Data.Graph.CyJsonEdgeGOParametersOverrideData.startWidthFactor"/>
            /// in <see cref="edgeParametersOverride"/> to <see paramref="_startWidthFactor"/>.
            /// </summary>
            /// <param name="_startWidthFactor">
            /// The new value of <see paramref="_startWidthFactor"/> in <see cref="edgeParametersOverride"/>.
            /// </param>
            public void SetEdgeStartWidthFactorOverride(float _startWidthFactor)
			{
				edgeParametersOverride.startWidthFactor = _startWidthFactor;
			}

            /// <summary>
            /// Sets the value of <see cref="ECellDive.Utility.Data.Graph.CyJsonEdgeGOParametersOverrideData.endWidthFactor"/>
            /// in <see cref="edgeParametersOverride"/> to <see paramref="_endWidthFactor"/>.
            /// </summary>
            /// <param name="_endWidthFactor">
            /// The new value of <see paramref="endWidthFactor"/> in <see cref="edgeParametersOverride"/>.
            /// </param>
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
			public void SetGraphData(IGraph<CyJsonEdge, CyJsonNode> _graphData)
			{
				graphData = _graphData;
				SetName(_graphData.name);
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
							edgeGO.GetComponent<CyJsonEdgeGO>().Knockout();
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
				//SetIndex(CyJsonModulesData.loadedData.Count - 1);

				SetGraphData(pathway);
			}

			/// <inheritdoc/>
			[ServerRpc(RequireOwnership = false)]
			public override void RequestSourceDataGenerationServerRpc(ulong _expeditorClientID)
			{
				int rootSceneId = GameNetDataManager.Instance.GetCurrentScene(_expeditorClientID);

				DiveScenesManager.Instance.AddNewDiveSceneServerRpc(rootSceneId, nameField.text);
				targetSceneId.Value = DiveScenesManager.Instance.scenesBank.Count - 1;
				LogSystem.AddMessage(LogMessageTypes.Debug,
					$"targetSceneId of the new module is: {targetSceneId}");
				RequestGraphGenerationServerRpc(_expeditorClientID, targetSceneId.Value);
			}
            #endregion

            #region - IMlprVisibility Methods -
			/// <inheritdoc/>
            public override void NetHide()
            {
                base.NetHide();
				if (m_nameTextFieldContainer != null)
				{
					m_nameTextFieldContainer.SetActive(false);
				}
            }

			/// <inheritdoc/>
			public override void NetShow()
			{
				base.NetShow();
				if (m_nameTextFieldContainer != null)
				{
                    m_nameTextFieldContainer.SetActive(true);
                }
			}
            #endregion

            #region - ISaveable Methods -
            /// <inheritdoc/>
            public void CompileModificationFile()
			{
				string author = GameNetDataManager.Instance.GetClientName(NetworkManager.Singleton.LocalClientId);
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
