@startuml "Multiplayer Spawn Broadcast Example"

box client //i//
actor diver as di
participant dataModule as DMi
participant "New Dive Scene Content" as NDSCi
endbox

box Server (Host)
participant dataModule as DMS
participant "Dive Scene Manager" as DSMS
participant "New Dive Scene Content" as NDSCS
endbox

box client //j//
participant "New Dive Scene Content" as NDSCj
participant dataModule as DMj

endbox

di -> DMi++: GenerativeDiveIn

DMi --> DMS: RequestSourceDataGenerationServerRpc(clientID)

note over DMi
Wait Until isReadyForDive
end note

DMS -> DSMS: AddNewDiveScene(rootSceneID) 

== CyJsonModule ==

DMS -> DMS ++ : RequestGraphGenerationServerRpc(\nclientID, targetSceneID)

DMS -> DSMS : SpawnModuleInScene(_rootSceneId, rootGO)
DSMS -> NDSCS ++: Spawn Root
NDSCS --> NDSCi ++: Replicate Root\n(order unknown)
NDSCS --> NDSCj ++: Replicate Root\n(order unknown)

DMS -> DMS ++ : NodesBatchSpawn(targetSceneID)

loop nbNodeBatch
    loop nodeBatchSize
        DMS -> DSMS: SpawnModuleInScene(targetSceneID, nodeGO)
        DSMS -> NDSCS ++: Spawn Node
        NDSCS --> NDSCj ++: Replicate Node\n(order unknown)
        NDSCS --> NDSCi ++: Replicate Node\n(order unknown)
    end
end
deactivate DMS

DMS -> DMS ++ : EdgesBatchSpawn(targetSceneID)

loop nbEdgeBatch
    loop edgeBatchSize
        DMS -> DSMS: SpawnModuleInScene(targetSceneID, EdgeGO)
        DSMS -> NDSCS ++: Spawn Edge
        NDSCS --> NDSCi ++: Replicate Edge\n(order unknown)
        NDSCS --> NDSCj ++: Replicate Edge\n(order unknown)
    end
end
deactivate DMS
deactivate DMS

DMS --> DMj: IsReadyForDive\n(order unknown)
DMS --> DMi: IsReadyForDive\n(order unknown)

DMi -> DMi: DirectDiveIn

@enduml