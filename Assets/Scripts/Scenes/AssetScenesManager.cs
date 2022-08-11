using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace ECellDive.SceneManagement
{
    /// <summary>
    /// The logic to handle switching between different scenes assets.
    /// </summary>
    public class AssetScenesManager : NetworkBehaviour
    {
        public SceneAsset[] scenes;

        /// <summary>
        ///  Calls<see cref="SceneManager.LoadScene(string)"/>  to loads the
        ///  scene at index <paramref name="_sceneIdx"/> in <see cref="Scene"/>.
        /// </summary>
        /// <param name="_sceneIdx">The index of the scene we wish to load as ordered in
        /// <see cref="scenes"/></param>
        public void LoadScene(int _sceneIdx)
        {
            if (IsServer && NetworkManager.Singleton.ConnectedClientsIds.Count == 1)
            {
                //SceneManager.LoadScene(scenes[_sceneIdx].name);
                SceneEventProgressStatus status = NetworkManager.SceneManager.LoadScene(
                                                        scenes[_sceneIdx].name, LoadSceneMode.Single);

            }
        }
    }
}

