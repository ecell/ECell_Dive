using UnityEngine;
using ECellDive.UI;

namespace ECellDive.Tutorials
{
    /// <summary>
    /// The class controlling the chronology and logic of the
    /// tutorial on modules navigation (example with iJO1366).
    /// </summary>
    public class ModNavTutorialManager : TutorialManager
    {
        protected override void Initialize()
        {
            base.Initialize();

            GUIManager guiManager = GameObject.
                                        FindGameObjectWithTag("AllUIAnchor").
                                        GetComponent<GUIManager>();

            //Hide the main menu
            guiManager.refMainMenu.SetActive(!guiManager.refMainMenu.activeSelf);

            //The user cannot interact with any of the buttons of the main menu
            guiManager.refMainMenu.GetComponent<MainMenuManager>().SwitchGroupInteractibility();
        }
    }
}
