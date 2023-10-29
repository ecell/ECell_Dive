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
[GenerativeDiveInC](xref:ECellDive.Interfaces.IDive.GenerativeDiveInC). This coroutine is where the user must define what to generate in the new dive scene. This is specific to every "diveable" module so, of course, it must be defined.

For [IMlprData](../../api/ECellDive.Interfaces.IMlprData.yml), there are two methods, [AssembleFragmentedData](xref:ECellDive.Interfaces.IMlprData.AssembleFragmentedData) and [RequestSourceDataGenerationServerRpc](xref:ECellDive.Interfaces.IMlprData.RequestSourceDataGenerationServerRpc(System.UInt64)). The former's role is to reassemble network-synchronized data that has been imported by one the user in the multiplayer session and that was broadcasted (as fragments) to all other clients of the session. The later's role is to request the server to generate the representation of the data stored in the module. It will likely be called inside [GenerativeDiveInC](xref:ECellDive.Interfaces.IDive.GenerativeDiveInC).

To enforce the implementation of [GenerativeDiveInC](xref:ECellDive.Interfaces.IDive.GenerativeDiveInC) and[AssembleFragmentedData](xref:ECellDive.Interfaces.IMlprData.AssembleFragmentedData), the [GameNetModule](../../api/ECellDive.Modules.GameNetModule.yml) was marked as `abstract` and so were the two previous functions. However, it is not possible to do so for [RequestSourceDataGenerationServerRpc](xref:ECellDive.Interfaces.IMlprData.RequestSourceDataGenerationServerRpc(System.UInt64)) as `RPCs` they cannot be `abstract`. So developers must be vigilant to not forget it.

## Examples (available in the assets)
The Unity project contains an example of basic setup for gameobjects using the Module and GameNetModule components. Those are respectively called `BaseModule` and `BaseGameNetModule`. You can find them under `Assets\Resources\Prefabs\Modules`.

Both gameobjects have exactly the same structure: \
<img src="../../resources/images/dev/BaseModule/GOHierarchy.jpg" alt="Base Module Hierarchy" style="width: 400px;"/>

<img src="../../resources/images/dev/BaseGameNetModule/GOHierarchy.jpg" alt="Base Game Net Module Hierarchy" style="width: 400px;"/>

- `Module Graphics` have the collider and renderer attached.
- `Module Name Canvas` is a container for the name of the module. It is a prefab you can find at the path `Assets\Resources\Prefabs\Modules\BaseModuleName`
- `All Info Tags` is a container for all the info tags that might be attached to the module. Despite the name, it is also used in other modules to contain any UI menu that can be associated to a module.
- `Info Display` is a prefab that can be instantiated to create information labels associated to the module and which will have `All Info Tags` as parent. This info display is just used for a reference; it is never used. It will be deactivated on module spawn to become invisible and it is never reactivated.

The are assigned similar textures and 3D models:\
<img src="../../resources/images/dev/BaseModule/3DModel.jpg" alt="Base Module 3DModel" style="height: 200px;"/>
<img src="../../resources/images/dev/BaseGameNetModule/3DModel.jpg" alt="Base Game Net Module 3DModel" style="height: 200px;"/>

What really matters are their respective components.
The [Module](../../api/ECellDive.Modules.Module.yml) component shows in the Inspector as:\
<img src="../../resources/images/dev/BaseModule/Component.jpg" alt="Base Module 3DModel" style="width: 500px;"/>

The [GameNetModule](../../api/ECellDive.Modules.GameNetModule.yml) component shows in the Inspector as:\
<img src="../../resources/images/dev/BaseGameNetModule/Component.jpg" alt="Base Module 3DModel" style="width: 500px;"/>

The `GameNetModule` component is marked as `abstract` so it cannot directly be a component. That is why, we created [DummyGameNetModule](../../api/ECellDive.Modules.DummyGameNetModule.yml) just for the purpose of demonstration of the component. It is in fact empty.

## General workflow to create a new module
1. We recommend to start by duplicating the `BaseModule` or the `BaseGameNetModule` to have a working basis.
2. Create a new script following the naming convention `XXXModule` somewhere under `Assets\Scripts\Modules`. You may create a new folder if you think it's relevant.
   1. If you are making a new locale module, then have your class inherit from [Module](../../api/ECellDive.Modules.Module.yml).
   ```csharp
    namespace ECellDive.Modules
    {
        /// <summary>
        /// DOC
        /// </summary>
        public class XXXModule : Module //add interfaces if needed
        {
            
            //The field of your class

            //The fields/properties of the interfaces (if any)

            //Unity's override methods (Start, Update, OnXXX,...)

            //The methods of you class in alphabetical order

            //The methods of the interfaces (if any) 

        }
    }
   ```
   2. If you are making a new multiplayer module, then have your class inherit from [GameNetModule](../../api/ECellDive.Modules.GameNetModule.yml) and implement the three mandatory methods.
   ```csharp
    namespace ECellDive.Modules
    {
        /// <summary>
        /// DOC
        /// </summary>
        public class XXXModule : GameNetModule //add interfaces if needed
        {
            //The field of your class

            //The fields/properties of the interfaces (if any)

            //Unity's overriden methods (Start, Update, OnXXX,...)

            //The methods of you class in alphabetical order

            //The methods of the interfaces with, at least, the following:

            #region - GameNetModule IDive Method -
            /// <inheritdoc/>
            public override IEnumerator GenerativeDiveInC()
            {
                //write how to generate the new dive scene and how you
                //want to "wait" (since this is a coroutine).
                //You might want to call RequestSourceDataGenerationServerRpc here.
            }
            #endregion

            #region - GameNetModule IMlprData Methods -
            /// <inheritdoc/>
            public override void AssembleFragmentedData()
            {
                //write how to assemble the fragmented data.
                //Assembling might be "easy" using ECellDive.Utility.ArrayManipulation.Assemble.
                //But you probably want to retrieve information from the
                //assembled data and assign it to your fields.
            }

            /// <inheritdoc/>
            [ServerRpc]
            public override void RequestSourceDataGenerationServerRpc(ulong _expeditorClientID)
            {
                //The code to be executed by the server to generate anything
                //related to the data associated to this gameobject.
            }
            #endregion
        }
    }
    ```
3. Add the new component to the gameobject of your module.
4. Compare with the [Module](../../api/ECellDive.Modules.Module.yml) component or the [DummyGameNetModule](../../api/ECellDive.Modules.DummyGameNetModule.yml) component to assign the correct values in the fields in the inspector.
5. Remove the [Module](../../api/ECellDive.Modules.Module.yml) component or the [DummyGameNetModule](../../api/ECellDive.Modules.DummyGameNetModule.yml) component.
6. Adapt the 3D model and textures of your gameobject via the child `Module Graphics`.
7. Add UI Menus if needed.