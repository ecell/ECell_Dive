using System.Collections.Generic;

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
            public static CyJsonModule activeData { get; set; }

            public static List<CyJsonModule> m_loadedData = new List<CyJsonModule>();
            public static List<CyJsonModule> loadedData {
                get => m_loadedData;
                private set => m_loadedData = value;
            }

            public static void AddData(CyJsonModule _network)
            {
                loadedData.Add(_network);
            }
        }
    }
}

