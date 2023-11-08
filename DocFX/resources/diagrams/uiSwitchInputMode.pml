@startuml "UI Switch Input Mode"

participant "Physical Controller" as PC
participant "Switch Action" as SA

box "Virtual Controller"
participant "Input Mode Manager" as IMM
participant "Input Mode //i//" as IMI
participant "Input Mode //j//" as IMJ
participant "Buttons Information Tags" as BIT
endbox

activate IMI

PC -> SA: Button Pressed

SA -> IMM: action.performed
note over IMM
    controllerModeID++;
    if controllerModeID > 2
        controllerModeID = 0
end note

IMM -> IMI: deactivate action map\ndeactivate interactor
deactivate IMI

IMM -> IMJ ++: activate action map\nactivate interactor

IMM -> BIT: update text

@enduml