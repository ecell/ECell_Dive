## Dive Travel Map
This module is a graph representing how a diver navigated in the dive scenes. The cubic nodes are representation of the dive scenes and the arrows indicate in which order the diver moved from one dive scene to another. Dive scenes are named after the model file they are encapsulating. The purpose of this map is to help divers build a mental model of how they explore the data.

<details>
  <summary>Demonstration</summary>

Here is the dive travel map after the user has moved from the root dive scene to iJO1366, and back to the root:

<img src="~/resources/images/divemap/dtm_Demo.gif" alt="Dive Travel Map Demo"/>
</details>

> [!NOTE]
> This map currently does not allow much interactions. For example, the nodes are fixed and the gradient of color for the nodes and the edges are always following the HUE. We plan to extend the functionalities of this map in future updates.