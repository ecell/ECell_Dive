using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Newtonsoft.Json.Linq;

namespace ECellDive
{
    namespace Interfaces
    {
        /// <summary>
        /// The interface for the data structure encoding a node in a graph.
        /// </summary>
        public interface INode
        {
            uint ID { get; set; }
            Vector3 position { get; set; }
            string name { get; set; }
            string label { get; set; }
            List<uint> incommingEdges { get; set; }
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
        public interface IEdge
        {
            uint ID { get; set; }
            uint source { get; set; }
            uint target { get; set; }
            string name { get; set; }

            string reaction_name { get; set; }
        }

        /// <summary>
        /// The interface for the data structure encoding a graph
        /// made of nodes connected by edges.
        /// </summary>
        public interface IGraph
        {
            string name { get; }
            JObject graphData { get;}
            JArray jNodes { get; }
            JArray jEdges { get;}

            INode[] nodes { get; }
            IEdge[] edges { get; }

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
        /// The interface encoding the required properties to use
        /// the an edge (<seealso cref="INode"/>) of a graph
        /// (<seealso cref="IGraph"/>) in a GameObject.
        /// </summary>
        public interface INodeGO
        {
            INode nodeData { get; }

            /// <summary>
            /// The string containing information to be displayed about 
            /// the node.
            /// </summary>
            string informationString { get; }

            void SetNodeData(INode _INode);
        }

        /// <summary>
        /// The interface encoding the required properties to use
        /// the an edge (<seealso cref="IEdge"/>) of a graph
        /// (<seealso cref="IGraph"/>) in a GameObject.
        /// </summary>
        public interface IEdgeGO
        {
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
            /// The LineRenderer representing the edge.
            /// </summary>
            LineRenderer refLineRenderer { get; }

            /// <summary>
            /// The game object containing a box collider to be placed
            /// dynamically around the edge so that the user can interact with it.
            /// </summary>
            GameObject refBoxColliderHolder { get; }

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
            /// Sets the start and end width of the <see cref="refLineRenderer"/> 
            /// to the values stored in <see cref="defaultStartWidth"/> and
            /// <see cref="defaultEndWidth"/> respectively.
            /// </summary>
            void SetLineRendererWidth();

            /// <summary>
            /// Sets the start and end positions of the <see cref="refLineRenderer"/>.
            /// </summary>
            /// <param name="_start">The transform of the node in the graph from
            /// where the edge is starting.</param>
            /// <param name="_end">The transform of the node in the graph to
            /// where the edge is heading.</param>
            void SetLineRendererPosition(Transform _start, Transform _end);
        }

        /// <summary>
        /// The interface encoding the required properties to use
        /// the a graph (<seealso cref="IGraph"/>) in a GameObject.
        /// </summary>
        public interface IGraphGO
        {
            IGraph graphData { get; }

            /// <summary>
            /// The list of prefabs that will be used as nodes or edges.
            /// </summary>
            /// <remarks>We decided the use a list of gameobjects rather than
            /// named members for a "node prefab" or an "edge prefab" to allow
            /// for flexibility.</remarks>
            List<GameObject> graphPrefabsComponents { get; }

            [ServerRpc(RequireOwnership = false)]
            void RequestGraphGenerationServerRpc(ulong _expeditorClientId, int _rootSceneId);

            void SetNetworkData(IGraph _INetwork);

        }
    }
}