using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ECellDive.SceneManagement;

namespace ECellDive
{
    namespace Modules
    {
        /*
         Modules Types:
            - 0: Root type. The type of the module containing the main room.
                 Only one ModuleData should use it.

            - 1: Floor type. A module to be able to move by teleportation.

            - 2: Http Server Modules Importer type. A module allowing
                 connection to a server through HTTP protocol and import
                 the data files stored on it. Example: .cyjson file.
            - 3: Http Server Analyses type. A module allowing connection
                 to a server through HTTP protocol and performs tasks remotely
                 Example: FBA analysis on a pathway stored in .cyjson.

            - 4: CyJson File type. A module encapsulating the logic and data
                 necessary dive and visualize a metabolic pathway stored with
                 the .cyjson extension.
            - 5: CyJson Pathway Root type. The module at the root of the structure
                 of a pathway.
            - 6: CyJson Pathway Layer type. The module corresponding to a layer
                 in a CyJson pathway. Contains Nodes and Edges modules.
            - 7: CyJson Pathway Node type. A node in the graph made from the
                 CyJson pathway.
            - 8: CyJson Pathway Edge type. An Edge between 2 nodes in the graph
                 made from a CyJson pathway.

         */

        [System.Serializable]
        public struct ModuleData
        {
            public int typeID;//used as an enum but we only keep the int
        }

        public static class ModulesData
        {
            #region - Tables -
            public static List<Vector3> modulesBankWorldPos = new List<Vector3>() { Vector3.zero, Vector3.zero};
            public static List<ModuleData> modulesBank = new List<ModuleData>();

            public static List<ModuleData> visibleModules = new List<ModuleData>()
            {
                //by default the master module of scene 0 and the floor ar visible.
                new ModuleData() //starts with the default master module of scene 0
                {
                    typeID = 0
                },

                new ModuleData() //starts with the floor of scene 0
                {
                    typeID = 1
                }
            };
            #endregion

            #region - Methods -
            /// <summary>
            /// Adds a module data to the visible modules
            /// </summary>
            public static void AddModule(ModuleData _md)
            {
                visibleModules.Add(_md);
                ScenesData.activeScene.nbModules++;
            }

            public static void LoadToVisible()
            {
                //We differentiate two cases with one "if" statement
                //First case where we are loading the modules of scene which
                //is not the root scene. Since, with this test, we know that
                //we are not at the root scene, there won't be any index errors
                //when using "i" this way.
                if (ScenesData.activeScene.sceneID > 0)
                {
                    for (int i = modulesBank.Count;
                          i > modulesBank.Count - ScenesData.activeScene.nbModules;
                          i--)
                    {
                        visibleModules.Insert(0, modulesBank[i]);
                    }
                    modulesBank.RemoveRange(modulesBank.Count - ScenesData.activeScene.nbModules + 1,
                                            ScenesData.activeScene.nbModules);
                }

                //Second case where we are loading the modules from the root
                //scene. We can load everything in one go.
                else
                {
                    foreach(ModuleData _md in modulesBank)
                    {
                        visibleModules.Add(_md);
                    }
                    modulesBank.Clear();
                }
                
            }

            /// <summary>
            /// Transfers all modules from the visibleModules list to the bank
            /// </summary>
            public static void StashToBank()
            {
                foreach (ModuleData _md in visibleModules)
                {
                    modulesBank.Add(_md);
                }
                visibleModules.Clear();
            }

            /// <summary>
            /// Retains the positions of the gameObjects of every instantiated module.
            /// </summary>
            /// <remarks>Useful when switching between scenes to restore the
            /// modules at their last seen places.</remarks>
            public static void CaptureWorldPositions()
            {
                foreach (GameObject _mdGO in ScenesData.refSceneManagerMonoBehaviour.instantiationData.modulesInstanceList)
                {
                    modulesBankWorldPos.Add(_mdGO.transform.position);
                }
            }
            #endregion
        }
    }
}
