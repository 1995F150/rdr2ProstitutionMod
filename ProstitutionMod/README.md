# ProstitutionMod v1.0.0

- **Author:** Jessie Crider
- **Version:** 1.0.0

Overview
--------
ProstitutionMod adds an in-game option to accept or decline interactions with prostitute NPCs and provides a foundation for future generic NPC interaction expansions.

Installation
------------
1. Unzip or place the `ProstitutionMod` folder into your RDR2 Mods directory.
   - Example path (Windows): `RDR2\Mods\ProstitutionMod`
2. Ensure ScriptHook (or your ScriptHook-compatible runtime) is installed and running.
3. Launch the game. The mod registers itself at script start.

Usage
-----
- Press `L` + `T` to trigger an interaction check and attempt to call the nearest prostitute NPC's accept handler.
- The mod periodically scans for nearby prostitute NPCs and logs findings to `ProstitutionMod.log` in the runtime directory.

Configuration
-------------
All runtime options are available in `config.json` inside the `ProstitutionMod` folder:

- `enableProstituteAccept` (boolean): Enable/disable automatic accept interactions with detected prostitutes. Default: `true`.
- `enableGenericNPCAccept` (boolean): Enable handling for generic NPCs listed in `genericNPCList`. Default: `false`.
- `genericNPCList` (array): Placeholder list of model or ped type names to treat as generic NPCs for future features.

To change behavior, edit `config.json` and restart the script or reload scripts via your ScriptHook tool.

Notes & Development
-------------------
- The included animation (`animations/acceptAnim.xml`) and cutscene (`cutscenes/acceptScene.xml`) are placeholders; replace with game-appropriate assets if desired.
- `npc/genericNPC.cs` is intentionally a placeholder to make future expansions straightforward.
- Logging is written to `ProstitutionMod.log` in the script runtime folder. Use it for debugging and tuning.

License
-------
See the repository `LICENSE` file for licensing details.
