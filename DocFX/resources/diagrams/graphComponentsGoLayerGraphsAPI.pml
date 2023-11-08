@startuml "Graph Components GO Layer Graphs API"

interface IGraphGO<EdgeType where EdgeType: IEdge\n NodeType where NodeType: INode> {
    IGraph<EdgeType, NodeType> graphData
    List<GameObject> graphPrefabsComponents
    Dictionary<uint, GameObject> DataID_to_DataGO
    void SetGraphData(IGraph<EdgeType, NodeType> _graphData)
}

interface IGraphGONet<EdgeType where EdgeType: IEdge\n NodeType where NodeType: INode> {
    GraphBatchSpawning graphBatchSpawning
    void RequestGraphGenerationServerRpc(ulong _expeditorClientId, int _rootSceneId)
}

interface IModifiable
interface ISaveable

IGraphGO <|-- IGraphGONet

class Module
class GameNetModule

class CyJsonModule{
    // The field for the property graphData
    // in IGraphGO 
    - ContiguousGraph m_graphData
    ...
}
class DiveTravelMapModule{
    // The field for the property graphData
    // in IGraphGO 
    - CyJsonPathway m_graphData
    ...
}

Module <|-- DiveTravelMapModule
IGraphGO *-- DiveTravelMapModule : <Edge, Node>

GameNetModule <|-- CyJsonModule
IGraphGONet *-- CyJsonModule : <CyJsonEdge, CyJsonNode>
IModifiable *-- CyJsonModule
ISaveable *-- CyJsonModule

@enduml