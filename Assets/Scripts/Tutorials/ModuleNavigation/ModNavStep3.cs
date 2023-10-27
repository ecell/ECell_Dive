using UnityEngine;
using UnityEngine.UI;
using ECellDive.UI;

namespace ECellDive.Tutorials
{
	/// <summary>
	/// The step 3 of the Tutorial on Modules Navigation.
	/// Add an Data importer to the scene.
	/// </summary>
	public class ModNavStep3 : Step
	{
		private GUIManager guiManager;
		private Button targetButton; //The button to add a data importer to the scene.
		private bool targetButtonSelected;

		public override bool CheckCondition()
		{
			return targetButtonSelected;
		}

		public override void Conclude()
		{
			base.Conclude();

			targetButton.onClick.RemoveListener(OnSelect);

			//We disable the button allowing to add a data importer to the scene.
			guiManager.refModulesMenuManager.SwitchSingleInteractibility(1);
		}

		public override void Initialize()
		{
			base.Initialize();

			guiManager = GameObject.
							  FindGameObjectWithTag("ExternalObjectContainer").
							  GetComponent<GUIManager>();

			targetButton = guiManager.refModulesMenuManager.targetGroup[1].GetComponent<Button>();
			targetButton.onClick.AddListener(OnSelect);
		}

		private void OnSelect()
		{
			targetButtonSelected = true;
		}
	}
}