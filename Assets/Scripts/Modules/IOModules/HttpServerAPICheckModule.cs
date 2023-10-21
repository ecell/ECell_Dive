using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using ECellDive.Interfaces;
using ECellDive.UI;
using ECellDive.Utility;
using ECellDive.Utility.PlayerComponents;

namespace ECellDive.Modules
{
    /// <summary>
    /// The class for checking which API a target server has and which
    /// server action modules can be used in ECellDive as a result.
    /// </summary>
    public class HttpServerAPICheckModule : HttpServerBaseModule
    {
        /// <summary>
		/// The color flash component to alter the visual feedback
		/// of the module in case of request.
		/// </summary>
        [Header("HttpServerAPICheckModule")]//A Header to make the inspector more readable
		[SerializeField] private ColorFlash colorFlash;

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
        }

        private IEnumerator CheckAPIC()
        {
            StartCoroutine(GetRequest(BuildCheckAPIRequest()));

            yield return new WaitUntil(isRequestProcessed);

            //stop the "Work In Progress" animation of this module
            //animLW.StopLoop();

            if (requestData.requestSuccess)
            {
                //Flash of the succesful color.
                colorFlash.Flash(1);

                byte[] modelContent = System.Text.Encoding.UTF8.GetBytes(requestData.requestText);
                List<string> apiCmdNames = requestData.requestJObject["apis"].Select(jv => (string)jv).ToList();

                apiCmdNames.Sort();
                IInteractibility interactibilityManager = StaticReferencer.Instance.refExternalObjectContainer.GetComponent<GUIManager>().refModulesMenuManager;
                for(int i = 0; i < interactibilityManager.targetGroup.Length; i++)
                {
                    bool allAPIImplemented = true;
                    HttpServerBaseModule httpServerBaseModule = interactibilityManager.targetGroup[i].GetComponent<GameObjectConstructor>().refPrefab.GetComponent<HttpServerBaseModule>();

                    if (httpServerBaseModule != null)
                    {
                        foreach (string apiCmdName in interactibilityManager.targetGroup[i].GetComponent<GameObjectConstructor>().refPrefab.GetComponent<HttpServerBaseModule>().implementedHttpAPI)
                        {
                            int idx = apiCmdNames.BinarySearch(apiCmdName);
                            allAPIImplemented &= (idx >= 0 && idx < apiCmdNames.Count);
                        }
                        interactibilityManager.ForceSingleInteractibility(i, allAPIImplemented);
                        Debug.Log($"Module {interactibilityManager.targetGroup[i].GetComponent<GameObjectConstructor>().refPrefab.name} is {(allAPIImplemented ? "" : "not ")}available.");
                    }
                }
            }
            else
            {
                //Flash of the fail color
                colorFlash.Flash(0);
            }
        }
    }

}
