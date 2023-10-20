@startuml "Scene Management Dive"

box **Client**
actor Diver as d
participant DataModule as DMC1
participant DiveSceneManager as DSMC1
endbox

box **Server (Host)**
participant DiveSceneManager as DSMH
endbox

d -> DMC1: GenerativeDiveIn()\nDirectDiveIn()
DMC1 -> DSMC1: SwitchingServerRPC(rootSceneID,\ntargetSceneID, diverID)

DSMC1 --> DSMH: execution on server
DSMH -> DSMH: HideScene(rootSceneID,\ndiverID)
DSMH -> DSMH: ShowScene(targetSceneID,\ndiverID)
DSMH -> DSMH: UpdatePlayerData(rootSceneID,\ndiverID)

DSMH --> DSMC1 --: UpdatePlayerDataClientRPC(\nClientRpcParams)

DSMC1 -> DMC1 --: SceneSwitchIsFinished()

DMC1 -> d: You are in the\nnew dive scene

@enduml