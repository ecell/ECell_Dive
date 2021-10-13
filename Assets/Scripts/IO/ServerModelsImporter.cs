using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using TMPro;
using ECellDive.Modules;



namespace ECellDive
{
    namespace IO
    {
        public class ServerModelsImporter : HTTPServer
        {
            [Header("UI references")]
            public GameObject refModelsUIContainer;
            public GameObject refModelUIPrefab;

            [Header("General references")]
            public ModulesManager refModulesManager;

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

                if (requestSuccess)
                {
                    requestJObject = JObject.Parse(requestText);

                    //Loading the file
                    NetworkComponents.Network network = NetworkLoader.Initiate(requestJObject, activeModelName);

                    //Instantiating relevant data structures to store the information about
                    //the layers, nodes and edges.
                    NetworkLoader.Populate(network);

                    NetworkModulesData.AddData(network);
                    refModulesManager.InstantiateModule(network);
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

                if (requestSuccess)
                {
                    requestJObject = JObject.Parse(requestText);
                    JArray jModelsArray = (JArray)requestJObject["models"];
                    List<string> modelsList = jModelsArray.Select(c => (string)c).ToList();

                    for (int i = 0; i < modelsList.Count; i++)
                    {
                        foreach (Transform _child in refModelsUIContainer.transform)
                        {
                            Destroy(_child.gameObject);
                        }
                        GameObject modelUIContainer = Instantiate(refModelUIPrefab, refModelsUIContainer.transform);
                        modelUIContainer.GetComponentInChildren<TextMeshProUGUI>().text = modelsList[i];
                        modelUIContainer.SetActive(true);
                    }
                }
            }
        }
    }
}

