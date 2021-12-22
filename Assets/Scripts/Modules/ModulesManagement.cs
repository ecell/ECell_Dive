using System.Collections;
using System.Collections.Generic;

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
            - 2: Http Server type. A module allowing connection to a server
                 through HTTP protocol.
            - 3: File type. A module representing a data file.

         */

        public struct ModuleData
        {
            public int moduleID;//a unique integer representing this module
            public int typeID;//used as an enum but we only keep the int
            public int dataIndex;//a pointer to the GameObject in the scene
                                 //manager's list of instantiated gameobjects
        }

        public static class ModulesData
        {
            #region - Tables -
            public static List<ModuleData> modulesBank = new List<ModuleData>();

            public static List<ModuleData> visibleModules = new List<ModuleData>()
            {
                //by default the master module of scene 0 and the floor ar visible.
                new ModuleData() //starts with the default master module of scene 0
                {
                    moduleID = 0,
                    typeID = 0,
                    dataIndex = 0
                },

                new ModuleData() //starts with the floor of scene 0
                {
                    moduleID = 1,
                    typeID = 1,
                    dataIndex = 1
                }
            };
            #endregion

            #region - Methods -
            /// <summary>
            /// Adds a module data to the visible modules
            /// </summary>
            public static void addModule(ModuleData _md)
            {
                visibleModules.Add(_md);
            }

            public static void loadToVisible()
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
            public static void stashToBank()
            {
                foreach (ModuleData _md in visibleModules)
                {
                    modulesBank.Add(_md);
                }
                visibleModules.Clear();
            }
            #endregion
        }
    }
}
