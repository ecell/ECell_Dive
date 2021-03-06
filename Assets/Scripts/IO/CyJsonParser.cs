using System.IO;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace ECellDive
{
    namespace IO
    {
        public static class CyJsonParser
        {
            /// <summary>
            /// Extract all the nodes as a JArray from the JObject representation
            /// of the Cytoscape Json file.
            /// </summary>
            /// <param name="_networkData">The JObject representation of a Json file</param>
            /// <returns>The nodes of the Cytoscape network as a JArray</returns>
            public static JArray GetNodes(JObject _networkData)
            {
                return (JArray)_networkData["elements"]["nodes"];
            }

            /// <summary>
            /// Extract all the edges as a JArray from the JObject representation
            /// of the Cytoscape Json file.
            /// </summary>
            /// <param name="_networkData">The JObject representation of the Cytoscape Json file</param>
            /// <returns>The edges of a Cytoscape network as a JArray</returns>
            public static JArray GetEdges(JObject _networkData)
            {
                return (JArray)_networkData["elements"]["edges"];
            }

            /// <summary>
            /// Extract and group the nodes according to their respective layers
            /// </summary>
            /// <param name="_nodes">Nodes extracted from a Cytoscape network Json file.</param>
            /// <returns>An IEnumerable of the nodes information. All the nodes of a layer are
            /// accessible at the corresponding index of the IEnumerable (layer1 at index 0,
            /// layer2 at index 1, etc...).</returns>
            //public static IEnumerable<IGrouping<string, JToken>> GroupNodesByLayers(JArray _nodes)
            //{
            //    return from node in _nodes
            //           group node by node["data"]["LAYER_INDEX"].Value<string>() into _nodes_per_layer
            //           orderby _nodes_per_layer.Key descending
            //           select _nodes_per_layer;
            //}

            /// <summary>
            /// Extract and group the edges according to their respective layers
            /// </summary>
            /// <param name="_edges">Edges extracted from a Cytoscape network Json file.</param>
            /// <returns>An IEnumerable of the edges information. All the edges of a layer are
            /// accessible at the corresponding index of the IEnumerable (layer1 at index 0,
            /// layer2 at index 1, etc...).</returns>
            //public static IEnumerable<IGrouping<string, JToken>> GroupEdgesByLayers(JArray _edges)
            //{
            //    return from edge in _edges.ToList()
            //           group edge by edge["data"]["LAYER_INDEX"].Value<string>() into _edges_per_layer
            //           orderby _edges_per_layer.Key descending
            //           select _edges_per_layer;
            //}

            /// <summary>
            /// Extract and group the the json nodes according to a key
            /// </summary>
            /// <param name="_data">A JArray containing json nodes.</param>
            /// <param name="_key">The key to use in the "group by" procedure.</param>
            /// <returns>An IEnumerable of the nodes information. All the nodes are grouped
            /// per their value of the key.</returns>
            public static IEnumerable<IGrouping<string, JToken>> GroupDataByKey(JArray _data, string _key)
            {
                IEnumerable<JToken> validData =  from data in _data.ToList()
                                                where ((JObject)data["data"]).ContainsKey(_key)
                                                select data;
                //Debug.Log(validData.Count());
                //Debug.Log(validData.ElementAt(0));
                return from data in validData.ToList()
                       group data by data["data"][$"{_key}"].Value<string>() into _data_per_key
                       orderby _data_per_key.Key descending
                       select _data_per_key;
            }

            /// <summary>
            /// Looks for either a "SUID" or "id" field
            /// </summary>
            public static int LookForID(JToken _jToken)
            {
                int id = -1;
                JObject jObj = (JObject)_jToken;

                if (jObj.ContainsKey("SUID"))
                {
                    id = jObj["data"]["SUID"].Value<int>();
                }
                else if (jObj.ContainsKey("id"))
                {
                    Debug.Log($"id stored:{jObj["data"]["id"]}");

                    id = jObj["data"]["id"].Value<int>();
                    Debug.Log($"id extracted: {id}");
                }

                return id;
            }

            /// <summary>
            /// Looks for a "label" field in a "data" field in a _JToken.
            /// </summary>
            public static string LookForLabel(JToken _jToken)
            {
                string name = "No Label";
                JObject jObj = (JObject)_jToken;
                if (jObj.ContainsKey("data"))
                {
                    if (((JObject)jObj["data"]).ContainsKey("label"))
                    {
                        name = jObj["data"]["label"].Value<string>();
                    }
                }
                return name;
            }

            /// <summary>
            /// Looks for a "name" field in a "data" field in a _JToken.
            /// </summary>
            public static string LookForName(JToken _jToken)
            {
                string name = "No Name";
                JObject jObj = (JObject)_jToken;
                if (jObj.ContainsKey("data"))
                {
                    if (((JObject)jObj["data"]).ContainsKey("name"))
                    {
                        name = jObj["data"]["name"].Value<string>();
                    }
                }
                return name;
            }

            public static Vector3 LookForNodePosition(JToken _node)
            {
                Vector3 nodePos = Vector3.zero;
                JObject jObjNode = (JObject)_node;
                
                nodePos.x = jObjNode["position"]["x"].Value<float>();
                nodePos.y = jObjNode["position"]["y"].Value<float>();
                nodePos.z = 0f;

                return nodePos;
            }

            public static bool LookForNodeType(JToken _node)
            {
                JObject jObjNode = (JObject)_node;

                return (jObjNode["data"]["node_type"].Value<string>() == "midmarker" ||
                        jObjNode["data"]["node_type"].Value<string>() == "multimarker");
            }
        }
    }
}

