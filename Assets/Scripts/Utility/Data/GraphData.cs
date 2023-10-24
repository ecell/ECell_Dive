using UnityEngine;

namespace ECellDive.Utility.Data.Graph
{
    /// <summary>
    /// The struct encapsulating the data of a graph.
    /// </summary>
    /// <typeparam name="TGraphData">
    /// The type of the data containing everything about the graph.
    /// </typeparam>
    /// <typeparam name="TEdgesData">
    /// The type of the data containing everything about the edges.
    /// </typeparam>
    /// <typeparam name="TNodesData">
    /// The type of the data containing everything about the nodes.
    /// </typeparam>
    public struct GraphData<TGraphData, TEdgesData, TNodesData>
    {
        /// <summary>
        /// The data containing everything about the graph.
        /// </summary>
        public TGraphData graphData;

        /// <summary>
        /// The data containing everything about the edges.
        /// </summary>
		public TEdgesData edgesData;

        /// <summary>
        /// The data containing everything about the nodes.
        /// </summary>
		public TNodesData nodesData;
    }

    /// <summary>
    /// The struct encapsulating the parameters relevant to
    /// controlling the scale of the graph.
    /// </summary>
    [System.Serializable]
    public struct GraphScalingData
    {
        /// <summary>
        /// Modulates the values of Transform.position for
        /// every node of the graph.
        /// </summary>
        /// <remarks>
        /// Useful when the original coordinates of the nodes spans over
        /// hundreds of equivalent Unity distance units.
        /// </remarks>
        [Min(0.001f)] public float positionScaleFactor;

        /// <summary>
        /// Modulates the values of Transform.localScale for
        /// every node and edge of the graph.
        /// </summary>
        /// <remarks>
        /// Usefull when to change the size of the whole graph.
        /// </remarks>
        [Min(0.001f)] public float sizeScaleFactor;

    }

    /// <summary>
    /// The struct encapsulating the parameters relevant to
    /// controlling the batched instantiation of the nodes and
    /// the edges of the graph.
    /// </summary>
    /// <remarks>
    /// Usefull when instantiating the nodes and edges of a graph
    /// that is synchronized over the network between a host and
    /// its clients. When the graph is big we need to batch the
    /// instantiation of the networkbjects to avoid overflowing
    /// the authorized bandwitdh (since the server sends the
    /// instantiation message to every clients).
    /// </remarks>
    [System.Serializable]
	public struct GraphBatchSpawning
	{
		/// <summary>
		/// The number of nodes instantiated within a frame.
		/// </summary>
		public int nodesBatchSize;

		/// <summary>
		/// The number of edges instantiated within a frame.
		/// </summary>
		public int edgesBatchSize;
	}
}
