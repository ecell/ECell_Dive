using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECellDive.GraphComponents;
using ECellDive.Utility;

namespace ECellDive
{
    namespace Modules
    {
        /// <summary>
        /// Manages the instances of the modules in the main room.
        /// </summary>

        [System.Obsolete]
        public class ModulesManager : MonoBehaviour
        {
            //public Utility.SettingsModels.ModulesTypesReferences modulesTypes;
            //private List<GameObject> networkModulesGO;

            //Start is called before the first frame update
            //void Start()
            //{
            //    networkModulesGO = new List<GameObject>();

            //    InstantiatesAllModules();
            //}

            //public void InstantiatesAllModules()
            //{
            //    InstantiateNetworkModules();
            //}

            //public void RegisterAllModulesPositions()
            //{
            //    RegisterNetworkModulesPositions();
            //}

            //#region - Network Module -

            /// <summary>
            /// Instantiate all the NetworkModules found in the
            /// NetworkModulesData.loadedData list.
            /// </summary>
            //public void InstantiateNetworkModules()
            //{
            //    if (CyJsonModulesData.loadedData != null)
            //    {
            //        for (int i = 0; i < CyJsonModulesData.loadedData.Count; i++)
            //        {
            //            InstantiateModule(CyJsonModulesData.loadedData[i],
            //                              CyJsonModulesData.dataPosition[i]);
            //        }
            //    }
            //}

            /// <summary>
            /// Instantiates the a Network Module in front of the main Camera.
            /// </summary>
            /// <param name = "_network" > A CyJsonPathway instance.</param>
            /// <remarks>Overloaded method.</remarks>
            //public void InstantiateModule(CyJsonPathway _cyJsonPathway)
            //{
            //    Vector3 pos = Positioning.PlaceInFrontOfTarget(Camera.main.transform, 2f, 0.8f);
            //    GameObject cyJsonPathwayModuleGO = Instantiate(cyJsonPathwayGO, pos, Quaternion.identity);
            //    cyJsonPathwayModuleGO.SetActive(true);
            //    NetworkModule networkModule = networkModuleGO.GetComponent<NetworkModule>();
            //    networkModule.SetName(_network.name);

            //    networkModule.InstantiateInfoTags(new string[] {$"nb layers: {_network.layers.Length}\n"+
            //                                                    $"nb edges: {_network.edges.Count}\n"+
            //                                                    $"nb nodes: {_network.nodes.Count}"});

            //    networkModule.SetIndex(NetworkModulesData.loadedData.Count - 1);

            //    networkModulesGO.Add(networkModuleGO);
            //}

            /// <summary>
            /// Instantiates the a Network Module in front of the main Camera.
            /// </summary>
            /// <param name = "_cyJsonPathway" > A CyJsonPathway instance.</param>
            /// <param name = "_pos" > The position where to instantiate the GameObject.</param>
            /// <remarks>Overloaded method.</remarks>
            //public void InstantiateModule(CyJsonPathway _cyJsonPathway, Vector3 _pos)
            //{
            //    GameObject cyJsonPathwayModuleGO = Instantiate(modulesTypes.cyJsonPathwayModule, _pos, Quaternion.identity);
            //    cyJsonPathwayModuleGO.SetActive(true);
            //    NetworkModule networkModule = networkModuleGO.GetComponent<NetworkModule>();
            //    networkModule.SetName(_network.name);

            //    networkModule.InstantiateInfoTags(new string[] {$"nb layers: {_network.layers.Length}\n"+
            //                                                    $"nb edges: {_network.edges.Count}\n"+
            //                                                    $"nb nodes: {_network.nodes.Count}"});

            //    networkModule.SetIndex(NetworkModulesData.loadedData.Count - 1);

            //    networkModulesGO.Add(networkModuleGO);
            //}

            /// <summary>
            /// Saves the positions of the current NetworkModules.
            /// </summary>
            //public void RegisterNetworkModulesPositions()
            //{
            //    CyJsonModulesData.ResetDataPositions();
            //    foreach (GameObject _networModule in networkModulesGO)
            //    {
            //        CyJsonModulesData.RegisterDataPosition(_networModule.transform);
            //    }
            //}
        }
    }
}

