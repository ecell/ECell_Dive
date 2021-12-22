using System.Collections;
using System.Collections.Generic;


namespace ECellDive
{
    namespace SceneManagement
    {
        public struct SceneData
        {
            public int sceneID;
            public int masterModuleID;//the ID of the module in the previous scene
                                      //which generated this scene.
            public int nbModules;
        }

        public static class ScenesData
        {
            public static List<SceneData> sceneDatas = new List<SceneData>()
            {
                new SceneData //starts with default scene
                {
                    sceneID = 0, //of index 0
                    masterModuleID = 0, //master module default index is 0
                    nbModules = 2 //in the default scen we start with the root
                                  //module and the floor
                }
            };
            public static SceneData activeScene = sceneDatas[0];

            public static void addSceneData(SceneData _sceneData)
            {
                sceneDatas.Add(_sceneData);
            }
        }
    }
}

