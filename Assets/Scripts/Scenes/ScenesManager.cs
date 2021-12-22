using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECellDive.Modules;


namespace ECellDive
{
    namespace SceneManagement
    {
        public class ScenesManager : MonoBehaviour
        {
            public GameObject[] instantiationTable;

            public List<GameObject> modulesInstanceList;//ultimately in the same order as
                                                        //ModulesData.visibleModules

            // Start is called before the first frame update
            void Start()
            {
                InstantiateGOOfVisibleModules();
            }

            private void InstantiateGOOfVisibleModule(int _mdIndex)
            {
                GameObject mdGO = Instantiate(instantiationTable[_mdIndex]);
                modulesInstanceList.Add(mdGO);
            }

            private void InstantiateGOOfVisibleModules()
            {
                foreach(ModuleData _md in ModulesData.visibleModules)
                {
                    GameObject mdGO = Instantiate(instantiationTable[_md.dataIndex]);
                    modulesInstanceList.Add(mdGO);
                }
            }

        }
    }
}

