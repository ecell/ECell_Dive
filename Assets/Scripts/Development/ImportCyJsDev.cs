using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ECellDive.Modules;
using ECellDive.Utility;

namespace ECellDive.CustomEditors
{
    /// <summary>
    /// The class to import a CyJson file and generate the associated
    /// graph from the editor ouside of runtime.
    /// </summary>
    public class ImportCyJsDev : ATServerInterface
    {
        public CyJsonModule cyJsonDataHolder;
        public string targetModelName;

        /// <summary>
        /// Requests the .cyjs file of a model to the server.
        /// </summary>
        /// <param name="_modelName">The name of the model as
        /// stored in the server.</param>
        private void GetModelCyJs(string _modelName)
        {
            string requestURL = AddPagesToURL(new string[] { "open_view", _modelName });
            StartCoroutine(GetRequest(requestURL));
        }

        /// <summary>
        /// The public interface to ask the server for the .cyjs
        /// file of a model and instantiating its corresponding
        /// module in the main room.
        /// </summary>
        public void GenerateGraphCyJs()
        {
            StartCoroutine(GenerateGraphCyJsC());
        }

        /// <summary>
        /// The coroutine handling the request to the server and the
        /// instantiation of the network module.
        /// </summary>
        /// <returns></returns>
        private IEnumerator GenerateGraphCyJsC()
        {
            GetModelCyJs(targetModelName);

            yield return new WaitUntil(() => requestData.requestProcessed);

            if (requestData.requestSuccess)
            {
                byte[] modelContent = System.Text.Encoding.UTF8.GetBytes(requestData.requestText);
                byte[] name = System.Text.Encoding.UTF8.GetBytes(targetModelName);
                List<byte[]> mCFs = ArrayManipulation.FragmentToList(modelContent, 1024);
                cyJsonDataHolder.DirectReceiveSourceData(name, mCFs);

                cyJsonDataHolder.GenerateGraph();
            }
        }
        
        /// <summary>
        /// The public interface to ask the server for the .cyjs
        /// file of a model and instantiating its corresponding
        /// module in the main room.
        /// </summary>
        public void GenerateGraphCyJsAsset()
        {
            StartCoroutine(GenerateGraphCyJsAssetC());
        }

        /// <summary>
        /// The coroutine handling the request to the server and the
        /// instantiation of the network module.
        /// </summary>
        /// <returns></returns>
        private IEnumerator GenerateGraphCyJsAssetC()
        {
            GetModelCyJs(targetModelName);

            yield return new WaitUntil(() => requestData.requestProcessed);

            if (requestData.requestSuccess)
            {
                byte[] modelContent = System.Text.Encoding.UTF8.GetBytes(requestData.requestText);
                byte[] name = System.Text.Encoding.UTF8.GetBytes(targetModelName);
                List<byte[]> mCFs = ArrayManipulation.FragmentToList(modelContent, 1024);
                cyJsonDataHolder.DirectReceiveSourceData(name, mCFs);

                cyJsonDataHolder.GenerateGraphAsset();
            }
        }
    }
}

