## Reaction Info Query
Requires a connection to [answer_talker](https://github.com/ecell/answer_talker).

<img src="../../../resources/images/modules/reaction_info_query.jpg" alt="Reaction Info Query" style="height: 300px;"/>

This is the module that is used to query reaction information from remote database through *answer_talker*.

<img src="../../../resources/images/modules/1x/reaction_info_query_panel.png" alt="RIQ panel" style="height: 300px;"/>

### Panel *Parameters*
1. Input Field *Data Server IP*. The IP address at which *answer_talker* is hosted. Default value is 127.0.0.1 but this only works during development if *answer_talker* is hosted on the same machine as the one we are developping ECellDive on.
2. Input Field *Data Server Port*. The Port at which *answer_talker* is listening. Default value is 8000. This is also the default value when launching *answer_talker* so, unless you specified a port at that time, you can leave the field empty in ECellDive.
3. Button *Database*. Will open a popup with a list of database you can chose from (**as of v0.11.x-alpha Only BIGG and MetanetX are available**).
4. Input Field *Target Reaction ID*. Input here the SUID of the reaction for which you wish to obtain details from the target database.
5. Button *Query*. Send the query to *answer_talker* that will translate it and sent it to remote database. If the query is successful, the module will flash <span style="color:green">*green*</span>; an information tag will be added to the reaction describing the species involved. If the query fails, the module will flash <span style="color:red">*red*</span>. In that case, you can check the error message in the [log](/articles/UserManual/UIMenus/log_menu.html).