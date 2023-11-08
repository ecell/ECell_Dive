using UnityEngine;
using UnityEngine.UI;
using ECellDive.UI;

namespace ECellDive.Tutorials
{
    /// <summary>
    /// The step 6 of the Tutorial on Modules Navigation.
    /// Add an FBA module.
    /// </summary>
    public class ModNavStep6 : Step
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
        }

        public override void Initialize()
        {
            base.Initialize();

            guiManager = GameObject.
                              FindGameObjectWithTag("ExternalObjectContainer").
                              GetComponent<GUIManager>();

            //The user can interact with the button to add a FBA module to the scene.
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