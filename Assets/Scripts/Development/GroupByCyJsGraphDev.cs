using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using ECellDive.Modules;
using ECellDive.Interfaces;
using ECellDive.IO;
using ECellDive.UI;


namespace ECellDive.CustomEditors
{
    /// <summary>
    /// The class to import a CyJson file and generate the associated
    /// graph from the editor ouside of runtime.
    /// </summary>
    public class GroupByCyJsGraphDev : MonoBehaviour
    {
        public CyJsonModule refCyJsonPathwayGO;
        public string edgesGroupAttribute;
        public string nodesGroupAttribute;

        public GroupData[] edgesGroups;
        public GroupData[] nodesGroups;

        public void ApplyGroupColors(GroupData[] _groups)
        {
            foreach(GroupData _group in _groups)
            {
                foreach (IColorHighlightable _highlightable in _group.members)
                {
                    _highlightable.defaultColor = _group.color;
                    _highlightable.ApplyColor(_group.color);
                }
            }
        }

        public void CreateGroupEdges()
        {
            IEnumerable<IGrouping<string, JToken>> groups = CyJsonParser.GroupDataByKey(refCyJsonPathwayGO.graphData.jEdges, edgesGroupAttribute);
            if (groups == null)
            {
                Debug.LogError("Failed to group data by " + edgesGroupAttribute);
            }
            else
            {
                Debug.Log("<color=green>Succesfully</color> grouped data by " + edgesGroupAttribute + $". {groups.Count()} were found.");
                int nbGroups = groups.Count();
                edgesGroups = new GroupData[nbGroups];
                for (int i = 0; i < nbGroups; i++)
                {
                    int nbMembers = groups.ElementAt(i).Count();
                    IColorHighlightable[] groupMembers = new IColorHighlightable[nbMembers];

                    Color grpColor = Random.ColorHSV();

                    //Retrieving group member ids
                    for (int j = 0; j < nbMembers; j++)
                    {
                        groupMembers[j] = refCyJsonPathwayGO.
                                                    DataID_to_DataGO[groups.ElementAt(i).ElementAt(j)["data"]["id"].Value<uint>()].
                                                    GetComponent<IColorHighlightable>();
                        groupMembers[j].defaultColor = grpColor;
                        groupMembers[j].ApplyColor(grpColor);
                    }

                    //Adding group information
                    edgesGroups[i] = new GroupData
                        {
                            value = groups.ElementAt(i).Key,
                            color = grpColor,
                            members = groupMembers
                        };
                }
            }
        }

        public void CreateGroupNodes()
        {
            IEnumerable<IGrouping<string, JToken>> groups = CyJsonParser.GroupDataByKey(refCyJsonPathwayGO.graphData.jNodes, nodesGroupAttribute);
            if (groups == null)
            {
                Debug.LogError("Failed to group data by " + nodesGroupAttribute);
            }
            else
            {
                Debug.Log("<color=green>Succesfully</color> grouped data by " + nodesGroupAttribute + $". {groups.Count()} were found.");
                int nbGroups = groups.Count();
                nodesGroups = new GroupData[nbGroups];
                for (int i = 0; i < nbGroups; i++)
                {
                    int nbMembers = groups.ElementAt(i).Count();
                    IColorHighlightable[] groupMembers = new IColorHighlightable[nbMembers];

                    Color grpColor = Random.ColorHSV();

                    //Retrieving group member ids
                    for (int j = 0; j < nbMembers; j++)
                    {
                        groupMembers[j] = refCyJsonPathwayGO.
                                                    DataID_to_DataGO[groups.ElementAt(i).ElementAt(j)["data"]["id"].Value<uint>()].
                                                    GetComponent<IColorHighlightable>();
                        groupMembers[j].defaultColor = grpColor;
                        groupMembers[j].ApplyColor(grpColor);
                    }

                    //Adding group information
                    nodesGroups[i] = new GroupData
                    {
                        value = groups.ElementAt(i).Key,
                        color = grpColor,
                        members = groupMembers
                    };
                }
            }
        }

        public void ReuseDefinedEdgeGroups()
        {
            IEnumerable<IGrouping<string, JToken>> groups = CyJsonParser.GroupDataByKey(refCyJsonPathwayGO.graphData.jEdges, edgesGroupAttribute);
            if (groups == null)
            {
                Debug.LogError("Failed to group data by " + edgesGroupAttribute);
            }
            else
            {
                Debug.Log("<color=green>Succesfully</color> grouped data by " + edgesGroupAttribute + $". {groups.Count()} were found.");
                int nbGroups = groups.Count();
                for (int i = 0; i < nbGroups; i++)
                {
                    int nbMembers = groups.ElementAt(i).Count();
                    IColorHighlightable[] groupMembers = new IColorHighlightable[nbMembers];

                    //Retrieving group member ids
                    for (int j = 0; j < nbMembers; j++)
                    {
                        groupMembers[j] = refCyJsonPathwayGO.
                                                    DataID_to_DataGO[groups.ElementAt(i).ElementAt(j)["data"]["id"].Value<uint>()].
                                                    GetComponent<IColorHighlightable>();
                        groupMembers[j].defaultColor = edgesGroups[i].color;
                        groupMembers[j].ApplyColor(edgesGroups[i].color);
                    }

                    //Adding group memebers information
                    edgesGroups[i].members = groupMembers;
                }
            }
        }

        public void ReuseDefinedNodesGroups()
        {
            IEnumerable<IGrouping<string, JToken>> groups = CyJsonParser.GroupDataByKey(refCyJsonPathwayGO.graphData.jNodes, nodesGroupAttribute);
            if (groups == null)
            {
                Debug.LogError("Failed to group data by " + nodesGroupAttribute);
            }
            else
            {
                Debug.Log("<color=green>Succesfully</color> grouped data by " + nodesGroupAttribute + $". {groups.Count()} were found.");
                int nbGroups = groups.Count();
                for (int i = 0; i < nbGroups; i++)
                {
                    int nbMembers = groups.ElementAt(i).Count();
                    IColorHighlightable[] groupMembers = new IColorHighlightable[nbMembers];

                    //Retrieving group member ids
                    for (int j = 0; j < nbMembers; j++)
                    {
                        groupMembers[j] = refCyJsonPathwayGO.
                                                    DataID_to_DataGO[groups.ElementAt(i).ElementAt(j)["data"]["id"].Value<uint>()].
                                                    GetComponent<IColorHighlightable>();
                        groupMembers[j].defaultColor = nodesGroups[i].color;
                        groupMembers[j].ApplyColor(nodesGroups[i].color);
                    }

                    //Adding group memebers information
                    nodesGroups[i].members = groupMembers;
                }
            }
        }



    }
}