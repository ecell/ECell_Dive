using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECellDive.Utility;
using ECellDive.INetworkComponents;


namespace ECellDive
{
    namespace NetworkComponents
    {
        public class LayerGO : LivingObject, ILayerGO
        {
            public ILayer layerData { get; set; }
            public void SetLayerData(ILayer _ILayer)
            {
                layerData = _ILayer;
            }
        }
    }
}

