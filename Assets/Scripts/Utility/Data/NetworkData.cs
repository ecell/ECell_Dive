using Newtonsoft.Json.Linq;
using TMPro;

namespace ECellDive.Utility.Data.Network
{
    /// <summary>
    /// A simple data structure to store information about a request.
    /// The output is a JObject (see Newtonsoft.Json.Linq.JObject).
    /// </summary>
    public struct RequestData
    {
        /// <summary>
        /// The string of the request.
        /// </summary>
        public string requestText;

        /// <summary>
        /// The result of the request.
        /// </summary>
        public JObject requestJObject;

        /// <summary>
        /// A boolean to check if the request has been processed.
        /// </summary>
        public bool requestProcessed;

        /// <summary>
        /// A boolean to check if the request was successful.
        /// </summary>
        public bool requestSuccess;
    }

    /// <summary>
    /// A simple data structure to store the server address and port.
    /// </summary>
    /// <remarks>
    /// Goes in pair with <see cref="ServerUIData"/>.
    /// </remarks>
    [System.Serializable]
    public struct ServerData
    {
        /// <summary>
        /// The name of the server.
        /// </summary>
        public string name;

        /// <summary>
        /// The IPv4 address of the server.
        /// </summary>
        public string serverIP;

        /// <summary>
        /// The port of the server.
        /// </summary>
        public string port;
    }

    /// <summary>
    /// A simple data structure to store two UI input fields
    /// representing the server's address and port.
    /// </summary>
    /// <remarks>
    /// Goes in pair with <see cref="ServerData"/>.
    /// </remarks>
    [System.Serializable]
    public struct ServerUIData
    {
        /// <summary>
        /// The input field representing the server's name.
        /// Goes in pair with <see cref="ServerData.name"/>.
        /// </summary>
        public TMP_InputField refNameInputField;

        /// <summary>
        /// The input field representing the server's address.
        /// Goes in pair with <see cref="ServerData.serverIP"/>.
        /// </summary>
        public TMP_InputField refIPInputField;

        /// <summary>
        /// The input field representing the server's port.
        /// Goes in pair with <see cref="ServerData.port"/>.
        /// </summary>
        public TMP_InputField refPortInputField;
    }
}
