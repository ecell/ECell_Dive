#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEditor;
using ECellDive.Modules;
using ECellDive.Interfaces;
using ECellDive.IO;
using ECellDive.Utility.Data.UI;


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
					uint[] groupMemberIds = new uint[nbMembers];

					Color grpColor = Random.ColorHSV();

					//Retrieving group member ids
					for (int j = 0; j < nbMembers; j++)
					{
						groupMemberIds[j] = groups.ElementAt(i).ElementAt(j)["data"]["id"].Value<uint>();
						groupMembers[j] = refCyJsonPathwayGO.DataID_to_DataGO[groupMemberIds[j]].GetComponent<IColorHighlightable>();
						groupMembers[j].defaultColor = grpColor;
						groupMembers[j].ApplyColor(grpColor);
					}

					//Adding group information
					edgesGroups[i] = new GroupData
						{
							value = groups.ElementAt(i).Key,
							color = grpColor,
							members = groupMembers,
							membersIds = groupMemberIds
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
					uint[] groupMemberIds = new uint[nbMembers];

					Color grpColor = Random.ColorHSV();

					//Retrieving group member ids
					for (int j = 0; j < nbMembers; j++)
					{
						groupMemberIds[j] = groups.ElementAt(i).ElementAt(j)["data"]["id"].Value<uint>();
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
						members = groupMembers,
						membersIds = groupMemberIds
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

		public void SaveEdgesAndNodesColors()
		{
			ColorDataSerializer colorDataSerializer = ScriptableObject.CreateInstance<ColorDataSerializer>();

			int count = 0;
			foreach (GroupData groupData in edgesGroups)
			{
				count += groupData.members.Length;
			}

			foreach (GroupData groupData in nodesGroups)
			{
				count += groupData.members.Length;
			}

			colorDataSerializer.data = new ColorData[count];

			int offset = 0;
			foreach (GroupData groupData in edgesGroups)
			{
				for (int i = 0; i < groupData.membersIds.Length; i++)
				{
					colorDataSerializer.data[offset + i] = new ColorData
					{
						color = groupData.color,
						targetGoID = groupData.membersIds[i]
					};
				}
				offset += groupData.membersIds.Length;
			}

			foreach (GroupData groupData in nodesGroups)
			{
				for (int i = 0; i < groupData.membersIds.Length; i++)
				{
					colorDataSerializer.data[offset + i] = new ColorData
					{
						color = groupData.color,
						targetGoID = groupData.membersIds[i]
					};
				}
				offset += groupData.membersIds.Length;
			}

			AssetDatabase.CreateAsset(colorDataSerializer, "Assets/Resources/Prefabs/Modules/Demo_iJO1366/" + refCyJsonPathwayGO.name + "_ColorData.asset");
		}
	}
}

#endif