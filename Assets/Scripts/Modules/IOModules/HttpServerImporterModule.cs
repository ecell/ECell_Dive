using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using ECellDive.Multiplayer;
using ECellDive.Utility;
using ECellDive.UI;

namespace ECellDive.Modules
{
	/// <summary>
	/// The module to import models from the Kosmogora-like server.
	/// </summary>
	public class HttpServerImporterModule : HttpServerBaseModule
	{
		/// <summary>
		/// The scroll list to display the models available on the server.
		/// </summary>
		public OptimizedVertScrollList refModelsScrollList;

		/// <summary>
		/// The buffer to the name of the model to import.
		/// </summary>
		protected string activeModelName = "";

		/// <summary>
		/// A reference to the game object used to spawn data module
		/// of the imported model.
		/// </summary>
		public GameNetModuleSpawner gameNetModuleSpawner;

		/// <summary>
		/// A delegate to be called when the data a user requested
		/// to import a model. This is called whether the request
		/// is successful or not.
		/// </summary>
		/// <remarks>
		/// Added for the tutorials check. There is probably a way to
		/// get rid of this.
		/// </remarks>
		public UnityAction<bool, string> OnDataModuleImport;

		/// <summary>
		/// The animation loop controller to control the visual feedback
		/// of the module in case of request.
		/// </summary>
		public AnimationLoopWrapper animLW;

		private void Start()
		{
			gameNetModuleSpawner = GameObject.FindGameObjectWithTag("GameNetModuleSpawner").GetComponent<GameNetModuleSpawner>();
		}

		/// <summary>
		/// Requests the models list to the server.
		/// </summary>
		private void GetModelsList()
		{
			string requestURL = AddPagesToURL(new string[] { "list_models" });
			StartCoroutine(GetRequest(requestURL));
		}

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
		/// Requests the SBML file of a model to the server.
		/// </summary>
		/// <param name="_modelName">The name of the model as
		/// stored in the server.</param>
		private void GetModelSBML(string _modelName)
		{
			string requestURL = AddPagesToURL(new string[] { "open_sbml", _modelName });
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
			animLW.PlayLoop("HttpServerImporterModule");
		}

		/// <summary>
		/// The coroutine handling the request to the server and the
		/// instantiation of the network module.
		/// </summary>
		/// <returns></returns>
		protected IEnumerator ImportModelCyJsC()
		{
			GetModelCyJs(activeModelName);

			yield return new WaitUntil(isRequestProcessed);

			//stop the "Work In Progress" animation of this module
			animLW.StopLoop();

			OnDataModuleImport?.Invoke(requestData.requestSuccess, activeModelName);

			if (requestData.requestSuccess)
			{
				//Flash of the succesful color.
				GetComponentInChildren<ColorFlash>().Flash(1);

				byte[] modelContent = System.Text.Encoding.UTF8.GetBytes(requestData.requestText);
				byte[] name = System.Text.Encoding.UTF8.GetBytes(activeModelName);
				List<byte[]> mCFs = ArrayManipulation.FragmentToList(modelContent, 1024);
				LogSystem.AddMessage(LogMessageTypes.Debug,
					"Just fragmented the Data. Requesting a module spawn to encapsulate it.");
				gameNetModuleSpawner.RequestModuleSpawnFromData(0, name, mCFs);
			}
			else
			{
				//Flash of the fail color
				GetComponentInChildren<ColorFlash>().Flash(0);
			}
		}

		/// <summary>
		/// The public interface to ask the server for the list of
		/// the available models.
		/// </summary>
		public void ShowModelsList()
		{
			StartCoroutine(ShowModelsListC());
			animLW.PlayLoop("HttpServerImporterModule");
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

			//stop the "Work In Progress" animation of this module
			animLW.StopLoop();

			if (requestData.requestSuccess)
			{
				//Flash of the succesful color.
				GetComponentInChildren<ColorFlash>().Flash(1);

				requestData.requestJObject = JObject.Parse(requestData.requestText);
				JArray jModelsArray = (JArray)requestData.requestJObject["models"];
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

			else
			{
				//Flash of the fail color
				GetComponentInChildren<ColorFlash>().Flash(0);
			}
		}
	}
}

