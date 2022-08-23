using UnityEngine;
using UnityEngine.UI;
using ECellDive.UI;

namespace ECellDive.Tutorials
{
    /// <summary>
    /// The step 4 of the Tutorial on Modules Navigation.
    /// Add an FBA module.
    /// </summary>
    public class ModNavStep4 : Step
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

            //The user can interact with the button to add a remote importer to the scene.
            refGuiManager.refModulesMenuManager.SwitchSingleInteractibility(2);
            targetButton = refGuiManager.refModulesMenuManager.targetGroup[2].GetComponent<Button>();
            targetButton.onClick.AddListener(OnSelect);
        }

        private void OnSelect()
        {
            targetButtonSelected = true;
        }
    }
}