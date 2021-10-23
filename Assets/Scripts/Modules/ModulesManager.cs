using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECellDive.Utility;

namespace ECellDive
{
    namespace Modules
    {
        /// <summary>
        /// Manages the instances of the modules in the main room.
        /// </summary>
        public class ModulesManager : MonoBehaviour
        {
            public Utility.SettingsModels.ModulesTypesReferences modulesTypes;
            private List<GameObject> networkModulesGO;

            // Start is called before the first frame update
            void Start()
            {
                networkModulesGO = new List<GameObject>();

                InstantiatesAllModules();
            }

            public void InstantiatesAllModules()
            {
                InstantiateNetworkModules();
            }

            public void RegisterAllModulesPositions()
            {
                RegisterNetworkModulesPositions();
            }

            #region - Network Module-

            /// <summary>
            /// Instantiate all the NetworkModules found in the
            /// NetworkModulesData.loadedData list.
            /// </summary>
            public void InstantiateNetworkModules()
            {
                if (NetworkModulesData.loadedData != null)
                {
                    for (int i = 0; i < NetworkModulesData.loadedData.Count; i++)
                    {
                        InstantiateModule(NetworkModulesData.loadedData[i],
                                          NetworkModulesData.dataPosition[i]);
                    }
                }
            }

            /// <summary>
            /// Instantiates the a Network Module in front of the main Camera.
            /// </summary>
            /// <param name="_network"> A NetworkComponents.Network instance.</param>
            /// <remarks>Overloaded method.</remarks>
            public void InstantiateModule(NetworkComponents.Network _network)
            {
                Vector3 pos = Positioning.PlaceInFrontOfTarget(Camera.main.transform, 2f, 0.8f);
                GameObject networkModuleGO = Instantiate(modulesTypes.networkModule, pos, Quaternion.identity);
                networkModuleGO.SetActive(true);
                NetworkModule networkModule = networkModuleGO.GetComponent<NetworkModule>();
                networkModule.SetName(_network.name);

                networkModule.InstantiateInfoTags(new string[] {$"nb layers: {_network.layers.Length}\n"+
                                                                $"nb edges: {_network.edges.Count}\n"+
                                                                $"nb nodes: {_network.nodes.Count}"});

                networkModule.SetIndex(NetworkModulesData.loadedData.Count - 1);

                networkModulesGO.Add(networkModuleGO);
            }

            /// <summary>
            /// Instantiates the a Network Module in front of the main Camera.
            /// </summary>
            /// <param name="_network"> A NetworkComponents.Network instance.</param>
            /// <param name="_pos"> The position where to instantiate the GameObject.</param>
            /// <remarks>Overloaded method.</remarks>
            public void InstantiateModule(NetworkComponents.Network _network, Vector3 _pos)
            {
                GameObject networkModuleGO = Instantiate(modulesTypes.networkModule, _pos, Quaternion.identity);
                networkModuleGO.SetActive(true);
                NetworkModule networkModule = networkModuleGO.GetComponent<NetworkModule>();
                networkModule.SetName(_network.name);

                networkModule.InstantiateInfoTags(new string[] {$"nb layers: {_network.layers.Length}\n"+
                                                                $"nb edges: {_network.edges.Count}\n"+
                                                                $"nb nodes: {_network.nodes.Count}"});

                networkModule.SetIndex(NetworkModulesData.loadedData.Count - 1);

                networkModulesGO.Add(networkModuleGO);
            }

            /// <summary>
            /// Saves the positions of the current NetworkModules.
            /// </summary>
            public void RegisterNetworkModulesPositions()
            {
                NetworkModulesData.ResetDataPositions();
                foreach (GameObject _networModule in networkModulesGO)
                {
                    NetworkModulesData.RegisterDataPosition(_networModule.transform);
                }
            }

            #endregion
        }
    }
}

