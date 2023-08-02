using UnityEngine;
using UnityEngine.UI;
using ECellDive.UI;
using ECellDive.Modules;

namespace ECellDive.Tutorials
{
    /// <summary>
    /// The step 6 of the Tutorial on Modules Navigation.
    /// Add a GroupBy Module and make groups.
    /// </summary>
    public class ModNavStep6 : Step
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

            //The user can interact with the button to open the group menu.
            guiManager.refMainMenu.GetComponent<MainMenuManager>().SwitchSingleInteractibility(1);

            GroupByModule GrpBM = FindObjectOfType<GroupByModule>();

            //We collect the FBA module (created at the previous step) to
            //make sure that it is cleaned up when the user quits the tutorial.
            ModNavTutorialManager.tutorialGarbage.Add(GrpBM.gameObject);
        }

        public override void Initialize()
        {
            base.Initialize();

            guiManager = GameObject.FindGameObjectWithTag("ExternalObjectContainer").GetComponent<GUIManager>();

            //The user can interact with the button to add a Group By module to the scene.
            guiManager.refModulesMenuManager.SwitchSingleInteractibility(4);
            targetButton = guiManager.refModulesMenuManager.targetGroup[4].GetComponent<Button>();
            targetButton.onClick.AddListener(OnSelect);
        }

        private void OnSelect()
        {
            targetButtonSelected = true;
        }
    }
}

