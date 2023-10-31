## Reaction Info Query
Requires:
- A [connection to Kosmogora](../Network/connecting_to_Kosmogora.md).
- The [API Checker](./api_checker.md) has validated the `Kosmogora` server.

<img src="../../../resources/images/modules/reaction_info_query.jpg" alt="Reaction Info Query" style="height: 300px;"/>

This is the module that is used to query reaction information from remote database through *Kosmogora*.

<img src="../../../resources/images/modules/1x/reaction_info_query_panel.png" alt="RIQ panel" style="height: 300px;"/>

### Panel *Parameters*
1. Button *Server*. Will open the Server choice panel to chose among the `Kosmogora-like` servers which API implements the logic to dispatch queries to remote databases. 
2. Button *Database*. Will open a popup with a list of database you can chose from (**as of v0.11.x-alpha Only BIGG and MetanetX are available**).
3. Input Field *Target Reaction ID*. Input here the SUID of the reaction for which you wish to obtain details from the target database.
4. Button *Query*. Send the query to *Kosmogora* that will translate it and sent it to remote database. If the query is successful, the module will flash <span style="color:green">*green*</span>; an information tag will be added to the reaction describing the species involved. If the query fails, the module will flash <span style="color:red">*red*</span>. In that case, you can check the error message in the [log](/articles/UserManual/UIMenus/log_menu.html).