using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using ECellDive.IO;
using ECellDive.INetworkComponents;

namespace ECellDive
{
    namespace NetworkComponents
    {
        public class Network : INetwork
        {
            public string name { get; protected set; }
            public JObject networkData { get;}
            public JArray nodes { get; protected set; }
            public JArray edges { get; protected set; }
            public ILayer[] layers { get; protected set; }
            public IInterLayer[] interLayers { get; protected set; }

            public Network(string _path, string _name)
            {
                networkData = JsonImporter.OpenFile(_path, _name);
                name = _name;
            }

            public void SetNodes()
            {
                nodes = CyJsonParser.GetNodes(networkData);
            }

            public void SetEdges()
            {
                edges = CyJsonParser.GetEdges(networkData);
            }

            public void PopulateLayers()
            {
                JObject _firstNode = (JObject)nodes[0]["data"];
                IEnumerable<IEnumerable<JToken>> nodesPerLayer;
                IEnumerable<IEnumerable<JToken>> edgesPerLayer;

                if (_firstNode.ContainsKey("LAYER_INDEX"))
                {
                    nodesPerLayer = CyJsonParser.GroupNodesByLayers(nodes);
                    edgesPerLayer = CyJsonParser.GroupEdgesByLayers(edges);
                }
                else
                {
                    nodesPerLayer = new[] { nodes };
                    edgesPerLayer = new[] { edges };
                }

                layers = new ILayer[nodesPerLayer.Count()];
                
                for (int i = 0; i < nodesPerLayer.Count(); i++)
                {
                    layers[i] = new Layer(nodesPerLayer.Count() - i - 1, nodesPerLayer.ElementAt(i), edgesPerLayer.ElementAt(i));
                    layers[i].PopulateNodes();
                    layers[i].PopulateEdges();
                }
            }

            public void PopulateInterLayers()
            {

            }
        }
    }
}
