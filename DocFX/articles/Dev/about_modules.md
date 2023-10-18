# About Modules
Modules are be 3D interactable objects in _ECellDive_ which encapsulate data or actions. They are the main mode of interaction for users to have an effect on the virtual environment of _ECellEngine_. Hence, general requirements about modules is to be able to receive input from user, to be moved around, to encapsulate data, or to encapsulate functions that the user can trigger.

Our approach is to use inheritance to define specific modules. Currently, there are two base classes that can be inherited that already implements the basics about interaction. The first one is [Module](../../api/ECellDive.Modules.Module.yml) and the seconf is [GameNetModule](../../api/ECellDive.Modules.GameNetModule.yml).

## Module or GameNetModule? That is the question.
The primary difference between the two is the [Module](../../api/ECellDive.Modules.Module.yml) is local while [GameNetModule](../../api/ECellDive.Modules.GameNetModule.yml) is synchronized over the multiplayer network. Hence, when making a new module, the first question to answer is whether every user in a session should see the module. For instance, when making a module to encapsulate data in which users can dive, it is likely that this must be shared on the network for every user to access the data and collaborate on it (so inherit from [GameNetModule](../../api/ECellDive.Modules.GameNetModule.yml)). However, if the purpose is to implement a module to modify the environment or the data, there might not be any reason to make an instance of the module accessible to every user(so inherit from [Module](../../api/ECellDive.Modules.Module.yml)). This is ultimately a design decision.

Modules that inherit from [GameNetModule](../../api/ECellDive.Modules.GameNetModule.yml):
- [CyJsonModule](../../api/ECellDive.Modules.CyJsonModule.yml)
- [EDgeGO](../../api/ECellDive.Modules.EdgeGO.yml)
- [NodeGO](../../api/ECellDive.Modules.NodeGO.yml)

Those represent data. And every module that inherit from [Module](../../api/ECellDive.Modules.Module.yml) are:
- [GroupByModule](../../api/ECellDive.Modules.GroupByModule.yml)
- [HttpServerFbaModule](../../api/ECellDive.Modules.HttpServerFbaModule.yml)
- [HttpServerImporterModule](../../api/ECellDive.Modules.HttpServerImporterModule.yml)
- [HttpServerInfoQueryModule](../../api/ECellDive.Modules.HttpServerInfoQueryModule.yml)
- [HttpServerModificationModule](../../api/ECellDive.Modules.HttpServerModificationModule.yml)

## GameNetModule's children must override 3 methods
Currently, [GameNetModule](../../api/ECellDive.Modules.GameNetModule.yml) inherits from the interface [IDive](../../api/ECellDive.Interfaces.IDive.yml) as well as [IMlprData](../../api/ECellDive.Interfaces.IMlprData.yml) which have methods that **MUST** be implemented by children.

For [IDive](../../api/ECellDive.Interfaces.IDive.yml) it is
[GenerativeDiveInC](xref:ECellDive.Interfaces.IDive.GenerativeDiveInC). This method is where the user must define what to generate in the new dive scene. This is specific to every "diveable" module so, of course, it must be defined.

For [IMlprData](../../api/ECellDive.Interfaces.IMlprData.yml), there are two methods, [AssembleFragmentedData](xref:ECellDive.Interfaces.IMlprData.AssembleFragmentedData) and [RequestSourceDataGenerationServerRpc](xref:ECellDive.Interfaces.IMlprData.RequestSourceDataGenerationServerRpc). The former's role is to reassemble network-synchronized data that has been imported by one the user in the multiplayer session and that was broadcasted (as fragments) to all other clients of the session. The later's role is to request the server to generate the representation of the data stored in the module. It will likely be called inside [GenerativeDiveInC](xref:ECellDive.Interfaces.IDive.GenerativeDiveInC).

## Dummy Modules in the projet's asset as examples

## General workflow to create a new module 