@startuml "Multiplayer Join Data Broadcast"


box "New Client"
participant Player as Pnc
participant DiveSceneManager as DSMnc
participant GameNetDataManager as GNDMnc
participant MlprData as MlprDatanc
endbox

box "Server (Host)"
participant DiveSceneManager as DSMs
participant GameNetDataManager as GNDMs
participant MlprData as MlprDatas
endbox

box "Already Connected Client //i//"
participant Player as Pac
participant GameNetDataManager as GNDMac
endbox

group OnNetworkSpawn [Execution order is unknown between\nPlayer and GameNetDataManager]
  Pnc -> GNDMnc : **UpdatePlayerNamesInContainers**\nsubscribes to\n**OnClientReceivedAllPlayerNetData**
  Pnc -> GNDMnc : **SpawnInDiveScene**\nsubscribes to\n**OnClientReceivedAllModules**
  GNDMnc -> GNDMnc : Clear
  GNDMnc --> GNDMs : SharePlayerNetDataServerRpc(newClientID,\nnewPlayerName, newPlayerGUID)
  GNDMnc --> GNDMs : ShareModuleDataServerRpc(newClientID)
  DSMnc --> DSMs : ShareSceneBankServerRpc(newClientID)
end

group SharePlayerNetDataServerRpc
  loop for all known players
    GNDMs --> GNDMnc : SharePlayerNetDataClientRpc(\nknownPlayerGUID, knownPlayerData)
  end
  alt if new player is not already known\n(i.e. not a reconnection)
    GNDMs --> GNDMac : SharePlayerNetDataClientRpc(\nnewPlayerGUID, newPlayerData)
  end
end

note over GNDMnc
if (nbPlayerDataReceived == nbTotPlayer)
    Trigger **OnClientReceivedAllPlayerNetData**
end note

note over GNDMac
if (nbPlayerDataReceived == nbTotPlayer)
    Trigger **OnClientReceivedAllPlayerNetData**
end note

group ShareModuleDataServerRpc
  GNDMs -> GNDMs : Start Coroutine

  loop for all IMlprData in dataModules
    GNDMs -> MlprDatas : SendSourceData(newClientID)
    loop for all data fragments
      MlprDatas --> MlprDatanc : SendFragment
    end
    note over GNDMs
    WaitUntil all MlprData
    fragments were received
    end note
  end
  GNDMs --> GNDMnc : OnClientReceivedAllModulesClientRpc
end
note over GNDMnc
  Trigger **OnClientReceivedAllModules**
end note

group ShareSceneBankServerRpc
  loop for all scene data
    DSMs --> DSMnc : ShareSceneBankClientRpc(sceneData)
  end
end


@enduml