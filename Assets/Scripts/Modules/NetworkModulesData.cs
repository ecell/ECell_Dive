using System.Collections.Generic;
using UnityEngine;

namespace ECellDive
{
    namespace Modules
    {
        /// <summary>
        /// Holds references and methods used to store and manipulate
        /// network modules across all scenes.
        /// </summary>
        public static class NetworkModulesData
        {
            public static NetworkComponents.Network activeData { get; set; }
            public static List<NetworkComponents.Network> loadedData { get; private set; }
            
            public static List<Vector3> dataPosition { get; private set; }

            public static void AddData(NetworkComponents.Network _network)
            {
                if (loadedData == null)
                {
                    loadedData = new List<NetworkComponents.Network>();
                }
                loadedData.Add(_network);
            }

            public static void RegisterDataPosition(Transform _networkModule)
            {
                if (dataPosition == null)
                {
                    dataPosition = new List<Vector3>();
                }
                dataPosition.Add(_networkModule.position);
            }

            public static void ResetDataPositions()
            {
                if (dataPosition != null)
                {
                    dataPosition.Clear();
                }
            }
        }
    }
}

