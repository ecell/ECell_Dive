using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using ECellDive.IO;
using ECellDive.Utility;
using ECellDive.Modules;
using ECellDive.NetworkComponents;

namespace ECellDive
{
    namespace SceneManagement
    {
        /// <summary>
        /// Class derived from <see cref="DiveRoomManager"/>.
        /// Handles the steps of the dive room where FBA is possible.
        /// It keeps track of the knockedout reactions (represented by edges)
        /// and communicates with the server to ask for the update of the FBA.
        /// </summary>
        [RequireComponent(typeof(ServerFBASolver))]
        public class FBADiveRoomManager : DiveRoomManager
        {
            public Utility.SettingsModels.NetworkComponentsReferences networkComponents;
            [HideInInspector] public GameObject LoadedNetwork;
            private NetworkGO LoadedNetworkGO;

            [HideInInspector] public string activeModelName = "";
            [HideInInspector] public Dictionary<string, int> EdgeName_to_EdgeID = new Dictionary<string, int>();
            [HideInInspector] public Dictionary<int, bool> Knockouts = new Dictionary<int, bool>();

            private ServerFBASolver refServerFBASolver;

            public InputActionReference runFBA;

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

            //private void Start()
            //{
            //    refServerFBASolver = GetComponent<ServerFBASolver>();

            //    if (ModulesData.typeActiveModule == ModulesData.ModuleType.NetworkModule &&
            //        NetworkModulesData.activeData != null)
            //    {
            //        //Instantiate the loaded network in the scene based on
            //        //the information retained in the data structures.
            //        LoadedNetwork = NetworkLoader.Generate(NetworkModulesData.activeData,
            //                                               networkComponents.networkGO,
            //                                               networkComponents.layerGO,
            //                                               networkComponents.nodeGO,
            //                                               networkComponents.edgeGO);
            //        LoadedNetworkGO = LoadedNetwork.GetComponent<NetworkGO>();
            //        LoadedNetwork.transform.parent = DiveContainer.transform;
            //        refXRRig.transform.position = Positioning.GetGravityCenter(LoadedNetwork.GetComponent<NetworkGO>().NodeID_to_NodeGO.Values);

            //        //Initial Solve of the Network
            //        activeModelName = LoadedNetworkGO.networkData.name;
            //        foreach (int _id in LoadedNetworkGO.EdgeID_to_EdgeGO.Keys)
            //        {
            //            Knockouts[_id] = false;
            //            EdgeName_to_EdgeID[LoadedNetworkGO.EdgeID_to_EdgeGO[_id].name] = _id;
            //        }
            //    }
            //}

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
            //    Knockouts[edgeGO.edgeData.ID] = edgeGO.knockedOut;
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
                foreach (GameObject _edgeGO in LoadedNetworkGO.EdgeID_to_EdgeGO.Values)
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
                refServerFBASolver.SolveModel(activeModelName, knockoutString);
            }

            /// <summary>
            /// Transfers the results of the FBA (fluxes values) to the shaders
            /// of the edges (representing the reactions).
            /// </summary>
            public void ShowComputedFluxes()
            {
                foreach(string _edgeName in refServerFBASolver.Fluxes.Keys)
                {
                    float level = 0f;
                    if (refServerFBASolver.objectiveValue != 0)
                    {
                        level = 2f * refServerFBASolver.Fluxes[_edgeName] / refServerFBASolver.objectiveValue;
                    }
                    LoadedNetworkGO.EdgeID_to_EdgeGO[EdgeName_to_EdgeID[_edgeName]].GetComponent<EdgeGO>().SetFlux(level, level);
                }
            }

        }
    }
}

