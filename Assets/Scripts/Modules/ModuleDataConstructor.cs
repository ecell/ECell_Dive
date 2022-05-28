using UnityEngine;
using ECellDive.SceneManagement;
using ECellDive.Utility;

namespace ECellDive
{
    namespace Modules
    {
        /// <summary>
        /// Public interface to add module data on callback in
        /// in the Unity Editor.
        /// </summary>
        public class ModuleDataConstructor : MonoBehaviour
        {
            public ModuleData moduleData;

            public void Constructor()
            {
                Vector3 pos = Positioning.PlaceInFrontOfTargetLocal(Camera.main.transform, 2f, 0.3f);
                ModulesData.AddModule(moduleData);
                ScenesData.refSceneManagerMonoBehaviour.InstantiateGOOfModuleData(moduleData, pos);
            }
        }
    }
}

