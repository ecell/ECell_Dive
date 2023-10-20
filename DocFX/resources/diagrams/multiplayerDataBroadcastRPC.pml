@startuml "Multiplayer Data Broadcast Example"

box "Client //i//"
participant HttpServerImporterModule as HttpSIMCi
participant GameNetModuleSpawner as GNMSCi
participant DataModule as DMCi
endbox

box Server (Host)
participant GameNetModuleSpawner as GNMSS
participant DataModule as DMS
endbox

box "Client //j//"
participant DataModule as DMCj
endbox 

HttpSIMCi -> GNMSCi : RequestModuleSpawnFromData(\nmoduleTypeID, dataName, fragments)
GNMSCi --> GNMSS: RequestModuleSpawnServerRpc(\nmoduleTypeID, clientID)

GNMSS -> DMS++: Spawn
DMS --> DMCj++: Replication\n(order unknown)
DMS --> DMCi++: Replication\n(order unknown)

GNMSS -> DMS: ChangeOwnership(clientID)
note over DMS
new owner is "Client //i//"
end note

GNMSS --> GNMSCi: GiveNetworkObjectReferenceClientRpc(\ndataModuleNetRef, clientRpcParams)

GNMSCi -> DMCi: DirectReceiveSourceData(\ndataName, fragments)
DMCi -> DMCi: AssembleData

DMCi --> DMS: BroadcastSourceDataNameServerRpc(dataName)
DMS --> DMCj: BroadcastSourceDataNameClientRpc(dataName)

DMCi --> DMS: BroadcastSourceDataNbFragsServerRpc(nbTotFrags)
DMS --> DMCj: BroadcastSourceDataNbFragsClientRpc(nbTotFrags)

loop nbFrags
DMCi --> DMS: BroadcastSourceDataFragServerRpc(frag)
DMS --> DMCj: BroadcastSourceDataFragClientRpc(frag)
end

note over DMCj
if (nbFragsReceived == nbTotFrags)
    confirm + assemble
end note

DMCj --> DMS: ConfirmSourceDataReceptionServerRpc
DMCj -> DMCj: AssembleData

note over DMS
if (nbClientConfirmation == nbTotClient)
    data is ready for generation
end note

@enduml