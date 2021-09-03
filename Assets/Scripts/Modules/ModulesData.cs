using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ECellDive
{
    namespace Modules
    {
        /// <summary>
        /// Holds references and methods used to keep track of general
        /// information about the modules that have been loaded in all
        /// scenes.
        /// </summary>
        public static class ModulesData
        {
            public enum ModuleType { NetworkModule }
            public static ModuleType typeActiveModule = ModuleType.NetworkModule;
        }
    }
}

