using System.Collections;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Unity.Netcode;
using ECellDive.IO;
using ECellDive.SceneManagement;
using ECellDive.Utility;

namespace ECellDive
{
    namespace Modules
    {
        /// <summary>
        /// Derived class with some more specific operations to handle
        /// CyJson modules.
        /// </summary>
        public class CyJsonModule : GameNetModule
        {
            public int refIndex { get; private set; }

            private Renderer refRenderer;
            private MaterialPropertyBlock mpb;
            private int colorID;

            private void OnEnable()
            {
                refRenderer = GetComponentInChildren<Renderer>();
                mpb = new MaterialPropertyBlock();
                colorID = Shader.PropertyToID("_Color");
                mpb.SetVector(colorID, defaultColor);
                refRenderer.SetPropertyBlock(mpb);
            }

            public void SetIndex(int _index)
            {
                refIndex = _index;
            }

            public void StartUpInfo()
            {
                SetIndex(CyJsonModulesData.loadedData.Count - 1);

                SetName(CyJsonModulesData.loadedData[refIndex].name);

                InstantiateInfoTags(new string[] {$"nb edges: {CyJsonModulesData.loadedData[refIndex].edges.Length}\n"+
                                                  $"nb nodes: {CyJsonModulesData.loadedData[refIndex].nodes.Length}"});
            }

            #region - IDive Methods -
            public override IEnumerator GenerativeDiveInC()
            {
                if (isFocused)// && !isFinalLayer)
                {
                    CyJsonModulesData.activeData = CyJsonModulesData.loadedData[refIndex];

                    yield return null;

                    ModulesData.CaptureWorldPositions();
                    ModulesData.StashToBank();

                    BroadcastIsReadyForDiveServerRpc();

                    ScenesData.DiveIn(new ModuleData
                    {
                        typeID = 5,
                    });
                }
            }
            #endregion

            #region - IHighlightable Methods -
            public override void SetHighlight()
            {
                mpb.SetVector(colorID, highlightColor);
                refRenderer.SetPropertyBlock(mpb);
            }

            public override void UnsetHighlight()
            {
                if (!forceHighlight)
                {
                    mpb.SetVector(colorID, defaultColor);
                    refRenderer.SetPropertyBlock(mpb);
                }
            }
            #endregion

            #region - IMlprDataExchange -
            public override void AssembleFragmentedData()
            {
                byte[] assembledSourceData = ArrayManipulation.Assemble(fragmentedSourceData);
                string assembledSourceDataName = System.Text.Encoding.UTF8.GetString(sourceDataName);

                JObject requestJObject = JObject.Parse(System.Text.Encoding.UTF8.GetString(assembledSourceData));

                //Loading the file
                NetworkComponents.Network network = NetworkLoader.Initiate(requestJObject,
                                                                           assembledSourceDataName);

                //Instantiating relevant data structures to store the information about
                //the layers, nodes and edges.
                NetworkLoader.Populate(network);
                CyJsonModulesData.AddData(network);
                
                StartUpInfo();

                isReadyForGeneration = true;
            }
            #endregion
        }
    }
}
