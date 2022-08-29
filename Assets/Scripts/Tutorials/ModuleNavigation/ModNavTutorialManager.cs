using System.Collections.Generic;
using UnityEngine;
using ECellDive.SceneManagement;
using ECellDive.UI;
using System.Collections;

namespace ECellDive.Tutorials
{
    /// <summary>
    /// The class controlling the chronology and logic of the
    /// tutorial on modules navigation (example with iJO1366).
    /// </summary>
    public class ModNavTutorialManager : TutorialManager
    {
        [Header("Local Tutorial Members")]
        public AssetScenesManager refAssetScenesManager;
        public ResurfaceManager refResurfaceManager;
        public static List<GameObject> tutorialGarbage = new List<GameObject>();
        private GUIManager guiManager;

        private IEnumerator CleanUp()
        {
            //In this tutorial, the user might have dived in a data module.
            //We try to bring him back to the main room by default knowing
            //that Resurface() handles the tests to make sure that there are
            //no problems even if the user hasn't dived into anything yet.
            refResurfaceManager.Resurface();

            yield return new WaitForSeconds(2f);

            //We destroy the gameobjects that may have been added to the scene
            //during the tutorial. Those are mainly modules such as the Remote
            //importer, the CyJson data module, the graph, etc...
            foreach (GameObject go in tutorialGarbage)
            {
                foreach (Transform child in go.transform)
                {
                    Destroy(child.gameObject);
                }
                Destroy(go);
            }

            refAssetScenesManager.UnloadScene(2);
        }

        protected override void Initialize()
        {
            base.Initialize();

            guiManager = GameObject.FindGameObjectWithTag("AllUIAnchor").GetComponent<GUIManager>();

            //Hide the main menu.
            guiManager.refMainMenu.SetActive(!guiManager.refMainMenu.activeSelf);

            //The user cannot interact with any of the buttons of the main menu.
            guiManager.refMainMenu.GetComponent<MainMenuManager>().SwitchGroupInteractibility();

            //Since we are going to progressively add modules in the scene,
            //we also deactivate the interactibility of the buttons in the 
            //modules menu.
            guiManager.refModulesMenuManager.SwitchGroupInteractibility();
        }

        public override void Quit()
        {
            //We make sure to reactivate interactibility of every buttons
            //of the UI menus that we deactivated at the begining of the
            //tutorial. Since the user can quit the tutorial anytime, we do so
            //indiscriminitaly.
            guiManager.refMainMenu.GetComponent<MainMenuManager>().ForceGroupInteractibility(true);
            guiManager.refModulesMenuManager.ForceGroupInteractibility(true);

            StartCoroutine(CleanUp());
        }
    }
}
