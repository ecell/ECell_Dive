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
        public OptimizedVertScrollList refModificationFilesScrollList;
        public OptimizedVertScrollList refBaseModelsScrollList;

        public TMP_Text refSelectedBaseModel;
        public TMP_InputField refSaveNewFileName;

        private string targetFileName;
        private IModifiable targetModifiable;
        private ISaveable targetSaveable;
        

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
        private IEnumerator ImportModFileC()
        {
            GetModFile(targetFileName);

            yield return new WaitUntil(isRequestProcessed);

            if (requestData.requestSuccess)
            {
                //Transfer the modifications to the target module.
                Debug.Log(requestData.requestText);
                requestData.requestJObject = JObject.Parse(requestData.requestText);
                ModificationFile modFile = new ModificationFile(targetFileName, requestData.requestJObject);

                targetModifiable = GetModifiable(modFile.baseModelName);

                if (targetModifiable != null)
                {
                    targetModifiable.readingModificationFile = new ModificationFile(targetFileName, requestData.requestJObject);
                    targetModifiable.ApplyFileModifications();
                }

                else
                {
                    LogSystem.refLogManager.AddMessage(LogSystem.MessageTypes.Errors,
                        "No modifiable data module called \"" + modFile.baseModelName + "\" was found. " +
                        "Please, make sure you imported the data before trying to import modifications" +
                        "associated with this data.");
                }
            }
        }

        private string RequestSaveModel(ModificationFile _modFile)
        {
            string fileName = "";
            if (refSaveNewFileName.text == "")
            {
                fileName = _modFile.baseModelName + "_" + System.DateTime.Now.ToString("yyyyMMddTHHmmssZ");
            }
            else
            {
                fileName = refSaveNewFileName.text;
            }

            string requestURL = AddPagesToURL(new string[]
            {
                "save_model",
                fileName,
                _modFile.baseModelName
            });

            requestURL = AddQueriesToURL(requestURL,
                new string[]
                {
                    "author",
                    "description",
                    "modification",
                    "view_name"
                },
                new string[]
                {
                    _modFile.author,
                    _modFile.description,
                    _modFile.modification,
                    _modFile.baseModelName
                });

            Debug.Log(requestURL);

            StartCoroutine(GetRequest(requestURL));

            return fileName;
        }

        /// <summary>
        /// The public interface to save the modiication file associated to a
        /// base model.
        /// </summary>
        public void SaveModificationFile()
        {
            StartCoroutine(SaveModificationFileC());
        }

        /// <summary>
        /// The coroutine handling the save request to the server.
        /// </summary>
        private IEnumerator SaveModificationFileC()
        {
            targetSaveable.CompileModificationFile();
            string fileName = RequestSaveModel(targetSaveable.writingModificationFile);

            yield return new WaitUntil(isRequestProcessed);

            if (requestData.requestSuccess)
            {
                LogSystem.refLogManager.AddMessage(LogSystem.MessageTypes.Trace,
                    "File \"" + fileName + "\" has been succesfully saved.");
            }
            else
            {
                LogSystem.refLogManager.AddMessage(LogSystem.MessageTypes.Errors,
                    "File \"" + fileName + "\" could not be saved.");
            }
        }

        /// <summary>
        /// Sets the <see cref="targetSaveable"/> to the object that is of the same index in
        /// <see cref="GameNetPortal.saveables"/> than the <paramref name="_container"/>
        /// sibling index.
        /// </summary>
        public void SetTargetSaveable(GameObject _container)
        {
            targetSaveable = GameNetPortal.Instance.saveables[_container.transform.GetSiblingIndex()];
            refSelectedBaseModel.text = _container.GetComponentInChildren<TMP_Text>().text;
        }

        /// <summary>
        /// The public interface to show the list of base models that are currently open
        /// on the server.
        /// </summary>
        public void ShowBaseModelsCandidates()
        {
            foreach (RectTransform _child in refBaseModelsScrollList.refContent)
            {
                if (_child.gameObject.activeSelf)
                {
                    Destroy(_child.gameObject);
                }
            }

            foreach(ISaveable _saveable in GameNetPortal.Instance.saveables)
            {
                GameObject modelUIContainer = refBaseModelsScrollList.AddItem();
                modelUIContainer.GetComponentInChildren<TextMeshProUGUI>().text = _saveable.writingModificationFile.baseModelName;
                modelUIContainer.SetActive(true);
                refModificationFilesScrollList.UpdateScrollList();
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
        private IEnumerator ShowModFilesListC()
        {
            GetModFilesList();

            yield return new WaitUntil(isRequestProcessed);

            if (requestData.requestSuccess)
            {
                requestData.requestJObject = JObject.Parse(requestData.requestText);
                JArray jModelsArray = (JArray)requestData.requestJObject["user_defined_models"];
                List<string> modelsList = jModelsArray.Select(c => (string)c).ToList();

                foreach (RectTransform _child in refModificationFilesScrollList.refContent)
                {
                    if (_child.gameObject.activeSelf)
                    {
                        Destroy(_child.gameObject);
                    }
                }

                for (int i = 0; i < modelsList.Count; i++)
                {
                    GameObject modelUIContainer = refModificationFilesScrollList.AddItem();
                    modelUIContainer.GetComponentInChildren<TextMeshProUGUI>().text = modelsList[i];
                    modelUIContainer.SetActive(true);
                    refModificationFilesScrollList.UpdateScrollList();
                }
            }
        }
    }
}

