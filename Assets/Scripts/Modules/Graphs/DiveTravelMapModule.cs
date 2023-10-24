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
		private ContiguousGraph m_graphData;

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

		private GameObject diveTravelMapRoot;

		/// <summary>
		/// Builds the dive travel map from the dive travel data of the
		/// local player.
		/// </summary>
		public void BuildMap()
		{
			Clear();
			List<int> diveSceneTrace = GameNetDataManager.Instance.GetSceneTrace(NetworkManager.Singleton.LocalClientId);



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
		}

		public void SetGraphData(IGraph<Edge, Node> _graphData)
		{
			graphData = _graphData;
		}
	}

}
