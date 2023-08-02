<img src="https://img.shields.io/badge/version-alpha 0.11.x-blue.svg?style=flat-square" alt="version alpha 0.11.x">

<img src="https://img.shields.io/badge/unity-2020.3-green.svg?style=flat-square" alt="unity 2020.3">

<img src="./resources/images/ecellDive_white_1280-990.png" width="640" height="495">

# Context
ECellDive is a VR application for systems biology. It is part of a research project exploring how biologists will be working tomorrow in the context of metaverses. Our first target is to provide an integrated workspace for visualization, simulation and modelling of biological systems in real-time between colleagues. In the meantime, it will be the space of experimentations regarding exploration of data in a virtual reality environment. As such, this project is under active development and subject to a lot of changes: features may come and go depending on their actual usefullness.

# Requirements
## App
### Software
- [SideQuest](https://sidequestvr.com/) to install the app.
- [Kosmogora v1.1.1](https://github.com/ecell/kosmogora). With the exception of the tutorial/demo with a  [pre-computed] example (./articles/UserManual/quickstart.md#pre-computed-ijo1366), everything requires a running instance of [Kosmogora](https://github.com/ecell/kosmogora).

### Hardware
- Meta Quest VR devices. Only tested on Quest 2. ECellDive was sufficiently optimized to not require to be connected to a high-end computer (through Link cable or Air Link). There will be a framedrop for a few seconds when generating the metabolic pathways to visualize, but visualization itself runs without any issues once it is loaded.
- A separate computer to run [Kosmogora](https://github.com/ecell/kosmogora).

## Project (for developers)
### Software
#### Manual installation required
- [Unity Hub](https://unity.com/download)
- [Unity 2020.3.27f1](https://unity.com/releases/editor/archive)
- [Kosmogora v1.1.0](https://github.com/ecell/kosmogora) (if developping features involving data management and/or visualization)

#### No action required (included in the project's [packages manifest](https://github.com/ecell/ECell_Dive/blob/main/Packages/manifest.json))
- [Universal Render Pipeline (URP)](https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@10.8/manual/). Unity's "artist-friendly" render pipeline for optimized graphics on a broad range of platforms. 
- [Oculus XR plugin 1.11.2](https://docs.unity3d.com/Packages/com.unity.xr.oculus@1.11/manual/index.html). Unity's plugin to develop for Meta Oculus Quests devices. This project is not using OpenXR as its development started just before Oculus devices support by OpenXR. Upgrade is planned, however.
- [Netcode for GameObjects 1.0.0-pre.9](https://docs.unity3d.com/Packages/com.unity.netcode.gameobjects@1.0/manual/index.html). Unity's solution for multiplayer games.
- [XR Interaction Toolkit 1.0.0-pre](https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@1.0/manual/index.html). Unity's solution for interaction systems in VR and AR.
- [ParrelSync](https://github.com/VeriorPies/ParrelSync). For multiplayer networking tests and debug.

### Hardware
- This project is developed with gaming laptops (MSI Raider GE76 and Alienware R4, both with a laptop version of a RTX3070).
- Meta Quest VR devices. Only used Quest 2.

# Installation
Please, check the [Installation page](./articles/UserManual/installation.md) for more details

## App
The application executable (*.apk file*) can be downloaded from the [release page](https://github.com/ecell/ECell_Dive/releases) and installed on any Quest 2 heasdset thanks to [SideQuest](https://sidequestvr.com/).

## Project
Cloning the project through the https uri has been tested but we suggest cloning the repo by dowloading the zip archive instead. Using zip archives has been proven more stable for Unity projects on Git compared to the former.

# Documentation
Documentation about [ECellDive's in-app features](https://ecell.github.io/ECell_Dive/articles/UserManual/QuickStart/quickstart.html) as well as the documentation for the source code [API](https://ecell.github.io/ECell_Dive/api/index.html).