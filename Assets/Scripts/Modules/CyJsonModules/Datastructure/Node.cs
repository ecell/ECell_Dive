using System.Collections.Generic;
using UnityEngine;
using ECellDive.Interfaces;

namespace ECellDive
{
    namespace GraphComponents
    {
        public class Node : INode
        {
            //Interface members
            public uint ID { get; set; }
            public Vector3 position { get; set; }
            public string NAME { get; set; }
            public List<uint> incommingEdges { get; set; }
            public List<uint> outgoingEdges { get; set; }
            public bool isVirtual { get; set; }

            public Node(uint _ID, string _name, Vector3 _position, bool _isVirtual)
            {
                ID = _ID;
                position = _position;
                NAME = _name;
                incommingEdges = new List<uint>();
                outgoingEdges = new List<uint>();
                isVirtual = _isVirtual;
            }
        }
    }
}

