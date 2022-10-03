using UnityEngine;
using ECellDive.Interfaces;

namespace ECellDive
{
    namespace GraphComponents
    {
        [System.Serializable]
        public class Edge : IEdge
        {
            [SerializeField] private uint m_ID;
            public uint ID { get => m_ID; set => m_ID = value; }

            [SerializeField] private uint m_source;
            public uint source { get => m_source; set => m_source = value; }

            [SerializeField] private uint m_target;
            public uint target { get => m_target; set => m_target = value; }

            [SerializeField] private string m_name;
            public string name { get => m_name; set => m_name = value; }

            [SerializeField] private string m_reaction_name;
            public string reaction_name { get => m_reaction_name; set => m_reaction_name = value; }

            public Edge(uint _ID, string _reaction_name, string _name, uint _source, uint _target)
            {
                ID = _ID;
                source = _source;
                target = _target;
                reaction_name = _reaction_name;
                name = _name;
            }
        }
    }
}
