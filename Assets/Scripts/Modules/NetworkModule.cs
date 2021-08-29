using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ECellDive
{
    namespace Modules
    {
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
                    NetworkModulesManager.activeNetwork = NetworkModulesManager.loadedNetworks[refIndex];
                    Debug.Log("Network Module Dive In");

                    StartCoroutine(SwitchScene(1));
                }
            }
        }
    }
}
