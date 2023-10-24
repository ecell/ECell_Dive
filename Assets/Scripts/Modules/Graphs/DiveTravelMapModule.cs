using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using ECellDive.Multiplayer;
using ECellDive.Interfaces;
using ECellDive.Utility.Data.Graph;
using ECellDive.GraphComponents;

namespace ECellDive.Modules
{
	/// <summary>
	/// The logic to draw the dive travel map showing which dive scenes
	/// a player has been to and in which order.
	/// </summary>
	public class DiveTravelMapModule : Module,
										IGraphGO<Edge, Node>
	{
		#region - IGraphGO Members -
		/// <summary>
		/// The field for the property <see cref="graphData"/>.
		/// </summary>
		[Header("DiveTravelMapModule Parameters")]
		[Header("IGraphGO Parameters")]
		private ContiguousGraph m_graphData = new ContiguousGraph("Dive Travel Map");

		/// <inheritdoc/>
		public IGraph<Edge, Node> graphData
		{
			get => m_graphData;
			protected set => m_graphData = (ContiguousGraph)value;
		}

		/// <summary>
		/// The field for the property <see cref="graphPrefabsComponents"/>.
		/// </summary>
		[SerializeField] List<GameObject> m_graphPrefabsComponents;

		/// <inheritdoc/>
		public List<GameObject> graphPrefabsComponents
		{
			get => m_graphPrefabsComponents;
		}

		/// <summary>
		/// The field for the property <see cref="graphScalingData"/>.
		/// </summary>
		[SerializeField] GraphScalingData m_graphScalingData;

		/// <inheritdoc/>
		public GraphScalingData graphScalingData
		{
			get => m_graphScalingData;
		}

		/// <inheritdoc/>
		public Dictionary<uint, GameObject> DataID_to_DataGO { get ; set ; }
		#endregion

		/// <summary>
		/// The reference to the root of the dive travel map.
		/// </summary>
		private GameObject diveTravelMapRoot;

		private void Start()
		{
			DataID_to_DataGO = new Dictionary<uint, GameObject>();
			BuildMap();
		}

		/// <summary>
		/// Builds the dive travel map from the dive travel data of the
		/// local player.
		/// </summary>
		public void BuildMap()
		{
			Clear();

			//List<int> diveSceneTrace = GameNetDataManager.Instance.GetSceneTrace(NetworkManager.Singleton.LocalClientId);
			List<int> diveSceneTrace = new List<int>() { 1, 2, 5, 4, 1, 5, 6 };
			m_graphData.Populate(diveSceneTrace);

			diveTravelMapRoot = Instantiate(m_graphPrefabsComponents[0], gameObject.transform);

			Vector3 nodePosition = Vector3.zero;
			foreach (Node node in m_graphData.nodes)
			{
				Debug.Log($"Building: Adding node {node.ID}");

				GameObject nodeGO = Instantiate(m_graphPrefabsComponents[1], diveTravelMapRoot.transform);
				NodeGO nodeGOcp = nodeGO.GetComponent<NodeGO>();
				nodeGOcp.SetNodeData(node);
				nodeGOcp.SetPosition(nodePosition, graphScalingData.positionScaleFactor);
				nodeGOcp.SetScale(Vector3.one, graphScalingData.sizeScaleFactor);
				
				nodeGOcp.SetName(node.name);
				nodeGOcp.SetNamePosition(graphScalingData.sizeScaleFactor);
				nodeGOcp.HideName();

				DataID_to_DataGO.Add(node.ID, nodeGO);

				nodePosition += Vector3.right * 1f;
			}

			foreach (Edge edge in m_graphData.edges)
			{
				GameObject edgeGO = Instantiate(m_graphPrefabsComponents[2], diveTravelMapRoot.transform);
				EdgeGO edgeGOcp = edgeGO.GetComponent<EdgeGO>();
				edgeGOcp.SetEdgeData(edge);
				edgeGOcp.SetLineRendererWidth();
				edgeGOcp.SetLineRendererPosition(DataID_to_DataGO[edge.source].transform,
												DataID_to_DataGO[edge.target].transform);
				edgeGOcp.SetCollider(DataID_to_DataGO[edge.source].transform,
									DataID_to_DataGO[edge.target].transform);

				edgeGOcp.SetName(edge.name);
				edgeGOcp.SetNamePosition(graphScalingData.sizeScaleFactor);
				edgeGOcp.HideName();

				DataID_to_DataGO.Add(edge.ID, edgeGO);
			}

		}

		/// <summary>
		/// Deleted every child of the dive travel map root.
		/// </summary>
		public void Clear()
		{
#if UNITY_EDITOR
			DestroyImmediate(diveTravelMapRoot);
#else
			Destroy(diveTravelMapRoot);
#endif
			DataID_to_DataGO.Clear();
		}

		public void SetGraphData(IGraph<Edge, Node> _graphData)
		{
			graphData = _graphData;
		}
	}

}
