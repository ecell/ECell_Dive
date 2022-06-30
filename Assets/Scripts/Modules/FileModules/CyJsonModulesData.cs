using System.Collections.Generic;
using UnityEngine;
using ECellDive.GraphComponents;

namespace ECellDive
{
    namespace Modules
    {
        /// <summary>
        /// Holds references and methods used to store and manipulate
        /// network modules across all scenes.
        /// </summary>
        public static class CyJsonModulesData
        {
            public static CyJsonPathway activeData { get; set; }
            public static List<CyJsonPathway > loadedData { get; private set; }

            public static void AddData(CyJsonPathway _network)
            {
                if (loadedData == null)
                {
                    loadedData = new List<CyJsonPathway >();
                }
                loadedData.Add(_network);
            }
        }
    }
}

