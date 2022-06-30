using System;
using System.Collections.Generic;
using ECellDive.Modules;

namespace ECellDive
{
    namespace SceneManagement
    {
        //public struct SceneData
        //{
        //    public int sceneID;
        //    public int nbModules;
        //}

        [Obsolete("Deprecated since the multiplayer update.")]
        public static class ScenesData
        {
            public static GameNetScenesManager refSceneManagerMonoBehaviour;

            public static List<SceneData> scenesBank = new List<SceneData>();

            public static SceneData activeScene = new SceneData //starts with default scene
            {
                sceneID = 0, //of index 0
                //nbModules = 2 //in the default scene we start with the root
                              //module and the floor
            };

            public static void DiveIn(ModuleData moduleData)
            {
                scenesBank.Add(activeScene);
                activeScene = new SceneData
                {
                    sceneID = scenesBank.Count,
                    //nbModules = 0
                };
                //refSceneManagerMonoBehaviour.DiveIn(moduleData);
            }

            public static void Resurface()
            {                
                activeScene = scenesBank[scenesBank.Count - 1];
                scenesBank.RemoveAt(scenesBank.Count - 1);

                //refSceneManagerMonoBehaviour.Resurface();
            }
        }
    }
}

