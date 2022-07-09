---
layout: default
title: Using iJO1366
parent: Quickstart
grand_parent: User Manual
---

## Importing iJO1366
Open the main menu and navigate to the module menu. Add a [remote importer module](/UserManual/Modules/remote_importer_module.html) to the dive scene. Enter the IP address where your instance of answer_talker is hosted and request the list of available modules. You should have access to iJO1366 by default: click on the corresponding button to import the data.

## Diving in iJO1366
Once iJO1366 is in the scene, point at the 3D model of the module with the [Ray-based Interaction Controls](/UserManual/Controls/ray_based_interaction_controls.html) and hold the main button of the controller you are using for 1 second to dive into it (either *A Button* or *X Button*). **This takes about 10 seconds. For now, there is no visual, sound or haptic feedback to tell you that you are currently diving. But there is actually a noticeable framerate drop because the process is too intense for the Meta Quest 2 hardware. This is obviously not the intended behaviour and we are still figuring out how to make this step smoother.**

## Visualizing the main parts of the pathway.
Once you can see the pathway, you are free to [move around](/UserManual/Controls/movement_controls.html) the dive scene to explore and familiarize yourself with its structure. Navigate to the Module menu and add a [GroupBy module](/UserManual/Modules/groupby_module.html) to the dive scene. Click on *edges* and select *Subsystem* to group edges according to the main parts of the pathway (membrane transport, glycolysis, citric acid cycle, etc...).

## Requesting a flux balance analysis.
Get back to the module menu and add a [FBA module](/UserManual/Modules/fba_module.html). Input the IP address where your instance of answer_talker is hosted and request the FBA to run. This takes 2-3 seconds.
Running the FBA will reset the color of the edges. You can visualize back the groups you defined by navigating to the [Groups menu](/UserManual/UIMenus/groups_menu.html) and checking/unchecking the checkbox associated with the group you are interrested in.
You can knockout reactions if you wish by pointing at them with the [Ray-based Interaction Controls](/UserManual/Controls/ray_based_interaction_controls.html) and pressing the *Front Trigger*. A knocked out reaction will appear as a solide edge instead of dashed. After knocking out the reactions you are interrested in, you can re-run the FBA.

## Saving the mofications.
You can save the current state of iJO1366 (knocked out reactions) thanks to the [Modifications Handler Module](/UserManual/Modules/modification_handler_module.html). In the *Server Parameters* panel, input the IP address where your instance of *answer_talker* is hosted. Then, select the base model for which you wish to save the modifications (in this case it should be iJO1366). Chose a name for the file if you wish to (by default it will be *NameOfBaseModel_yyyyMMddTHHmmssZ*). After hitting the *Save Button*, you can confirm whether the file was correctly saved by taking a look at the last Trace message in the [Log menu](/UserManual/UIMenus/log_menu.html) or by clicking the *Query Available Modification Files Button*. In the latter case, the new file should appear in the *Modification Files* panel.