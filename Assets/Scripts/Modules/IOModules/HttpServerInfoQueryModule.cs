using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using TMPro;

using ECellDive.UI;
using ECellDive.Utility;
using ECellDive.IO;
using ECellDive.Utility.Data.Network;

namespace ECellDive.Modules
{
	/// <summary>
	/// A module to request the Kosmogora-like server to get additional
	/// information about a reaction on reachable databases.
	/// </summary>
	public class HttpServerInfoQueryModule : HttpServerBaseModule
	{
		/// <summary>
		/// The scroll list of the available servers this module can use.
		/// </summary>
		[Header("HttpServerInfoQueryModule")]//A Header to make the inspector more readable
		public OptimizedVertScrollList refAvailableServersScrollList;

		/// <summary>
		/// The reference to the text mesh displaying the name of the server
		/// this module is using.
		/// </summary>
		public TMP_Text refTargetServer;

		/// <summary>
		/// The scroll list of the databases we can try to query.
		/// </summary>
		public OptimizedVertScrollList refTargetDatabaseScrollList;

		/// <summary>
		/// The test mesh displaying the database to use for the query.
		/// </summary>
		public TMP_Text refTargetDatabase;

		/// <summary>
		/// The input field for the reaction ID to query.
		/// </summary>
		/// <remarks>
		/// TODO: This is a temporary solution that only works for pathways.
		/// </remarks>
		public TMP_InputField refReactionID;

		/// <summary>
		/// The animation loop controller to control the visual feedback
		/// of the module in case of request.
		/// </summary>
		[SerializeField] private AnimationLoopWrapper animLW;

		/// <summary>
		/// The color flash component to alter the visual feedback
		/// of the module in case of request.
		/// </summary>
		[SerializeField] private ColorFlash colorFlash;

		/// <summary>
		/// A buffer to store the cyjson pathway that is currently loaded
		/// in the dive scene.
		/// </summary>
		/// <remarks>
		/// TODO: This is a temporary solution that only works for pathways.
		/// </remarks>
		private CyJsonModule LoadedCyJsonPathway;

		private void Start()
		{
			GameObject loadedCyJsonGO = GameObject.FindGameObjectWithTag("CyJsonModule");
			if (loadedCyJsonGO == null)
			{
				LogSystem.AddMessage(LogMessageTypes.Errors,
					"[HttpServerInfoQueryModule] There is no active metabolic pathway (CyJson) module detected.");
				colorFlash.Flash(0);//fail flash
			}
			else
			{
				LoadedCyJsonPathway = loadedCyJsonGO.GetComponent<CyJsonModule>();

				LogSystem.AddMessage(LogMessageTypes.Trace,
					$"[HttpServerInfoQueryModule] Metabolic pathway (CyJson) module detected {LoadedCyJsonPathway.GetName()}");
			}
		}

		/// <summary>
		/// The utility function to build the query to the server.
		/// Also sends the request after building it.
		/// </summary>
		private void BuildReactionInfoQuery()
		{
			string requestURL = AddPagesToURL(new string[] { "reaction_information", LoadedCyJsonPathway.GetName(), refReactionID.text});
			requestURL = AddQueriesToURL(requestURL,
				new string[] { "db_src", "view_name" },
				new string[] { refTargetDatabase.text, LoadedCyJsonPathway.GetName() });

			StartCoroutine(GetRequest(requestURL));
		}

		/// <inheritdoc/>
		protected override List<ServerData> GetAvailableServers()
		{
			return HttpNetPortal.Instance.GetModuleServers("HttpServerInfoQueryModule");
		}

		/// <summary>
		/// The public interface to ask the server for the additional
		/// information.
		/// </summary>
		public void QueryReactionInfo()
		{
			StartCoroutine(QueryReactionInfoC());
			animLW.PlayLoop("HttpServerInfoQueryModule");
		}

		/// <summary>
		/// The coroutine handling the request to the server and the
		/// processing of the response.
		/// </summary>
		private IEnumerator QueryReactionInfoC()
		{
			BuildReactionInfoQuery();

			yield return new WaitUntil(isRequestProcessed);

			//stop the "Work In Progress" animation of this module
			animLW.StopLoop();

			if (requestData.requestSuccess)
			{

				requestData.requestJObject = JObject.Parse(requestData.requestText);
				string reactionString = requestData.requestJObject["reaction_information"]["REACTION"].Value<string>();

				if (string.IsNullOrEmpty(reactionString))
				{
					colorFlash.Flash(0);//fail flash
					LogSystem.AddMessage(LogMessageTypes.Errors,
										 $"[HttpServerInfoQueryModule] Request succeeded but target information is unavailable : " +
											 requestData.requestJObject["detail"].Value<string>());
				}
				else
				{
					colorFlash.Flash(1);//success flash
					EdgeGO edge = LoadedCyJsonPathway.DataID_to_DataGO[Convert.ToUInt16(refReactionID.text)].GetComponent<EdgeGO>();
					edge.InstantiateInfoTag(new Vector2(0.15f, 0.15f), reactionString);
				}
			}
			else
			{
				//Flash of the fail color
				colorFlash.Flash(0);
				LogSystem.AddMessage(LogMessageTypes.Errors,
									   $"[HttpServerInfoQueryModule] Request failed with error: {requestData.requestText}");
			}
		}

		/// <summary>
		/// Sets the <see cref="serverData"/> to the server selected by retrieving
		/// the server data based in the index of the button representing the server
		/// in the <see cref="refAvailableServersScrollList"/>.
		/// </summary>
		/// <param name="_serverButtonGO">
		/// The gameobject encapsulating the button representing the server
		/// in the <see cref="refAvailableServersScrollList"/>.
		/// </param>
		/// <remarks>
		/// Used as callback from the editor.
		/// </remarks>
		public void SetTargetServer(GameObject _serverButtonGO)
		{
			List<ServerData> availableServers = GetAvailableServers();
			serverData = availableServers[ _serverButtonGO.transform.GetSiblingIndex()];
			refTargetServer.text = serverData.name;
		}

		/// <summary>
		/// Sets the text value of the database to use for the query.
		/// </summary>
		/// <remarks>Used as callback from the editor.</remarks>
		/// <param name="dataBaseButtonLabel">
		/// The text mesh of the button representing the database in
		/// <see cref="refTargetDatabaseScrollList"/>.
		/// </param>
		public void SetTargetDatabase(TextMeshProUGUI dataBaseButtonLabel)
		{
			refTargetDatabase.text = dataBaseButtonLabel.text;
		}

		/// <summary>
		/// The public interface to populate the scroll list <see cref="refAvailableServersScrollList"/>
		/// of available servers for this module.
		/// </summary>
		/// <remarks>
		/// Used as callback from the editor.
		/// </remarks>
		public void UpdateAvailableServers()
		{
			List<ServerData> availableServers = GetAvailableServers();

			refAvailableServersScrollList.ClearScrollList();
			foreach(ServerData server in availableServers)
			{
				GameObject serverUIContainer = refAvailableServersScrollList.AddItem();
				serverUIContainer.GetComponentInChildren<TextMeshProUGUI>().text = server.name + "\n<size=0.025>" + server.serverIP + ":" + server.port+ "</size>";
				serverUIContainer.SetActive(true);
			}
			refAvailableServersScrollList.UpdateScrollList();
		}
	}
}