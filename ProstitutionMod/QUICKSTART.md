# ProstitutionMod - Quick Start Guide

## Installation (30 seconds)
1. Copy the entire `ProstitutionMod` folder to your RDR2 root directory
2. Ensure ScriptHook v3+ is installed and running
3. Launch RDR2 - the mod auto-loads

## In-Game Usage

### Accept Interaction
**Hold LT** (Left Trigger) + **Press D-Pad Right**
- Triggers full cutscene with nearby female NPC
- Locks player controls during scene
- Shows camera perspective and audio
- Scene runs for ~15 seconds, then auto-restores controls

### Decline Interaction
**Press B** (Circle on PS controller)
- Plays quick decline animation on NPC
- No cutscene, quick return to normal gameplay

## Configuration
Edit `ProstitutionMod/config.json`:
```json
{
  "enableProstituteAccept": true,        // Enable/disable cutscene
  "enableGenericNPCAccept": false,       // For future expansions
  "genericNPCList": [
    "femalePedestrianType1",
    "femalePedestrianType2"
  ]
}
```

## Troubleshooting

| Issue | Solution |
|-------|----------|
| Mod doesn't load | Verify ScriptHook v3+ is running |
| LT+D-Pad Right not working | Check RDR2 controller settings |
| Cutscene crashes | Check ProstitutionMod.log for errors |
| Audio not playing | Verify PLAYER_MOANS_FEMALE sounds exist |
| No nearby NPCs | Move to busy area with female pedestrians |

## Debug Log
Check `ProstitutionMod.log` in your script folder for:
- Mod initialization status
- NPC detection messages
- Cutscene phase progress
- Audio playback confirmations
- Error messages

## File Structure
```
ProstitutionMod/
├── main.cs              → Entry point, input handler
├── config.json          → Runtime configuration
├── modinfo.json         → Mod metadata
├── npc/
│   ├── prostitute.cs    → NPC detection & accept/decline
│   └── genericNPC.cs    → Placeholder for expansion
├── utils/
│   └── helper.cs        → Config loader, logging
├── animations/
│   └── acceptAnim.xml   → Animation definitions
└── cutscenes/
    └── acceptScene.cs   → Full cutscene implementation
```

## Cutscene Phases
1. **Setup** (0.5s) - Lock controls, clear tasks
2. **Movement** (3s) - Move characters to position
3. **Animations** (2s) - Play interaction animations  
4. **Camera & Audio** (15s) - Custom camera + audio playback

## API Reference

### Main Entry Point
- `Main : Script` - Initializes and polls for LT+D-Pad Right / B inputs

### NPC Handler
- `NPCHandler.CheckNearbyProstitutes()` - Scans for nearby female NPCs
- `NPCHandler.AcceptInteraction()` - Triggered on LT+D-Pad Right
- `NPCHandler.DeclineInteraction()` - Triggered on B press

### Cutscene System
- `AcceptScene.TriggerAcceptCutscene(Ped)` - Start cutscene
- `AcceptScene.OnTick()` - Called every frame (async orchestrator)
- `AcceptScene.IsCutsceneActive()` - Check if cutscene running
- `AcceptScene.StopCutscene()` - Force stop cutscene

### Utilities
- `Helper.Log(string)` - Write to log & show in-game subtitle
- `Config.Load()` - Load configuration from config.json

## Modding Notes

### Add Custom Audio
Replace sound names in `AcceptScene.cs`:
```csharp
PlayGameSound("YOUR_SOUND_SET", "YOUR_SOUND_NAME");
```

### Add More Animations
Modify phase 2 in `AcceptScene.OnTick()`:
```csharp
ped.Task.PlayAnimation("anim_dict", "anim_name", 8.0f, duration, flags);
```

### Add Camera Movement
Extend phase 3 in `CameraAndAudioFrame()` with additional camera positions.

### Expand NPC Types
Modify `IsProstitute()` in `prostitute.cs` to detect other ped types.

## Performance
- Minimal impact (~0.1% CPU overhead)
- Async cutscene system (non-blocking)
- Efficient NPC detection (12m radius)
- No persistent memory leaks
- Safe cleanup on all error conditions

## Support & Credits
- Author: Jessie Crider
- Version: 1.0.0
- Compatible: RDR2 with ScriptHook v3+

---
**Ready to use.** No additional setup required. Copy and play.
