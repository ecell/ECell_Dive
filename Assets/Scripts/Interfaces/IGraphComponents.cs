using Newtonsoft.Json.Linq;
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
		/// Position in the 3D space of the node
		/// </summary>
		Vector3 position { get; set; }

		/// <summary>
		/// The name of the node.
		/// </summary>
		string name { get; set; }

		/// <summary>
		/// A string to store additional textual information about the 
		/// node.
		/// </summary>
		/// <remarks>
		/// In CyJson graphs, the user-readable name for nodes is
		/// actually encoded the Label and while the Name is shorter
		/// and less explicit.
		/// </remarks>
		string label { get; set; }

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
		/// A utility state variable to describe whether the node is
		/// simply there to structure the network or if it's a node
		/// representing important data for the user.
		/// </summary>
		bool isVirtual { get; set; }
	}

	/// <summary>
	/// The interface for the data structure encoding an edge in a graph.
	/// </summary>
	/// <remarks>
	/// This interface is probably overdefined with cyjson graphs in mind.
	/// For example, the <see cref="reaction_name"/> field is too specific.
	/// </remarks>
	public interface IEdge
	{
		/// <summary>
		/// A unique ID.
		/// </summary>
		uint ID { get; set; }

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

		/// <summary>
		/// The name of the edge.
		/// </summary>
		string name { get; set; }

		/// <summary>
		/// The name of the reaction associated with the edge.
		/// </summary>
		/// <remarks>
		/// This is a field that is specific to the Cytoscape Json format.
		/// This should be moved to a more specific data structure.
		/// </remarks>
		string reaction_name { get; set; }
	}

	/// <summary>
	/// The interface for the data structure encoding a graph
	/// made of nodes connected by edges.
	/// </summary>
	public interface IGraph
	{
		/// <summary>
		/// The name of the graph.
		/// </summary>
		string name { get; }

		/// <summary>
		/// The JObject extracted from the Json file that encodes the graph.
		/// </summary>
		JObject graphData { get;}

		/// <summary>
		/// The JArray (from Newtonsoft Json Linq) representing the nodes in <see cref="graphData"/>.
		/// </summary>
		JArray jNodes { get; }

		/// <summary>
		/// The JArray (from Newtonsoft Json Linq) representing the edges in <see cref="graphData"/>.
		/// </summary>
		JArray jEdges { get;}

		/// <summary>
		/// The array of <see cref="INode"/> translating the nodes stored in <see cref="jNodes"/>.
		/// </summary>
		INode[] nodes { get; }

		/// <summary>
		/// The array of <see cref="IEdge"/> translating the edges stored in <see cref="jEdges"/>.
		/// </summary>
		IEdge[] edges { get; }

		/// <summary>
		/// Uses the information stored in the <see cref="edges"/> to fill the 
		/// <see cref="INode.incommingEdges"/> and <see cref="INode.outgoingEdges"/> lists.
		/// </summary>
		void MapInOutEdgesIntoNodes();

		/// <summary>
		/// Creates the <see cref="nodes"/> array mirroring the information
		/// stored in <see cref="jNodes"/> but in a more accessible way.
		/// </summary>
		void PopulateNodes();

		/// <summary>
		/// Creates the <see cref="edges"/> array mirroring the information
		/// stored in <see cref="jNodes"/> but in a more accessible way.
		/// </summary>
		void PopulateEdges();

		/// <summary>
		/// Sets the <see cref="jNodes"/>.
		/// </summary>
		void SetNodes();

		/// <summary>
		/// Sets the <see cref="jEdges"/>.
		/// </summary>
		void SetEdges();

	}

	/// <summary>
	/// The interface defining the required logic to manipulate
	/// the information stored in a <see cref="INode"/> of a
	/// <see cref="IGraph"/>.
	/// </summary>
	public interface INodeGO
	{
		/// <summary>
		/// The data structure encoding the node.
		/// </summary>
		INode nodeData { get; }

		/// <summary>
		/// The string containing information to be displayed about 
		/// the node.
		/// </summary>
		string informationString { get; }

		/// <summary>
		/// Sets the value for <see cref="nodeData"/>.
		/// </summary>
		/// <param name="_INode">
		/// The value to pass on to <see cref="nodeData"/>.
		/// </param>
		void SetNodeData(INode _INode);
	}

	/// <summary>
	/// The interface defining the required logic to manipulate
	/// the information stored in a <see cref="IEdge"/> of a
	/// <see cref="IGraph"/>.
	/// </summary>
	public interface IEdgeGO
	{
		/// <summary>
		/// The data structure encoding the edge.
		/// </summary>
		IEdge edgeData { get; }

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
		/// <param name="_IEdge">The value to pass on to <see cref="edgeData"/>.</param>
		void SetEdgeData(IEdge _IEdge);

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
	public interface IGraphGO
	{
		/// <summary>
		/// The data structure encoding the graph.
		/// </summary>
		IGraph graphData { get; }

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

		/// <summary>
		/// Sets the value for <see cref="graphData"/>.
		/// </summary>
		/// <param name="_INetwork"></param>
		void SetNetworkData(IGraph _INetwork);

	}

	/// <summary>
	/// The interface defining the logic to manipulate a graph
	/// synchronized over the network between the host and the 
	/// clients
	/// </summary>
	public interface IGraphGONet : IGraphGO
	{
		/// <summary>
		/// The struct encapsulating the parameters relevant to
		/// the batch instantiation of the nodes and edges of the graph.
		/// </summary>
		GraphBatchSpawning graphBatchSpawning { get; }
	}
}