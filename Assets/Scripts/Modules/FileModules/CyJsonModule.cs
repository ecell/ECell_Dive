using System.Collections;
using UnityEngine;
using ECellDive.IO;
using ECellDive.SceneManagement;

namespace ECellDive
{
    namespace Modules
    {
        /// <summary>
        /// Derived class with some more specific operations to handle
        /// CyJson modules.
        /// </summary>
        public class CyJsonModule : Module
        {
            public int refIndex { get; private set; }

            private void Start()
            {
                SetIndex(CyJsonModulesData.loadedData.Count - 1);

                SetName(CyJsonModulesData.loadedData[refIndex].name);

                InstantiateInfoTags(new string[] {$"nb layers: {CyJsonModulesData.loadedData[refIndex].layers.Length}\n"+
                                                  $"nb edges: {CyJsonModulesData.loadedData[refIndex].edges.Count}\n"+
                                                  $"nb nodes: {CyJsonModulesData.loadedData[refIndex].nodes.Count}"});
            }

            protected override IEnumerator DiveInC()
            {
                if (isFocused && !finalLayer)
                {
                    CyJsonModulesData.activeData = CyJsonModulesData.loadedData[refIndex];

                    ScenesData.refSceneManagerMonoBehaviour.divingData.refAnimator.SetTrigger("DiveStart");

                    yield return new WaitForSeconds(ScenesData.refSceneManagerMonoBehaviour.divingData.duration);

                    ScenesData.AddNewScene();
                    ModulesData.CaptureWorldPositions();
                    ModulesData.StashToBank();

                    ScenesData.activeScene = ScenesData.sceneDatas[ScenesData.sceneDatas.Count - 1];
                    ScenesData.refSceneManagerMonoBehaviour.NewScene(new ModuleData
                    {
                        typeID = 5,
                    });
                }
            }

            public void SetIndex(int _index)
            {
                refIndex = _index;
            }
        }
    }
}
