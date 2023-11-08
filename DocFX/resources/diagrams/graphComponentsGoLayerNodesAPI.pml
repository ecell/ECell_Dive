@startuml "Graph Components GO Layer Nodes API"

interface INodeGO<NodeType where NodeType: INode> {
    NodeType nodeData
    string informationString
    void SetNodeData(NodeType _nodeData)
}

class Module
class GameNetModule
class NodeGO
class CyJsonNodeGO

Module <|-- NodeGO
INodeGO *-- NodeGO : <Node>

GameNetModule <|-- CyJsonNodeGO
INodeGO *-- CyJsonNodeGO : <CyJsonNode>

@enduml