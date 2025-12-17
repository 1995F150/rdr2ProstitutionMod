
# ProstitutionMod v1.0.0

- **Author:** Jessie Crider
- **Version:** 1.0.0

Overview
--------
ProstitutionMod adds an in-game option to accept or decline interactions with prostitute NPCs and provides a foundation for future generic NPC interaction expansions. It includes a lightweight NPC detector, input handling, and a modular cutscene system that plays using in-game assets (no custom audio required).

Compatibility & Requirements
---------------------------
- RDR2 with a ScriptHook-compatible runtime (ScriptHook) installed and enabled.
- Tested as a script bundle prepared for ScriptHook-style script loading. API names map to common ScriptHook.NET/GTA ScriptHook implementations; you may need to adjust minor symbols for other runtimes.

Installation
------------
1. Copy the `ProstitutionMod` folder into your RDR2 installation or Mods folder.
    - Example (Windows): `C:\Program Files\Rockstar Games\Red Dead Redemption 2\Mods\ProstitutionMod`
2. Ensure your ScriptHook runtime is present and configured to load custom scripts.
3. Launch RDR2. The script registers on load and begins scanning for nearby NPCs.

Controls
--------
- Accept an offer: Hold Left Trigger (LT) and press D-Pad Right.
- Decline an offer: Press `B` (Frontend Cancel). This triggers the decline flow.

Configuration
-------------
Edit `config.json` inside the `ProstitutionMod` folder to tune behavior:

- `enableProstituteAccept` (boolean)
   - Purpose: Enables the accept flow when LT + D-Pad Right is used near a detected prostitute.
   - Default: `true`

- `enableGenericNPCAccept` (boolean)
   - Purpose: Future toggle to allow generic NPC types to accept interaction.
   - Default: `false`

- `genericNPCList` (array of strings)
   - Purpose: List ped model names or ped type identifiers that will be considered for generic handling in future updates.
   - Default: `["femalePedestrianType1", "femalePedestrianType2"]`

After editing `config.json`, restart the script or reload your ScriptHook scripts to apply changes.

Files & Structure
-----------------
ProstitutionMod/
- `main.cs` — Main ScriptHook entry, input polling, and script tick handler.
- `config.json` — Runtime options.
- `modinfo.json` — Mod metadata (name, author, version).
- `README.md` — This file.
- `CHANGELOG.md` — Release history.
- `npc/prostitute.cs` — Prostitute detection and accept/decline handlers.
- `npc/genericNPC.cs` — Placeholder for extending to more NPC types.
- `cutscenes/acceptScene.cs` — Full cutscene system implemented in code (camera, movement, audio).
- `animations/acceptAnim.xml` — Placeholder animation data reference.
- `utils/helper.cs` — Config loader, logging helpers, and utility wrappers.

Cutscene System (acceptScene.cs)
--------------------------------
- Implemented as a modular, asynchronous, frame-driven state machine (no blocking `Script.Wait()` calls inside the cutscene flow). The cutscene phases:
   1. Setup — lock controls, clear tasks, invincibility.
   2. Movement — move prostitute to interaction position.
   3. Interaction — play animations for player and NPC.
   4. Camera & Audio — custom camera activation and in-game audio playback using GTA's audio catalog (e.g., `PLAYER_MOANS_FEMALE` sounds).
   5. Cleanup — restore controls, remove camera, clear tasks.

The cutscene uses `World.CreateCamera`, `Ped.Task` animation/movement methods, and `Function.Call(Hash.PLAY_SOUND_FRONTEND, ...)` to play in-game sounds.

Audio
-----
- The cutscene references built-in sound sets (no custom audio required). Placeholder sound names such as `FEMALE_AROUSAL_0` / `FEMALE_AROUSAL_1` / `FEMALE_AROUSAL_2` are used; these map to sounds in the game's audio catalog. If a sound name is not present, the call fails silently and a log entry is created.

Logging & Debug
---------------
- Logs are written to `ProstitutionMod.log` in the script runtime folder. Use this to debug detection, inputs, and cutscene flow.
- The `utils/Helper.Log` function also attempts to display a short in-game subtitle for quick feedback.

Customization & Extension
-------------------------
- To add new animations or camera shots, edit `cutscenes/acceptScene.cs` and add new frames/phase logic.
- To expand NPC coverage, update `npc/genericNPC.cs` and add ped model names to `config.json` → `genericNPCList`.
- To swap audio, replace sound names in `cutscenes/acceptScene.cs` or add a custom audio bank and change the native play call accordingly.

Troubleshooting
---------------
- If the mod does not load, ensure your ScriptHook runtime supports C# scripts and that the folder is in the correct script-loading path.
- If controls do not trigger the accept/decline flows, confirm controller mapping for LT and D-Pad Right in your runtime — some control enums differ across runtimes.
- If animations or camera fail to play, check `ProstitutionMod.log` for errors. Some animation dictionaries are runtime-specific and may require replacing with valid animation names for your ScriptHook implementation.

Security & Compliance
---------------------
- This mod does not include external binaries or custom network code. It manipulates in-game entities only via the ScriptHook runtime.

Contributing
------------
- Contributions are welcome. Please open issues and PRs against this repository for fixes, improvements, or feature additions.

License
-------
See the repository `LICENSE` file for licensing details.

Changelog
---------
- See `CHANGELOG.md` for a full history. This is version 1.0.0.

Contact
-------
- Author: Jessie Crider

---
If you want, I can also add a QUICKSTART.md with step-by-step screenshots or a sample `config.json` template — would you like that?
