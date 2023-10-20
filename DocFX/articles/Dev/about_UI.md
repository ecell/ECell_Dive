The User Interface (UI) of the project is in a state of "means to an end". It has been tweaked enough to work and it is not very well organized.

## The remote Interaction with Rays
input mode manager
The organization of input switch
Grab Manager
how do I make use of the XRInteractable and XRGrabInteractors just enough to detect the collisions but the rest is custom.

## 2D UI Menus
All 2D UI panel menus are hand-made. Until this point during development, it has been more cost-efficient to duplicate already existing 2D UI items than to spend time making a procedural script that could generate some base menus. As a consequence there is quite a number of 2D UI items in the projects that resemble each other and have only been slightly modified.

### In the UI folder
The UI menus are at the path `Assets/Resources/UI`.

<img src="~/resources/images/dev/UIAssets/UIAssets.jpg" alt="UI Assets" style="width: 300px;"/>

- The `External Object Container` is the prefab holding the [GUIManager](xref:ECellDive.UI.GUIManager) that is mandatory for our the Main scene.
- `Info Display` is an example of a 2D UI panel with a `LineRenderer` that can make a connection with another object to make them seem attached and related. That's what is used on the demonstration gameobject (GO) `BaseModule` found at `Assets/Resources/Modules`
- `Info Tag` is a specialization of `Info Display` to inform about the actions bound to the buttons of the XR controllers. It has the component [InfoTagManager](xref:ECellDive.UI.InfoTagManager) to control its content based on the current input mode of the controller.
- `Picker 2.0 VR` is color picker downloaded from the Unity Asset Store adapted to work in an XR scene.
- `Picker 2.0 VR Holder` is simply a `Picker 2.0 VR` encapsulated into a GO that can react to user's interaction to move it ([GrabManager](xred:ECellDive.PlayerComponents.GrabManager)) and automatically rotate to be readable from the point of view of the user ([FaceCamera](xref:ECellDive.Utility.FaceCamera)). This one should probably be deleted and replaced with a compound scene GO made from the `Picker 2.0 VR` and `UI Graphics Holder`.
- `Surge Info Tag` is a specialized info tag with a _surge and shrink_ animation. It is used on the left and right controllers to momentarily inform the users about the input mode they are switching to. 
- `UI Graphics Holder` is a container prefab in which to put (as child) 2D UI panels that users can move and should automatically face the user to be readable.

### In the UI/Elements folder
In this folder are some smaller 2D UI elements that are used to make bigger 2D UI menus.

<img src="~/resources/images/dev/UIAssets/ElementsAssets.jpg" alt="Elements UI" style="width: 300px;"/>

- `Button Scroll List` is an vertical UI with a scroll bar in which `Menu Button` can be instantiated. It uses [OptimizedVertScrollList](xref:ECellDive.UI.OptimizedVertScrollList) to be able to display a lot of elements while limiting performance hit (**but needs further improvements**).
- `Close Button` is a small square cross button that can be use to suggest a way to close 2D UI menus or destroy modules.
- `Drop Down Field` is a button with a name and an arrow sign that switches orientation on button click to suggest that some content can be opened/closed. 
- `Drop Down Scroll List` is an vertical UI with a scroll bar in which `Drop Down Field` can be instantiated. It uses [OptimizedVertScrollList](xref:ECellDive.UI.OptimizedVertScrollList) to be able to display a lot of elements while limiting performance hit (**but needs further improvements**).
- `Group Field` is specialized to display groups in a list. It is a composition of a `Close Button` (to destroy the group), a toggle (to show/hide the group), a color button (triggers opening the `Picker 2.0 VR Holder` to chose the color of the group)
- `Groups Scroll List` is an vertical UI with a scroll bar in which `Group Field` can be instantiated. It uses [OptimizedVertScrollList](xref:ECellDive.UI.OptimizedVertScrollList) to be able to display a lot of elements while limiting performance hit (**but needs further improvements**).
- `Menu Button` is a basic button. Just added a [UIHover](xref:ECellDive.UI.UIHover)
- `Named Input Field` & `Named Input Field 2` are a composition of a text mesh on the left and an input field on the right. The main difference is the appearance: they have different default values of `Pixel Per Unit Multiplier` in the `Image` components.
- `Position Handle` is the prefab to the white horizontal bar handle that is present on almost every other 2D UI to allow user to move the 2D UI they are part of.
- `Semantic Group Field` is a specialized `Group Field` to represent every groups that were grouped using a specific semantic.
- `Semantics Groups Scroll List` is an vertical UI with a scroll bar in which `Semantic Group Field` can be instantiated. It uses [OptimizedVertScrollList](xref:ECellDive.UI.OptimizedVertScrollList) to be able to display a lot of elements while limiting performance hit (**but needs further improvements**).
- `Slider Value Color Combo` is a composition of a name, a slider, a field to display the value, `+` and `-` button to slowly increment or decrement the value, and a color button to assign a color to this value. Two `Slider Value Color Combo` are used to linearly map a range of values between colors.
- `Slider Value Control` is a composition of a name, a slider, a field to display the value, `+` and `-` button to slowly increment or decrement the value. It is used to control numerical values.
- `Toggle Field` is a composition of a toggle on the left and a name on the right.
- `Toggle Scroll List` is an vertical UI with a scroll bar in which `Toggle Field` can be instantiated. It uses [OptimizedVertScrollList](xref:ECellDive.UI.OptimizedVertScrollList) to be able to display a lot of elements while limiting performance hit (**but needs further improvements**).

All scroll lists could certainly be refactored into one.

### In the UI/Menu Elements
In this folder are 2D UI composed by elements found in `UI/Elements`.

<img src="~/resources/images/dev/UIAssets/MenuElementsAssets.jpg" alt="Menu Elements UI" style="width: 300px;"/>

- `Buttons Menu` is a composition of `Button Scroll List` with a `Position Handle`, a name, a `Close Button` and a pin/unpin pair of buttons.
- `Drop Down Menu` is a composition of `Drop Down Scroll List` with a `Position Handle`, a name, a `Close Button` and a pin/unpin pair of buttons. Clicking a `Drop Down Field` in the scroll list will spawn a `Buttons Menu` with the content associated to the drop down.
- `Groups Menu` is a composition of `Groups Scroll List` with a `Position Handle`, a name, a `Close Button` and a pin/unpin pair of buttons.
- `Log Menu` is the menu where users can consult log messages. It is a composition of a `Button Scroll List`, a wide message space, toggles to control the visibility of the messages, a `Close Button` and a pin/unpin pair of buttons. [LogManager](xref:ECellDive.UI.LogManager) is attached to the root GO.
- `Main Menu` is a a custom display of buttons to open other menus. [MainMenuManager](xref:ECellDive.UI.MainMenuManager) is attached to the root GO.
- `Modules Menu` is a a custom display of buttons to spawn modules. [ModulesMenuManager](xref:ECellDive.UI.ModulesMenuManager) is attached to the root GO.
- `Multiplayer Menu` is a composition of various input fields and buttons to enter information about the multiplayer server to host or join. [MultiplayerMenuManager](xref:ECellDive.UI.ModulesMenuManager) is attached to the root GO.
- `Semantic Groups Menu` is a composition of `Semantic Groups Scroll List` with a `Position Handle`, a name, a `Close Button` and a pin/unpin pair of buttons. [GroupsMenu](xref:ECellDive.UI.GroupsMenu) is attached to the root GO.
- `Short Demo Menu` i is a a custom display of button to switch between aspects of the demo, to reset the visuals, or to quit the demo.
- `Toggles Menu` is a composition of `Toggle Scroll List` with a `Position Handle`, a name, a `Close Button` and a pin/unpin pair of buttons.
- `Tutorial Panel` is a custom display of buttons and text to give the tutorial instructions.
- `Tutorials Menu` is a custom display of buttons to open the tutorials and the demo. The buttons triggers an _Unity Scene_ change so their callbacks must be set in the main scene to use the [AssetScenesManager](xref:ECellDive.SceneManagement.AssetScenesManager).

Most of the menus are encapsulated in a `UI Graphics Holder` in the main scene to manage their translations and automatically face the user to be readable.

### In the UI/Tag Menus
In this folder are 2D UI elements that are attached to modules. So it is a combination of UI elements and the graphic link system to display a line between the menus and the modules they are attached to. They all use the [InfoDisplayManager](xref:ECellDive.UI.InfoDisplayManager).

<img src="~/resources/images/dev/UIAssets/TagMenusAssets.jpg" alt="Tag Menus UI" style="width: 300px;"/>

`Anchored Dynamic Content` encapsulates a `Button Scroll List` to display a list of options that may be associated to a module. The rest of the UI panel's names indicate to which module they are part of. `FBA Visuals Parameters` uses [FbaParametersManager](xref:ECellDive.UI.FbaParametersManager) and `GroupByAttributsDisplay` uses [GroupByAttributsManager](xref:ECellDive.UI.GroupByAttributsManager).

## Open comments
For one thing, UI in Virtual Reality, or Extended Reality in general, is not well defined. There are major trends such as using rays to point at and interact with remote objects; directly touch the 3D models with the controllers; pinch the fingers in the air. And so on. But, none of them seem to have convinced a majority of users to impose itself for a long time. There are still a lot of dreams about future haptic technologies and hand movement recognitions that the field lives with the expectations that something better will eventually popup. Bottom line, there is work to do.

### Toward fully interactive 3D modules for the UX of ECellDive?
- object functions such as in reality?