using UnityEngine;
using UnityEngine.UI;
using ECellDive.UI;

namespace ECellDive.Tutorials
{
    /// <summary>
    /// The step 6 of the Tutorial on Modules Navigation.
    /// Add a GroupBy Module and make groups.
    /// </summary>
    public class ModNavStep6 : Step
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
            refGuiManager.refModulesMenuManager.SwitchSingleInteractibility(3);
            targetButton = refGuiManager.refModulesMenuManager.targetGroup[3].GetComponent<Button>();
            targetButton.onClick.AddListener(OnSelect);
        }

        private void OnSelect()
        {
            targetButtonSelected = true;
        }
    }
}

