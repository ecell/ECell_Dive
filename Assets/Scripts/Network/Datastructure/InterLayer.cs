using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECellDive.INetworkComponents;

namespace ECellDive
{
    namespace NetworkComponents
    {
        public class InterLayer : IInterLayer
        {
            public int index { get; set; }
            public IEdge[] edges { get; set; }

            public void PopulateEdges()
            {

            }
        }
    }
}
