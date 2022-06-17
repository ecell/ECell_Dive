using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using ECellDive.Utility;
using ECellDive.UI;
using ECellDive.Interfaces;

namespace ECellDive
{
    namespace Modules
    {
        public struct FbaAnalysisData
        {
            public string activeModelName;
            public float objectiveValue;
            public Dictionary<string, List<int>> edgeName_to_EdgeID;// 1 name maps to multiple IDs
            public Dictionary<int, bool> knockOuts;
            public Dictionary<string, float> fluxes;
        }

        public class HttpServerFbaModule : HttpServerBaseModule
        {
            private FbaAnalysisData fbaAnalysisData = new FbaAnalysisData
            {
                activeModelName = "",
                objectiveValue = 0f,
                edgeName_to_EdgeID = new Dictionary<string, List<int>>(),
                knockOuts = new Dictionary<int, bool>(),
                fluxes = new Dictionary<string, float>()
            };

            public FbaParametersManager fbaParametersManager;

            private CyJsonModule LoadedCyJsonPathway;

            private void Start()
            {
                if (CyJsonModulesData.activeData == null)
                {
                    LogSystem.refLogManager.AddMessage(LogSystem.MessageTypes.Errors,
                        "There is no active (ie. Dived In) CyJson pathway modules detected.");
                }
                else
                {
                    fbaAnalysisData.activeModelName = CyJsonModulesData.activeData.name;

                    LogSystem.refLogManager.AddMessage(LogSystem.MessageTypes.Trace,
                        $"FBA analyses module connected to {fbaAnalysisData.activeModelName}");

                    LoadedCyJsonPathway = GameObject.FindGameObjectWithTag("CyJsonRootModule").GetComponent<CyJsonModule>();

                    foreach (IEdge _edgeData in LoadedCyJsonPathway.graphData.edges)
                    {
                        fbaAnalysisData.knockOuts[_edgeData.ID] = false;
                        string _name = LoadedCyJsonPathway.DataID_to_DataGO[_edgeData.ID].name;
                        if (fbaAnalysisData.edgeName_to_EdgeID.ContainsKey(_name))
                        {
                            fbaAnalysisData.edgeName_to_EdgeID[_name].Add(_edgeData.ID);
                        }
                        else
                        {
                            fbaAnalysisData.edgeName_to_EdgeID[_name] = new List<int> { _edgeData.ID };
                        }
                    }
                }
            }

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

                foreach (IEdge _edgeData in LoadedCyJsonPathway.graphData.edges)
                {
                    if (LoadedCyJsonPathway.DataID_to_DataGO[_edgeData.ID].GetComponent<EdgeGO>().knockedOut)
                    {
                        knockouts += _edgeData.NAME + ",";
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
                    if (fbaAnalysisData.edgeName_to_EdgeID.ContainsKey(_edgeName))
                    {
                        float level = fbaAnalysisData.fluxes[_edgeName];
                        float levelClamped = Mathf.Clamp(level,
                                                         fbaParametersManager.fluxLowerBoundSlider.slider.value,
                                                         fbaParametersManager.fluxUpperBoundSlider.slider.value);
                        float t = levelClamped / (fbaParametersManager.fluxUpperBoundSlider.slider.value - fbaParametersManager.fluxLowerBoundSlider.slider.value);
                        Color levelColor = Color.Lerp(fbaParametersManager.fluxLowerBoundColorPicker.button.colors.normalColor,
                                                      fbaParametersManager.fluxUpperBoundColorPicker.button.colors.normalColor,
                                                      t);

                        foreach (int _id in fbaAnalysisData.edgeName_to_EdgeID[_edgeName])
                        {
                            LoadedCyJsonPathway.DataID_to_DataGO[_id].GetComponent<EdgeGO>().SetDefaultColor(levelColor);
                            LoadedCyJsonPathway.DataID_to_DataGO[_id].GetComponent<EdgeGO>().SetFlux(level, levelClamped);
                        }
                    }
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

                    float maxFlux = float.MinValue;
                    float minFlux = float.MaxValue;
                    float fluxValue = 0f;
                    foreach (JArray _flux in jFluxesArray)
                    {
                        fluxValue = _flux.ElementAt(1).Value<float>();
                        fbaAnalysisData.fluxes[_flux.ElementAt(0).Value<string>()] = fluxValue;

                        if (fluxValue > maxFlux)
                        {
                            maxFlux = fluxValue;
                        }
                        else if (fluxValue < minFlux)
                        {
                            minFlux = fluxValue;
                        }
                    }

                    fbaParametersManager.SetFluxValueControllersBounds(minFlux, maxFlux);

                    ShowComputedFluxes();
                }
            }
        }
    }
}

