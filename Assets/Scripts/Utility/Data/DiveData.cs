using System.Collections.Generic;
using UnityEngine;
using ECellDive.Modules;

namespace ECellDive.Utility.Data.Dive
{
    /// <summary>
    /// A simple struct to store data to manage the dive animation.
    /// </summary>
    [System.Serializable]
    public struct DivingAnimationData
    {
        [Tooltip("The gameobject with the diving animator")]
        public Animator refAnimator;

        [Tooltip("The minimum time we wait for the dive.")]
        [Min(1f)] public float duration;
    }

    /// <summary>
    /// A struct to keep track about what is hapenning in the dive.
    /// Which divers are in (or out) of the scene and the modules that are loaded.
    /// </summary>
    [System.Serializable]
    public struct SceneData
    {
        /// <summary>
        /// ID of the scene.
        /// </summary>
        public int sceneID;

        /// <summary>
        /// ID of the parent scene.
        /// </summary>
        public int parentSceneID;

        /// <summary>
        /// The name of the scene.
        /// </summary>
        public string sceneName;

        /// <summary>
        /// List of NetworkManager Client Ids that are present in the scene
        /// </summary>
        public List<ulong> inDivers;

        /// <summary>
        /// List of NetworkManager Client Ids that are NOT present in the scene
        /// </summary>
        public List<ulong> outDivers;

        /// <summary>
        /// List of Network Object. Some of them are potential seeds for child scenes.
        /// </summary>
        public List<GameNetModule> loadedModules;

        public SceneData(int _sceneID, int _parentSceneID, string _sceneName)
        {
            sceneID = _sceneID;
            parentSceneID = _parentSceneID;
            sceneName = _sceneName;
            inDivers = new List<ulong>();
            outDivers = new List<ulong>();
            loadedModules = new List<GameNetModule>();
        }

        /// <summary>
        /// Add the client id <paramref name="_diverClientId"/> to the the list of divers
        /// that are not in this scene (<see cref="outDivers"/>).
        /// </summary>
        /// <param name="_diverClientId">
        /// The client id of the diver to add to <see cref="outDivers"/>.
        /// </param>
        public void AddOutDiver(ulong _diverClientId)
        {
            outDivers.Add(_diverClientId);
        }

        /// <summary>
        /// Add the reference of <paramref name="_gameNetModule"/> to the list of loaded
        /// modules (<see cref="loadedModules"/>).
        /// </summary>
        /// <param name="_gameNetModule">
        /// The reference of the module to add to <see cref="loadedModules"/>.
        /// </param>
        public void AddModule(GameNetModule _gameNetModule)
        {
            loadedModules.Add(_gameNetModule);
        }

        /// <summary>
        /// Remove the client id <paramref name="_diverClientId"/> from <see cref="outDivers"/>
        /// and add it to <see cref="inDivers"/>.
        /// </summary>
        /// <param name="_diverClientId">
        /// The ID of the client getting in the scene.
        /// </param>
        public void DiverGetsIn(ulong _diverClientId)
        {
            outDivers.Remove(_diverClientId);
            inDivers.Add(_diverClientId);
        }

        /// <summary>
        /// Remove the client id <paramref name="_diverClientId"/> from <see cref="inDivers"/>
        /// and add it to <see cref="outDivers"/>.
        /// </summary>
        /// <param name="_diverClientId">
        /// The ID of the client getting out of the scene.
        /// </param>
        public void DiverGetsOut(ulong _diverClientId)
        {
            inDivers.Remove(_diverClientId);
            outDivers.Add(_diverClientId);
        }

        /// <summary>
        /// For DEBUG.
        /// Prints information about the scene.
        /// </summary>
        public override string ToString()
        {
            string inDiversStr = "";
            foreach (ulong id in inDivers)
            {
                inDiversStr += id.ToString() + " ";
            }
            inDiversStr += "\n";

            string outDiversStr = "";
            foreach (ulong id in outDivers)
            {
                outDiversStr += id.ToString() + " ";
            }
            outDiversStr += "\n";

            string loadedModulesStr = "";
            foreach (GameNetModule _gameNetMod in loadedModules)
            {
                loadedModulesStr += _gameNetMod.name.ToString() + " ";
            }
            loadedModulesStr += "\n";

            string final = $"Scene Id: {sceneID}\n" +
                            $"Parent Scene Id: {parentSceneID}\n" +
                            $"In Divers: " + inDiversStr +
                            $"Out Divers: " + outDiversStr +
                            $"Loaded modules: " + loadedModulesStr;

            return final;
        }
    }
}
