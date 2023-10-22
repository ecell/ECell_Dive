using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ECellDive.Utility.Data.Network;

namespace ECellDive.IO
{
    /// <summary>
    /// The class to store the list of servers that can be used
    /// by each module in ECellDive. It MUST be present in the 
    /// scene for the HttpModules to work.
    /// </summary>
	public class HttpNetPortal : MonoBehaviour
	{
        /// <summary>
        /// The singleton instance of the HttpNetPortal.
        /// </summary>
        [HideInInspector] public static HttpNetPortal Instance;

        /// <summary>
        /// The dictionary storing the list of servers that can be used
        /// for each module in ECellDive. The key is the name of the module.
        /// </summary>
		private Dictionary<string, List<ServerData>> modulesServers = new Dictionary<string, List<ServerData>>();

        private void Start()
        {
            Instance = this;
        }

        /// <summary>
        /// Returns the list of servers that can be used by a module.
        /// </summary>
        /// <param name="_moduleName">
        /// The name of the module.
        /// </param>
        /// <returns>
        /// The list of servers that can be used by the module.
        /// </returns>
        public List<ServerData> GetModuleServers(string _moduleName)
		{
            if (modulesServers.ContainsKey(_moduleName))
			{
                return modulesServers[_moduleName];
            }
            return new List<ServerData>();
        }

        /// <summary>
        /// Adds a server to the list of servers that can be used by a module.
        /// Adds a new entry if the module is not already in <see cref="modulesServers"/>
        /// </summary>
        /// <param name="_moduleName">
        /// The name of the module.
        /// </param>
        /// <param name="_serverData">
        /// The server that can be used by the module.
        /// </param>
		public void AddModuleServer(string _moduleName, ServerData _serverData)
		{
            if (!modulesServers.ContainsKey(_moduleName))
			{
                modulesServers.Add(_moduleName, new List<ServerData>());
            }
			modulesServers[_moduleName].Add(_serverData);
        }

	}
}
