using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using ECellDive.UI;
using ECellDive.Utility;
using ECellDive.Interfaces;
using Newtonsoft.Json.Linq;
using System;

namespace ECellDive.Modules
{
    public class HttpServerInfoQueryModule : HttpServerBaseModule
    {
        public OptimizedVertScrollList refTargetDatabaseScrollList;

        public TMP_Text refTargetDatabase;
        public TMP_InputField refReactionID;

        [SerializeField] private AnimationLoopWrapper animLW;
        [SerializeField] private ColorFlash colorFlash;

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

        private void BuildReactionInfoQuery()
        {
            string requestURL = AddPagesToURL(new string[] { "reaction_information2", LoadedCyJsonPathway.GetName(), refReactionID.text});
            requestURL = AddQueriesToURL(requestURL,
                new string[] { "db_src", "view_name" },
                new string[] { refTargetDatabase.text, LoadedCyJsonPathway.GetName() });

            StartCoroutine(GetRequest(requestURL));
        }

        public void QueryReactionInfo()
        {
            StartCoroutine(QueryReactionInfoC());
            animLW.PlayLoop("HttpServerInfoQueryModule");
        }

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