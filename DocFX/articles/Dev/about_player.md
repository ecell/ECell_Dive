The player gameobject (GO) in the project is at location `Assets/Resources/Prefabs/Player/Player`. This is the GO used to represent users. It has children GOs to define the `XRRig` (a sort of physics anchor for XR environments in Unity), the `Camera`, the `Head Model`, the `Movement Systems`, and the `Controllers`.

<img src="../../resources/images/dev/Player/GOHierarchy_Player.jpg" alt="Player GO Hierarchy"/>

# Player
The top parent GO `Player` has essential components to make it XR-compatible (`XRRig`) and visible on the multiplayer network (`NetworkObject`). It also has custom components to:
- manage its name and visibility on the network ([Player](xref:ECellDive.PlayerComponents.Player))
- to activate/deactivate children components depending on whether the instance of the gameobject is for the local client or a replicated player on the client's session ([NetworkSpawnComponentActivation](xref:ECellDive.Utility.Multiplayer.NetworkSpawnComponentActivation))
- to expose some children GOs or components to outside ([StaticReferencer](xref:ECellDive.Utility.PlayerComponents.StaticReferencer))

## Main Camera
Contains the `Camera` (and `UniversalAdditionalCameraData`) component to render the world from the view point of the player, an `AudioListener` to receive sounds, `TrackedPoseDriver` to track the position and rotation of the head of the player, `NetAvatarTransformTracker` to broadcast the position of the head on the multiplayer network. The only custom component in [PerLayerCulling](xref:ECellDive.Utility.PerLayerCulling) to control distance visibility of GOs in the scene that are part of some [layers](https://docs.unity3d.com/Manual/Layers.html).

### Diving Occulation
Manages the occultation animation of the camera when the player is diving but is **currently disabled**.

### Player Name Container
A container GO for the canvas and text mesh to write the name of this player GO. It is always deactivated for the local client, but it will be visible on its replicated version in the sessions of other clients. The [FaceCamera](xref:ECellDive.Utility.FaceCamera) and [AlwaysLookAt](xref:ECellDive.Utility.AlwaysLookAt) components are used in order for the container of a replicated player to always look at the camera of a local client. This way, the name of the replicated player is always visible. 

### Head
Is the simple 3D model of a head.

## Camera Floor Offset
An empty GO used to set the reference position of the floor (currently at `(0, 0, 0)`). It is a required field for `XRRig`.

## Controllers
The base GO to encapsulate the 3D models and input logic of the XR controllers. The custom components are:
- [InputModeManager](xref:ECellDive.Input.InputModeManager) to switch the input modes between the `Ray-Based Interactions`, the `Movement`, and the `Groups` controls (see the dedicated section in the [User Manual](../UserManual/Controls/controls.md)).
- [ContextualHelpManager](xref:ECellDive.UI.ContextualHelpManager) to link the information tags about each buttons to the switch of input modes. 
- [GroupsMakingManager](xref:ECellDive.PlayerComponents.GroupsMakingManager) to keep track of the objects manually selected to form a group with the discrete or volumetric selectors.

### Main Pointer
Empty GO to encapsulate the 3D models and and input logic of the XR controllers. 
**This is legacy structure and could be simplified**

#### MP Left
Everything that concerns the left controller. It has a `XRController (Acion-based)` component to link left controller input (position, rotation, buttons) to actions and a `NetAvatarTransformTracker` to broadcast its position to the replicated version on other clients' session.

##### Left Input Mode Surge Info Tag
An information label that surges and shrinks when switching input modes on the left controller to tell the player what input mode it is switching to. It contains and `Animation` component to drive the surge and shrink animations. The custom components are:
- [FaceCamera](xref:ECellDive.Utility.FaceCamera) and [AlwaysLookAt](xref:ECellDive.Utility.AlwaysLookAt) so that the information tag is always readable from the Player's view.
- [SurgeAndShrinkInfoTag](xref:ECellDive.UI.SurgeAndShrinkInfoTag) to trigger the animation and manage the content of the string. 

##### left_quest2_controller_world
The 3D models of the Quest 2 controller. Downloaded from the [open assets of Meta](https://developer.oculus.com/downloads/package/oculus-controller-art/#oculus-touch-for-quest-2). It is a modified version to with UI information tags attached to each button.

##### Input Modes Graphics Local Client
All renderers, 3D models, `XRRayInteractor`, and `XRController (Action-based)` to enable the local client to interact and navigate in the virtual environment. It is those children GOs that are Activated/Deactivated when the player switches between input modes on the left controller.

##### Input Modes Graphics Replicated Client
The `LineRenderer` to represent the line of the ray controls that will be visible by local clients when they look at the replicated player GOs of the other clients on the multiplayer network.

#### MP Right
The symetrical GO to MP Left for the right controller.

### XR Locomotion System
A GO with a unique component (`Locomotion System`) provided by Unity to help with moving the player's `XRRig` in the virtual environment.

### XR Teleportation Provider
A GO with a unique component (`Teleportation Provider`) provided by Unity to help teleport the player's `XRRig` in the virtual environment.

### Internal Object Container
An anchor GO with no component. It's only purpose is to provide a parent position for every items that are pinned to the player (e.g., UI menu or modules).

# Player_Debug
This version of the player can be found under the path `Assets/Resources/Prefabs/Player/Player_Debug`. It is a copy of `Player` with only an additional child GO named `XR Device Simulator`. It is provided by Unity to be able to manipulate the position of the head and the controllers with a Keyboard & Mouse. It is extremely practical to test and debug without the VR headset. Be sure that the Headset is **NOT** connected to the computer. Otherwise the inputs will collide and the behavior is undefined.