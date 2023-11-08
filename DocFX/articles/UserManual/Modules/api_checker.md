# API Checker
Requires a [Kosmogora](../Network/connecting_to_Kosmogora.md) server.

<img src="../../../resources/images/modules/api_checker.jpg" alt="API Checker" style="height: 300px;"/>

This is the module to check that a `Kosmogora-like` server implements the HTTP API requested for ever server action modules (the [Remote Importer](./remote_importer_module.md), [Flux Balance Analysis](./fba_module.md), [Modification Handler](./modification_handler_module.md), and [Reaction Info Query](./reaction_info_query.md)). If the server implements the API, the buttons to spawn the modules in the [Modules Menu](../UIMenus/modules_menu.md) in the scene will be activated.

### The *Server Parameters* pannel:
<img src="../../../resources/images/modules/1x/api_Checker_panel.png" alt="API Checker" style="height: 300px;"/>

1. Input Field *IPv4*. The IP address at which *Kosmogora* is hosted. Default value is 127.0.0.1 but this only works during development if *Kosmogora* is hosted on the same machine as the one we are developping ECellDive on.
2. Input Field *Port*. The Port at which *Kosmogora* is listening. Default value is 8000. This is also the default value when launching *Kosmogora* so, unless you specified a port at that time, you can leave the field empty in ECellDive.
3. Input Field *Name*. The name this server will be displayed with in the rest of _ECellDive_ when using modules that can connect to this server.
3. Button *Check API*. Will send an http request called `/apis` to retrieve the list of the names of the commands that the `Kosmogora-like` server implements. Then, it will compare the http commands used by every module in _ECellDive_ and allow users to use such module if the server implements them all.