using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using ECellDive.Utility;

namespace ECellDive
{
    namespace Modules
    {
        public struct FbaAnalysisData
        {
            public string activeModelName;
            public float objectiveValue;
            public Dictionary<string, int> edgeName_to_EdgeID;
            public Dictionary<int, bool> knockOuts;
            public Dictionary<string, float> fluxes;
        }

        public class HttpServerFbaAnalysisModule : HttpServerBaseModule
        {
            public InputActionReference runFBA;

            private FbaAnalysisData fbaAnalysisData = new FbaAnalysisData
            {
                activeModelName = "",
                objectiveValue = 0f,
                edgeName_to_EdgeID = new Dictionary<string, int>(),
                knockOuts = new Dictionary<int, bool>(),
                fluxes = new Dictionary<string, float>()
            };

            private NetworkGO LoadedCyJsonRoot;

            private void Awake()
            {
                runFBA.action.performed += RequestModelSolveAction;
            }

            private void OnEnable()
            {
                runFBA.action.Enable();
            }

            private void OnDisable()
            {
                runFBA.action.Disable();
            }

            private void OnDestroy()
            {
                runFBA.action.performed -= RequestModelSolveAction;
            }

            private void Start()
            {
                if (CyJsonModulesData.activeData == null)
                {
                    LogSystem.refLogManager.AddMessage(LogSystem.MessageTypes.Errors,
                        "There is no active (ie. Dived In) CyJson pathway modules detected.");
                }
                else
                {
                    //Initial Solve of the Network
                    fbaAnalysisData.activeModelName = CyJsonModulesData.activeData.name;

                    LogSystem.refLogManager.AddMessage(LogSystem.MessageTypes.Trace,
                        $"FBA analyses module connected to {fbaAnalysisData.activeModelName}");

                    LoadedCyJsonRoot = GameObject.FindGameObjectWithTag("CyJsonRootModule").GetComponent<NetworkGO>();

                    foreach (int _id in LoadedCyJsonRoot.EdgeID_to_EdgeGO.Keys)
                    {
                        fbaAnalysisData.knockOuts[_id] = false;
                        fbaAnalysisData.edgeName_to_EdgeID[LoadedCyJsonRoot.EdgeID_to_EdgeGO[_id].name] = _id;
                    }

                    RequestModelSolve();
                }

                
            }

            /// <summary>
            /// Updates the knockout dictionnary <see cref="Knockouts"/>.
            /// </summary>
            /// <param name="_edgeGO">The gameobject representing the reaction
            /// that may be knockedout.</param>
            /// <remarks>This method is mainly called back as a Unity event
            /// when the user is selecting an Edge.</remarks>
            //public void AccountForModifiedEdge(GameObject _edgeGO)
            //{
            //    EdgeGO edgeGO = _edgeGO.GetComponent<EdgeGO>();
            //    fbaAnalysisData.Knockouts[edgeGO.edgeData.ID] = edgeGO.knockedOut;
            //}

            /// <summary>
            /// Translates the information about knockedout reactions stored
            /// in <see cref="Knockouts"/> as a string usable for a request
            /// to the server.
            /// </summary>
            /// <returns>A string listing the knockedout reactions.</returns>
            public string GetKnockoutString()
            {
                string knockouts = "";
                int counter_true = 0;
                //foreach (int _id in Knockouts.Keys)
                //{
                //    if (Knockouts[_id])
                //    {
                //        knockouts += LoadedNetworkGO.EdgeID_to_EdgeGO[_id].name + ",";
                //        counter_true++;
                //    }
                //}
                foreach (GameObject _edgeGO in LoadedCyJsonRoot.EdgeID_to_EdgeGO.Values)
                {
                    if (_edgeGO.GetComponent<EdgeGO>().knockedOut)
                    {
                        knockouts += _edgeGO.name + ",";
                        counter_true++;
                    }
                }

                if (counter_true > 0)
                {
                    knockouts = knockouts.Substring(0, knockouts.Length - 1);
                }

                return knockouts;
            }

            /// <summary>
            /// Builds and sends the URI of the request that should activate the
            /// computation of the FBA on the server.
            /// </summary>
            /// <param name="_modelName">The name of the model as stored
            /// in the server.</param>
            /// <param name="_knockouts">The string listing the names of
            /// the knockedout reactions. The string can be generated by
            /// the <see cref="FBADiveRoomManager"/></param>
            public void GetModelSolution(string _modelName, string _knockouts)
            {
                string requestURL = AddPagesToURL(new string[] { "solve", _modelName });
                if (_knockouts != "")
                {
                    requestURL = AddQueryToURL(requestURL, "knockouts", _knockouts, true);
                }
                StartCoroutine(GetRequest(requestURL));
            }

            /// <summary>
            /// The interface to use when binding the <see cref="RequestModelSolve"/>
            /// method to an input button
            /// </summary>
            private void RequestModelSolveAction(InputAction.CallbackContext callbackContext)
            {
                RequestModelSolve();
            }

            /// <summary>
            /// Starts the process to ask the server for the FBA of the model.
            /// </summary>
            public void RequestModelSolve()
            {
                string knockoutString = GetKnockoutString();
                SolveModel(fbaAnalysisData.activeModelName, knockoutString);
            }

            /// <summary>
            /// Transfers the results of the FBA (fluxes values) to the shaders
            /// of the edges (representing the reactions).
            /// </summary>
            public void ShowComputedFluxes()
            {
                foreach (string _edgeName in fbaAnalysisData.fluxes.Keys)
                {
                    float level = 0f;
                    if (fbaAnalysisData.objectiveValue != 0)
                    {
                        level = 2f * fbaAnalysisData.fluxes[_edgeName] / fbaAnalysisData.objectiveValue;
                    }
                    LoadedCyJsonRoot.EdgeID_to_EdgeGO[fbaAnalysisData.edgeName_to_EdgeID[_edgeName]].GetComponent<EdgeGO>().SetFlux(level);
                }
            }

            /// <summary>
            /// The public interface to ask the server for the FBA.
            /// </summary>
            /// <param name="_modelName">The name of the model as stored
            /// in the server.</param>
            /// <param name="_knockouts">The string listing the names of
            /// the knockedout reactions. The string can be generated by
            /// the <see cref="FBADiveRoomManager"/></param>
            public void SolveModel(string _modelName, string _knockouts)
            {
                StartCoroutine(SolveModelC(_modelName, _knockouts));
            }

            /// <summary>
            /// The coroutine handling the request to the server and the 
            /// parsing of the FBA results if the request was successful.
            /// </summary>
            /// <param name="_modelName">The name of the model as stored
            /// in the server.</param>
            /// <param name="_knockouts">The string listing the names of
            /// the knockedout reactions. The string can be generated by
            /// the <see cref="FBADiveRoomManager"/></param>
            /// <returns></returns>
            private IEnumerator SolveModelC(string _modelName, string _knockouts)
            {
                GetModelSolution(_modelName, _knockouts);

                yield return new WaitUntil(isRequestProcessed);

                if (requestData.requestSuccess)
                {
                    requestData.requestJObject = JObject.Parse(requestData.requestText);
                    JArray jFluxesArray = (JArray)requestData.requestJObject["fluxes"];
                    fbaAnalysisData.objectiveValue = requestData.requestJObject["objective_value"].Value<float>();

                    foreach (JArray _flux in jFluxesArray)
                    {
                        fbaAnalysisData.fluxes[_flux.ElementAt(0).Value<string>()] = _flux.ElementAt(1).Value<float>();
                    }
                    ShowComputedFluxes();
                }
            }
        }
    }
}

