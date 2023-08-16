# Stationeers mod "Events" for BepInEx.

This mod is intended to inject into your game (saves or new game) predefined and previously created events in a random and parameterized way.

What to expect from this mod?

<h3><b><u>---------spoiler-------------</u></b></h3>

<h2>random event injections</h2>

- vanilla event injection
- injection of custom events (created by you)

<h2>events</h2>

- Loading new events directly from the mod

<h3>predefined events:</h3>

- ColdFlare => type : 94
- Damage Crops => type: 95
- Power Surge => type : 96
- SolarFlare => type : 97
- Supply Crate (Random) => type : 98
 
<span style="color: red"> To add an event to your game, you must add them to the mod's configuration (EventsMod.conf) in the BeepInEx config folder after the first run.</span>

<h3>adding Events</h3>

Search for "Config custom incidents Values" and put the following text in the value for each event you want to add (for one more event you must separate them by <b>#</b>)

<b><span style="color: red">type</span>|MaxPerTile|SpawnChance|IsRepeating|MaxDelay|MinDelay|CanLaunchOutsideTile|Serialize|RequiresHumanInTile|RunOnTileEnter|ContainStructures</b>

<h4>example of adding two events:</h4>
type|MaxPerTile|SpawnChance|IsRepeating|MaxDelay|MinDelay|CanLaunchOutsideTile|Serialize|RequiresHumanInTile|RunOnTileEnter|ContainStructures<b>#</b>type|MaxPerTile|SpawnChance|IsRepeating|MaxDelay|MinDelay|CanLaunchOutsideTile|Serialize|RequiresHumanInTile|R unOnTileEnter|ContainStructures


For info about what this mod does, read the description in Steam Workshop


# Instructions

* If you don't have BepInEx installed, download the x64 version available at https://github.com/BepInEx/BepInEx/releases and follow the BepInEx installation instructions but basically you will need to:
     - Drop and unpack it inside Stationeers folder
     - Start the game once to finish installing BepInEx and check if he created the folders called \Stationeers\BepInEx\plugins, if yes, the BepInEx installation is completed.
* Start the game and close again.
* Download the last version on StationeersMods in https://github.com/jixxed/StationeersMods/releases .
* Unpack StationeersMods inside the folder \BepInEx\plugins.
* Edit BepInEx.cfg in ...\Stationeers\BepInEx\config replace HideManagerGameObject from false to true.
* Subcribe in this mod on workshop steam
* Start the game and play.

# Multiplayer Support

I think this work fine but, for secure put then in both side, client and server must have the mod, preferably in the same version.

# Developers

Pch91
Thunder321
