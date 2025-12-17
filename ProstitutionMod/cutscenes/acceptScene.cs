using System;
using System.Collections.Generic;
using GTA;
using GTA.Math;
using GTA.Native;

namespace ProstitutionMod.Cutscenes
{
    /// <summary>
    /// Cutscene system for prostitute accept interactions.
    /// Handles full cutscene flow including movement, camera control, audio, and timing.
    /// </summary>
    public static class AcceptScene
    {
        // Cutscene state
        private static bool cutsceneActive = false;
        private static Ped currentProstitute = null;
        private static Camera cutsceneCamera = null;

        // Audio event names from GTA's audio catalog (hardcoded in-game files)
        private const string GROAN_SOUND_SET = "PLAYER_MOANS_FEMALE";
        private const string PLEASURE_SOUND = "SHORT_MALE_PAIN";

        // ==================== EVENT SYSTEM ====================
        /// <summary>
        /// Static listener that hooks into accept interactions.
        /// Call this once during mod initialization.
        /// </summary>
        public static void Initialize()
        {
            ProstitutionMod.Utils.Helper.Log("AcceptScene system initialized");
        }

        /// <summary>
        /// Trigger the cutscene when player accepts prostitute offer.
        /// Called from NPCHandler.AcceptInteraction()
        /// </summary>
        public static void TriggerAcceptCutscene(Ped prostitute)
        {
            if (prostitute == null || !prostitute.Exists())
            {
                ProstitutionMod.Utils.Helper.Log("ERROR: Invalid prostitute ped for cutscene");
                return;
            }

            if (cutsceneActive)
            {
                ProstitutionMod.Utils.Helper.Log("Cutscene already active, cannot start new one");
                return;
            }

            ProstitutionMod.Utils.Helper.Log("Starting accept cutscene with prostitute handle: " + prostitute.Handle);
            currentProstitute = prostitute;
            cutsceneActive = true;

            // Launch cutscene coroutine
            StartCutsceneSequence();
        }

        // ==================== MAIN CUTSCENE SEQUENCE ====================
        /// <summary>
        /// Main cutscene flow orchestrator.
        /// Executes all cutscene phases in order with proper timing.
        /// </summary>
        private static void StartCutsceneSequence()
        {
            try
            {
                // PHASE 1: Setup
                ProstitutionMod.Utils.Helper.Log("[Cutscene] Phase 1: Setup");
                SetupCutscene();
                Script.Wait(500);

                // PHASE 2: Move to secluded location
                ProstitutionMod.Utils.Helper.Log("[Cutscene] Phase 2: Movement");
                MoveCharactersToLocation();
                Script.Wait(3000);

                // PHASE 3: Face each other and interact
                ProstitutionMod.Utils.Helper.Log("[Cutscene] Phase 3: Interaction");
                PlayInteractionAnimations();
                Script.Wait(2000);

                // PHASE 4: Camera focus and audio
                ProstitutionMod.Utils.Helper.Log("[Cutscene] Phase 4: Camera and Audio");
                PlayCameraSequence();
                PlayAudioSequence();

                // PHASE 5: Duration (cutscene plays for ~15 seconds)
                Script.Wait(15000);

                // PHASE 6: Cleanup
                ProstitutionMod.Utils.Helper.Log("[Cutscene] Phase 6: Cleanup");
                CleanupCutscene();
            }
            catch (Exception ex)
            {
                ProstitutionMod.Utils.Helper.Log("ERROR in cutscene sequence: " + ex.Message);
                CleanupCutscene();
            }
            finally
            {
                cutsceneActive = false;
                currentProstitute = null;
            }
        }

        // ==================== PHASE 1: SETUP ====================
        /// <summary>
        /// Prepare the cutscene environment.
        /// - Lock player controls
        /// - Make player invincible
        /// - Clear conflicting tasks
        /// </summary>
        private static void SetupCutscene()
        {
            Ped player = Game.Player.Character;
            if (player == null || !player.Exists()) return;

            // Lock player input
            Game.DisableControlThisFrame(0, Control.LookUpDown);
            Game.DisableControlThisFrame(0, Control.LookLeftRight);
            Game.DisableControlThisFrame(0, Control.MoveUpDown);
            Game.DisableControlThisFrame(0, Control.MoveLeftRight);
            Game.DisableControlThisFrame(0, Control.Sprint);

            // Make invincible during cutscene
            player.IsInvincible = true;

            // Clear any active tasks
            player.Task.ClearAllImmediately();
            currentProstitute.Task.ClearAllImmediately();

            ProstitutionMod.Utils.Helper.Log("Cutscene setup complete: controls locked, invincibility enabled");
        }

        // ==================== PHASE 2: MOVEMENT ====================
        /// <summary>
        /// Move player and prostitute to a discreet location.
        /// Placeholder: moves to nearby coordinates. Can be customized.
        /// </summary>
        private static void MoveCharactersToLocation()
        {
            Ped player = Game.Player.Character;
            if (player == null || !player.Exists() || currentProstitute == null || !currentProstitute.Exists())
                return;

            // Get player's current position and offset
            Vector3 playerPos = player.Position;
            float heading = player.Heading;

            // Calculate offset positions (side-by-side, facing each other)
            Vector3 offset = new Vector3((float)Math.Cos(heading * Math.PI / 180.0), (float)Math.Sin(heading * Math.PI / 180.0), 0);
            Vector3 prostiturePos = playerPos + offset * 1.5f;

            // Task prostitute to move to position
            currentProstitute.Task.GoStraightToCoord(prostiturePos, 1.0f, 5000, 0.5f, 0);

            ProstitutionMod.Utils.Helper.Log($"Moving prostitute to position: {prostiturePos}");

            // Wait for prostitute to arrive
            int timeout = 0;
            while (Vector3.Distance(currentProstitute.Position, prostiturePos) > 1.5f && timeout < 100)
            {
                Script.Wait(100);
                timeout++;
            }

            ProstitutionMod.Utils.Helper.Log("Characters moved to location");
        }

        // ==================== PHASE 3: INTERACTION ====================
        /// <summary>
        /// Play interaction animations for both characters.
        /// Uses in-game animation sets.
        /// PLACEHOLDER: Add more animations or reactions as needed.
        /// </summary>
        private static void PlayInteractionAnimations()
        {
            Ped player = Game.Player.Character;
            if (player == null || !player.Exists() || currentProstitute == null || !currentProstitute.Exists())
                return;

            // Face each other
            currentProstitute.Face(player);
            player.Face(currentProstitute);
            Script.Wait(500);

            // Play greeting/acceptance animation on prostitute
            // Dictionary: "amb@prop_human_seat_female@backhand_idle@base"
            currentProstitute.Task.PlayAnimation(
                "misscarsteal4@actor",
                "actor_react_succeed",
                8.0f,
                3000,
                AnimationFlags.None
            );

            // Play corresponding animation on player
            player.Task.PlayAnimation(
                "combat@damage@rb_writhe",
                "rb_writhe_loop",
                8.0f,
                3000,
                AnimationFlags.None
            );

            ProstitutionMod.Utils.Helper.Log("Interaction animations playing");
        }

        // ==================== PHASE 4A: CAMERA ====================
        /// <summary>
        /// Control in-game camera during cutscene.
        /// Smoothly transitions between positions for cinematic effect.
        /// PLACEHOLDER: Add additional camera shots, pan, or zoom effects.
        /// </summary>
        private static void PlayCameraSequence()
        {
            Ped player = Game.Player.Character;
            if (player == null || !player.Exists() || currentProstitute == null || !currentProstitute.Exists())
                return;

            try
            {
                // Calculate mid-point between player and prostitute for camera focus
                Vector3 midpoint = Vector3.Add(player.Position, currentProstitute.Position) / 2.0f;
                Vector3 cameraPos = midpoint + new Vector3(0, 0, 2.0f);

                // Create cutscene camera
                cutsceneCamera = World.CreateCamera(cameraPos, new Vector3(0, 0, 0), 50.0f);
                cutsceneCamera.PointAt(midpoint);

                // Activate camera
                World.RenderingCamera = cutsceneCamera;
                ProstitutionMod.Utils.Helper.Log("Camera activated and pointed at characters");

                // PLACEHOLDER: Add additional camera movements here
                // Example - pan left:
                // Vector3 newCameraPos = cameraPos + Vector3.Distance(cameraPos, midpoint) * Vector3.Normalize(Vector3.Cross(midpoint - cameraPos, Vector3.WorldUp));
                // CameraTransition(cutsceneCamera, newCameraPos, midpoint, 2000);
            }
            catch (Exception ex)
            {
                ProstitutionMod.Utils.Helper.Log("ERROR in camera sequence: " + ex.Message);
            }
        }

        /// <summary>
        /// Smoothly transition camera from one position to another.
        /// Used for cinematic panning effects.
        /// PLACEHOLDER: Expand with rotation, zoom, or easing effects.
        /// </summary>
        private static void CameraTransition(Camera cam, Vector3 targetPos, Vector3 targetLookAt, int duration)
        {
            if (cam == null) return;

            Vector3 startPos = cam.Position;
            int elapsed = 0;
            int step = 50; // ms per frame

            while (elapsed < duration && cam != null)
            {
                float progress = Math.Min((float)elapsed / duration, 1.0f);
                Vector3 newPos = Vector3.Lerp(startPos, targetPos, progress);
                cam.Position = newPos;
                cam.PointAt(targetLookAt);

                Script.Wait(step);
                elapsed += step;
            }

            ProstitutionMod.Utils.Helper.Log("Camera transition complete");
        }

        // ==================== PHASE 4B: AUDIO ====================
        /// <summary>
        /// Play audio/sound effects during cutscene.
        /// References in-game sound sets without requiring custom audio files.
        /// PLACEHOLDER: Add more sounds, ambient audio, or voice lines.
        /// </summary>
        private static void PlayAudioSequence()
        {
            try
            {
                // Initial greeting sound
                ProstitutionMod.Utils.Helper.Log("Playing audio: initial greeting");
                PlayGameSound(GROAN_SOUND_SET, "FEMALE_AROUSAL_0", 0.8f);
                Script.Wait(2000);

                // Mid-sequence reaction
                ProstitutionMod.Utils.Helper.Log("Playing audio: reaction");
                PlayGameSound(GROAN_SOUND_SET, "FEMALE_AROUSAL_1", 0.8f);
                Script.Wait(3000);

                // Peak moment
                ProstitutionMod.Utils.Helper.Log("Playing audio: peak");
                PlayGameSound(GROAN_SOUND_SET, "FEMALE_AROUSAL_2", 0.9f);
                Script.Wait(2000);

                // Wind down
                ProstitutionMod.Utils.Helper.Log("Playing audio: wind down");
                PlayGameSound(GROAN_SOUND_SET, "FEMALE_AROUSAL_0", 0.6f);

                // PLACEHOLDER: Add ambient sounds, music, or custom sequences here
                // Example: PlayGameSound("GENERIC_IDLE", "RAGGED_BREATHING", 0.7f);
            }
            catch (Exception ex)
            {
                ProstitutionMod.Utils.Helper.Log("ERROR in audio sequence: " + ex.Message);
            }
        }

        /// <summary>
        /// Play a sound from the game's audio catalog.
        /// Uses native ScriptHook call to access GTA's audio system.
        /// </summary>
        private static void PlayGameSound(string soundSet, string soundName, float volume = 1.0f)
        {
            try
            {
                // Use native function to play audio frontend sound
                // Parameters: sound set, sound name
                Function.Call(Hash.PLAY_SOUND_FRONTEND, soundSet, soundName, true, 0);
                ProstitutionMod.Utils.Helper.Log($"Playing sound: {soundSet}/{soundName} at volume {volume}");
            }
            catch (Exception ex)
            {
                ProstitutionMod.Utils.Helper.Log("ERROR playing sound: " + ex.Message);
            }
        }

        // ==================== CLEANUP ====================
        /// <summary>
        /// Restore normal game state after cutscene completes.
        /// - Release player controls
        /// - Remove custom camera
        /// - Reset character states
        /// - Clear animations
        /// </summary>
        private static void CleanupCutscene()
        {
            try
            {
                Ped player = Game.Player.Character;
                if (player != null && player.Exists())
                {
                    // Restore controls
                    player.IsInvincible = false;
                    player.Task.ClearAllImmediately();

                    ProstitutionMod.Utils.Helper.Log("Player controls restored, invincibility disabled");
                }

                // Clean up prostitute
                if (currentProstitute != null && currentProstitute.Exists())
                {
                    currentProstitute.Task.ClearAllImmediately();
                }

                // Restore camera
                if (cutsceneCamera != null)
                {
                    cutsceneCamera.IsActive = false;
                    World.RenderingCamera = null;
                    cutsceneCamera.Delete();
                    cutsceneCamera = null;

                    ProstitutionMod.Utils.Helper.Log("Camera removed, world rendering restored");
                }

                ProstitutionMod.Utils.Helper.Log("Cutscene cleanup complete");
            }
            catch (Exception ex)
            {
                ProstitutionMod.Utils.Helper.Log("ERROR during cleanup: " + ex.Message);
            }
        }

        // ==================== UTILITY METHODS ====================
        /// <summary>
        /// Check if a cutscene is currently active.
        /// Useful for preventing overlapping cutscenes or input interference.
        /// </summary>
        public static bool IsCutsceneActive()
        {
            return cutsceneActive;
        }

        /// <summary>
        /// Forcefully stop the current cutscene.
        /// Used for emergency exits or player interruption.
        /// PLACEHOLDER: Add fade-out or transition effects.
        /// </summary>
        public static void StopCutscene()
        {
            if (cutsceneActive)
            {
                ProstitutionMod.Utils.Helper.Log("Stopping cutscene (emergency)");
                CleanupCutscene();
                cutsceneActive = false;
            }
        }

        /// <summary>
        /// Move this into the main Script if you want animated transitions.
        /// Called continuously to handle per-frame cutscene logic.
        /// PLACEHOLDER: Add condition-based branches (e.g., skip if B is pressed).
        /// </summary>
        public static void OnTick()
        {
            if (!cutsceneActive) return;

            // Disable inputs during cutscene to prevent player interference
            Game.DisableControlThisFrame(0, Control.Sprint);
            Game.DisableControlThisFrame(0, Control.MoveUpDown);
            Game.DisableControlThisFrame(0, Control.MoveLeftRight);

            // PLACEHOLDER: Allow ESC/B to skip cutscene early
            // if (Game.IsControlJustPressed(Control.FrontendCancel))
            // {
            //     StopCutscene();
            // }
        }
    }
}
