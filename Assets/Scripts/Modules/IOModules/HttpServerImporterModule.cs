using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Unity.Netcode;
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

        //[RequireComponent(typeof(ModuleImportRPCs))]
        public class HttpServerImporterModule : HttpServerBaseModule
        {
            public UIDisplayData uiDisplayData;
            private string activeModelName = "";

            public GameNetModuleSpawner gameNetModuleSpawner;

            private void Start()
            {
                gameNetModuleSpawner = GameObject.FindGameObjectWithTag("GameNetModuleSpawner").GetComponent<GameNetModuleSpawner>();
            }

            //[ClientRpc]
            //public void DistributeCyJsonModuleClientRpc(NetworkObjectReference _cyJsonModule)
            //{
            //    Debug.Log($"DistributeCyJsonModuleClientRpc on client:{NetworkManager.Singleton.LocalClientId}");

            //    byte[] modelContent = ArrayManipulation.Assemble(modelContentFrags);
            //    //Debug.Log(System.Text.Encoding.UTF8.GetString(modelContent));
            //    requestData.requestJObject = JObject.Parse(System.Text.Encoding.UTF8.GetString(modelContent));

            //    //Loading the file
            //    NetworkComponents.Network network = NetworkLoader.Initiate(requestData.requestJObject,
            //                                                                activeModelName);

            //    //Instantiating relevant data structures to store the information about
            //    //the layers, nodes and edges.
            //    NetworkLoader.Populate(network);
            //    CyJsonModulesData.AddData(network);

            //    NetworkObject cyJsonModuleNet = _cyJsonModule;
            //    cyJsonModuleNet.GetComponent<CyJsonModule>().StartUpInfo();
            //}

            //[ServerRpc]
            //public void SpawnCyJsModuleServerRpc()//ModuleData _cyJsonMD, string _modelName, string _modelContent)
            //{
            //    //Instantiation of the CyJson module corresponding to encapsulate the
            //    //CyJson pathway that just has been populated.
            //    ModuleData cyJsonMD = new ModuleData
            //    {
            //        typeID = 4 // 4 is the type ID of a CyJsonModule
            //    };
            //    ModulesData.AddModule(cyJsonMD);
            //    Vector3 pos = Positioning.PlaceInFrontOfTarget(Camera.main.transform, 2f, 0.8f);
            //    GameObject cyJsonModule = ScenesData.refSceneManagerMonoBehaviour.InstantiateGOOfModuleData(cyJsonMD, pos);
                
                
            //    cyJsonModule.GetComponent<NetworkObject>().Spawn();

            //    DistributeCyJsonModuleClientRpc(cyJsonModule);
            //}

            //[ClientRpc]
            //public void DistributeFragmentClientRpc(byte[] _frag)
            //{
            //    Debug.Log($"Client {NetworkManager.Singleton.LocalClientId} received a fragment of the model of size {_frag.Length}");
            //    modelContentFrags.Add(_frag);
            //}

            //[ServerRpc]
            //public void DistributeFragmentServerRpc(byte[] _frag)
            //{
            //    //Debug.Log($"The server received a fragment of the model of size {_frag.Length}");
            //    DistributeFragmentClientRpc(_frag);
            //}

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
                    //FileStream fs = File.OpenWrite("C:/Users/EliottJacopin/Documents/modelContent.json");
                    //StreamWriter sw = new StreamWriter(fs);
                    //fs.Write(modelContent, 0, modelContent.Length);
                    //fs.Close();

                    //Debug.Log($"The request data text represents: {modelContent.Length} bytes. " +
                    //          $"You need {Mathf.CeilToInt(modelContent.Length / 4096)} FixedString4096Bytes to represent it.");

                    byte[] name = System.Text.Encoding.UTF8.GetBytes(activeModelName);
                    List<byte[]> mCFs = ArrayManipulation.FragmentToList(modelContent, 4096);

                    gameNetModuleSpawner.RequestModuleSpawnFromData(4, name, mCFs);

                    //Debug.Log($"We fragmented the model into {mCFs.Count} chunks");
                    //StartCoroutine(DistributeFragmentC(mCFs));
                    

                    //DistributeFragmentServerRpc(mCFs[0]);

                    //byte[] modelContentBytes = ArrayManipulation.Assemble(modelContentFrags);
                    //string modelContentRebuilt = System.Text.Encoding.UTF8.GetString(modelContentBytes);

                    //Debug.Log($"The assembled data text represents: {modelContentBytes.Length} bytes.");
                    //Debug.Log($"This is same length as the original: {modelContentBytes.Length == modelContent.Length}.");

                    //fs = File.OpenWrite("C:/Users/EliottJacopin/Documents/modelContentRebuilt.json");
                    //sw = new StreamWriter(fs);
                    //fs.Write(modelContentBytes, 0, modelContentBytes.Length);
                    //fs.Close();

                    //Debug.Log(modelContentRebuilt == requestData.requestText);

                    
                }
            }
            
            //IEnumerator DistributeFragmentC(List<byte[]> _mCFs)
            //{
            //    foreach (byte[] _frag in _mCFs)
            //    {
            //        DistributeFragmentServerRpc(_frag);
            //        yield return new WaitForEndOfFrame();
            //    }

            //    SpawnCyJsModuleServerRpc();
            //}

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

