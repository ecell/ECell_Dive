using Newtonsoft.Json.Linq;
using UnityEngine;
using ECellDive.GraphComponents;


namespace ECellDive
{
    namespace IO
    {
        public static class CyJsonPathwayLoader
        {
            /// <summary>
            /// Instantiate a <see cref="CyJsonPathway"/> data structure
            /// from the Cystoscape network Json file.
            /// </summary>
            /// <param name="_path">Path of the Cytoscape network Json file</param>
            /// <param name="_name">Name of the Cytoscape network Json file</param>
            /// <returns></returns>
            public static CyJsonPathway Initiate(string _path, string _name)
            {
                return new CyJsonPathway(_path, _name);
            }

            /// <summary>
            /// Instantiate a Network data structure from the content of a Cystoscape
            /// network Json file.
            /// </summary>
            /// <param name="_CyJspathway">Content of the Cytoscape network Json file
            /// serialized as a JObject</param>
            /// <param name="_name">Name of the Cytoscape network Json file</param>
            /// <returns></returns>
            public static CyJsonPathway Initiate(JObject _CyJspathway, string _name)
            {
                return new CyJsonPathway(_CyJspathway, _name);
            }

            /// <summary>
            /// Parse the Cytoscape network Json file and fill in the info about the
            /// layers, nodes and edges.
            /// </summary>
            /// <param name="_refNetwork">The network object previously instantiated
            /// for the Initiate function.</param>
            public static void Populate(CyJsonPathway _cyJsonPathway)
            {
                //Get raw nodes data from the cyjson file
                _cyJsonPathway.SetNodes();

                //Get raw edges data from the cyjson file
                _cyJsonPathway.SetEdges();

                //Organize edges and nodes information
                _cyJsonPathway.PopulateNodes();
                _cyJsonPathway.PopulateEdges();

                //Maps the node-edge continuity
                _cyJsonPathway.MapInOutEdgesIntoNodes();
            }
        }
    }
}

