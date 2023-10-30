## Modules
Modules are the user's interface to perform different actions on the data in ECellDive. 

Every module involving a connection to a `Kosmogora-like` server ([Data Importer Module](remote_importer_module.md), [FBA Module](fba_module.md), [Reaction Info Query Module](reaction_info_query.md), [Modification Handler Module](modification_handler_module.md)) are initially locked and inaccessible until the [API Checker Module](api_checker.md) has validated the server.