# Before you start...

> [!NOTE]
> You must have prior knowledge about Unity or another game engine. We discourage learning Unity by diving in this project even if you have prior programming knowledge. 

This documentation aims to give a comprehensive understanding of how _ECellDive_ is currently implemented so that anyone can copy assets for their own projects or contribute to the project independantly.

However, _ECellDive_ is still considered in alpha stage and parts of the code base might be refactored several times. As a general guideline, we will try to keep the explanations section matching any minor version number update. Currently, _ECellDive_ versioning follows the classic format **vMAJOR.MINOR.PATCH-stage**. Hence, a release update from **v0.11.5-alpha** to **v0.11.6-alpha** will not contain an update of this section. A release update from **v0.11.5-alpha** to **v0.12.X-alpha** or above will, however.

In this section, we give context to answer the following questions:
- [What are the general coding guidelines?](./about_code.md)
- [How are action modules implemented?](./about_modules.md)
- [How to add new modules?](./about_modules.md#general-workflow-to-create-a-new-module)
- [How to communicate with `Kosmogora-like` servers?](./about_modules.md#modules-to-communicate-with-kosmogora)
- How to use the API to generate graphs? (coming soon)
- [What makes a _Player_?](./about_player.md)
- [How is the main scene structured?](./about_scenes.md#main-scene-of-the-project)
- [How works the _Dive Scene_ system?](./about_scenes.md#dive-scenes)
- [How works the multiplayer hosting/joining?](./about_multiplayer.md#hosting-and-joining)
- How works the multiplayer data broadcast when a new client joins? (coming soon)
- [How works the multiplayer data broadcast among connected clients?](./about_multiplayer.md#broadcast-data)
- [What UI is available?](./about_UI.md#2d-ui-menus)
- [How do we handle interactions?](./about_UI.md#interactions)
- and probably others...

Do not hesitate [to start an issue](https://github.com/ecell/ECell_Dive/issues) if you have other questions that would be like to be answered here.