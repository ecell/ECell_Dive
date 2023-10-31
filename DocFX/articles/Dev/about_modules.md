# About Modules
Modules are be 3D interactable objects in _ECellDive_ which encapsulate data or actions. They are the main mode of interaction for users to have an effect on the virtual environment of _ECellEngine_. Hence, general requirements about modules is to be able to receive input from user, to be moved around, to encapsulate data, or to encapsulate functions that the user can trigger.

Our approach is to use inheritance to define specific modules. Currently, there are two base classes that can be inherited that already implements the basics about interaction. The first one is [Module](xref:ECellDive.Modules.Module) and the seconf is [GameNetModule](xref:ECellDive.Modules.GameNetModule).

## Module or GameNetModule? That is the question.
The primary difference between the two is the [Module](xref:ECellDive.Modules.Module) is local while [GameNetModule](xref:ECellDive.Modules.GameNetModule) is synchronized over the multiplayer network. Hence, when making a new module, the first question to answer is whether every user in a session should see the module. For instance, when making a module to encapsulate data in which users can dive, it is likely that this must be shared on the network for every user to access the data and collaborate on it (so inherit from [GameNetModule](xref:ECellDive.Modules.GameNetModule)). However, if the purpose is to implement a module to modify the environment or the data, there might not be any reason to make an instance of the module accessible to every user(so inherit from [Module](xref:ECellDive.Modules.Module)). This is ultimately a design decision.

Modules that inherit from [GameNetModule](xref:ECellDive.Modules.GameNetModule):
- [CyJsonModule](xref:ECellDive.Modules.CyJsonModule)
- [CyJsonEdgeGO](xref:ECellDive.Modules.CyJsonEdgeGO)
- [CyJsonNodeGO](xref:ECellDive.Modules.CyJsonNodeGO)
- [DummyGameNetModule](xref:ECellDive.Modules.DummyGameNetModule)

Those represent data. And every module that inherit from [Module](xref:ECellDive.Modules.Module) are:
- [DiveTravelMapModule](xref:ECellDive.Modules.DiveTravelMapModule)
- [EdgeGO](xref:ECellDive.Modules.EdgeGO)
- [GroupByModule](xref:ECellDive.Modules.GroupByModule)
- [HttpServerBaseModule](xref:ECellDive.Modules.HttpServerBaseModule)
  - [HttpServerAPICheckModule](xref:ECellDive.Modules.HttpServerAPICheckModule)
  - [HttpServerFbaModule](xref:ECellDive.Modules.HttpServerFbaModule)
  - [HttpServerImporterModule](xref:ECellDive.Modules.HttpServerImporterModule)
  - [HttpServerInfoQueryModule](xref:ECellDive.Modules.HttpServerInfoQueryModule)
  - [HttpServerModificationModule](xref:ECellDive.Modules.HttpServerModificationModule)
- [NodeGO](xref:ECellDive.Modules.NodeGO)

### GameNetModule's children must override 3 methods
Currently, [GameNetModule](xref:ECellDive.Modules.GameNetModule) inherits from the interface [IDive](xref:ECellDive.Interfaces.IDive) as well as [IMlprData](xref:ECellDive.Interfaces.IMlprData) which have methods that **MUST** be implemented by children.

For [IDive](xref:ECellDive.Interfaces.IDive) it is
[GenerativeDiveInC](xref:ECellDive.Interfaces.IDive.GenerativeDiveInC). This coroutine is where the user must define what to generate in the new dive scene. This is specific to every "diveable" module so, of course, it must be defined.

For [IMlprData](xref:ECellDive.Interfaces.IMlprData), there are two methods, [AssembleFragmentedData](xref:ECellDive.Interfaces.IMlprData.AssembleFragmentedData) and [RequestSourceDataGenerationServerRpc](xref:ECellDive.Interfaces.IMlprData.RequestSourceDataGenerationServerRpc(System.UInt64)). The former's role is to reassemble network-synchronized data that has been imported by one the user in the multiplayer session and that was broadcasted (as fragments) to all other clients of the session. The later's role is to request the server to generate the representation of the data stored in the module. It will likely be called inside [GenerativeDiveInC](xref:ECellDive.Interfaces.IDive.GenerativeDiveInC).

To enforce the implementation of [GenerativeDiveInC](xref:ECellDive.Interfaces.IDive.GenerativeDiveInC) and[AssembleFragmentedData](xref:ECellDive.Interfaces.IMlprData.AssembleFragmentedData), the [GameNetModule](xref:ECellDive.Modules.GameNetModule) was marked as `abstract` and so were the two methods. However, it is not possible to do so for [RequestSourceDataGenerationServerRpc](xref:ECellDive.Interfaces.IMlprData.RequestSourceDataGenerationServerRpc(System.UInt64)) as `RPCs` cannot be `abstract`. So developers must be vigilant to not forget it.

### Examples (available in the assets)
The Unity project contains an example of basic setup for gameobjects (GOs) using the Module and GameNetModule components. Those are respectively called `BaseModule` and `BaseGameNetModule`. You can find them under `Assets\Resources\Prefabs\Modules`.

Both GOs have exactly the same structure: \
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
The [Module](xref:ECellDive.Modules.Module) component shows in the Inspector as:\
<img src="../../resources/images/dev/BaseModule/Component.jpg" alt="Base Module 3DModel" style="width: 500px;"/>

The [GameNetModule](xref:ECellDive.Modules.GameNetModule) component shows in the Inspector as:\
<img src="../../resources/images/dev/BaseGameNetModule/Component.jpg" alt="Base Module 3DModel" style="width: 500px;"/>

The `GameNetModule` component is marked as `abstract` so it cannot directly be a component. That is why, we created [DummyGameNetModule](xref:ECellDive.Modules.DummyGameNetModule) just for the purpose of demonstration of the component. It is in fact empty.

### General workflow to create a new module
1. We recommend to start by duplicating the `BaseModule` or the `BaseGameNetModule` to have a working basis.
2. Create a new script following the naming convention `XXXModule` somewhere under `Assets\Scripts\Modules`. You may create a new folder if you think it's relevant.
   1. If you are making a new locale module, then have your class inherit from [Module](xref:ECellDive.Modules.Module). Alternatively, if you are making a new server action module to communicate with a `Kosmogora-like` server, you can have your class inherit from [HttpServerBaseModule](xref:ECellDive.Modules.HttpServerBaseModule) which already inherits from [Module](xref:ECellDive.Modules.Module) (see also the [next section](about_modules.md#modules-to-communicate-with-kosmogora) about HTTP modules).
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
   2. If you are making a new multiplayer module, then have your class inherit from [GameNetModule](xref:ECellDive.Modules.GameNetModule) and implement the three mandatory methods.
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
                //related to the data associated to this GO.
            }
            #endregion
        }
    }
    ```
3. Add the new component to the GO of your module.
4. Compare with the [Module](xref:ECellDive.Modules.Module) component or the [DummyGameNetModule](xref:ECellDive.Modules.DummyGameNetModule) component to assign the correct values in the fields in the inspector.
5. Remove the [Module](xref:ECellDive.Modules.Module) component or the [DummyGameNetModule](xref:ECellDive.Modules.DummyGameNetModule) component.
6. Adapt the 3D model and textures of your GO via the child `Module Graphics`.
7. Add UI Menus if needed.

## Modules to communicate with _Kosmogora_
Every module in _ECellDive_ with the prefix `HttpServer` is a _server action module_ that relies on HTTP requests and expect a `Kosmogora-like` server to implement these requests. In theory, some of the tasks performed remotely on a `Kosmogora-like` server could also be performed locally on the standalone VR device. Nonetheless, several reasons might motivate to perform them remotely including:
- Computational resources management. A standalone VR device already has its hands full with rendering ECellDive so delegating the work to remote computers avoid impacting the framerate at the cost of some latency.
- Reusing packages. Many scientific tools are implemented in Python or have a Python binding but not a C# binding.
- Modularity of the implementation of the tasks. Anyone can implement its own solution to a task (its own `Kosmogora-like` server) and connect it with _ECellDive_ as long as the interface of the HTTP request is respected.

### HttpServerBaseModule
Currently, every `HttpServerXXXModule` in _ECellDive_ inherits from [HttpServerBaseModule](xref:ECellDive.Modules.HttpServerBaseModule) which provides the basic utility to get which `Kosmogora-like` server is suitable for this module, to build URLs for HTTP requests, to send HTTP requests and store the result.

#### GetAvailableServers
[HttpServerBaseModule](xref:ECellDive.Modules.HttpServerBaseModule) is marked `abstract` (consequently, it cannot be attached to a GO) and has one `abstract` method ([GetAvailableServers](xref:ECellDive.Modules.HttpServerBaseModule.GetAvailableServers)) that must be implemented by the derived classes. [GetAvailableServers](xref:ECellDive.Modules.HttpServerBaseModule.GetAvailableServers) defers the responsibility to derived classes to find out which `Kosmogora-like` server is suitable to perform the tasks this module encapsulates. Typically, a `HttpServerXXXModule` implements [GetAvailableServers](xref:ECellDive.Modules.HttpServerBaseModule.GetAvailableServers) as such:
```csharp
protected override List<ServerData> GetAvailableServers()
{
	return HttpNetPortal.Instance.GetModuleServers("HttpServerXXXModule");
}
```
The only exception is [HttpServerAPICheckModule](xref:ECellDive.Modules.HttpServerAPICheckModule) which returns `null` because it attempts to connect to any server in order to check the HTTP commands implemented by this server.
If the contacted server implements the required HTTP commands of, at least, one module in _ECellDive_, the server contact information is stored in the dictionary [modulesServers](xref:ECellDive.IO.HttpNetPortal.modulesServers) in the singleton [HttpNetPortal](xref:ECellDive.IO.HttpNetPortal) (hence, we use [GetModuleServers](xref:ECellDive.IO.HttpNetPortal.GetModuleServers(System.String)) in the code snippet above to retrieve this information).

#### Build URLs
[HttpServerBaseModule](xref:ECellDive.Modules.HttpServerBaseModule) exposes methods to its children to write the URLs sent in the HTTP requests.
We can [add pages](xref:ECellDive.Modules.HttpServerBaseModule.AddPagesToURL(System.String[])):
```csharp
string url = AddPagesToURL(new string[] { "page1", "page2" });
// url = "http://serverIP:port/page1/page2"
```
We can add [one query](xref:ECellDive.Modules.HttpServerBaseModule.AddQueryToURL(System.String,System.String,System.String,System.Boolean)) to an existing URL:
```csharp
string url = "http://serverIP:port/page1/page2"

url = AddQueryToURL(url, "query1", "param1", true);
// url = "http://serverIP:port/page1/page2?query1=param1"

url = AddQueryToURL(url, "query2", "param2");
// url = "http://serverIP:port/page1/page2?query1=param1#query2=param2"
```
Finally we can add [multiple queries](xref:ECellDive.Modules.HttpServerBaseModule.AddQueriesToURL(System.String,System.String[],System.String[])) to an existing URL:
```csharp
string url = "http://serverIP:port/page1/page2"

url = AddQueriesToURL(url,
		new string[] { "query1", "query2" },
		new string[] { "param1", "param2" });
// url = "http://serverIP:port/page1/page2?query1=param1#query2=param2"
```

#### Send HTTP requests and receive the result
[HttpServerBaseModule](xref:ECellDive.Modules.HttpServerBaseModule) exposes [GetRequest(string uri)](xref:ECellDive.Modules.HttpServerBaseModule.GetRequest(System.String)) to wrap around Unity's `UnityWebRequest.Get(uri)`. [GetRequest](xref:ECellDive.Modules.HttpServerBaseModule.GetRequest(System.String)) is a coroutine that will wait until it hears back from the `UnityWebRequest` (with a time out of 10 seconds) and store the information about the request [requestData](xref:ECellDive.Modules.HttpServerBaseModule.requestData). This data structure can be used to know whether the request has been processed and what is the result.   
If the `Kosmogora-like` server sent back data, it is stored in the string encoding of a Json file [requestData.requestText](xref:ECellDive.Utility.Data.Network.RequestData.requestText). Typically, the code to send a request to a `Kosmogora-like` server from a module deriving from [HttpServerBaseModule](xref:ECellDive.Modules.HttpServerBaseModule) looks like the following:
```csharp

private string BuildURL(/*parameters?*/)
{
	//Use AddPagesToURL, AddQueryToURL, and AddQueriesToURL to build the url of the request
    return /*URL*/;
}

//This method is the public interface.
//Maybe called back after clicking a button in ECellDive's UI.
public void Request()
{
	StartCoroutine(RequestC(/*parameters?*/));
}

private IEnumerator RequestC(/*parameters?*/)
{
	string requestURL = BuildURL(/*parameters?*/);

    StartCoroutine(GetRequest(requestURL));//GetRequest is inherited from HttpServerBaseModule

	yield return new WaitUntil(isRequestProcessed);//isRequestProcessed is inherited from HttpServerBaseModule

	if (requestData.requestSuccess)
	{
		//Parse the output text to Json
		requestData.requestJObject = JObject.Parse(requestData.requestText);

        //Process the Json data as needed

        //Give feedback where relevant to show that it's a success
	}
	else
	{
		//Give feedback to the user to show that it's a failure. 
	}
}
```

### Required HTTP API per module
The list of HTTP commands that an `HttpServerXXXModule` requires in a `Kosmogora-like` server is stored by each `HttpServerXXXModule` in the array [implementedHttpAPI](xref:ECellDive.Modules.HttpServerBaseModule.implementedHttpAPI) inherited from [HttpServerBaseModule](xref:ECellDive.Modules.HttpServerBaseModule). We set the values from the Unity editor.

| __Server Action Module__                                                           | Values in `implementedHttpAPI`                       |
|------------------------------------------------------------------------------------|------------------------------------------------------|
|[HttpServerAPICheckModule](xref:ECellDive.Modules.HttpServerAPICheckModule)         | None                                                 |
|[HttpServerFbaModule](xref:ECellDive.Modules.HttpServerFbaModule)                   | `solve`                                              |
|[HttpServerImporterModule](xref:ECellDive.Modules.HttpServerImporterModule)         | `list_models` <br> `open_view`                       |
|[HttpServerInfoQueryModule](xref:ECellDive.Modules.HttpServerInfoQueryModule)       | `reaction_information`                               |
|[HttpServerModificationModule](xref:ECellDive.Modules.HttpServerModificationModule) | `list_user_model` <br> `open_user_model` <br> `save` |

To check the API of a candidate `Kosmogora-like` server, [HttpServerAPICheckModule](xref:ECellDive.Modules.HttpServerAPICheckModule) sends the request `http://serverIP:serverPort/apis` and expects to get back a string encoding a JSON file with a list of names: 
```json
{
    "apis":[
        /*name of commands implemented by the server*/
    ]
}
```
Therefore, if a candidate `Kosmogora-like` returns a list containing the names in the table above, the corresponding module will be unlocked in _ECellDive_. For example, if the server returns `[solve, list_models, reaction_information]`, then only HttpServerFbaModule and HttpServerInfoQueryModule will be accessible.

> [!NOTE]
> Currently, [HttpServerAPICheckModule](xref:ECellDive.Modules.HttpServerAPICheckModule) performs a loose API check which only verifies that the command names above are present in the list that the candidate `Kosmogora` server sent. In the future, we will constrain the checking process by including unit tests of every commands with dummy parameters.