<img src="https://img.shields.io/badge/version-alpha 0.11.x-blue.svg?style=flat-square" alt="version alpha 0.11.x"> 
<img src="https://img.shields.io/badge/unity-2020.3-green.svg?style=flat-square" alt="unity 2020.3">
<img src="https://img.shields.io/badge/Virtual Reality-Meta Quest 2-green.svg?style=flat-square" alt="unity 2020.3">

<img src="./resources/images/ecellDive_white_1280-990.png" width="640" height="495">

# Context
ECellDive is a VR application for systems biology. It is part of a research project exploring how biologists will be working tomorrow in the context of metaverses. Our first target is to provide an integrated workspace for visualization, simulation and modelling of biological systems in real-time between colleagues. In the meantime, it will be the space of experimentations regarding exploration of data in a virtual reality environment. As such, this project is under active development and subject to a lot of changes: features may come and go depending on their actual usefullness.

# Installation
## App
ECellDive runs on Meta Oculus VR devices. I was developped and tested on Meta Quest 2. ECellDive was sufficiently optimized to not require to be connected to a computer (through Link cable or Air Link). There will be a framedrop for a few seconds when generating the metabolic pathways to visualize, but visualization itself runs without any issues once it is loaded.
The application executable (*.apk file*) can be downloaded from the [release page](https://github.com/ecell/ECell_Dive/releases) and installed on any Quest 2 heasdset thanks to [SideQuest](https://sidequestvr.com/).

## Project
Cloning the project through the https uri has been tested but we suggest cloning the repo by dowloading the zip archive instead. using zip archives have benn proven more stable for Unity projects on Git compared to the former.

**Dependencies**
- Unity 2020.3.27f1
- Oculus XR plugin 1.11.2 (this project is not using OpenXR as its development started just before Oculus devices support by OpenXR. Upgrade is planned, however)
- Netcode for GameObjects 1.0.0-pre.9
- XR Interaction Toolkit 1.0.0-pre.5

# Documentation
Documentation about [ECellDive's in-app features](https://ecell.github.io/ECell_Dive/articles/UserManual/QuickStart/quickstart.html) as well as the documentation for the source code [API](https://ecell.github.io/ECell_Dive/api/index.html).