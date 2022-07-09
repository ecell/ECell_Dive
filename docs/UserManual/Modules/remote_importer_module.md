---
layout: default
title: Remote Importer
parent: Modules
grand_parent: User Manual
---

## Remote Importer
Requires a connection to [answer_talker](https://github.com/ecell/answer_talker).

This is the module that is used to import the data stored on *answer_talker*.
- Panel *Server Parameters*:
    - Input Field *Data Server IP*. The IP address at which *answer_talker* is hosted. Default value is 127.0.0.1 but this only works during development if *answer_talker* is hosted on the same machine as the one we are developping ECellDive on.
    - Input Field *Data Server Port*. The Port at which *answer_talker* is listening. Default value is 8000. This is also the default value when launching *answer_talker* so, unless you specified a port at that time, you can leave the field empty in ECellDive.
    - Button *Query Available Models*. Will try to contact *answer_talker* at the address and port given to request the list of models currently available. This is fast. If nothing happens check the Log Menu to see if an error was not raised.
- Panel *Models List*. Displays the list of models available in *answer_talker* if the request was successful.