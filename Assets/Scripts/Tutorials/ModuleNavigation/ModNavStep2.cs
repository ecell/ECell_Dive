using UnityEngine;

using ECellDive.Modules;
using ECellDive.UI;
using ECellDive.Utility;

namespace ECellDive.Tutorials
{
	/// <summary>
	/// The step 2 of the Tutorial on Modules Navigation.
	/// Request for an API check of the Kosmogora server.
	/// </summary>
    public class ModNavStep2 : Step
    {
		private GUIManager guiManager;
		private HttpServerAPICheckModule httpAPIChecker;
		bool allHttpModulesInteractable = false;

		public override bool CheckCondition()
		{
			return allHttpModulesInteractable;
		}

		public override void Conclude()
		{
			base.Conclude();

			httpAPIChecker.onAPICheck -= APICheckProcessed;

			//The API Checker normally unlocks every button spawning Http Modules.
			//But for this tutorial, we only wish to unlock the buttons to add a remote importer.
			//So we forcefully lock everything and unlock only the button to add a remote importer.
			guiManager.refModulesMenuManager.ForceGroupInteractibility(false);
			guiManager.refModulesMenuManager.ForceSingleInteractibility(1, true);
		}

		public override void Initialize()
		{
			guiManager = GameObject.
							  FindGameObjectWithTag("ExternalObjectContainer").
							  GetComponent<GUIManager>();

			httpAPIChecker = FindObjectOfType<HttpServerAPICheckModule>();
			httpAPIChecker.onAPICheck += APICheckProcessed;

			//We collect the API Checker module (added at the previous step)
			//to make sure that it is cleaned up when the user quits the tutorial.
			ModNavTutorialManager.tutorialGarbage.Add(httpAPIChecker.gameObject);
		}

		private void APICheckProcessed(bool _serverReached)
		{
			if (_serverReached)
			{
				bool _allHttpModulesInteractable = true;
				//We want to check that the user connected to a Kosmogora server
				//which implements all the API (i.e. all buttons for Http modules
				//are interactable).
				for (int i = 0; i < guiManager.refModulesMenuManager.targetGroup.Length; i++)
				{
					//Get the HttpServerBaseModule component of the module.
					//This is to check only the interactibility of the buttons of Http modules.
					HttpServerBaseModule httpServerBaseModule = guiManager.refModulesMenuManager.targetGroup[i].GetComponent<GameObjectConstructor>().refPrefab.GetComponent<HttpServerBaseModule>();
					if (httpServerBaseModule != null)
					{
						_allHttpModulesInteractable &= guiManager.refModulesMenuManager.targetGroup[i].interactable;
					}
				}
				allHttpModulesInteractable = _allHttpModulesInteractable;
			}

		}
	}

}
