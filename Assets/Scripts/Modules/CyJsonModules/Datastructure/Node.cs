using System.Collections.Generic;
using UnityEngine;
using ECellDive.Interfaces;

namespace ECellDive
{
    namespace GraphComponents
    {
        [System.Serializable]
        public class Node : INode
        {
            //Interface members
            [SerializeField] private uint m_ID;
            public uint ID { get => m_ID; set => m_ID = value; }

            [SerializeField] private Vector3 m_position;
            public Vector3 position { get => m_position; set => m_position = value; }

            [SerializeField] private string m_name;
            public string name { get => m_name; set => m_name = value; }

            [SerializeField] private string m_label;
            public string label { get => m_label; set => m_label = value; }

            [SerializeField] private List<uint> m_incommingEdges;
            public List<uint> incommingEdges { get => m_incommingEdges; set => m_incommingEdges = value; }

            [SerializeField] private List<uint> m_outgoingEdges;
            public List<uint> outgoingEdges { get => m_outgoingEdges; set => m_outgoingEdges = value; }

            [SerializeField] private bool m_isVirtual;
            public bool isVirtual { get => m_isVirtual; set => m_isVirtual = value; }

            public Node(uint _ID, string _label, string _name, Vector3 _position, bool _isVirtual)
            {
                ID = _ID;
                position = _position;
                label = _label;
                name = _name;
                incommingEdges = new List<uint>();
                outgoingEdges = new List<uint>();
                isVirtual = _isVirtual;
            }
        }
    }
}

