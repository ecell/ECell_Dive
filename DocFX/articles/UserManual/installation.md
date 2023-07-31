# Kosmogora
To work properly, ECellDive should [be connected to Kosmogora](~/articles/UserManual/Network/connecting_to_Kosmogora.md). It is a server to which the user can connect in ECellDive to import data, request calculations and save modification files. To properly communicate, the computer running *Kosmogora* and the Meta Quest 2 headset **must be on the same local network**; preferably the same router but a VPN might also work. When both are runing (see [Kosmogora](https://github.com/ecell/kosmogora) page for details on how to launch the server), you can access the server from ECellDive with different [modules](~/articles/UserManual/Modules/modules.md) in-app.

# ECellDive
## App
The application executable (*.apk file*) can be downloaded from the [release page](https://github.com/ecell/ECell_Dive/releases) and installed on any Quest 2 headset thanks to [SideQuest](https://sidequestvr.com/).

### Connect the headset to SideQuest
Requirement: After installing SideQuest
Plug-in your headset to your computer with a USB-C cable (a [Link](https://www.meta.com/en-gb/help/quest/articles/headsets-and-accessories/oculus-link/connect-link-with-quest-2/) cable shouldn't be necessary at this step).

The connection is successful if you see a green circle on the top left corner (see image bellow).

<img src="~/resources/images/installation/confirm_quest_connection.png" alt="link cable success"/>

### Select the .apk file
In SideQuest, click on the button *Install APK file from folder on computer*

<img src="~/resources/images/installation/install_apk_button.png" alt="install apk from folder"/>

Then, you will be invited to navigate to the directory where you are storing the .*apk file* of *ECellDive* that you downloaded from the [release page](https://github.com/ecell/ECell_Dive/releases) and open that file.

Upon clicking on *Open* in the explorer, you will see a notification green band at the bottom of SideQuest's window indicating that the file is being installed.

<img src="~/resources/images/installation/installing_apk_file.png" alt="installing apk from folder"/>

Then, once the installation is successful, the message in the green band changes accordingly.

<img src="~/resources/images/installation/apk_file_installed.png" alt="apk installed"/>

The installation should take less than 5 seconds.

## Project (for developers)
[TODO]