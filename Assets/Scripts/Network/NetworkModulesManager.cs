using System.Collections.Generic;
using UnityEngine;

namespace ECellDive
{
    namespace Modules
    {
        public static class NetworkModulesManager
        {
            //Network Instances
            public static NetworkComponents.Network activeNetwork { get; set; }
            public static List<NetworkComponents.Network> loadedNetworks { get; private set; }
            //public static InformationPanels informationPanels;

            public static void AddNetwork(NetworkComponents.Network _network)
            {
                if (loadedNetworks == null)
                {
                    loadedNetworks = new List<NetworkComponents.Network>();
                }
                loadedNetworks.Add(_network);
            }
        }
    }
}

