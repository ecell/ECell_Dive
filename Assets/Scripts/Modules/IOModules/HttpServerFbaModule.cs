using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Events;
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
            public Dictionary<string, List<uint>> edgeName_to_EdgeID;// 1 name maps to multiple IDs
            public Dictionary<uint, bool> knockOuts;
            public Dictionary<string, float> fluxes;
        }

        public class HttpServerFbaModule : HttpServerBaseModule
        {
            private FbaAnalysisData fbaAnalysisData = new FbaAnalysisData
            {
                activeModelName = "",
                objectiveValue = 0f,
                edgeName_to_EdgeID = new Dictionary<string, List<uint>>(),
                knockOuts = new Dictionary<uint, bool>(),
                fluxes = new Dictionary<string, float>()
            };

            public FbaParametersManager fbaParametersManager;

            public AnimationLoopWrapper animLW;

            public UnityAction<bool> OnFbaResultsReceive;

            private CyJsonModule LoadedCyJsonPathway;

            private void Start()
            {
                GameObject loadedCyJsonGO = GameObject.FindGameObjectWithTag("CyJsonModule");
                if (loadedCyJsonGO == null)
                {
                    LogSystem.AddMessage(LogMessageTypes.Errors,
                        "There is no active (ie. Dived In) CyJson pathway modules detected.");
                    GetComponentInChildren<ColorFlash>().Flash(0);//fail flash
                }
                else
                {
                    LoadedCyJsonPathway = loadedCyJsonGO.GetComponent<CyJsonModule>();
                    fbaAnalysisData.activeModelName = LoadedCyJsonPathway.graphData.name;

                    LogSystem.AddMessage(LogMessageTypes.Trace,
                        $"FBA analyses module connected to {fbaAnalysisData.activeModelName}");

                    foreach (IEdge _edgeData in LoadedCyJsonPathway.graphData.edges)
                    {
                        fbaAnalysisData.knockOuts[_edgeData.ID] = false;
                        //string _name = LoadedCyJsonPathway.DataID_to_DataGO[_edgeData.ID].name;
                        if (fbaAnalysisData.edgeName_to_EdgeID.ContainsKey(_edgeData.name))
                        {
                            fbaAnalysisData.edgeName_to_EdgeID[_edgeData.name].Add(_edgeData.ID);
                        }
                        else
                        {
                            fbaAnalysisData.edgeName_to_EdgeID[_edgeData.name] = new List<uint> { _edgeData.ID };
                        }
                    }
                }
            }
            
            /// <summary>
            /// Builds and sends the URI of the request that should activate the
            /// computation of the FBA on the server.
            /// </summary>
            /// <param name="_modelName">The name of the model as stored
            /// in the server.</param>
            /// <param name="_knockouts">The string listing the names of
            /// the knockedout reactions. The string can be generated by
            /// the <see cref="GetKnockoutString"/></param>
            public void GetModelSolution(string _modelName, List<string> _knockouts)
            {                
                string requestURL = AddPagesToURL(new string[] { "solve2", _modelName });
                
                int koCount = _knockouts.Count();
                if (koCount > 0)
                {
                    requestURL = AddQueryToURL(requestURL, "command", _knockouts[0], true);

                    for (int i  = 1; i < koCount; i++)
                    {
                        requestURL = AddQueryToURL(requestURL, "command", _knockouts[i]);
                    }
                }
                requestURL = AddQueryToURL(requestURL, "view_name", LoadedCyJsonPathway.graphData.name, koCount == 0);
                
                Debug.Log("FBA query: " + requestURL);
                StartCoroutine(GetRequest(requestURL));
            }

            /// <summary>
            /// Starts the process to ask the server for the FBA of the model.
            /// </summary>
            public void RequestModelSolve()
            {
                animLW.PlayLoop("HttpServerFbaModule");
                if (LoadedCyJsonPathway != null)
                {
                    List<string> knockouts = LoadedCyJsonPathway.GetKnockouts();
                    SolveModel(fbaAnalysisData.activeModelName, knockouts);
                }
                else
                {
                    //stop the "Work In Progress" animation of this module
                    animLW.StopLoop();
                    //Flash of the fail color
                    GetComponentInChildren<ColorFlash>().Flash(0);
                    LogSystem.AddMessage(LogMessageTypes.Errors,
                    "There is no active (ie. Dived In) CyJson pathway modules detected. " +
                    "So, no FBA can be performed.");
                }
            }

            public void ShowComputedFluxes()
            {
                StartCoroutine(ShowComputedFluxesC());
            }

            /// <summary>
            /// Transfers the results of the FBA (fluxes values) to the shaders
            /// of the edges (representing the reactions).
            /// </summary>
            private IEnumerator ShowComputedFluxesC()
            {
                int counter = 0;
                EdgeGO edgeGO;
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
                        
                        foreach (uint _id in fbaAnalysisData.edgeName_to_EdgeID[_edgeName])
                        {
                            edgeGO = LoadedCyJsonPathway.DataID_to_DataGO[_id].GetComponent<EdgeGO>();
                            
                            //control direction of the edge if the flux is reversed
                            if ((edgeGO.fluxLevel.Value < 0 && level > 0) || (edgeGO.fluxLevel.Value > 0 && level < 0))
                            {
                                edgeGO.ReverseOrientation();
                            }
                            edgeGO.SetFlux(level, levelClamped);

                            edgeGO.defaultColor = levelColor;
                            edgeGO.SetCurrentColorToDefaultServerRpc();

                            counter++;

                            if (counter == 25)
                            {
                                yield return new WaitForEndOfFrame();
                                counter = 0;
                            }
                        }
                    }
                }
            }

            /// <summary>
            /// The public interface to ask the server for the FBA.
            /// </summary>
            /// <param name="_modelName">The name of the model as stored
            /// in the server.</param>
            /// <param name="_knockouts">The list the knock out commands.
            /// The list is generated by the <see cref="GetKnockouts"/></param>
            public void SolveModel(string _modelName, List<string> _knockouts)
            {
                StartCoroutine(SolveModelC(_modelName, _knockouts));
            }

            /// <summary>
            /// The coroutine handling the request to the server and the 
            /// parsing of the FBA results if the request was successful.
            /// </summary>
            /// <param name="_modelName">The name of the model as stored
            /// in the server.</param>
            /// <param name="_knockouts">The list the knock out commands.
            /// The list is generated by the <see cref="GetKnockouts"/></param>
            private IEnumerator SolveModelC(string _modelName, List<string> _knockouts)
            {
                GetModelSolution(_modelName, _knockouts);

                yield return new WaitUntil(isRequestProcessed);

                //stop the "Work In Progress" animation of this module
                animLW.StopLoop();

                OnFbaResultsReceive?.Invoke(requestData.requestSuccess);

                if (requestData.requestSuccess)
                {
                    //Flash of the succesful color.
                    GetComponentInChildren<ColorFlash>().Flash(1);

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
                else
                {
                    //Flash of the fail color
                    GetComponentInChildren<ColorFlash>().Flash(0);
                }
            }
        }
    }
}

