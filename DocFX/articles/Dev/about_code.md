# Project Code conventions
The following are loose constraints. We understand first draft of a code might not respect that. But as it stabilizes and is updated, any code file should tend toward those conventions.

## Naming
- Generally, the name of the file matches the name of the main class in the file (if any).
- File names, class names, and method names are Pascal Case (aka upper camel case) so every word in a classe name start by an upper case letter (e.g., `SomeScript.cs`, `AClassName`, `AMethodName`).
- Interface names start with a capital `I` and are then Pascal Case; that's pretty standard in C# (e.g., `IHereIsAnInterface`)
- Field names and property names are camel Case so they start with a low case letter and then wevery word start with an upper case letter (e.g. `hereIsAFieldName`, `andHereIsAPropertyName`). Camel case for property name is apparently not C# standard.
- Private fields of properties are prefixed with `m_` and then repeats the name of the property (e.g., `propertyWithFieldName`, `m_propertyWithFieldName`):
  ```csharp
  ///<summary>
  ///The field of the <see cref="aBoolProperty"/> property.
  ///</summary>
  private bool m_aBoolProperty

  ///<summary>
  ///The property using field <see cref="m_aBoolProperty"/>.
  ///</summary>
  public bool aBoolProperty
  {
    get => m_aBoolProperty;
    set => m_aBoolProperty = value;
  }
  ```
- Region statement' names are bounded by hyphens (`-`), include the name of the interface or class the methods or fields they contain come from, and tell if the content of the region are methods or members. Generally speaking, no region contain both.
  ```csharp
  #region - IHereIsTheInterfaceName Members -
  //here are fields and properties of interface IHereIsTheInterfaceName
  #endregion

  //
  //some code, maybe other regions.
  //

  #region - IHereIsTheInterfaceName Methods -
  //here are the implementation of the methods of interface IHereIsTheInterfaceName
  #endregion

  ```


## File Layout
- Methods inplementations should appear in alphabetical order.
- There is no order constraint for properties nor fields. This is to allow developers to organise the order in which the fields appear in the Inspector of Unity as they want.
- Interface implementations should appear in alphabetical oder of their interface name.
- Interface implementations should be bounded by `#region` and `#endregion` statements to allow popular IDEs to create automatic foldout buttons for readability.
- Unity methods overriden in a class (e.g., `Start`, `Update`, `OnTriggerEnter`, `OnNetworkSpawn`,...) should appear before methods of that class.
- Members and methods of a class appear before that of the interface implementation.

Here is a typical file structure for a gameobject component:
```csharp
public class ANewGOComponent: MonoBehaviour, IAnInterface, IBebopInterface, ICowboyInterface
{
    //
    // fields of ANewGOComponent in no particular order
    //

    #region - IAnInterface Members -
    //members in no particular order
    #endregion

    #region - IBebopInterface Members -
    //members in no particular order
    #endregion

    #region - ICowboyInterface Members -
    //members in no particular order
    #endregion

    //
    //methods of Unity overriden in ANewGOComponent
    //

    //
    //methods of ANewGOComponent in alphabetical order
    //

    #region - IAnInterface Methods -
    //methods alphabetical order
    #endregion

    #region - IBebopInterface Methods -
    //members alphabetical order
    #endregion

    #region - ICowboyInterface Methods -
    //members alphabetical order
    #endregion
}
```

## Namespaces
All the code base is encapsulated in namespaces. At the root is `ECellDive` and then various other namespace levels branche out from there.
Generally speaking, the layout of the directories under `Assets\Scripts` in the project represent the namespace ramifications where the folder names correspond to namespace levels. There are a few mismatches that will eventually be adjusted.

## Document everything!
Use the [XML based](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/xmldoc/) documentation to explain the purpose of every class, field, property and method. This will be automatically picked up by [DocFx](https://dotnet.github.io/docfx/) to generate the [Scripting API](xref:ECellDive).