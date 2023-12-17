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
		/// A coroutine to scale the node to contain the text of its name.
		/// We must waitfor the end of the frame to be sure that the scales ratio
		/// between the node, the child GO for the graphics of the node, and the
		/// name container, are all set.
		/// 
		/// Once it is done, we can retrieve the abounds of the TMP_Text of the name
		/// and assign it to the highlight scale of the node. 
		/// </summary>
		/// <param name="_nodeGO"></param>
		/// <returns></returns>
		IEnumerator NodeScalingToContainText(NodeGO _nodeGO)
		{
            nameField.ForceMeshUpdate();
			yield return new WaitForEndOfFrame();

			//After tests, I prefered to use the "bounds" and not the "textBouds"
			_nodeGO.highlightScale = graphScalingData.sizeScaleFactor * (_nodeGO.nameField.bounds.max - _nodeGO.nameField.bounds.min);
            
			//We guarentee that the highlight scale is always bigger than the default scale
			//and we increase it by 5% to allow a little margin between the text and the node.
			_nodeGO.highlightScale = new Vector3(
				1.05f*Mathf.Max(_nodeGO.defaultScale.x, _nodeGO.highlightScale.x),
				1.05f*Mathf.Max(_nodeGO.defaultScale.y, _nodeGO.highlightScale.y),
				1f);

            //We also finish by hiding the name since nodes in the dive
			//travel map do not display their name unless they are highlighted.
            _nodeGO.HideName();
        }

        /// <summary>
        /// Builds the dive travel map from the dive travel data of the
        /// local player.
        /// </summary>
        public void BuildMap()
		{
			Clear();

			List<int> diveSceneTrace = GameNetDataManager.Instance.GetSceneTrace(NetworkManager.Singleton.LocalClientId);
			//List<int> diveSceneTrace = new List<int>() { 1, 2, 555, 4, 1, 555, 6 }; //For tests and debug
			m_graphData.Populate(diveSceneTrace);

			Dictionary<uint, Color> nodesColors = new Dictionary<uint, Color>();
			Vector3 nodePosition = Vector3.zero;
			Vector3 nodePosInc = Vector3.right * 0.5f + Vector3.down * 0.25f;
			for (int i = 0; i < m_graphData.nodes.Length; i++)
			{
				GameObject nodeGO = Instantiate(m_graphPrefabsComponents[0], refDiveTravelMapRoot.transform);
				NodeGO nodeGOcp = nodeGO.GetComponent<NodeGO>();
				nodeGOcp.SetNodeData(m_graphData.nodes[i]);
				nodeGOcp.SetPosition(nodePosition, graphScalingData.positionScaleFactor);
				nodeGOcp.SetScale(Vector3.one, graphScalingData.sizeScaleFactor);

				nodeGOcp.SetName(DiveScenesManager.Instance.scenesBank[diveSceneTrace[i]].sceneName);
				//nodeGOcp.SetName(m_graphData.nodes[i].name); //for tests and debug
				nodeGOcp.SetNamePosition(0);
				StartCoroutine(NodeScalingToContainText(nodeGOcp));

                DataID_to_DataGO.Add(m_graphData.nodes[i].ID, nodeGO);

				nodeGOcp.defaultColor = Color.HSVToRGB((float)i / m_graphData.nodes.Length, 1, 1);
				nodeGOcp.ApplyColor(nodeGOcp.defaultColor);
				nodeGOcp.highlightColor = new Color(1 - nodeGOcp.defaultColor.r,
					1 - nodeGOcp.defaultColor.g, 1 - nodeGOcp.defaultColor.b, 0.25f);

				nodesColors.Add(m_graphData.nodes[i].ID, nodeGOcp.defaultColor);
				nodePosition += nodePosInc;
			}

			for (int i = 0; i < m_graphData.edges.Length; i++)
			{
				GameObject edgeGO = Instantiate(m_graphPrefabsComponents[1], refDiveTravelMapRoot.transform);
				EdgeGO edgeGOcp = edgeGO.GetComponent<EdgeGO>();
				edgeGOcp.curvePointsCount = 20;
				edgeGOcp.SetDefaultWidth(0.1f*edgeGOcp.defaultStartWidth, edgeGOcp.defaultEndWidth);
				edgeGOcp.SetEdgeData(m_graphData.edges[i]);
				edgeGOcp.SetLineRendererWidth();

				//Because of the rotation, we need to calculate the start and end position which
				//are in the local space of the Dive Map, to the local space of the edge GO.
				edgeGOcp.SetLineRendererPosition(DataID_to_DataGO[m_graphData.edges[i].source].transform.localPosition,
												DataID_to_DataGO[m_graphData.edges[i].target].transform.localPosition);
				edgeGOcp.SetCollider(DataID_to_DataGO[m_graphData.edges[i].source].transform,
									DataID_to_DataGO[m_graphData.edges[i].target].transform);

				edgeGOcp.SetName($"{i+1}");
				edgeGOcp.SetNamePosition(graphScalingData.sizeScaleFactor);
				edgeGOcp.HideName();

				edgeGOcp.defaultGradient[0] = nodesColors[m_graphData.edges[i].source];
				edgeGOcp.defaultGradient[1] = nodesColors[m_graphData.edges[i].target];
				edgeGOcp.ApplyGradient(edgeGOcp.defaultGradient);
				Color blend = Color.Lerp(edgeGOcp.defaultGradient[0], edgeGOcp.defaultGradient[1], 0.5f);
				edgeGOcp.highlightColor = new Color(1 - blend.r, 1 - blend.g, 1 - blend.b, 0.75f);

				DataID_to_DataGO.Add(m_graphData.edges[i].ID, edgeGO);
			}

			//Center the dive travel map root
			refDiveTravelMapRoot.transform.localPosition = -0.5f * (nodePosition - nodePosInc) * graphScalingData.positionScaleFactor;
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
