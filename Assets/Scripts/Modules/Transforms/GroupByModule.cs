using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using ECellDive.Interfaces;
using ECellDive.IO;
using ECellDive.UI;
using ECellDive.Utility;
using ECellDive.Utility.Data.UI;
using ECellDive.Utility.PlayerComponents;

namespace ECellDive.Modules
{
	/// <summary>
	/// The code implementing the "Group By" operation.
	/// </summary>
	public class GroupByModule : Module
	{
		[Header("UI")]
		public GroupByAttributsManager refAttributsManager;

		private CyJsonModule refCyJsonPathwayGO;
		List<JArray> data = new List<JArray>();

		private void Start()
		{
			//For now (2022-03-17) we are only looking for a NetworkGO spawned from a cyjson file.
			refCyJsonPathwayGO = FindObjectOfType<CyJsonModule>();
			if (refCyJsonPathwayGO != null)
			{
				AddData("-- Nodes --", refCyJsonPathwayGO.GetJsonGraphData().nodesData, (JObject)refCyJsonPathwayGO.GetJsonGraphData().nodesData[0]["data"]);
				AddData("-- Edges --", refCyJsonPathwayGO.GetJsonGraphData().edgesData, (JObject)refCyJsonPathwayGO.GetJsonGraphData().edgesData[0]["data"]);
			}
			else
			{
				LogSystem.AddMessage(LogMessageTypes.Errors,
					"The GroupBy Module could not find any data to link to.");
			}
		}

		/// <summary>
		/// Adds a reference to a JArray from which we might want to extract groups later.
		/// Also calls for the update of the GUI to allow user to chose which groups he wants
		/// to make.
		/// </summary>
		/// <param name="_dataName">Name of the data we want to display in the GUI.</param>
		/// <param name="_data">The JArray encapsulating the data we might want to parse later.</param>
		/// <param name="_dataNodeSample">A representative node of what's in <paramref name="_data"/>
		/// (understand with the same keys) used to extract the keys the user will
		/// be able to use to query for groups.</param>
		private void AddData(string _dataName, JArray _data, JObject _dataNodeSample)
		{
			data.Add(_data);
			refAttributsManager.AddDataUI(_dataName, _dataNodeSample);
		}

		/// <summary>
		/// Tries to group data represented by <paramref name="_dataID"/> according to
		/// the key field encoded in <paramref name="_attribute"/>.
		/// </summary>
		/// <param name="_dataID">The index of the JArray in <see cref="data"/> from which
		/// we are trying to extract groups.</param>
		/// <param name="_attribute">The name of the field in the JArray of interest in 
		/// <see cref="data"/> that we want to use to make groups.</param>
		public void Execute(int _dataID, string _attribute)
		{
				
			IEnumerable<IGrouping<string, JToken>> groups = CyJsonParser.GroupDataByKey(data[_dataID], _attribute);
			if (groups == null)
			{
				LogSystem.AddMessage(LogMessageTypes.Errors,
					"Failed to group data by " + _attribute);
			}
			else
			{
				LogSystem.AddMessage(LogMessageTypes.Trace,
					"Succesfully groupe data by " + _attribute+ $". {groups.Count()} were found.");

				List<GroupData> groupsData = new List<GroupData>();
				int nbGroups = groups.Count();
				int groupsCounter = 0;
				foreach (IGrouping<string, JToken> _group in groups)
				{
					IHighlightable[] groupMembers = new IHighlightable[_group.Count()];
					int memberCounter = 0;

					//Retrieving group member ids
					foreach (JToken _member in _group)
					{
						groupMembers[memberCounter] = refCyJsonPathwayGO.
													DataID_to_DataGO[_member["data"]["id"].Value<uint>()].
													GetComponent<IHighlightable>();
						//Debug.Log(groupMembers[counter]);
						memberCounter++;
					}

					//Adding group information
					groupsData.Add(new GroupData
					{
						value = _group.Key,
						color = Color.HSVToRGB((float)groupsCounter / nbGroups, 1f, 1f),
						members = groupMembers
					}
					);
					groupsCounter++;

				}

				StartCoroutine(StaticReferencer.Instance.refGroupsMenu.AddSemanticTermUI(_attribute, groupsData));
			}
		}
	}
}

