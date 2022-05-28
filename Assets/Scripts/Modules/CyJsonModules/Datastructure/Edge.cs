using ECellDive.Interfaces;

namespace ECellDive
{
    namespace NetworkComponents
    {
        public class Edge : IEdge
        {
            public int ID { get; set; }
            public int source { get; set; }
            public int target { get; set; }
            public string NAME { get; set; }

            public Edge(int _ID, string _name, int _source, int _target)
            {
                ID = _ID;
                source = _source;
                target = _target;
                NAME = _name;
            }
        }
    }
}
