using UnityEngine;
using ECellDive.INetworkComponents;


namespace ECellDive
{
    namespace Modules
    {
        public class LayerGO : MonoBehaviour,
                                ILayerGO
        {
            public ILayer layerData { get; set; }

            public void Initialize(ILayer _ILayer)
            {
                SetLayerData(_ILayer);
                gameObject.name = $"Layer {layerData.index}";
            }

            public void SetLayerData(ILayer _ILayer)
            {
                layerData = _ILayer;
            }
        }
    }
}

