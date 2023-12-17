using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using ECellDive.Multiplayer;
using ECellDive.Interfaces;
using ECellDive.Utility.Data.Graph;
using ECellDive.GraphComponents;
using ECellDive.SceneManagement;

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
		[Header("DiveTravelMapModule Parameters")]
		[Header("IGraphGO Parameters")]
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
		[SerializeField] private GameObject refDiveTravelMapRoot;

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

			List<int> diveSceneTrace = GameNetDataManager.Instance.GetSceneTrace(NetworkManager.Singleton.LocalClientId);
			//List<int> diveSceneTrace = new List<int>() { 1, 2, 5, 4, 1, 5, 6 }; //For tests and debug
			m_graphData.Populate(diveSceneTrace);

			Dictionary<uint, Color> nodesColors = new Dictionary<uint, Color>();
			Vector3 nodePosition = Vector3.zero;
			for (int i = 0; i < m_graphData.nodes.Length; i++)
			{
				GameObject nodeGO = Instantiate(m_graphPrefabsComponents[0], refDiveTravelMapRoot.transform);
				NodeGO nodeGOcp = nodeGO.GetComponent<NodeGO>();
				nodeGOcp.SetNodeData(m_graphData.nodes[i]);
				nodeGOcp.SetPosition(nodePosition, graphScalingData.positionScaleFactor);
				nodeGOcp.SetScale(Vector3.one, graphScalingData.sizeScaleFactor);
				
				nodeGOcp.SetName(DiveScenesManager.Instance.scenesBank[diveSceneTrace[i]].sceneName);
				//nodeGOcp.SetName(m_graphData.nodes[i].name); //for tests and debug
				nodeGOcp.SetNamePosition(graphScalingData.sizeScaleFactor);
				nodeGOcp.HideName();

				DataID_to_DataGO.Add(m_graphData.nodes[i].ID, nodeGO);

				nodeGOcp.defaultColor = Color.HSVToRGB((float)i / m_graphData.nodes.Length, 1, 1);
				nodeGOcp.ApplyColor(nodeGOcp.defaultColor);

				nodesColors.Add(m_graphData.nodes[i].ID, nodeGOcp.defaultColor);
				nodePosition += Vector3.right * 1f;
			}

			Matrix4x4 displayBase = Matrix4x4.identity;
			displayBase.m00 = -1f;
			for (int i = 0; i < m_graphData.edges.Length; i++)
			{
				GameObject edgeGO = Instantiate(m_graphPrefabsComponents[1], refDiveTravelMapRoot.transform);
				EdgeGO edgeGOcp = edgeGO.GetComponent<EdgeGO>();
				edgeGOcp.curvePointsCount = 20;
				edgeGOcp.SetDefaultWidth(0.1f*edgeGOcp.defaultStartWidth, edgeGOcp.defaultEndWidth);
				edgeGOcp.SetEdgeData(m_graphData.edges[i]);
				edgeGOcp.SetLineRendererWidth();

                //We rotate the line so that the edge is oriented toward the opposite direction
				//of the forward vector of the GO encasulating the Dive Travel Map.
                edgeGOcp.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);

				//Because of the rotation, we need to calculate the start and end position which
				//are in the local space of the Dive Map, to the local space of the edge GO.
                edgeGOcp.SetLineRendererPosition(displayBase.MultiplyPoint3x4(DataID_to_DataGO[m_graphData.edges[i].source].transform.localPosition),
                                                displayBase.MultiplyPoint3x4(DataID_to_DataGO[m_graphData.edges[i].target].transform.localPosition));
				edgeGOcp.SetCollider(DataID_to_DataGO[m_graphData.edges[i].source].transform,
									DataID_to_DataGO[m_graphData.edges[i].target].transform);

				edgeGOcp.SetName(m_graphData.edges[i].name);
				edgeGOcp.SetNamePosition(graphScalingData.sizeScaleFactor);
				edgeGOcp.HideName();

				edgeGOcp.defaultGradient[0] = nodesColors[m_graphData.edges[i].source];
				edgeGOcp.defaultGradient[1] = nodesColors[m_graphData.edges[i].target];
				edgeGOcp.ApplyGradient(edgeGOcp.defaultGradient);

				DataID_to_DataGO.Add(m_graphData.edges[i].ID, edgeGO);
			}

			//Center the dive travel map root
			refDiveTravelMapRoot.transform.localPosition = -0.5f * (nodePosition - Vector3.right) * graphScalingData.positionScaleFactor;
		}

		/// <summary>
		/// Deleted every child of the dive travel map root.
		/// </summary>
		public void Clear()
		{
			while (refDiveTravelMapRoot.transform.childCount > 0)
			{
#if UNITY_EDITOR
			DestroyImmediate(refDiveTravelMapRoot.transform.GetChild(0).gameObject);
#else
			Destroy(refDiveTravelMapRoot.transform.GetChild(0).gameObject);
#endif
            }

			DataID_to_DataGO.Clear();
		}

		public void SetGraphData(IGraph<Edge, Node> _graphData)
		{
			graphData = _graphData;
		}
	}

}
