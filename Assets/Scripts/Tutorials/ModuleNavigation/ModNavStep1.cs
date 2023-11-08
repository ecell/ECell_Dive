using UnityEngine;
using UnityEngine.UI;
using ECellDive.UI;

namespace ECellDive.Tutorials
{
    /// <summary>
    /// The step 1 of the Tutorial on Modules Navigation.
    /// Add an API Checker to the scene.
    /// </summary>
    public class ModNavStep1 : Step
    {
        private GUIManager guiManager;
        private Button targetButton; //The button to add an API Checker to the scene.
        private bool targetButtonSelected;

        public override bool CheckCondition()
        {
            return targetButtonSelected;
        }

        public override void Conclude()
        {
            base.Conclude();

            targetButton.onClick.RemoveListener(OnSelect);

            //We disable the button allowing to add an API Checker to the scene.
			guiManager.refModulesMenuManager.SwitchSingleInteractibility(0);
		}

        public override void Initialize()
        {
            base.Initialize();

            guiManager = GameObject.
                              FindGameObjectWithTag("ExternalObjectContainer").
                              GetComponent<GUIManager>();

            //The user can interact with the button leading to the Module Menu
            guiManager.refMainMenu.GetComponent<MainMenuManager>().SwitchSingleInteractibility(0);

            //The user can interact with the button leading to the Log Menu
            guiManager.refMainMenu.GetComponent<MainMenuManager>().SwitchSingleInteractibility(5);

            //The user can interact with the button to add an API Checker to the scene.
            guiManager.refModulesMenuManager.SwitchSingleInteractibility(0);
            targetButton = guiManager.refModulesMenuManager.targetGroup[0].GetComponent<Button>();
            targetButton.onClick.AddListener(OnSelect);
        }

        private void OnSelect()
        {
            targetButtonSelected = true;
        }
    }
}