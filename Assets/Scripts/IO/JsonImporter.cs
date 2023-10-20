using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ECellDive.IO
{
    /// <summary>
    /// Static class containing methods to import Json files.
    /// </summary>
    public static class JsonImporter
    {
        /// <summary>
        /// Uses Newtonsoft JSon.Net to deserialize a Json file.
        /// </summary>
        /// <param name="_path">Path to the Json file</param>
        /// <param name="_name">Name of the Json file</param>
        /// <returns>The content of the Json file as a JObject.</returns>
        public static JObject OpenFile(string _path, string _name)
        {
            StreamReader streamReader = File.OpenText(_path + "/" + _name);

            JObject jObject = (JObject)JToken.ReadFrom(new JsonTextReader(streamReader));

            streamReader.Close();

            return jObject;
        }
    }
}
