# Download the zip of the project
Cloning the git repo through the https uri is possible (last tested on 2023-07-19) but it is more stable to download the zip archive of the project instead. On the github page of the code:
1. click on the green button *Code*
2. click on the button *Download ZIP*.

> [!WARNING]
> [FOR WINDOWS] If you plan on developing for the multiplayer networking, you will probably use [ParrelSync](https://github.com/VeriorPies/ParrelSync) in the project. ParrelSync makes clones of the whole project which involves copying files very deep in the project. If you are not careful, the paths **may exceed the 260 characters limit** on Windows. To work around this, we advise you put the project at a directory close to the root. Something like `C:\Users\username\src`. This is a known limit in ParrelSync (see [FAQ](https://github.com/VeriorPies/ParrelSync/wiki/Troubleshooting-&-FAQs#cant-clone-the-project--cant-open-the-cloned-project) and [issue#13](https://github.com/VeriorPies/ParrelSync/issues/13)) 

Once you have downloaded the archive (a few dozen Mo), unzip it (see [Windows](https://www.7-zip.org/), [MacOs](https://support.apple.com/en-gb/guide/mac-help/mchlp2528/mac), [Linux](https://askubuntu.com/questions/86849/how-to-unzip-a-zip-file-from-the-terminal) for relevant ways) at a location of your choice.

<details>
  <summary>Example Download ZIP</summary>

<img src="~/resources/images/installation/download_zip.gif" alt="download zip"/>

</details>

# Download Unity
> [!WARNING]
> Currently, the project works with Unity 2023.3.27f1. We recommend to not open the project with a more recent version of Unity if it is not your goal to upgrade the whole project. That is because packages dependencies will be updated along in Unity 2021.X or 2022.X and this may break the project.

## Download Unity Hub
[Unity Hub](https://unity.com/download) is the official launcher application by Unity to manage your installation of Unity versions as well as start and open projects. Follow the instructions to install Unity Hub on your system.

## Download version 2022.3.27f1
1. Open Unity Hub.
2. Click on the button *Installs* on the left panel.
3. Click on the button *Install Editor* on the top right.
4. Click on the tab *Archive*.
5. Click on the link *download archive*.

<details>
  <summary>Get To Unity Archive</summary>

<img src="~/resources/images/installation/get_to_unity_archive.gif" alt="get to unity archive"/>
</details>

6. Click on the tab *Unity 2020.X*.
7. Scroll down until you find Unity 2023.3.27 (January 31, 2022).
8. Click on the button *Unity Hub*.

<details>
  <summary>Find version 2020.3.27</summary>

<img src="~/resources/images/installation/find_2023_3_27.gif" alt="find 2023.3.27"/>
</details>

9. Accept to open the link with Unity Hub (this depends on your internet navigator and OS).
10. In Unity Hub, make sure to select the options related to *Android* if you want to be able to build the application for the Meta Quest headeset series.

<details>
  <summary>Mandatory options to build for Quest 2 (Android)</summary>

<img src="~/resources/images/installation/select_android_options.png" alt="select android options"/>
</details>


11. Click on the blue button *Continue* in the bottom left corner.
12. Read and Agree to the terms of use; click on the blue button *Install*.

Installation typically takes 10-15 minutes on a gaming laptop.

# Open the project
1. Open Unity Hub.
2. Click on the button *Projects* in the left panel.
3. Click on the button *Open* in the top right corner.
    1. If you click on the button itself, the file explorer of your local system should open by default.
    2. If you click on the arrow, a drop down will appear, giving you the choice to *Add a project from disk* or *Open remote project*.
4. Navigate to where you the uncompressed  folder of *ECellDive* is located on your system.
    1. If you chose option 3.1, then clicking on the "Open" should immediately trigger the opening of the project in Unity.
    2. If you chose option 3.2, then the project will be added to the list of project in Unity Hub and you need to click on it once more to really open it.

<details>
  <summary>Open project</summary>

<img src="~/resources/images/installation/open_project.gif" alt="open project"/>
</details>

Opening the project for the first time might take 5 mins on a gaming laptop.