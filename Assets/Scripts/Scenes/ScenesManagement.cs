using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace ECellDive
{
    namespace SceneManagement
    {
        public struct SceneData
        {
            public int sceneID;
            public int nbModules;
        }

        public static class ScenesData
        {
            public static ScenesManager refSceneManagerMonoBehaviour;

            public static List<SceneData> sceneDatas = new List<SceneData>()
            {
                new SceneData //starts with default scene
                {
                    sceneID = 0, //of index 0
                    nbModules = 2 //in the default scene we start with the root
                                  //module and the floor
                }
            };
            public static SceneData activeScene = sceneDatas[0];

            public static void AddNewScene()
            {
                sceneDatas.Add(new SceneData
                {
                    sceneID = sceneDatas.Count,
                    nbModules = 0
                });
            }

            public static void AddSceneData(int _sceneID, int _nbModules)
            {
                sceneDatas.Add(new SceneData
                {
                    sceneID = _sceneID,
                    nbModules = _nbModules
                });
            }
        }
    }
}

