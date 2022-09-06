using System.Linq;
using System.Collections;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using ECellDive.Utility;


namespace ECellDive
{
    namespace Modules
    {
        [System.Serializable]
        public struct ServerData
        {
            public string serverIP;
            public string port;
        }

        [System.Serializable]
        public struct ServerUIData
        {
            public TMP_InputField refIPInputField;
            public TMP_InputField refPortInputField;
        }

        public struct RequestData
        {
            public string requestText;
            public JObject requestJObject;

            public bool requestProcessed;
            public bool requestSuccess;
        }

        public class HttpServerBaseModule : Module
        {
            public ServerData serverData = new ServerData
            {
                port = "8000",
                serverIP = "127.0.0.1"
            };

            public ServerUIData serverUIData;

            protected RequestData requestData = new RequestData
            {
                requestText = "",
                requestJObject = new JObject(),
                requestProcessed = true,
                requestSuccess = true
            };

            private Renderer refRenderer;
            private MaterialPropertyBlock mpb;
            private int colorID;

            private void OnEnable()
            {
                refRenderer = GetComponentInChildren<Renderer>();
                mpb = new MaterialPropertyBlock();
                colorID = Shader.PropertyToID("_Color");
                mpb.SetVector(colorID, defaultColor);
                refRenderer.SetPropertyBlock(mpb);
            }

            /// <summary>
            /// Simple API to add pages to the base server url.
            /// </summary>
            /// <param name="_pages">The name of successive pages.</param>
            /// <returns>The server address plus the concanated pages.</returns>
            protected string AddPagesToURL(string[] _pages)
            {
                string url = "http://" + serverData.serverIP + ":" + serverData.port;
                foreach (string _page in _pages)
                {
                    url += "/" + _page;
                }
                return url;
            }

            /// <summary>
            /// Simple API to add queries to a base URL.
            /// </summary>
            /// <param name="_baseURL">The base URL. It typically already
            /// contains the server address and the pages path.</param>
            /// <param name="_queryNames">The array storing the names
            /// of the query to add.</param>
            /// <param name="_queryContents">The array storing the parameters
            /// associated to each query.</param>
            /// <returns>The <paramref name="_baseURL"/> plus all concanated
            /// queries.</returns>
            /// <remarks><paramref name="_queryContents"/> and <paramref name="_queryNames"/>
            /// must be of same size since there is a one to one relationship
            /// in the elements stored at each index.</remarks>
            protected string AddQueriesToURL(string _baseURL, string[] _queryNames, string[] _queryContents)
            {
                string url = AddQueryToURL(_baseURL, _queryNames[0], _queryContents[0], true);

                if (_queryNames.Length > 1)
                {
                    for (int i = 1; i < _queryNames.Length; i++)
                    {
                        url = AddQueryToURL(url, _queryNames[i], _queryContents[i]);
                    }
                }

                return url;
            }

            /// <summary>
            /// Simple API to add one query to a base URL.
            /// </summary>
            /// <param name="_baseURL">he base URL. It typically already
            /// contains the server address, the pages path and possibly
            /// other queries.</param>
            /// <param name="_queryName">The name of the query to add</param>
            /// <param name="_queryContent">The parameter to associate to
            /// the query.</param>
            /// <param name="_first">A boolean to indicate if this is the
            /// first query being added to <paramref name="_baseURL"/>.</param>
            /// <returns>The <paramref name="_baseURL"/> plus the concanated
            /// query.</returns>
            protected string AddQueryToURL(string _baseURL, string _queryName, string _queryContent, bool _first = false)
            {
                return _first ?
                        _baseURL + "?" + _queryName + "=" + _queryContent :
                        _baseURL + "&" + _queryName + "=" + _queryContent;
            }

            /// <summary>
            /// Send a request to the server.
            /// </summary>
            /// <param name="uri">The request uri.</param>
            /// <returns></returns>
            protected IEnumerator GetRequest(string uri)
            {
                requestData.requestProcessed = false;
                requestData.requestSuccess = false;
                using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
                {
                    LogSystem.AddMessage(LogMessageTypes.Debug,
                                                   "Sending Request: " + uri);
                    // Request and wait for the desired page.
                    yield return webRequest.SendWebRequest();

                    string[] pages = uri.Split('/');
                    int page = pages.Length - 1;

                    switch (webRequest.result)
                    {
                        case UnityWebRequest.Result.ConnectionError:
                            LogSystem.AddMessage(LogMessageTypes.Errors,
                                                               pages[page] + ": Connection Error: " + webRequest.error);
                            break;

                        case UnityWebRequest.Result.DataProcessingError:
                            //Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                            LogSystem.AddMessage(LogMessageTypes.Errors,
                                                               pages[page] + ": Error: " + webRequest.error);
                            break;
                        case UnityWebRequest.Result.ProtocolError:
                            //Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                            LogSystem.AddMessage(LogMessageTypes.Errors,
                                                               pages[page] + ": HTTP Error: " + webRequest.error);
                            break;
                        case UnityWebRequest.Result.Success:
                            //Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                            LogSystem.AddMessage(LogMessageTypes.Trace,
                                                               pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                            requestData.requestText = webRequest.downloadHandler.text;
                            requestData.requestSuccess = true;
                            break;
                    }
                    requestData.requestProcessed = true;
                }
            }

            /// <summary>
            /// Typically used by the coroutines to wait until the
            /// request sent to the servers has been processed.
            /// </summary>
            /// <returns><see cref="RequestData.requestProcessed"/></returns>
            protected bool isRequestProcessed()
            {
                return requestData.requestProcessed;
            }

            /// <summary>
            /// Sets the value for the IP in <see cref="HttpServerBaseModule.serverData"/>
            /// </summary>
            /// <remarks>
            /// Called back on value change of the input field dedicated to the IP
            /// </remarks>
            public void UpdateIP()
            {
                serverData.serverIP = serverUIData.refIPInputField.text;
            }

            /// <summary>
            /// Sets the value for the Port in <see cref="HttpServerBaseModule.serverData"/>
            /// </summary>
            /// <remarks>
            /// Called back on value change of the input field dedicated to the Port
            /// </remarks>
            public void UpdatePort()
            {
                serverData.port = serverUIData.refPortInputField.text;
            }

            #region - IHighlightable -
            public override void SetHighlight()
            {
                mpb.SetVector(colorID, highlightColor);
                refRenderer.SetPropertyBlock(mpb);
            }

            public override void UnsetHighlight()
            {
                if (!forceHighlight)
                {
                    mpb.SetVector(colorID, defaultColor);
                    refRenderer.SetPropertyBlock(mpb);
                }
            }
            #endregion
        }
    }
}
