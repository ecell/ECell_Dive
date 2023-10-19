Putting aside the scenes dedicated to the tutorials, there is only one scene where everything happens (even diving from one dive scene to another)

# Main scene of the project
The following is a screenshot of the content of the `Main` Unity scene.

<img src="~/resources/images/dev/Scenes/Hierarchy_Main.jpg" alt="Main Scene Hierarchy"/>

## Directional Light
A gameobject with unique component (`Light`) provided by Unity to setup a default lighting of the scene.

## Event System
A gameobject with mandatory Unity components to enable the input detection for XR controllers. The first (`EventSystem`) makes sure the scene is compatible with Unity [Input System](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.7/manual/index.html) which is the latest recommended API to listen to any kind of controllers. The second (`XRUIInputModule`) sets up some global values to control the click speed, move deadzone, repeat delay, and so on.

## Scene Manager
This is the gameobject that manages any scene transition including the transition between scene assets (e.g., main, tutorials, demo) and the dive scenes (which all exists within the main scene). Transitioning between dive scenes implies sending data over the multiplayer network so it is mandatory to attach a `NetworkObject` component the scene manager. This is one of the major component provided by Unity's [Netcode for GameObjects](https://docs-multiplayer.unity3d.com/netcode/current/basics/networkobject/) package. Then the custom components are:
- [AssetScenesManager](xref:ECellDive.SceneManagement.AssetScenesManager) which handles the transitions between the main scene and the tutorials or the demo scene.
- [DiveScenesManager](xref:ECellDive.SceneManagement.DiveScenesManager) which handles diving and "resurfacing" (returning to the previous scene).
- [ResurfaceManager](xref:ECellDive.SceneManagement.ResurfaceManager) which listens to controllers input to detect when a user wishes to return to the dive scene he was before the current one.

## Input Action Manager
A gameobject with a unique component (`InputActionManager`) part of the of the [Input System](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.7/manual/index.html) which task is to enable every input actions defined into the action assets specified. In the project, there is only one such asset located at `Assets/Input/Controls`. If actions are not enabled at the start, then any subsequent subscription to the action events `started`, `performed`, or `canceled` will never be invoked. In is possible to micro-manage enable/disable of every action and in the action assets, but we do not need to do so in this project so it is much more convenient to enable everything in one go.

## Network Manager
The base gameobject to support the multiplayer network. It has a component of the same name (`NetworkManager`) which handles the spawn of game objects that must be replicated in the session of every client. Currently, this includes the `Player`, the `CyjsonModule`, the `CyJsonNodeModule` and the `CyJsonEdgeModule`. its second component is `UnityTransport` which is the solution from Unity to define the lowlevel data exchange and communication on the network.

It is possible to implement our own transport solution and to replace the default one. It might prove useful to facilitate sharing big model files within the multiplayer network but it is not a priority.

## Game Net Portal
The gameobject with custom components which rely on the `NetworkManager` to implement the host/client architecture for _ECellDive_. The components are:
- [GameNetPortal](xref:ECellDive.Multiplayer.GameNetPortal) to manage the communications the creation of a host as well as communication of incomming new clients.
- [ClientGameNetPortal](xref:ECellDive.Multiplayer.ClientGameNetPortal) to manage the details for a client trying to connect to a host.
- [ServerGameNetPortal](xref:ECellDive.Multiplayer.ServerGameNetPortal) to manage the details for a host client sorting out the clients trying to connect to it.

## Game Net Spawner
The gameobject with a custom component [GameNetModuleSpawner](xref:ECellDive.Multiplayer.GameNetModuleSpawner) of the same name which role is to link user UI interaction with the creation of new network object (game objects synchronized for every clients of the network).

## External Object Manager
The gameobject responsible for the management of global UI menus through the component [GUIManager](xref:ECellDive.UI.GUIManager). It also acts as a world anchor for UI Menus and modules that are not pinned to the Player's position.

# Diving System


## Dive scenes do not exist explicitly in the code