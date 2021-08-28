using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECellDive.Utility;
using ECellDive.Utility.SettingsModels;
using ECellDive.INetworkComponents;

namespace ECellDive
{
    namespace NetworkComponents
    {
        public class NetworkGO : LivingObject, INetworkGO
        {
            public INetwork networkData { get; protected set; }

            public NetworkGOSettings networkGOSettingsModel;

            public Dictionary<int, GameObject> NodeID_to_NodeGO;
            public Dictionary<int, GameObject> EdgeID_to_EdgeGO;

            public override GameObject SelfInstance()
            {
                GameObject selfInstance =  base.SelfInstance();
                selfInstance.GetComponent<NetworkGO>().NodeID_to_NodeGO = new Dictionary<int, GameObject>();
                selfInstance.GetComponent<NetworkGO>().EdgeID_to_EdgeGO = new Dictionary<int, GameObject>();

                return selfInstance;
            }

            public void SetNetworkData(INetwork _INetwork)
            {
                networkData = _INetwork;
            }
        }
    }
}
