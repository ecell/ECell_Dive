using UnityEngine;
using UnityEngine.UI;
using ECellDive.UI;
using ECellDive.Modules;

namespace ECellDive.Tutorials
{
    /// <summary>
    /// The step 4 of the Tutorial on Modules Navigation.
    /// Add an FBA module.
    /// </summary>
    public class ModNavStep4 : Step
    {
        private Button targetButton;
        private GUIManager guiManager;
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
            guiManager.refModulesMenuManager.SwitchSingleInteractibility(2);
        }

        public override void Initialize()
        {
            base.Initialize();

            guiManager = GameObject.
                              FindGameObjectWithTag("AllUIAnchor").
                              GetComponent<GUIManager>();

            //The user can interact with the button to add a FBA module to the scene.
            guiManager.refModulesMenuManager.SwitchSingleInteractibility(2);
            targetButton = guiManager.refModulesMenuManager.targetGroup[2].GetComponent<Button>();
            targetButton.onClick.AddListener(OnSelect);
        }

        private void OnSelect()
        {
            targetButtonSelected = true;
        }
    }
}