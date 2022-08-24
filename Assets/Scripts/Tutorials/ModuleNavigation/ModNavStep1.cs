using UnityEngine;
using UnityEngine.UI;
using ECellDive.UI;

namespace ECellDive.Tutorials
{
    /// <summary>
    /// The step 1 of the Tutorial on Modules Navigation.
    /// Add a remote importer to the scene.
    /// </summary>
    public class ModNavStep1 : Step
    {
        private GUIManager guiManager;
        private Button targetButton;
        private bool targetButtonSelected;

        public override bool CheckCondition()
        {
            return targetButtonSelected;
        }

        public override void Conclude()
        {
            base.Conclude();

            targetButton.onClick.RemoveListener(OnSelect);

            //We block the usage of the button to import a FBA module
            //since the user already added one.
            guiManager.refModulesMenuManager.SwitchSingleInteractibility(0);
        }

        public override void Initialize()
        {
            base.Initialize();

            guiManager = GameObject.
                              FindGameObjectWithTag("AllUIAnchor").
                              GetComponent<GUIManager>();

            //The user can interact with the button leading to the Module Menu
            guiManager.refMainMenu.GetComponent<MainMenuManager>().SwitchSingleInteractibility(0);

            //The user can interact with the button leading to the Log Menu
            guiManager.refMainMenu.GetComponent<MainMenuManager>().SwitchSingleInteractibility(4);

            //The user can interact with the button to add a remote importer to the scene.
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