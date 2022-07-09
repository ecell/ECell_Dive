---
layout: default
title: Modification Handler
parent: Modules
grand_parent: User Manual
---

## Modification Handler
Requires a connection to [answer_talker](https://github.com/ecell/answer_talker).

This module is used to import and save modification files associated to a model loaded in one of the dive scene. One must input the IP address of the server to query available files but also to save new files (even if the buttons for these two actions are in different panels).
- Panel *Server Parameters*:
    - Input Field *Data Server IP*. The IP address at which *answer_talker* is hosted. Default value is 127.0.0.1 but this only works during development if *answer_talker* is hosted on the same machine as the one we are developping ECellDive on.
    - Input Field *Data Server Port*. The Port at which *answer_talker* is listening. Default value is 8000. This is also the default value when launching *answer_talker* so, unless you specified a port at that time, you can leave the field empty in ECellDive.
    - Button *Query Available Modification Files*. Will try to contact *answer_talker* at the address and port given to request the list of modification files currently available. This is fast. If nothing happens check the Log Menu to see if an error was not raised.

- Panel *Save Modification Files*:
    - Button *Base Model*. Will scan for all the base models loaded in ECellDive and activate the panel *Loaded Base Models* for the user to chose one.
    - Input Field *File Name*. The name of the modification file when saved on the server. If you don't input anything, the name of the file will be set by default to *NameOfBaseModel_yyyyMMddTHHmmssZ*. 
    - Button *Save*. Interactable only if a base model was selected.  Will try to contact *answer_talker* at the address and port given to save a modification file. This is fast. You can check whether the file was correctly saved by checking the Log Menu. A trace message confirming the save should be displayed. Otherwise, an error should be newly registered. **Saving a modification file is closely related to the development of the format of modification file on the side of answer_talker. The content of the saved modification file is therefore subject to a lot of changes.**

- Panel *Modification Files*. Displays the list of modification files available in *answer_talker* if the request was successful.
    - Button *NameOfModificationFile*. There should be one button by modification file saved on the server. Upon clicking the button, the modification file is imported and immediately applied to the corresponding model if it is loaded in a dive scene.

- Panel *Loaded Base Models*. Displays the models loaded in the dive scenes for which we can save the current modifications as compared to when it was loaded.
    - Button *NameOfTheModel*. There should be one button by model loaded in ECellDive (indescriminately of the Dive Scenes). Clicking on the button registers the target base model.