# Changelog

All notable changes to this project will be documented in this file.

## [1.0.0] - 2025-12-16
### Added
- Initial release of ProstitutionMod
- `main.cs` script registration and L+T interaction handling
- `config.json` with runtime toggles for prostitute and generic NPC handling
- `npc/prostitute.cs` detection and accept interaction flow (placeholder game bridge)
- `npc/genericNPC.cs` placeholder for future NPC expansions
- Placeholder animation (`animations/acceptAnim.xml`) and cutscene (`cutscenes/acceptScene.xml`)
- Utilities and helpers in `utils/helper.cs` for logging, timing, and config loading

### Notes
- Placeholder assets and runtime bridge (`GameBridge`) must be replaced or connected to your ScriptHook runtime APIs for full functionality.
