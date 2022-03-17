﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECellDive.Interfaces;

namespace ECellDive
{
    namespace NetworkComponents
    {
        public class Node : INode
        {
            //Interface members
            public int ID { get; set; }
            public Vector3 position { get; set; }
            public string NAME { get; set; }
            public List<int> incommingEdges { get; set; }
            public List<int> outgoingEdges { get; set; }

            //AdditionalFields
            public string keggNodeLabel;

            public Node(int _ID, string _name, Vector3 _position)
            {
                ID = _ID;
                position = _position;
                NAME = _name;
                incommingEdges = new List<int>();
                outgoingEdges = new List<int>();
            }
        }
    }
}

