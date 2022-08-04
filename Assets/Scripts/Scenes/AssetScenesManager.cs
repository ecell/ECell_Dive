using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace ECellDive.SceneManagement
{
    /// <summary>
    /// The logic to handle switching between different scenes assets.
    /// </summary>
    /// <remarks>
    /// Not synchronized with multiplayer (i.e. it makes use of <see cref=
    /// "UnityEngine.SceneManagement.SceneManager"/> and not <see cref=
    /// "Unity.Netcode.NetworkSceneManager"/>). Usefull to switch between
    /// scenes that are only supposed to be single player like some tutorials.
    /// </remarks>
    public class AssetScenesManager : MonoBehaviour
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
            SceneManager.LoadScene(scenes[_sceneIdx].name);
        }
    }
}

