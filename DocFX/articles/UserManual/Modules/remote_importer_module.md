## Remote Importer
Requires:
- A [connection to Kosmogora](../Network/connecting_to_Kosmogora.md).
- The [API Checker](./api_checker.md) has validated the `Kosmogora` server.

<img src="../../../resources/images/modules/remote_importer.jpg" alt="UI point" style="height: 300px;"/>

This is the module that is used to import the data stored on *Kosmogora*.

<img src="../../../resources/images/modules/1x/remote_importer_panels.png" alt="UI point" style="height: 300px;"/>

### Panel *Server Parameters*
1. Button *Server*. Will open the Server choice panel to chose among the `Kosmogora-like` servers which API allows to store and load data modules in _ECellDive_. 
2. Button *Query Available Models*. Will try to contact *Kosmogora* at the address and port given to request the list of models currently available. This is fast. If nothing happens check the Log Menu to see if an error was not raised. If the query is successful, the module will flash <span style="color:green">*green*</span>; the list of available files will be displayed in panel *Models List*. If the query fails, the module will flash <span style="color:red">*red*</span>. In that case, you can check the error message in the [log](/articles/UserManual/UIMenus/log_menu.html).