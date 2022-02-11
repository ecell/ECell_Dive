using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ECellDive
{
    namespace Utility
    {
        public class PerLayerCulling : MonoBehaviour
        {
            public float[] layerCulling = new float[32];
            void Start()
            {
                Camera camera = GetComponent<Camera>();
                camera.layerCullDistances = layerCulling;
            }
        }
    }
}

