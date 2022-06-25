using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using TMPro;
using ECellDive.Interfaces;
using ECellDive.Multiplayer;
using ECellDive.UI;
using ECellDive.Utility;

namespace ECellDive.Modules
{
    /// <summary>
    /// The class to handle modification files requests and save with 
    /// a server.
    /// </summary>
    public class HttpServerModificationModule : HttpServerBaseModule
    {
        public OptimizedVertScrollList refModelsScrollList;
        private string targetFileName;
        private IModifiable targetModifiable;

        private string ExtractModelNameFromModelPath(string _modelPath)
        {
            string modelFileName = _modelPath.Split('/').Last();
            return modelFileName.Split('.').First();
        }

        /// <summary>
        /// Requests the list of modification file to the server.
        /// </summary>
        private void GetModFilesList()
        {
            string requestURL = AddPagesToURL(new string[] { "list_user_model" });
            StartCoroutine(GetRequest(requestURL));
        }

        /// <summary>
        /// Requests the a specific modification file to the server.
        /// </summary>
        /// <param name="_filelName">The name of the model as
        /// stored in the server.</param>
        private void GetModFile(string _filelName)
        {
            string requestURL = AddPagesToURL(new string[] { "get_user_modification", _filelName });
            StartCoroutine(GetRequest(requestURL));
        }

        /// <summary>
        /// Gets the first modifiable from <see cref="GameNetPortal.modifiables"/>
        /// that matches the <paramref name="_name"/>
        /// </summary>
        private IModifiable GetModifiable(string _name)
        {
            foreach(IModifiable _mod in GameNetPortal.Instance.modifiables)
            {
                if (_mod.CheckName(_name))
                {
                    return _mod;
                }
            }
            return null;
        }

        /// <summary>
        /// The public interface to ask the server for the modification
        /// file.
        /// </summary>
        /// <param name="_textMeshProUGUI">The UI Text Mesh Pro of the 
        /// button dedicated to importing the modification files. This
        /// button must be displaying the name of the file.</param>
        public void ImportModFile(TextMeshProUGUI _textMeshProUGUI)
        {
            targetFileName = _textMeshProUGUI.text;
            StartCoroutine(ImportModFileC());
        }

        /// <summary>
        /// The coroutine handling the request to the server and the
        /// instantiation of the network module.
        /// </summary>
        /// <returns></returns>
        private IEnumerator ImportModFileC()
        {
            GetModFile(targetFileName);

            yield return new WaitUntil(isRequestProcessed);

            if (requestData.requestSuccess)
            {
                //Transfer the modifications to the target module.
                Debug.Log(requestData.requestText);
                requestData.requestJObject = JObject.Parse(requestData.requestText);
                string modelpath = requestData.requestJObject["base_model_path"].Value<string>();
                string modelName = ExtractModelNameFromModelPath(modelpath);

                targetModifiable = GetModifiable(modelName);

                if (targetModifiable != null)
                {
                    targetModifiable.ApplyFileModifications(requestData.requestJObject["modification"].Value<string>());
                }

                else
                {
                    LogSystem.refLogManager.AddMessage(LogSystem.MessageTypes.Errors,
                        "No modifiable data module called \"" + modelName + "\" was found. " +
                        "Please, make sure you imported the data before trying to import modifications" +
                        "associated with this data.");
                }
            }
        }

        /// <summary>
        /// The public interface to ask the server for the list of
        /// the available modification files.
        /// </summary>
        public void ShowModFilesList()
        {
            StartCoroutine(ShowModFilesListC());
        }

        /// <summary>
        /// The coroutine handling the request to the server and
        /// the parsing+display of the content of the list.
        /// </summary>
        /// <returns></returns>
        private IEnumerator ShowModFilesListC()
        {
            GetModFilesList();

            yield return new WaitUntil(isRequestProcessed);

            if (requestData.requestSuccess)
            {
                requestData.requestJObject = JObject.Parse(requestData.requestText);
                JArray jModelsArray = (JArray)requestData.requestJObject["user_defined_models"];
                List<string> modelsList = jModelsArray.Select(c => (string)c).ToList();

                foreach (RectTransform _child in refModelsScrollList.refContent)
                {
                    if (_child.gameObject.activeSelf)
                    {
                        Destroy(_child.gameObject);
                    }
                }

                for (int i = 0; i < modelsList.Count; i++)
                {
                    GameObject modelUIContainer = refModelsScrollList.AddItem();
                    modelUIContainer.GetComponentInChildren<TextMeshProUGUI>().text = modelsList[i];
                    modelUIContainer.SetActive(true);
                    refModelsScrollList.UpdateScrollList();
                }
            }
        }

    }
}

