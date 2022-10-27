using Newtonsoft.Json.Linq;
using UnityEngine;
using ECellDive.Interfaces;

namespace ECellDive.CustomEditors
{
    /// <summary>
    /// A Scriptable object to generate assets (i.e. serialize) data on 
    /// of a graph during development time.
    /// Usefull when we want to save the results of a metabolic network
    /// (CyJson file) to load and at run time.
    /// </summary>
    public class GraphDataSerializer : ScriptableObject
    {
        public JObject graphData;
        public JArray jNodes;
        public JArray jEdges;

        [SerializeReference] public INode[] nodes;
        [SerializeReference] public IEdge[] edges;

        /// <summary>
        /// Used to initialize the field values of the instance.
        /// </summary>
        /// <param name="_graph">The original graph data we want
        /// to serialize.</param>
        public void Initialize(IGraph _graph)
        {
            name = _graph.name;
            graphData = _graph.graphData;
            jNodes = _graph.jNodes;
            jEdges = _graph.jEdges;
            nodes = _graph.nodes;
            edges = _graph.edges;
        }
    }
}

