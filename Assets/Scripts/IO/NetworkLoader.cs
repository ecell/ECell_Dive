using Newtonsoft.Json.Linq;
using UnityEngine;
using ECellDive.INetworkComponents;
using ECellDive.NetworkComponents;


namespace ECellDive
{
    namespace IO
    {
        public static class NetworkLoader
        {
            /// <summary>
            /// Instantiate a <see cref="Network"/> data structure
            /// from the Cystoscape network Json file.
            /// </summary>
            /// <param name="_path">Path of the Cytoscape network Json file</param>
            /// <param name="_name">Name of the Cytoscape network Json file</param>
            /// <returns></returns>
            public static NetworkComponents.Network Initiate(string _path, string _name)
            {
                return new NetworkComponents.Network(_path, _name);
            }

            /// <summary>
            /// Instantiate a Network data structure from the content of a Cystoscape
            /// network Json file.
            /// </summary>
            /// <param name="_networkCyJs">Content of the Cytoscape network Json file
            /// serialized as a JObject</param>
            /// <param name="_name">Name of the Cytoscape network Json file</param>
            /// <returns></returns>
            public static NetworkComponents.Network Initiate(JObject _networkCyJs, string _name)
            {
                return new NetworkComponents.Network(_networkCyJs, _name);
            }

            /// <summary>
            /// Parse the Cytoscape network Json file and fill in the info about the
            /// layers, nodes and edges.
            /// </summary>
            /// <param name="_refNetwork">The network object previously instantiated
            /// for the Initiate function.</param>
            public static void Populate( NetworkComponents.Network _refNetwork)
            {
                //get all nodes
                _refNetwork.SetNodes();
                
                //get all edges
                _refNetwork.SetEdges();

                //Organize layers, edges and nodes information
                _refNetwork.PopulateLayers();
            }
        }
    }
}

