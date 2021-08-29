using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECellDive.IO;
using ECellDive.Utility;
using ECellDive.NetworkComponents;

namespace ECellDive
{
    namespace Modules
    {
        /// <summary>
        /// Loads the content of the module in the Diving room.
        /// </summary>
        public class ModuleLoader : MonoBehaviour
        {
            public Utility.SettingsModels.NetworkComponentsReferences networkComponents;

            public GameObject LoadedNetwork;

            void Start()
            {
                //Instantiate the loaded network in the scene based on
                //the information retained in the data structures.
                LoadedNetwork = NetworkLoader.Generate(NetworkModulesManager.activeNetwork,
                                                       networkComponents.networkGO,
                                                       networkComponents.layerGO,
                                                       networkComponents.nodeGO,
                                                       networkComponents.edgeGO);
            }
        }
    }
}

