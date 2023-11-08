@startuml "Multiplayer Host-Join"

actor Player as p
participant "Multiplayer Menu" as MM
participant "Multiplayer Module" as MMod
participant GameNetPortal as GNP
participant ClientGameNetPortal as CGNP
participant ServerGameNetPortal as SGNP
participant NetworkManager as NM

p -> MM : Input Server Information
p -> MM : Buttons Host/Join
MM -> MMod : OnConnectionStart
MMod -> MMod : Start Animation
MM -> GNP : SetConnectionSettings

alt "Host Session"
  MM -> GNP : StartHost

  alt "Is Host"
    GNP -> GNP : SetConnectionPayload
    GNP -> GNP : Restart
    GNP -> NM : ShutDown
    note over GNP
    Wait Until NetworkManager
    is not listening
    end note
  end
  GNP -> NM : StartHost

else "Join Session"
  MM -> GNP : StartClient
  GNP -> CGNP : StartClient
  CGNP -> CGNP : SetConnectionPayload
  CGNP -> CGNP : Restart
  CGNP -> NM : ShutDown
  note over CGNP
  Wait Until NetworkManager
  is not listening
  end note
  CGNP -> NM : StartClient
end

NM -> SGNP : ConnectionApprovalCallback
SGNP -> SGNP : ApprovalCheck

alt #LightGreen "Connection Success"
  SGNP -> NM : ConnectionApproved(true)
  NM -> MMod : OnConnectionSuccess
  MMod -> MMod : Green Color Flash

  NM -> GNP : OnClientConnectedCallback
  GNP -> GNP : OnNetworkReady

  GNP -> CGNP : OnNetworkReady
  alt Is Not CLient
    CGNP -> CGNP : Disable
    Destroy CGNP 
  end

  GNP -> SGNP : OnNetworkReady
  alt Is Not Server
    SGNP -> SGNP : Disable
    Destroy SGNP 
  end

else #Pink "Connection Failure"
  SGNP -> NM : ConnectionApproved(false)
  NM -> MMod : OnConnectionFailure
  MMod -> MMod : Red Color Flash
  group ref
  GNP -> GNP : Restart Self Hosting
  end
end

@enduml