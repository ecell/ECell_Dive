using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using ECellDive.Interfaces;
using ECellDive.IO;
using ECellDive.SceneManagement;
using ECellDive.UI;
using ECellDive.Utility;

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
            //private GroupsMenu refGroupsMenu;
            //List<int> attributsToDataIndexMap = new List<int>();
            List<JArray> data = new List<JArray>();

            private void Start()
            {
                refNetworkGO = FindObjectOfType<NetworkGO>();
                //refGroupsMenu = FindObjectOfType<GroupsMenu>();
                //Debug.Log(refGroupsMenu);
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
                
                IEnumerable<IGrouping<string, JToken>> groups = CyJsonParser.GroupDataByKey(data[_dataID], _attribute);
                if (groups == null)
                {
                    LogSystem.refLogManager.AddMessage(LogSystem.MessageTypes.Errors,
                        "Failed to group data by " + _attribute);
                }
                else
                {
                    LogSystem.refLogManager.AddMessage(LogSystem.MessageTypes.Trace,
                        "Succesfully groupe data by " + _attribute+ $". {groups.Count()} were found.");
                    
                    //SemanticGroupData semanticGroupData = new SemanticGroupData
                    //{
                    //    name = _attribute,
                    //};

                    List<GroupData> groupsData = new List<GroupData>();
                    foreach (IGrouping<string, JToken> _group in groups)
                    {
                        IHighlightable[] groupMembers = new IHighlightable[_group.Count()];
                        int counter = 0;

                        //Retrieving group member ids
                        foreach (JToken _member in _group)
                        {
                            groupMembers[counter] = refNetworkGO.
                                                        DataID_to_DataGO[_member["data"]["id"].Value<int>()].
                                                        GetComponent<IHighlightable>();
                            //Debug.Log(groupMembers[counter]);
                            counter++;
                        }

                        //Adding group information
                        groupsData.Add(new GroupData
                        {
                            value = _group.Key,
                            color = Random.ColorHSV(),
                            members = groupMembers
                        }
                        );
                    }

                    GroupsManagement.refGroupsMenu.AddSemanticTermUI(_attribute, groupsData);
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

