using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECellDive.SceneManagement;

namespace ECellDive
{
    namespace Modules
    {
        /// <summary>
        /// Derived class with some more specific operations to handle
        /// network modules.
        /// </summary>
        public class NetworkModule : Module
        {
            public int refIndex { get; private set; }

            public void SetIndex(int _index)
            {
                refIndex = _index;
            }

            protected override void DiveIn()
            {
                if (isFocused)
                {
                    base.DiveIn();

                    //ModulesData.typeActiveModule = ModulesData.ModuleType.NetworkModule;
                    NetworkModulesData.activeData = NetworkModulesData.loadedData[refIndex];

                    StartCoroutine(Loading.SwitchScene(1, divingTime));
                }
            }
        }
    }
}
