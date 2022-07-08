<img src="https://img.shields.io/badge/version-alpha 0.10.x-blue.svg?style=flat-square" alt="version alpha 0.10.x"> 
<img src="https://img.shields.io/badge/unity-2020.3-green.svg?style=flat-square" alt="unity 2020.3">
<img src="https://img.shields.io/badge/Virtual Reality-Meta Quest 2-green.svg?style=flat-square" alt="unity 2020.3">

<img src="./docs/_include/images/ecellDive_white_1280-990.png" width="640" height="495">

# Context
ECellDive is a VR application for systems biology. It is part of a research project exploring how biologists will be working tomorrow in the context of metaverses. Our first target is to provide an integrated workspace for visualization, simulation and modelling of biological systems in real-time between colleagues. In the meantime, it will be the space of experimentations regarding exploration of data in a virtual reality environment. As such, this project is under active development and subject to a lot of changes: features may come and go depending on their actual usefullness.

# ECellDive user manual

## Quick start-up with iJO1366
### Installation
ECellDive runs on the Meta Quest 2. The .apk file can be downloaded from the release page and installed on any Quest 2 heasdset thanks to [SideQuest](https://sidequestvr.com/).

### Have answer_talker running
To work properly, ECellDive needs [answer_talker](https://github.com/ecell/answer_talker). It is a server to which the user can connect in ECellDive to import data, request calculations and save modification files. To properly communicate, the computer running *answer_talker* and the Meta Quest 2 headset **must be on the same local network**; preferably the same router but a VPN might also work. When both are runing (see [answer_talker](https://github.com/ecell/answer_talker) page for details on how to launch the server), you can access the server from ECellDive with different modules in-app

### Importing iJO1366
Open the main menu and navigate to the module menu. Add a [remote importer module](#remote-importer-module) to the dive scene. Enter the IP address where your instance of answer_talker is hosted and request the list of available modules. You should have access to iJO1366 by default: click on the corresponding button to import the data.

### Diving in iJO1366
Once iJO1366 is in the scene, point at the 3D model of the module with the [Ray-based Interaction Controls](#ray-based-interaction-controls) and hold the main button of the controller you are using for 1 second to dive into it (either *A Button* or *X Button*). **This takes about 10 seconds. For now, there is no visual, sound or haptic feedback to tell you that you are currently diving. But there is actually a noticeable framerate drop because the process is too intense for the Meta Quest 2 hardware. This is obviously not the intended behaviour and we are still figuring out how to make this step smoother.**

### Visualizing the main parts of the pathway.
Once you can see the pathway, you are free to [move around](#movement-controls) the dive scene to explore and familiarize yourself with its structure. Navigate to the Module menu and add a [GroupBy module](#groupby) to the dive scene. Click on *edges* and select *Subsystem* to group edges according to the main parts of the pathway (membrane transport, glycolysis, citric acid cycle, etc...).

### Requesting a flux balance analysis.
Get back to the module menu and add a [FBA module](#flux-balance-analysis-fba-module). Input the IP address where your instance of answer_talker is hosted and request the FBA to run. This takes 2-3 seconds.
Running the FBA will reset the color of the edges. You can visualize back the groups you defined by navigating to the [groups menu](#groups) and checking/unchecking the checkbox associated with the group you are interrested in.
You can knockout reactions if you wish by pointing at them with the [Ray-based Interaction Controls](#ray-based-interaction-controls) and pressing the *Front Trigger*. A knocked out reaction will appear as a solide edge instead of dashed. After knocking out the reactions you are interrested in, you can re-run the FBA.

### Saving the mofications.


## Controls
There are three sets of controls in ECellDive which can be used in either the left or the right controller.
You can switch between a control set to another by pressing the *Y Button* on the left controller and the *B Button* on the right controller. By pressing the aforementioned buttons, the the controls will circle through *Groups Controls*-->*Movement Controls*-->*Ray-based Interaction Controls* on the corresponding controller.

When a user is dropped in the root Dive Scene, the left controller will use the *Ray-based Interaction Controls* and the right controller will use the *Movement Controls*. 
*Groups Controls* and *Movement controls* each have 2 alternative modes. A user can switch between them by pressing the *X Button* on the left controller and the *A Button* on the right controller.

For each control set, the effect of the buttons are clearly visible in-app thanks to little tags attached to the 3D models of the controllers. 

### Groups Controls
This is a set of controls to define custom groups of elements in the Dive Scenes which will be recognizable by color and accessible in the [Groups Menu](#groups). There are two alternative selectors to build custom groups:
- Ray selector. Adds elements one-by-one to the group being built. Point at the element with the ray and press the *Front Trigger*
- Volumetric Selector. Uses a sphere as the selector. Any elements that collides with the sphere while the *Front Trigger* is pressed are added (removed) from the group if they are not already in it (if already in it). Scale and position of the sphere can be controlled using the *Joystick*r.

It is not possible to modify the content of a group (remove od add elements) once it has been created.

### Movement Controls
This allows the user to move around in the Dive Scenes.
There are two alternative movement modes:
- Teleportation. Represented by a green ray and a green sphere at the end of the ray. The position of the sphere indicates the teleportation target. The user can teleport by pressing the *Front Trigger*. The teleportation distance can be changed using the *Joystick*.
- Continous movements. **Use moderately for beginners as it is a source of motion sickness.** Upon pressing the *Front Trigger*, the user can move in the direction he translates his controller to. The direction and magnitude of the movement is composed from the individual vectors on the X, Y and Z axis represented by the red, green and blue line respectively.

### Ray-based Interaction Controls
This is the main control set built to interact with UI, grab & move objects, display the modules' parameters, knockout reactions, etc... Point at an element with the ray and press different buttons for different action on those elements.
- Grab elements with the *Grip Button*
- UI press with the *Front Trigger*
- Open a module's parameter panel with the *X Button* or *A Button*
- Dive into a module by holding the *X Button* or *A Button*

## UI Menus
You can move UI panel around by pointing at the white horizontal bar while having the *Ray-based Interaction Controls* activated.
### Main
The main menu gives access to the other menu described below. Open by pressing the *start button* on the left controller.

### Modules
Gives the list of the modules the user can instantiate to help him perfom a set of task in ECellDive. The list of the modules currently availabe is [below](#modules).

### Groups
The menu to visualize the groups of elements that have been either specifically defined by the user or [automatically grouped](#groupby) together according to some metadate.

### Multiplayer
The menu to host or join a multiplayer session. Actually, even in single player, the application runs a server but it is running on local host (127.0.0.1:7777).
- Input field *Player Name*. The name that will be displayed to the other players.
- Input field *Server IP*. The IP of the host-to-be on the Local Area Network (LAN) where every users are connected. The host can know his IP address by looking at the advance settings of the WiFi connection from the Meta Quest 2 main menu. To open the Meta Quest 2 main menu, click on the *Oculus Button* on the right controller. Then, spot the WiFi icon and click on it. Open the current active connection and go to the bottom of the menu.
- Input field *Connection Port*. Default is 7777. That default port should be usable on the Meta Quest 2. You can leave the field empty to use the default value.
- Input Field *Password*. To be customized at the user's discretion.
- Button *Host*. Will try to start a server with the information given in the input fields. Will fall back to a local single player server (127.0.0.1:7777) if it fails to do so. The main reason for failure is usually a mistake in the IP address.
- Button *Join*. Will try to connect to a host with the information provided in the input fields. **KNOWN ISSUE: failed connection to the host currently freezes the app. You need to force quit ECellDive via the Meta Quest 2 menu.**

### Log
The menu to follow what the application is doing. There are different types of messages that are defined internally during development and can be filtered by the user. Clicking on a message displays extended content but only the 450 first characters are displayed for performances reasons. **KNOWN ISSUE: the filtering of the message do not repositions the message in the scroll list.**

## Modules
Modules are the user's interface to perform different actions on the data in ECellDive

### Flux Balance Analysis (FBA) Module
Requires a connection to *answer_talker*.

This is the module to request a flux balance analysis for the model currently imported.

- The *Server Parameters* pannel:
    - Input Field *Data Server IP*. The IP address at which *answer_talker* is hosted. Default value is 127.0.0.1 but this only works during development if *answer_talker* is hosted on the same machine as the one we are developping ECellDive on.
    - Input Field *Data Server Port*. The Port at which *answer_talker* is listening. Default value is 8000. This is also the default value when launching *answer_talker* so, unless you specified a port at that time, you can leave the field empty in ECellDive.
    - Button *Run FBA*. Will try to perform the FBA calculations while taking into account the reactions that may have been knocked out in the loaded model. It typically takes a few seconds before being able to see the result of the FBA. If nothing happens check the Log Menu to see if an error was not raised.
- The *FBA Visuals Parameters* pannel:
    - Slider *Lower Bound*. The global value used to clamp the small flux values. If a reaction's flux value is lower than the lower bound, then the lower bound value will be used when updating the visuals of the reaction edge. This makes every reaction visually identical when their flux value is lower than the lower bound.
    - Color Button *Lower Bound*. The button used to set the color corresponding to the lower bound for the color gradient going from the lower bound to the upper bound.
    - Slider *Upper Bound*. The global value used to clamp the big flux values. If a reaction's flux value is greater than the upper bound, then the upper bound value will be used when updating the visuals of the reaction edge. This makes every reaction visually identical when their flux value is greater than the upper bound.
    - Color Button *Lower Bound*. The button used to set the color corresponding to the lower bound for the color gradient going from the lower bound to the upper bound.
    - Button *Update Visuals*. Updates the visuals of every reaction edge according to the setting that were just defined. This is local, there is no connection to *answer_talker* involved so the update should be fast.

### GroupBy
This is the module used to automatically make coloured groups according to metadata of the elements loaded in the scene. 

- Panel *Grouping Attributes*:
    - Scroll List. If the module detected elements it can try to group according to their metadata, then the names of the elements will be displayed in this list. For example, if a user loaded a CyJson pathway then the *nodes* and *edges* will appear.
    - Button in the Scroll List. Upon clicking on the button corresponding to an element that can undergo automatic grouping, a new scroll list will be displayed showing the metadata that can used to make the groups. If several metadata are selected then every group types will be generated individually and accessible through the *Groups UI Menu*. No set operation (such as INTERSECTION or UNION) are performed on the groups. 
    - Button *Process Grouping*. Make groups according to every metadata selected for every elements detected. Usually very fast but the time actually depends on the number of groups to generate and the number of elements loaded in the dive scene.

### Remote Importer Module
Requires a connection to *answer_talker*.

This is the module that is used to import the data stored on *answer_talker*.
- Panel *Server Parameters*:
    - Input Field *Data Server IP*. The IP address at which *answer_talker* is hosted. Default value is 127.0.0.1 but this only works during development if *answer_talker* is hosted on the same machine as the one we are developping ECellDive on.
    - Input Field *Data Server Port*. The Port at which *answer_talker* is listening. Default value is 8000. This is also the default value when launching *answer_talker* so, unless you specified a port at that time, you can leave the field empty in ECellDive.
    - Button *Query Available Models". Will try to contact *answer_talker* at the address and port given to request the list of models currently available. This is fast. If nothing happens check the Log Menu to see if an error was not raised.
- Panel *Models List*. Displays the list of models available in *answer_talker* if the request was successful.

### Modification Handler
Requires a connection to *answer_talker*.

This module is used to import and save modification files associated to a model loaded in one of the dive scene. One must input the IP address of the server to query available files but also to save new files (even if the buttons for these two actions are in different panels).
- Panel *Server Parameters*:
    - Input Field *Data Server IP*. The IP address at which *answer_talker* is hosted. Default value is 127.0.0.1 but this only works during development if *answer_talker* is hosted on the same machine as the one we are developping ECellDive on.
    - Input Field *Data Server Port*. The Port at which *answer_talker* is listening. Default value is 8000. This is also the default value when launching *answer_talker* so, unless you specified a port at that time, you can leave the field empty in ECellDive.
    - Button *Query Available Modification Files". Will try to contact *answer_talker* at the address and port given to request the list of modification files currently available. This is fast. If nothing happens check the Log Menu to see if an error was not raised.

- Panel *Save Modification Files*:
    - Button *Base Model*. Will scan for all the base models loaded in ECellDive and activate the panel *Loaded Base Models* for the user to chose one.
    - Input Field *File Name*. The name of the modification file when saved on the server.
    - Button *Save*. Interactable only if a base model was selected.  Will try to contact *answer_talker* at the address and port given to save a modification file. This is fast. You can check whether the file was correctly saved by checking the Log Menu. A trace message confirming the save should be displayed. Otherwise, an error should be newly registered. **Saving a modification file is closely related to the development of the format of modification file on the side of answer_talker. The content of the saved modification file is therefore subject to a lot of changes.**

- Panel *Modification Files*. Displays the list of modification files available in *answer_talker* if the request was successful.
    - Button *NameOfModificationFile*. There should be one button by modification file saved on the server. Upon clicking the button, the modification file is imported and immediately applied to the corresponding model if it is loaded in a dive scene.

- Panel *Loaded Base Models*. Displays the models loaded in the dive scenes for which we can save the current modifications as compared to when it was loaded.
    - Button *NameOfTheModel*. There should be one button by model loaded in ECellDive (indescriminately of the Dive Scenes). Clicking on the button registers the target base model.

# Developers Documentation


# External resources
## Papers
In progress

## Videos

- *ECellDive alpha-0.8.3 | WIP*. March 2022. Supplementary Material of a poster presented at RIKEN BDR Symposium on the theme ["Emergence in Biological Systems: Challenges to Bridging Hierarchies"](https://www2.bdr.riken.jp/sympo/2022/)

[![Alt text](https://img.youtube.com/vi/bJ2kC5_XLx8/0.jpg)](https://youtu.be/bJ2kC5_XLx8)