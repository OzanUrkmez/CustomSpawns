# CustomSpawns

 A M&B:Bannerlord mod/API to allow for spawning troops and stacks independent of the games' main spawning system.

The nexus link with more Bannerlord content can be found at https://www.nexusmods.com/mountandblade2bannerlord/mods/411?tab=description

**Each section has a bold sentence or two at the start to help those who want to read over the documentation quickly**

## Custom Spawns API vs Calradia at War Disambiguation

**Calradia at War is not any different than any other sub mod that makes use of the Custom Spawns API. It only has a different file path for easier debugging.**

Calradia at War is simply a mod that runs on top of Custom Spawns API. It is one of many mods that has been developed with the API, though it holds a "special" position in the sense that there is a version of the API that comes packaged with Calradia at War. However, the way data is loaded and handled is no different than any other Custom Spawns mod, and multiple mods may use the API at once. The only difference is that the pre-packaged version can hold the API in the same file as Calradia at War rather than needing a separate submodule file.

This was a decision for facilitating ease of use, both for the developers of the API, and for the users. File juggling by the users is kept to a minimum, and the team working on the API can quickly test features by implementing content with the API in a short workflow.

## General Use of the API and Overview

**The API replicates the XML modding workflow that Bannerlord uses for highest ease of use.**

There are 3 different file locations that you need to be concerned with to use the API. The ModuleData folder of your module, the ModuleData folder of the Custom Spawns Clean API module or Calradia at War module (both can serve as the API but neither can exist together as CaW comes with the API), and a special CustomSpawns folder (named exactly that!) which holds all your API-specific content. We will refer to the CaW/API ModuleData folder as the API ModuleData folder.

We created an XML-based API to replicate the Bannerlord modding process. The API ModuleData folder houses all the config files (config.xml, custom_spawns_campaign_data_config.xml, and prisoner_recruitment_config.xml). The config.xml folder pertains to the mod in general, custom_spawns_campaign_data_config.xml
