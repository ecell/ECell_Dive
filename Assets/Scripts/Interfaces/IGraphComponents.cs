using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

using ECellDive.Utility.Data.Graph;

namespace ECellDive.Interfaces
{
	/// <summary>
	/// The interface for the data structure encoding a node in a graph.
	/// </summary>
	/// <remarks>
	/// This interface is probably overdefined with cyjson graphs in mind.
	/// For example, the <see cref="isVirtual"/> field might be too specific.
	/// </remarks>
	public interface INode
	{
		/// <summary>
		/// A unique ID.
		/// </summary>
		uint ID { get; set; }

		/// <summary>
		/// The list of the <see cref="IEdge.ID"/> of the edges that
		/// started from somewhere and are arriving to this node.
		/// </summary>
		List<uint> incommingEdges { get; set; }

		/// <summary>
		/// The list of the <see cref="IEdge.ID"/> of the edges that
		/// are starting from this node.
		/// </summary>
		List<uint> outgoingEdges { get; set; }


		/// <summary>
		/// The name of the node.
		/// </summary>
		string name { get; set; }
	}

	/// <summary>
	/// The interface for the data structure encoding an edge in a graph.
	/// </summary>
	public interface IEdge
	{
		/// <summary>
		/// A unique ID.
		/// </summary>
		uint ID { get; set; }

		/// <summary>
		/// The name of the edge.
		/// </summary>
		string name { get; set; }

		/// <summary>
		/// The <see cref="INode.ID"/> of the node used from where the
		/// edge is starting.
		/// </summary>
		uint source { get; set; }

		/// <summary>
		/// The <see cref="INode.ID"/> of the node used where the
		/// edge is heading.
		/// </summary>
		uint target { get; set; }
	}

	/// <summary>
	/// The interface for the data structure encoding a graph
	/// made of nodes connected by edges.
	/// </summary>
	public interface IGraph<EdgeType, NodeType> where EdgeType : IEdge where NodeType : INode
	{
		/// <summary>
		/// The name of the graph.
		/// </summary>
		string name { get; }

		/// <summary>
		/// The nodes of this graph.
		/// </summary>
		NodeType[] nodes { get; }

		/// <summary>
		/// The edges of this graph.
		/// </summary>
		EdgeType[] edges { get; }
	}

	/// <summary>
	/// The interface defining the required logic to manipulate
	/// the information stored in a <see cref="INode"/> of a
	/// <see cref="IGraph"/>.
	/// </summary>
	/// <typeparam name="T">
	/// The type of the data structure encoding the node.
	/// </typeparam>
	public interface INodeGO<T> where T : INode
	{
		/// <summary>
		/// The data structure encoding the node.
		/// </summary>
		T nodeData { get; }

		/// <summary>
		/// The string containing information to be displayed about 
		/// the node.
		/// </summary>
		string informationString { get; }

		/// <summary>
		/// Sets the value for <see cref="nodeData"/>.
		/// </summary>
		/// <param name="_nodeData">
		/// The value to pass on to <see cref="nodeData"/>.
		/// </param>
		void SetNodeData(T _nodeData);
	}

	/// <summary>
	/// The interface defining the required logic to manipulate
	/// the information stored in a <see cref="IEdge"/> of a
	/// <see cref="IGraph"/>.
	/// </summary>
	/// <typeparam name="T">
	/// The type of the data structure encoding the edge.
	/// </typeparam>
	public interface IEdgeGO<T> where T : IEdge
	{
		/// <summary>
		/// The data structure encoding the edge.
		/// </summary>
		T edgeData { get; }

		/// <summary>
		/// The string containing information to be displayed about 
		/// the edge.
		/// </summary>
		string informationString { get; }

		/// <summary>
		/// The default width of the edge on its starting end.
		/// </summary>
		float defaultStartWidth { get; }

		/// <summary>
		/// The default width of the edge on its ending end.
		/// </summary>
		float defaultEndWidth { get; }

		/// <summary>
		/// The game object containing a box collider to be placed
		/// dynamically around the edge so that the user can interact with it.
		/// </summary>
		GameObject refBoxColliderHolder { get; }

		/// <summary>
		/// The logic to graphically reverse the orientation of the edge GO.
		/// The current start point becomes the end and the end becomes the start point.
		/// </summary>
		void ReverseOrientation();

		/// <summary>
		/// Set the value for <see cref="edgeData"/>.
		/// </summary>
		/// <param name="_edgeData">The value to pass on to <see cref="edgeData"/>.</param>
		void SetEdgeData(T _edgeData);

		/// <summary>
		/// Sets the values of <see cref="defaultStartWidth"/> and
		/// <see cref="defaultEndWidth"/>.
		/// </summary>
		/// <param name="_start">The value for <see cref="defaultStartWidth"/></param>
		/// <param name="_end">The value for <see cref="defaultEndWidth"/></param>
		void SetDefaultWidth(float _start, float _end);

		/// <summary>
		/// Adapts the position and rotation of the <see cref="refBoxColliderHolder"/>.
		/// </summary>
		/// <param name="_start">The transform of the node in the graph from
		/// where the edge is starting.</param>
		/// <param name="_end">The transform of the node in the graph to
		/// where the edge is heading.</param>
		void SetCollider(Transform _start, Transform _end);

		/// <summary>
		/// Sets the start and end width of the line renderer
		/// to the values stored in <see cref="defaultStartWidth"/> and
		/// <see cref="defaultEndWidth"/> respectively.
		/// </summary>
		/// <remarks>A line renderer should be made accessible somehow.</remarks>
		void SetLineRendererWidth();

		/// <summary>
		/// Sets the start and end positions of the line renderer.
		/// </summary>
		/// <param name="_start">The transform of the node in the graph from
		/// where the edge is starting.</param>
		/// <param name="_end">The transform of the node in the graph to
		/// where the edge is heading.</param>
		/// <remarks>A line renderer should be made accessible somehow.</remarks>
		void SetLineRendererPosition(Transform _start, Transform _end);
	}

	/// <summary>
	/// The interface defining the required logic to manipulate
	/// the information stored in a <see cref="IGraph"/>.
	/// </summary>
	public interface IGraphGO<EdgeType, NodeType> where EdgeType : IEdge where NodeType : INode
	{
		/// <summary>
		/// The data structure encoding the graph.
		/// </summary>
		IGraph<EdgeType, NodeType> graphData { get; }

		/// <summary>
		/// The list of prefabs that will be used as nodes or edges.
		/// </summary>
		/// <remarks>We decided the use a list of gameobjects rather than
		/// named members for a "node prefab" or an "edge prefab" to allow
		/// for flexibility.</remarks>
		List<GameObject> graphPrefabsComponents { get; }

		/// <summary>
		/// The struct encapsulating the parameters relevant to
		/// controlling the scale of the graph.
		/// </summary>
		GraphScalingData graphScalingData { get; }

		/// <summary>
		/// The dictionnary to find <see cref="INodeGO"/> and <see cref="IEdgeGO"/>
		/// of the graph according to their ids.
		/// </summary>
		Dictionary<uint, GameObject> DataID_to_DataGO { get; set; }

		/// <summary>
		/// Sets the value for <see cref="graphData"/>.
		/// </summary>
		/// <param name="_graphData"></param>
		void SetGraphData(IGraph<EdgeType, NodeType> _graphData);

	}

	/// <summary>
	/// The interface defining the logic to manipulate a graph
	/// synchronized over the network between the host and the 
	/// clients
	/// </summary>
	public interface IGraphGONet<EdgeType, NodeType> : IGraphGO<EdgeType, NodeType> where EdgeType : IEdge where NodeType : INode
	{
		/// <summary>
		/// The struct encapsulating the parameters relevant to
		/// the batch instantiation of the nodes and edges of the graph.
		/// </summary>
		GraphBatchSpawning graphBatchSpawning { get; }

		/// <summary>
		/// Executes the logic on the server side that is needed to 
		/// spawn all the nodes and edges of the graph.
		/// </summary>
		/// <param name="_expeditorClientId">The ID of the client
		/// that made the request.</param>
		/// <param name="_rootSceneId">The ID of the dive scene
		/// where the graph should be generated (i.e. where the
		/// nodes and edges will be spawned.</param>
		[ServerRpc(RequireOwnership = false)]
		void RequestGraphGenerationServerRpc(ulong _expeditorClientId, int _rootSceneId);
	}
}