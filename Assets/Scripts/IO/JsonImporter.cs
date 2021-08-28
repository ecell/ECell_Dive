using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ECellDive
{
    namespace IO
    {
        public static class JsonImporter
        {
            /// <summary>
            /// Uses Newtonsoft JSon.Net to deserialize the Cytoscape network
            /// Json file.
            /// </summary>
            /// <param name="_path">Path of the Cytoscape network Json file</param>
            /// <param name="_name">Name of the Cytoscape network Json file</param>
            /// <returns>The content of the Cytoscape network Json file as a JObject.</returns>
            public static JObject OpenFile(string _path, string _name)
            {
                StreamReader streamReader = File.OpenText(_path + "/" + _name);

                JObject jObject = (JObject)JToken.ReadFrom(new JsonTextReader(streamReader));

                streamReader.Close();

                return jObject;
            }
        }
    }
}
