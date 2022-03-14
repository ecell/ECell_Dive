using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using ECellDive.IO;
using ECellDive.UI;

namespace ECellDive
{
    namespace Modules
    {
        public class GroupByModule : Module
        {
            [Header("UI")]
            public GroupByAttributsManager refAttributsManager;

            private Renderer refRenderer;
            private MaterialPropertyBlock mpb;
            private int colorID;

            private NetworkGO refNetworkGO;
            //List<int> attributsToDataIndexMap = new List<int>();
            List<JArray> data = new List<JArray>();

            private void Start()
            {
                refNetworkGO = FindObjectOfType<NetworkGO>();
                AddData("-- Nodes --", refNetworkGO.networkData.jNodes, (JObject)refNetworkGO.networkData.jNodes[0]["data"]);
                AddData("-- Edges --", refNetworkGO.networkData.jEdges, (JObject)refNetworkGO.networkData.jEdges[0]["data"]);
            }

            private void OnEnable()
            {
                refRenderer = GetComponentInChildren<Renderer>();
                mpb = new MaterialPropertyBlock();
                colorID = Shader.PropertyToID("_Color");
                mpb.SetVector(colorID, defaultColor);
                refRenderer.SetPropertyBlock(mpb);
            }

            private void AddData(string _dataName, JArray _data, JObject _dataNodeSample)
            {
                data.Add(_data);
                refAttributsManager.AddDataUI(_dataName, _dataNodeSample);
            }

            public void Execute(int _dataID, string _attribute)
            {
                IEnumerable<IGrouping<string, JToken>> groups;

                Debug.Log($"Trying to group {_dataID} by " + _attribute+".");
                groups = CyJsonParser.GroupDataByKey(data[_dataID], _attribute);
                
                Debug.Log($"Grouping data {_dataID} by " + _attribute + $" yielded {groups.Count()} groups.");
                
                if (_dataID == 0)
                {
                    ApplyGroupingOnNodes(groups);
                }
                else
                {
                    ApplyGroupingOnEdges(groups);
                }
            }

            private void ApplyGroupingOnEdges(IEnumerable<IGrouping<string, JToken>> _groups)
            {
                foreach (IGrouping<string, JToken> _group in _groups)
                {
                    Color rdColor = Random.ColorHSV();
                    foreach (JToken jToken in _group)
                    {
                        GameObject _go = refNetworkGO.EdgeID_to_EdgeGO[jToken["data"]["id"].Value<int>()];
                        _go.GetComponent<EdgeGO>().defaultColor = rdColor;
                        _go.GetComponent<EdgeGO>().UnsetHighlight();
                    }
                }
            }

            private void ApplyGroupingOnNodes(IEnumerable<IGrouping<string, JToken>> _groups)
            {
                foreach (IGrouping<string, JToken> _group in _groups)
                {
                    Color rdColor = Random.ColorHSV();
                    foreach (JToken jToken in _group)
                    {
                        GameObject _go = refNetworkGO.NodeID_to_NodeGO[jToken["data"]["id"].Value<int>()];
                        _go.GetComponent<NodeGO>().defaultColor = rdColor;
                        _go.GetComponent<NodeGO>().UnsetHighlight();
                    }
                }
            }

            #region - IHighlightable -
            public override void SetHighlight()
            {
                mpb.SetVector(colorID, highlightColor);
                refRenderer.SetPropertyBlock(mpb);
            }

            public override void UnsetHighlight()
            {
                mpb.SetVector(colorID, defaultColor);
                refRenderer.SetPropertyBlock(mpb);
            }
            #endregion
        }
    }
}

