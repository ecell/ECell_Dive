using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using TMPro;
using ECellDive.IO;
using ECellDive.Utility;
using ECellDive.SceneManagement;

namespace ECellDive
{
    namespace Modules
    {
        [System.Serializable]
        public struct UIDisplayData
        {
            public GameObject UISelectorsContainer;
            public GameObject UISelectorPrefab;
            public TMP_InputField refIPInputField;
            public TMP_InputField refPortInputField;
        }

        public class HttpServerImporterModule : HttpServerBaseModule
        {
            public UIDisplayData uiDisplayData;
            [HideInInspector] public string activeModelName = "";

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
                    requestData.requestJObject = JObject.Parse(requestData.requestText);

                    //Loading the file
                    NetworkComponents.Network network = NetworkLoader.Initiate(requestData.requestJObject,
                                                                                activeModelName);

                    //Instantiating relevant data structures to store the information about
                    //the layers, nodes and edges.
                    NetworkLoader.Populate(network);
                    CyJsonModulesData.AddData(network);

                    //Instantiation of the CyJson module corresponding to encapsulate the
                    //CyJson pathway that just has been populated.
                    ModuleData cyJsonMD = new ModuleData
                    {
                        typeID = 4 // 4 is the type ID of a CyJsonModule
                    };
                    ModulesData.AddModule(cyJsonMD);
                    Vector3 pos = Positioning.PlaceInFrontOfTarget(Camera.main.transform, 2f, 0.8f);
                    ScenesData.refSceneManagerMonoBehaviour.InstantiateGOOfModuleData(cyJsonMD, pos);                }
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

                    foreach (Transform _child in uiDisplayData.UISelectorsContainer.transform)
                    {
                        if (_child.gameObject.activeSelf)
                        {
                            Destroy(_child.gameObject);
                        }
                    }

                    for (int i = 0; i < modelsList.Count; i++)
                    {                        
                        GameObject modelUIContainer = Instantiate(uiDisplayData.UISelectorPrefab,
                                                                  uiDisplayData.UISelectorsContainer.transform);
                        modelUIContainer.GetComponentInChildren<TextMeshProUGUI>().text = modelsList[i];
                        modelUIContainer.SetActive(true);

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

