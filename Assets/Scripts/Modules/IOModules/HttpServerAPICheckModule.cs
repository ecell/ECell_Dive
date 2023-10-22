using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

using ECellDive.Interfaces;
using ECellDive.IO;
using ECellDive.UI;
using ECellDive.Utility;
using ECellDive.Utility.PlayerComponents;
using ECellDive.Utility.Data.Network;

namespace ECellDive.Modules
{
	/// <summary>
	/// The class for checking which API a target server has and which
	/// server action modules can be used in ECellDive as a result.
	/// </summary>
	public class HttpServerAPICheckModule : HttpServerBaseModule
	{
		/// <summary>
		/// The data structure storing the input fields for the server address and port.
		/// </summary>
		[Header("HttpServerAPICheckModule")]//A Header to make the inspector more readable
		public ServerUIData serverUIData;

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
		/// The method to build the request URL for checking the API
		/// </summary>
		/// <returns>
		/// The request URL for checking the API
		/// </returns>
		private string BuildCheckAPIRequest()
		{
			return AddPagesToURL(new string[] { "apis" });
		}

		/// <summary>
		/// The public interface to request the list of implemented API
		/// from the server and update which server action modules can
		/// be used in ECellDive as a result.
		/// </summary>
		public void CheckAPI()
		{
			StartCoroutine(CheckAPIC());
			animLW.PlayLoop("HttpServerAPICheckModule");
		}

		/// <summary>
		/// The coroutine to request the list of implemented API
		/// from the server and update which server action modules can
		/// be used in ECellDive as a result.
		/// </summary>
		private IEnumerator CheckAPIC()
		{
			StartCoroutine(GetRequest(BuildCheckAPIRequest()));

			yield return new WaitUntil(isRequestProcessed);

			//stop the "Work In Progress" animation of this module
			animLW.StopLoop();

			if (requestData.requestSuccess)
			{
				//Flash of the succesful color.
				colorFlash.Flash(1);

				requestData.requestJObject = JObject.Parse(requestData.requestText);
				List<string> apiCmdNames = requestData.requestJObject["apis"].Select(jv => (string)jv).ToList();

				apiCmdNames.Sort();

				//Get the interactibility manager of the modules menu
				IInteractibility interactibilityManager = StaticReferencer.Instance.refExternalObjectContainer.GetComponent<GUIManager>().refModulesMenuManager;
				for(int i = 0; i < interactibilityManager.targetGroup.Length; i++)
				{
					//Get the HttpServerBaseModule component of the module.
					//This is to get the list of implemented API of the module.
					HttpServerBaseModule httpServerBaseModule = interactibilityManager.targetGroup[i].GetComponent<GameObjectConstructor>().refPrefab.GetComponent<HttpServerBaseModule>();

					bool allAPIImplemented = true;
					if (httpServerBaseModule != null)
					{
						foreach (string apiCmdName in httpServerBaseModule.implementedHttpAPI)
						{
							int idx = apiCmdNames.BinarySearch(apiCmdName);
							allAPIImplemented &= (idx >= 0 && idx < apiCmdNames.Count);
						}

						//If the button is not interactable yet, update its interactibility.
						if (!interactibilityManager.targetGroup[i].interactable)
						{
							//Update the interactibility of the button to spawn the module.
							interactibilityManager.ForceSingleInteractibility(i, allAPIImplemented);
						}

						//If all the API are implemented, add the server to the list of servers the 
						//module can be used with.
						if (allAPIImplemented)
						{
							HttpNetPortal.Instance.AddModuleServer(httpServerBaseModule.name, serverData);
						}
					}
				}
			}
			else
			{
				//Flash of the fail color
				colorFlash.Flash(0);
			}
		}

        /// <summary>
        /// Sets the value for the IP in <see cref="serverData"/>.
        /// </summary>
        /// <remarks>
        /// Called back on value change of the input field dedicated to the IP.
        /// </remarks>
        public void UpdateIP()
        {
            serverData.serverIP = serverUIData.refIPInputField.text;
        }

        /// <summary>
        /// Sets the value for the Port in <see cref="serverData"/>.
        /// </summary>
        /// <remarks>
        /// Called back on value change of the input field dedicated to the Port.
        /// </remarks>
        public void UpdatePort()
        {
            serverData.port = serverUIData.refPortInputField.text;
        }

        /// <summary>
        /// Sets the value for the Name in <see cref="serverData"/>.
        /// </summary>
        /// <remarks>
        /// Called back on value change of the input field dedicated to the Name.
        /// </remarks>
        public void UpdateName()
        {
            serverData.name = serverUIData.refNameInputField.text;
        }
    }

}
