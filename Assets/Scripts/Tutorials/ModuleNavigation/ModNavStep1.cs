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
        }

        public override void Initialize()
        {
            base.Initialize();

            GUIManager refGuiManager = GameObject.
                              FindGameObjectWithTag("AllUIAnchor").
                              GetComponent<GUIManager>();

            //The user can interact with the button leading to the Module Menu
            refGuiManager.refMainMenu.GetComponent<MainMenuManager>().SwitchSingleInteractibility(0);

            //The user can interact with the button leading to the Log Menu
            refGuiManager.refMainMenu.GetComponent<MainMenuManager>().SwitchSingleInteractibility(4);


            //The user cannot interact with any of the buttons of the module menu
            refGuiManager.refModulesMenuManager.SwitchGroupInteractibility();

            //The user can interact with the button to add a remote importer to the scene.
            refGuiManager.refModulesMenuManager.SwitchSingleInteractibility(0);
            targetButton = refGuiManager.refModulesMenuManager.targetGroup[0].GetComponent<Button>();
            targetButton.onClick.AddListener(OnSelect);
        }

        private void OnSelect()
        {
            targetButtonSelected = true;
        }
    }
}