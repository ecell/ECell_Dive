using Newtonsoft.Json.Linq;
using UnityEngine;
using ECellDive.Interfaces;

namespace ECellDive.CustomEditors
{
    public class GraphDataSerializer : ScriptableObject
    {
        public JObject graphData;
        public JArray jNodes;
        public JArray jEdges;

        [SerializeReference] public INode[] nodes;
        [SerializeReference] public IEdge[] edges;

        public void Initialize(IGraph _graph)
        {
            name = _graph.name;
            graphData = _graph.graphData;
            jNodes = _graph.jNodes;
            jEdges = _graph.jEdges;
            Debug.Log(_graph.nodes.Length);
            nodes = _graph.nodes;
            edges = _graph.edges;
        }
    }
}

