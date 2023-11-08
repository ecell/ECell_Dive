@startuml "Graph Components Data Layer API"

interface IEdge {
    uint ID
    string name
    uint source
    uint target
}

interface INode {
    uint ID
    string name
    List<uint> incommingEdges
    List<uint> outgoingEdges
}

interface IGraph<EdgeType where EdgeType: IEdge\n NodeType where NodeType: INode> {
    string name
    NodeType[] nodes
    EdgeType[] edges
}

struct Edge
struct Node
struct CyJsonEdge{
    + string reactionName
}
struct CyJsonNode{
    + Vector3 position
    + string label
    + bool isVirtual
}

class CyJsonPathway{
    + GraphData<JObject, JArray, JArray> cyJsonGraphData
    + CyJsonPathway(string _path, string _name)
    + CyJsonPathway(JObject _cyJspathway, string _name)
    + void MapInOutEdgesIntoNodes()
    + void PopulateNodes()
    + void PopulateEdges()
}

class ContinuousGraph{
    + ContiguousGraph(string _name)
    + void Populate(List<int> contiguousNodes)
}

IEdge --> IGraph::edges
INode --> IGraph::nodes

IEdge *-- Edge
INode *-- Node
IEdge *-- CyJsonEdge
INode *-- CyJsonNode
IGraph *-- CyJsonPathway : <CyJsonEdge, CyJsonNode>
IGraph *-- ContinuousGraph : <Edge, Node>

@enduml