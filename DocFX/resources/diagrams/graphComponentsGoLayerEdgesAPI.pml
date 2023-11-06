@startuml "Graph Components GO Layer Edge API"
interface IEdgeGO<EdgeType where EdgeType: IEdge> {
    EdgeType edgeData
    string informationString
    float defaultStartWidth
    float defaultEndWidth
    GameObject refColliderHolder
    void ReverseOrientation()
    void SetEdgeData(EdgeType _edgeData)
    void SetDefaultWidth(float _start, float _end)
    void SetCollider(Transform _start, Transform _end)
    void SetLineRendererWidth()
    void SetLineRendererPosition(Transform _start, Transform _end)
}
interface IBezierCurve
interface IGradient
interface IModulateFlux

class Module
class GameNetModule
class EdgeGO
class CyJsonEdgeGO

Module <|-- EdgeGO
IEdgeGO *-- EdgeGO : <Edge>
IBezierCurve *-- EdgeGO
IGradient *-- EdgeGO

GameNetModule <|-- CyJsonEdgeGO
IEdgeGO *-- CyJsonEdgeGO : <CyJsonEdge>
IModulateFlux *-- CyJsonEdgeGO

@enduml