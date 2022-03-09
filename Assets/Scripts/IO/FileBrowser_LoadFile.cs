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
            public ModulesManager refModulesManager;
            public GameObject refFileBrowser;
            public Utility.SettingsModels.Data data;

            /// <summary>
            /// The coroutine used to handle the Runtime File Browser.
            /// </summary>
            private IEnumerator LoadFileC()
            {
                yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, initialPath: data.DefaultPath);

                if (FileBrowser.Success)
                {
                    string _path = FileBrowserHelpers.GetDirectoryName(FileBrowser.Result[0]);
                    string _name = FileBrowserHelpers.GetFilename(FileBrowser.Result[0]);

                    LoadingHandler(_path, _name);
                }
            }

            /// <summary>
            /// Method assigned in the Editor and called back on the import button click event.
            /// Shows the runtime file browser and places it in front of the main camera
            /// </summary>
            public void LoadFile()
            {
                refFileBrowser.SetActive(true);

                //Reset position to be in front of the user's camera
                //and correctly rotated.
                refFileBrowser.transform.position = Positioning.PlaceInFrontOfTarget(Camera.main.transform, 1.5f, 0.8f);
                Positioning.UIFaceTarget(refFileBrowser, Camera.main.transform);

                FileBrowser.SetFilters(true,
                                        new FileBrowser.Filter("Cytoscape", ".cyjson", ".cyjs"));
                FileBrowser.SetDefaultFilter(".cyjson");

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
                
                if (name_split[name_split.Length - 1] == "cyjson" ||
                    name_split[name_split.Length - 1] == "cyjs")
                {
                    //Loading the file
                    NetworkComponents.Network network = NetworkLoader.Initiate(_path, _name);

                    //Instantiating relevant data structures to store the information about
                    //the layers, nodes and edges.
                    NetworkLoader.Populate(network);
                    
                    CyJsonModulesData.AddData(network);
                    refModulesManager.InstantiateModule(network);
                }
            }

            private void Start()
            {
                //Immediately hides the RuntimeFileBrowser UI after it has been loaded
                refFileBrowser.SetActive(false);
            }
        }
    }
}

