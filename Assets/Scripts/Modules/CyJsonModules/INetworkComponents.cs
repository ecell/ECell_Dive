using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

namespace ECellDive
{
    namespace INetworkComponents
    {
        public interface INode
        {
            int ID { get; set; }
            Vector3 position { get; set; }
            string NAME { get; set; }
            List<int> incommingEdges { get; set; }
            List<int> outgoingEdges { get; set; }
        }

        public interface IEdge
        {
            int ID { get; set; }
            int source { get; set; }
            int target { get; set; }
            string NAME { get; set; }
        }

        public interface ILayer
        {
            int index { get; set; }
            INode[] nodes { get; }
            IEdge[] edges { get; }

            void PopulateNodes();
            void PopulateEdges();
        }

        public interface IInterLayer
        {
            int index { get; set; }
            IEdge[] edges { get; }

            void PopulateEdges();
        }

        public interface INetwork
        {
            string name { get; }
            JObject networkData { get;}
            JArray nodes { get; }
            JArray edges { get;}
            ILayer[] layers { get;}
            IInterLayer[] interLayers { get;}

            void SetNodes();
            void SetEdges();
            void PopulateLayers();
            void PopulateInterLayers();

        }

        public interface INodeGO
        {
            INode nodeData { get; }
            string informationString { get; }
            void SetNodeData(INode _INode);
        }

        public interface IEdgeGO
        {
            IEdge edgeData { get; }
            string informationString { get; }
            float defaultStartWidth { get; }
            float defaultEndWidth { get; }
            LineRenderer refLineRenderer { get; }
            GameObject refBoxColliderHolder { get; }
            void SetEdgeData(IEdge _IEdge);
            void SetDefaultWidth(float _start, float _end);
            void SetCollider(Transform _start, Transform _end);
            void SetLineRenderer();
            void SetPosition(Transform _start, Transform _end);
        }

        public interface ILayerGO
        {
            ILayer layerData { get; }
            void SetLayerData(ILayer _ILayer);
        }

        public interface IInterLayerGO
        {
            IInterLayer interLayerData { get; }
            void SetInterLayerData(IInterLayer _IInterLayer);
        }

        public interface INetworkGO
        {
            INetwork networkData { get; }
            void SetNetworkData(INetwork _INetwork);

        }
    }
}
