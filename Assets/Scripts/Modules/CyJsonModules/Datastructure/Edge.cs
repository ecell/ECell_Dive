using ECellDive.Interfaces;

namespace ECellDive
{
    namespace GraphComponents
    {
        public class Edge : IEdge
        {
            public uint ID { get; set; }
            public uint source { get; set; }
            public uint target { get; set; }
            public string name { get; set; }
            public string reaction_name { get; set; }

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
