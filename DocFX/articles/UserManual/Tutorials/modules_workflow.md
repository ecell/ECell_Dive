# Tutorial on the workflow with modules

> [!WARNING]
> Please, make sure that you can [connect to Kosmogora](../Network/connecting_to_Kosmogora.md)

The purpose of this tutorial is to learn how to manipulate some of the [modules](../Modules/modules.md) in *ECellDive* as well as the communication workflow between *ECellDive* and [Kosmogora](https://github.com/ecell/kosmogora). In this tutorial, we will learn how to manually reach the same state as [the demo where everything is pre-computed](../quickstart.md#pre-computed-ijo1366).

## Duration
it usually takes around **40 minutes** to complete this tutorial (it all depends how long you wish to freely explore the model before moving to another step). Novices that had not tried VR before *ECellDive* were not any slower than experts if they had taken the [tutorial on controls](./controls.md) beforehand. Therefore, we recommend everybody to start with [tutorial on controls](./controls.md).

## Launch the Tutorial
This tutorial is accessible through the general [tutorial menu](../UIMenus/tutorials_menu.md). The steps to find it are:
1. Open the [main menu](../UIMenus/main_menu.md)
2. Click on the button *Tutorials*
3. Click on the button *Modules*

<details>
  <summary>Example</summary>

<img src="~/resources/images/tutorials/modules/launch.gif" alt="Launch"/>
</details>

## Step 1
### Goal
Learn how to add a [module](../Modules/modules.md).

### Task
Open the [Modules Menu](../UIMenus/modules_menu.md) and add a [Data Importer](../Modules/remote_importer_module.md).

### Details
On the left controller, press the button that says [Open Main Menu](../UIMenus/main_menu.md) From here, click on the button *Modules* to open the corresponding sub-menu. In this submenu, you will find the modules that will help you interact with data. For now, click on the *Data Importer* button to add that module to the scene.

> [!NOTE]
> You will automatically advance to the next step of the tutorial once you add the module.

<details>
  <summary>Example</summary>

<img src="~/resources/images/tutorials/modules/step1.gif" alt="modules_step1"/>
</details>

## Step 2
### Goal
Learn how to import data from a server.

### Task
Add the data module of iJO1366 to the scene.

### Details
Open the menu of the data importer. Input the IP adress of the [Kosmogora](https://github.com/ecell/kosmogora) server. If you did not specify a *Port* when launching [Kosmogora](https://github.com/ecell/kosmogora), you can leave that input field blank. Then, click on the corresponding button to query the available models and select iJO1366 on the second panel.

> [!NOTE]
> You will automatically advance to the next step of the tutorial once you add iJO1366 to the scene.

<details>
  <summary>Example</summary>

In the case of this example, the instance of [Kosmogora](https://github.com/ecell/kosmogora) was launched with the following command line: `uvicorn --host 192.168.0.174 --port 8421 app:app`. So, in *ECellDive* we use `192.168.0.174` as the IP and `8421` as the port where needed.

<img src="~/resources/images/tutorials/modules/step2.gif" alt="modules_step2"/>
</details>

## Step 3
### Goal
Learn how to dive into a module.

### Task
Dive and explore the network.

### Details
Point at the data module iJO1366 you just imported with the <span style="color:orange">Ray-Based Controls</span> and press the primary button. This will open the information menu of this module as well as the portal associated with this data module. To dive in the module, point at the portal with the <span style="color:orange">Ray-Based Controls</span> and hold the primary button.

> [!WARNING]
> The loading of the next dive scene will take a about seconds and framerate will drop significantly

Click the button *Next* on the tutorial panel when you are ready to move on to the next step.

<details>
  <summary>Example</summary>

<img src="~/resources/images/tutorials/modules/step3.gif" alt="modules_step3"/>
</details>


## Step 4
### Goal
Learn how to request a Flux Balance Analysis (1/2).

### Task
Add a [FBA module](../Modules/fba_module.md) to the dive scene via [Modules Menu](../UIMenus/modules_menu.md).

### Details
Do not hesitate to [move around](../Controls/movement_controls.md) and explore the network. 

> [!NOTE]
> You can click on the <span style="color:red">red pin icon</span> of the tutorial panel to pin the panel to your position. This way, the panel will follow you when you move. The panel is successfully pinned to you when the <span style="color:green">pin icon is green</span>.

Go back to the [Modules Menu](../UIMenus/modules_menu.md) that you previously used to add a Data Importer to the scene.
This time, click on the button *FBA* to add a [FBA module](../Modules/fba_module.md).

> [!NOTE]
> You will automatically advance to the next step of the tutorial once you add the FBA module to the scene.

<details>
  <summary>Example</summary>

<img src="~/resources/images/tutorials/modules/step4.gif" alt="modules_step4"/>
</details>

## Step 5
### Goal
Learn how to request a Flux Balance Analysis (2/2).

### Task
Request the FBA from the server.

### Details
Open the menu of the [FBA module](../Modules/fba_module.md). Input the [IP adress of the Kosmogora](../Network/connecting_to_Kosmogora.md) server. If you did not specify a *Port* when launching [Kosmogora](../Network/connecting_to_Kosmogora.md), you can leave that input field blank. Once you entered the information, click on the buton *Run FBA* to request the simulation from the server.

> [!WARNING]
> It will take a few seconds. The duration depends on the quality of the network you are on.

The result of the FBA will be clearly visible as the edges's widths are scaled according to the value of the fluxes.

<details>
  <summary>Example</summary>

In the case of this example, the instance of [Kosmogora](https://github.com/ecell/kosmogora) was launched with the following command line: `uvicorn --host 192.168.0.174 --port 8421 app:app`. So, in *ECellDive* we use `192.168.0.174` as the IP and `8421` as the port where needed.

<img src="~/resources/images/tutorials/modules/step5.gif" alt="modules_step5"/>
</details>

You can use the options to clamp the values or set a color gradient.

<details>
  <summary>Example</summary>

> [!WARNING]
> You can see some strange green pixels in this gif. This is due to the [recording system](https://blog.bahraniapps.com/category/gifcam/), they are not present in *ECellDive*.

<img src="~/resources/images/tutorials/modules/step5_bonus1.gif" alt="modules_step5_b1"/>
</details>

You can also KNOCKOUT reactions by pointing at the edges with the <span style="color:orange">Ray-Based Controls</span> and pressing the front trigger. Then, click on *Run FBA* again to update the simulation. When a reaction is knockedout, the texture of the arrow will change (gradients of gray with wholes).

<details>
  <summary>Example</summary>

<img src="~/resources/images/tutorials/modules/step5_bonus2.gif" alt="modules_step5_b2"/>
</details>

## Step 6
### Goal
Learn how to make groups.

### Task
Add a [GroupBy module](../Modules/groupby_module.md) and make groups.

### Details
Go to the [Modules Menu](../UIMenus/modules_menu.md) and click on the button *Group By Operator* to add the module in the scene.
Open its menus (point + primary button) and group the nodes and/or the edges according to one of their respective metadata.

> [!NOTE]
> Only ONE metadata can be selected for each object.

We recommand grouping edges and nodes according to *Subsystems* and *Compartments* respectively. Once you have selected your metadata of interest, click on *Process Grouping*. Check the [Groups Menu](../UIMenus/groups_menu.md) if you want o change the default color of the generated groups.

<details>
  <summary>Example</summary>

<img src="~/resources/images/tutorials/modules/step6.gif" alt="modules_step6"/>
</details>
