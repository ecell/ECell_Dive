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
using ECellDive.Utility.Data.Modification;

namespace ECellDive.Modules
{
	/// <summary>
	/// The class to handle modification files requests and save with 
	/// a server.
	/// </summary>
	public class HttpServerModificationModule : HttpServerBaseModule
	{
		/// <summary>
		/// The scroll list displaying the modification files.
		/// </summary>
		public OptimizedVertScrollList refModificationFilesScrollList;

		/// <summary>
		/// The scroll list displaying the models which can be modified.
		/// </summary>
		public OptimizedVertScrollList refBaseModelsScrollList;

		/// <summary>
		/// The text mesh displaying the name of the selected base model.
		/// </summary>
		public TMP_Text refSelectedBaseModel;

		/// <summary>
		/// The input field to set the name of the new modification file.
		/// </summary>
		public TMP_InputField refSaveNewFileName;

		/// <summary>
		/// The animation loop controller to control the visual feedback
		/// of the module in case of request.
		/// </summary>
		public AnimationLoopWrapper animLW;

		/// <summary>
		/// Buffer storing the name of the modification file to import.
		/// </summary>
		private string targetFileName;

		/// <summary>
		/// Buffer storing the modifiable object to which the modification
		/// will be applied.
		/// </summary>
		private IModifiable targetModifiable;

		/// <summary>
		/// Buffer storing the information to save the modification file.
		/// </summary>
		private ISaveable targetSaveable;
		

		/// <summary>
		/// Requests the list of modification file to the server.
		/// </summary>
		private void GetModFilesList()
		{
			string requestURL = AddPagesToURL(new string[] { "list_user_model" });
			requestURL = AddQueryToURL(requestURL, "base_model_name", refSelectedBaseModel.text, true);
			StartCoroutine(GetRequest(requestURL));
		}

		/// <summary>
		/// Requests the a specific modification file to the server.
		/// </summary>
		/// <param name="_filelName">The name of the model as
		/// stored in the server.</param>
		private void GetModFile(string _filelName)
		{
			string requestURL = AddPagesToURL(new string[] { "open_user_model", _filelName });
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
			animLW.PlayLoop("HttpServerModificationModule");
		}

		/// <summary>
		/// The coroutine handling the request to the server and the
		/// instantiation of the network module.
		/// </summary>
		private IEnumerator ImportModFileC()
		{
			GetModFile(targetFileName);

			yield return new WaitUntil(isRequestProcessed);
			
			//stop the "Work In Progress" animation of this module
			animLW.StopLoop();

			if (requestData.requestSuccess)
			{
				//Transfer the modifications to the target module.
				requestData.requestJObject = JObject.Parse(requestData.requestText);
				ModificationFile modFile = new ModificationFile(targetFileName, requestData.requestJObject);

				targetModifiable = GetModifiable(modFile.baseModelName);

				if (targetModifiable != null)
				{
					//Flash of the succesful color.
					GetComponentInChildren<ColorFlash>().Flash(1);

					targetModifiable.readingModificationFile = modFile;//new ModificationFile(targetFileName, requestData.requestJObject);
					targetModifiable.ApplyFileModifications();
				}

				else
				{
					//Flash of the fail color.
					GetComponentInChildren<ColorFlash>().Flash(0);

					LogSystem.AddMessage(LogMessageTypes.Errors,
						"No modifiable data module called \"" + modFile.baseModelName + "\" was found. " +
						"Please, make sure you imported the data before trying to import modifications" +
						"associated with this data.");
				}
			}
			else
			{
				//Flash of the fail color.
				GetComponentInChildren<ColorFlash>().Flash(0);
			}
		}

		/// <summary>
		/// Builds the request to save a modification file and calls 
		/// <see cref="ECellDive.Modules.HttpServerBaseModule.GetRequest(string)"/>
		/// </summary>
		/// <param name="_modFile">
		/// The ModificationFile to save.
		/// </param>
		/// <returns>
		/// The name of the file to save. The name is generated if the
		/// nothing is entered in <see cref="refSaveNewFileName"/>.
		/// </returns>
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
				"save",
				_modFile.baseModelName,
				_modFile.GetAuthorOfMod(0),
				fileName
			});

			List<string[]> allModifications = _modFile.GetAllCommands();
			foreach (string[] _mod in allModifications)
			{
				int koCount = _mod.Count();
				if (koCount > 0)
				{
					requestURL = AddQueryToURL(requestURL, "command", _mod[0], true);

					for (int i = 1; i < koCount; i++)
					{
						requestURL = AddQueryToURL(requestURL, "command", _mod[i]);
					}
				}
			}
			
			requestURL = AddQueryToURL(requestURL, "view_name", _modFile.baseModelName, allModifications.Count == 0);

			Debug.Log("Save Request" + requestURL);

			StartCoroutine(GetRequest(requestURL));

			return fileName;
		}

		/// <summary>
		/// The public interface to save the modiication file associated to a
		/// base model.
		/// </summary>
		public void SaveModificationFile()
		{
			animLW.PlayLoop("HttpServerModificationModule");
			StartCoroutine(SaveModificationFileC());
		}

		/// <summary>
		/// The coroutine handling the save request to the server.
		/// </summary>
		private IEnumerator SaveModificationFileC()
		{
			if(targetSaveable != null)
			{
				targetSaveable.CompileModificationFile();
				string fileName = RequestSaveModel(targetSaveable.writingModificationFile);

				yield return new WaitUntil(isRequestProcessed);

				//stop the "Work In Progress" animation of this module
				animLW.StopLoop();

				if (requestData.requestSuccess)
				{
					//Flash of the succesful color.
					GetComponentInChildren<ColorFlash>().Flash(1);
					LogSystem.AddMessage(LogMessageTypes.Trace,
						"File \"" + fileName + "\" has been succesfully saved.");
				}
				else
				{
					//Flash of the fail color.
					GetComponentInChildren<ColorFlash>().Flash(0);
					LogSystem.AddMessage(LogMessageTypes.Errors,
						"File \"" + fileName + "\" could not be saved.");
				}
			}
			else
			{
				//stop the "Work In Progress" animation of this module
				animLW.StopLoop();
				//Flash of the fail color.
				GetComponentInChildren<ColorFlash>().Flash(0);
				LogSystem.AddMessage(LogMessageTypes.Errors,
					"You have not selected any target base model for which" +
					"to save the modifications");
			}
		}

		/// <summary>
		/// Sets the <see cref="targetSaveable"/> to the object that is of the same index in
		/// <see cref="ECellDive.Multiplayer.GameNetPortal.saveables"/> than the <paramref name="_container"/>
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
			animLW.PlayLoop("HttpServerModificationModule");

			foreach (RectTransform _child in refBaseModelsScrollList.refContent)
			{
				if (_child.gameObject.activeSelf)
				{
					Destroy(_child.gameObject);
				}
			}

			foreach(CyJsonModule _model in CyJsonModulesData.loadedData)
			{
				GameObject modelUIContainer = refBaseModelsScrollList.AddItem();
				modelUIContainer.GetComponentInChildren<TextMeshProUGUI>().text = _model.nameField.text;
				modelUIContainer.SetActive(true);
				refModificationFilesScrollList.UpdateScrollList();
			}

			//stop the "Work In Progress" animation of this module
			animLW.StopLoop();

			if (GameNetPortal.Instance.saveables.Count > 0)
			{
				//Flash of the succesful color.
				GetComponentInChildren<ColorFlash>().Flash(1);
			}
			else
			{
				//Flash of the fail color.
				GetComponentInChildren<ColorFlash>().Flash(0);
				LogSystem.AddMessage(LogMessageTypes.Errors,
					"No base model has been found. Have you imported eligible data?" +
					" If yes, have you generated it? (Dive into it at least once)");
			}

		}

		/// <summary>
		/// The public interface to ask the server for the list of
		/// the available modification files.
		/// </summary>
		public void ShowModFilesList()
		{
			StartCoroutine(ShowModFilesListC());
			animLW.PlayLoop("HttpServerModificationModule");
		}

		/// <summary>
		/// The coroutine handling the request to the server and
		/// the parsing+display of the content of the list.
		/// </summary>
		private IEnumerator ShowModFilesListC()
		{
			GetModFilesList();

			yield return new WaitUntil(isRequestProcessed);

			//stop the "Work In Progress" animation of this module
			animLW.StopLoop();

			if (requestData.requestSuccess)
			{
				//Flash of the succesful color.
				GetComponentInChildren<ColorFlash>().Flash(1);

				requestData.requestJObject = JObject.Parse(requestData.requestText);
				JArray jModelsArray = (JArray)requestData.requestJObject["user_models"];

				foreach (RectTransform _child in refModificationFilesScrollList.refContent)
				{
					if (_child.gameObject.activeSelf)
					{
						Destroy(_child.gameObject);
					}
				}

				for (int i = 0; i < jModelsArray.Count; i++)
				{
					GameObject modelUIContainer = refModificationFilesScrollList.AddItem();
					modelUIContainer.GetComponentInChildren<TextMeshProUGUI>().text = jModelsArray.ElementAt(i).Value<string>();
					modelUIContainer.SetActive(true);
					refModificationFilesScrollList.UpdateScrollList();
				}
			}
			else
			{
				//Flash of the fail color.
				GetComponentInChildren<ColorFlash>().Flash(0);
			}
		}
	}
}

