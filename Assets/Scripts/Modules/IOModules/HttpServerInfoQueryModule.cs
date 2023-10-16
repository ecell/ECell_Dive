using System.Collections;
using TMPro;
using UnityEngine;
using ECellDive.UI;
using ECellDive.Utility;
using Newtonsoft.Json.Linq;
using System;

namespace ECellDive.Modules
{
	/// <summary>
	/// A module to request the Kosmogora-like server to get additional
	/// information about a reaction on reachable databases.
	/// </summary>
	public class HttpServerInfoQueryModule : HttpServerBaseModule
	{
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
		/// Sets the text value of the database to use for the query.
		/// </summary>
		/// <remarks>Used as callback from the editor.</remarks>
		/// <param name="dataBaseButtonLabel"></param>
		public void SetTargetDatabase(TextMeshProUGUI dataBaseButtonLabel)
		{
			refTargetDatabase.text = dataBaseButtonLabel.text;
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
	}
}