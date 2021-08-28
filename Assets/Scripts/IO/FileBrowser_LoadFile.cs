using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using SimpleFileBrowser;
using ECellDive.Utility;
using ECellDive.Modules;

namespace ECellDive
{
    namespace IO
    {
        /// <summary>
        /// The class managing the file loading processing thanks to the 
        /// Runtime File Browser plugin.
        /// </summary>
        public class FileBrowser_LoadFile : MonoBehaviour
        {
            public GameObject refCamera;
            public GameObject refFileBrowser;
            public Utility.SettingsModels.Data data;
            
            public Utility.SettingsModels.ModulesTypesReferences modulesTypes;

            /// <summary>
            /// The coroutine used to handle the Runtime File Browser.
            /// </summary>
            private IEnumerator LoadFileC()
            {
                yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, initialPath: data.DefaultPath);
                Debug.Log("out");
                if (FileBrowser.Success)
                {
                    Debug.Log("Success");
                    string _path = FileBrowserHelpers.GetDirectoryName(FileBrowser.Result[0]);
                    string _name = FileBrowserHelpers.GetFilename(FileBrowser.Result[0]);

                    LoadingHandler(_path, _name);
                }
            }

            /// <summary>
            /// Instantiates the GameObject representing the loaded file in the scene.
            /// </summary>
            /// <param name="_network"> A NetworkComponents.Network instance.</param>
            /// <remarks>Overloaded method.</remarks>
            private void InstantiateModule(NetworkComponents.Network _network)
            {
                Vector3 pos = Positioning.PlaceInFrontOfTarget(refCamera.transform, 2f, 0.8f);
                GameObject networkModuleGO = Instantiate(modulesTypes.networkModule, pos, Quaternion.identity);
                networkModuleGO.SetActive(true);
                NetworkModule networkModule = networkModuleGO.GetComponent<NetworkModule>();
                networkModule.SetName(_network.name);
                networkModule.SetIndex(NetworkModulesManager.loadedNetworks.Count - 1);
            }

            /// <summary>
            /// Method assigned in the Editor and called back on the import button click event.
            /// </summary>
            public void LoadFile()
            {
                refFileBrowser.SetActive(true);

                //Reset position to be in front of the user's camera
                //and correctly rotated.
                refFileBrowser.transform.position = Positioning.PlaceInFrontOfTarget(refCamera.transform, 1.5f, 0.8f);
                Positioning.UIFaceTarget(refFileBrowser, refCamera.transform);

                StartCoroutine(LoadFileC());
            }

            /// <summary>
            /// Handles the flow of code when a file has been selected
            /// </summary>
            /// <param name="_path">Folders directory</param>
            /// <param name="_name">Complete file name (with extension)</param>
            private void LoadingHandler(string _path, string _name)
            {
                string[] name_split = _name.Split('.');
                
                switch(name_split[name_split.Length - 1])
                {
                    case ("json"):
                        //Loading the file
                        NetworkComponents.Network network = NetworkLoader.Initiate(_path, _name);

                        //Instantiating relevant data structures to store the information about
                        //the layers, nodes and edges.
                        NetworkLoader.Populate(network);
                        
                        NetworkModulesManager.AddNetwork(network);
                        InstantiateModule(network);
                        break;
                }
            }

            private void Start()
            {
                //Immediately hiding the RuntimeFileBrowser UI after it has been loaded
                refFileBrowser.SetActive(false);
            }
        }
    }
}

