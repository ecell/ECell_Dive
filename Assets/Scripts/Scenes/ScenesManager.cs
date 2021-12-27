using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using ECellDive.Modules;



namespace ECellDive
{
    namespace SceneManagement
    {
        [System.Serializable]
        public struct InstantiationData
        {
            public GameObject[] instantiationTable;
            public List<GameObject> modulesInstanceList;//ultimately in the same order as
                                                        //ModulesData.visibleModules
        }

        [System.Serializable]
        public struct DivingData
        {
            [Tooltip("The gameobject with the diving animator")]
            public Animator refAnimator;

            [Tooltip("The minimum time we wait for the dive.")]
            [Min(1f)] public float duration;

            public GameObject diveGrabHelper;

        }

        [System.Serializable]
        public struct RemoteGrabData
        {
            public XRRayInteractor leftInteractor;
            public XRRayInteractor rightInteractor;
        }

        public class ScenesManager : MonoBehaviour
        {
            public InstantiationData instantiationData;
            public DivingData divingData;
            public RemoteGrabData remoteGrabData;

            private void Awake()
            {
                ScenesData.refSceneManagerMonoBehaviour = this;
            }

            void Start()
            {
                InstantiateGOOfVisibleModules();
            }

            public void AddInstantiatedGOOfModuleData(GameObject _mdGO)
            {
                instantiationData.modulesInstanceList.Add(_mdGO);
            }

            public void CleanInstantiationList()
            {
                foreach (GameObject go in instantiationData.modulesInstanceList)
                {
                    Destroy(go);
                }
                instantiationData.modulesInstanceList.Clear();
            }

            /// <summary>
            /// Instantiation of a single module.
            /// </summary>
            /// <remarks>Useful when the user adds a module manually to
            /// a scene or when a new set of modules is built dynamically.</remarks>
            public GameObject InstantiateGOOfModuleData(ModuleData _md, Vector3 _pos)
            {
                GameObject mdGO = Instantiate(instantiationData.instantiationTable[_md.typeID],
                                                _pos,
                                                Quaternion.identity);
                instantiationData.modulesInstanceList.Add(mdGO);

                return mdGO;
            }

            public GameObject InstantiateGOOfModuleDataFromParent(ModuleData _md, Vector3 _pos, Transform _parent)
            {
                GameObject mdGO = Instantiate(instantiationData.instantiationTable[_md.typeID],
                                                _pos,
                                                Quaternion.identity,
                                                _parent);
                instantiationData.modulesInstanceList.Add(mdGO);

                return mdGO;
            }

            /// <summary>
            /// Group instantiation of the visible modules.
            /// </summary>
            /// <remarks>Useful when returning from a deeper scene.</remarks>
            public void InstantiateGOOfVisibleModules()
            {
                for (int i = 0; i < ModulesData.visibleModules.Count; i++)
                {
                    GameObject mdGO = Instantiate(instantiationData.instantiationTable[ModulesData.visibleModules[i].typeID],
                                                    ModulesData.modulesBankWorldPos[i],
                                                    Quaternion.identity);
                    instantiationData.modulesInstanceList.Add(mdGO);
                }
            }

            /// <summary>
            /// Called to instantiate a new scene upon diving in a module
            /// </summary>
            /// <param name="_rootModule"></param>
            public void NewScene(ModuleData _rootModule)
            {
                ModulesData.AddModule(_rootModule);
                CleanInstantiationList();
                InstantiateGOOfModuleData(_rootModule, Vector3.zero);

                divingData.refAnimator.SetTrigger("DiveEnd");
            }
        }
    }
}

