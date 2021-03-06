using System;
using System.Collections.Generic;
using UnityEngine;

using ECellDive.SceneManagement;

namespace ECellDive
{
    namespace Modules
    {
        /*
         Modules Types:
            - 0: Floor type. A module to be able to move by teleportation.

            - 1: CyJson File type. A module encapsulating the logic and data
                 necessary dive and visualize a metabolic pathway stored with
                 the .cyjson extension.
            - 2: CyJson Pathway Root type. The module at the root of the structure
                 of a pathway.
            - 3: CyJson Pathway Node type. A node in the graph made from the
                 CyJson pathway.
            - 4: CyJson Pathway Edge type. An Edge between 2 nodes in the graph
                 made from a CyJson pathway.
         */

        [Serializable, Obsolete("Deprecated since the multiplayer update")]
        public struct ModuleData
        {
            public int typeID;
        }

        [Obsolete("Deprecated since the multiplayer update.")]
        public static class ModulesData
        {
            #region - Tables -
            public static List<Vector3> modulesBankWorldPos = new List<Vector3>() { Vector3.zero, Vector3.zero};
            public static List<ModuleData> modulesBank = new List<ModuleData>();

            public static List<ModuleData> visibleModules = new List<ModuleData>()
            {
                //by default the master module of scene 0 and the floor are visible.
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
               //ScenesData.activeScene.nbModules++;
            }

            /// <summary>
            /// Clears <see cref="modulesBankWorldPos"/>.
            /// Useful to clean the stashed positions of the modules in the scene
            /// that we just resurfaced into.
            /// </summary>
            public static void ClearModulesBankWorldPos()
            {
                //modulesBankWorldPos.RemoveRange(modulesBankWorldPos.Count - ScenesData.activeScene.nbModules,
                //                                ScenesData.activeScene.nbModules);
            }

            /// <summary>
            /// Loads the modules of the newly activated scene from the 
            /// <see cref="modulesBank"/> list to the <see cref="visibleModules"/> list.
            /// </summary>
            /// <remarks>
            /// Clears the <see cref="visibleModules"/> list at the
            /// beginning by default
            /// </remarks>
            public static void LoadToVisible()
            {
                visibleModules.Clear();
                //We differentiate two cases with one "if" statement
                //First case where we are loading the modules of scene which
                //is not the root scene. Since, with this test, we know that
                //we are not at the root scene, there won't be any index errors
                //when using "i" this way.
                if (ScenesData.activeScene.sceneID > 0)
                {
                    //for (int i = modulesBank.Count;
                    //      i > modulesBank.Count - ScenesData.activeScene.nbModules;
                    //      i--)
                    //{
                    //    visibleModules.Insert(0, modulesBank[i]);
                    //}
                    //modulesBank.RemoveRange(modulesBank.Count - ScenesData.activeScene.nbModules + 1,
                    //                        ScenesData.activeScene.nbModules);
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
                //foreach (GameObject _mdGO in ScenesData.refSceneManagerMonoBehaviour.instantiationData.modulesInstanceList)
                //{
                //    modulesBankWorldPos.Add(_mdGO.transform.position);
                //}
            }
            #endregion
        }
    }
}
