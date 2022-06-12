using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using TMPro;
using ECellDive.Multiplayer;
using ECellDive.Utility;
using ECellDive.UI;

namespace ECellDive
{
    namespace Modules
    {
        [System.Serializable]
        public struct UIDisplayData
        {
            public OptimizedVertScrollList UISelectorsContainer;
            public TMP_InputField refIPInputField;
            public TMP_InputField refPortInputField;
        }

        public class HttpServerImporterModule : HttpServerBaseModule
        {
            public UIDisplayData uiDisplayData;
            private string activeModelName = "";

            public GameNetModuleSpawner gameNetModuleSpawner;

            private void Start()
            {
                gameNetModuleSpawner = GameObject.FindGameObjectWithTag("GameNetModuleSpawner").GetComponent<GameNetModuleSpawner>();
            }

            /// <summary>
            /// Requests the models list to the server.
            /// </summary>
            private void GetModelsList()
            {
                string requestURL = AddPagesToURL(new string[] { "models" });
                StartCoroutine(GetRequest(requestURL));
            }

            /// <summary>
            /// Requests the .cyjs file of a model to the server.
            /// </summary>
            /// <param name="_modelName">The name of the model as
            /// stored in the server.</param>
            private void GetModelCyJs(string _modelName)
            {
                string requestURL = AddPagesToURL(new string[] { "model", _modelName });
                StartCoroutine(GetRequest(requestURL));
            }

            /// <summary>
            /// Requests the SBML file of a model to the server.
            /// </summary>
            /// <param name="_modelName">The name of the model as
            /// stored in the server.</param>
            private void GetModelSBML(string _modelName)
            {
                string requestURL = AddPagesToURL(new string[] { "sbml", _modelName });
                StartCoroutine(GetRequest(requestURL));
            }

            /// <summary>
            /// The public interface to ask the server for the .cyjs
            /// file of a model and instantiating its corresponding
            /// module in the main room.
            /// </summary>
            /// <param name="_textMeshProUGUI">The UI Text Mesh Pro of the 
            /// button dedicated to importing the .cyjs of the model. This
            /// button must be displaying the name of the model.</param>
            public void ImportModelCyJs(TextMeshProUGUI _textMeshProUGUI)
            {
                activeModelName = _textMeshProUGUI.text;
                StartCoroutine(ImportModelCyJsC());
            }

            /// <summary>
            /// The coroutine handling the request to the server and the
            /// instantiation of the network module.
            /// </summary>
            /// <returns></returns>
            private IEnumerator ImportModelCyJsC()
            {
                GetModelCyJs(activeModelName);

                yield return new WaitUntil(isRequestProcessed);

                if (requestData.requestSuccess)
                {
                    byte[] modelContent = System.Text.Encoding.UTF8.GetBytes(requestData.requestText);
                    byte[] name = System.Text.Encoding.UTF8.GetBytes(activeModelName);
                    List<byte[]> mCFs = ArrayManipulation.FragmentToList(modelContent, 4096);

                    gameNetModuleSpawner.RequestModuleSpawnFromData(4, name, mCFs);
                }
            }

            /// <summary>
            /// The public interface to ask the server for the list of
            /// the available models.
            /// </summary>
            public void ShowModelsList()
            {
                StartCoroutine(ShowModelsListC());
            }

            /// <summary>
            /// The coroutine handling the request to the server and
            /// the parsing+display of the content of the list.
            /// </summary>
            /// <returns></returns>
            private IEnumerator ShowModelsListC()
            {
                GetModelsList();

                yield return new WaitUntil(isRequestProcessed);

                if (requestData.requestSuccess)
                {
                    requestData.requestJObject = JObject.Parse(requestData.requestText);
                    JArray jModelsArray = (JArray)requestData.requestJObject["models"];
                    List<string> modelsList = jModelsArray.Select(c => (string)c).ToList();

                    foreach (RectTransform _child in uiDisplayData.UISelectorsContainer.refContent)
                    {
                        if (_child.gameObject.activeSelf)
                        {
                            Destroy(_child.gameObject);
                        }
                    }

                    for (int i = 0; i < modelsList.Count; i++)
                    {
                        GameObject modelUIContainer = uiDisplayData.UISelectorsContainer.AddItem();
                        modelUIContainer.GetComponentInChildren<TextMeshProUGUI>().text = modelsList[i];
                        modelUIContainer.SetActive(true);
                        uiDisplayData.UISelectorsContainer.UpdateScrollList();
                    }
                }
            }

            public void UpdateIP()
            {
                serverData.serverIP = uiDisplayData.refIPInputField.text;
            }

            public void UpdatePort()
            {
                serverData.port = uiDisplayData.refPortInputField.text;
            }
        }
    }
}

