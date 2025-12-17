using System;
using System.Collections.Generic;
using GTA;
using GTA.Math;
using GTA.Native;

namespace ProstitutionMod.Cutscenes
{
    /// <summary>
    /// Cutscene system for prostitute accept interactions.
    /// Operates asynchronously with per-frame state management - NO blocking Script.Wait() calls.
    /// </summary>
    public static class AcceptScene
    {
        // Cutscene state
        private static bool cutsceneActive = false;
        private static Ped currentProstitute = null;
        private static Ped playerCharacter = null;
        private static Camera cutsceneCamera = null;
        private static int cutscenePhase = 0;
        private static int phaseTimer = 0;

        // Phase state flags
        private static bool movementInitialized = false;
        private static Vector3 targetPos = Vector3.Zero;
        private static bool cameraInitialized = false;
        private static int audioPhase = 0;

        private const string GROAN_SOUND_SET = "PLAYER_MOANS_FEMALE";

        /// <summary>
        /// Initialize the AcceptScene system.
        /// </summary>
        public static void Initialize()
        {
            ProstitutionMod.Utils.Helper.Log("AcceptScene system initialized");
        }

        /// <summary>
        /// Trigger the cutscene when player accepts prostitute offer.
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

            playerCharacter = Game.Player.Character;
            if (playerCharacter == null || !playerCharacter.Exists())
            {
                ProstitutionMod.Utils.Helper.Log("ERROR: Player character not found");
                return;
            }

            ProstitutionMod.Utils.Helper.Log("Starting accept cutscene with prostitute handle: " + prostitute.Handle);
            currentProstitute = prostitute;
            cutsceneActive = true;
            cutscenePhase = 0;
            phaseTimer = 0;
            ResetPhaseStates();
        }

        /// <summary>
        /// Per-frame cutscene orchestrator. Called from main.cs OnTick().
        /// No blocking calls - all timing handled asynchronously.
        /// </summary>
        public static void OnTick()
        {
            if (!cutsceneActive) return;

            // Validate characters
            if (playerCharacter == null || !playerCharacter.Exists() || currentProstitute == null || !currentProstitute.Exists())
            {
                ProstitutionMod.Utils.Helper.Log("ERROR: Character validation failed");
                CleanupCutscene();
                return;
            }

            try
            {
                phaseTimer++;

                switch (cutscenePhase)
                {
                    case 0: // Setup (~5 frames)
                        SetupCutsceneFrame();
                        if (phaseTimer >= 5) { cutscenePhase++; phaseTimer = 0; }
                        break;

                    case 1: // Movement (~30 frames)
                        MoveCharactersFrame();
                        if (phaseTimer >= 30) { cutscenePhase++; phaseTimer = 0; }
                        break;

                    case 2: // Animations (~20 frames)
                        InteractionFrame();
                        if (phaseTimer >= 20) { cutscenePhase++; phaseTimer = 0; }
                        break;

                    case 3: // Camera and Audio (~150 frames)
                        CameraAndAudioFrame();
                        if (phaseTimer >= 150) { cutscenePhase++; phaseTimer = 0; }
                        break;

                    case 4: // Cleanup
                        CleanupCutscene();
                        break;
                }

                // Disable player controls during cutscene
                Game.DisableControlThisFrame(0, Control.Sprint);
                Game.DisableControlThisFrame(0, Control.MoveUpDown);
                Game.DisableControlThisFrame(0, Control.MoveLeftRight);
                Game.DisableControlThisFrame(0, Control.LookUpDown);
                Game.DisableControlThisFrame(0, Control.LookLeftRight);
            }
            catch (Exception ex)
            {
                ProstitutionMod.Utils.Helper.Log("ERROR in cutscene OnTick: " + ex.Message);
                CleanupCutscene();
            }
        }

        /// <summary>
        /// Phase 1: Setup cutscene environment.
        /// </summary>
        private static void SetupCutsceneFrame()
        {
            if (phaseTimer == 1)
            {
                ProstitutionMod.Utils.Helper.Log("[Cutscene] Phase 1: Setup");
                playerCharacter.IsInvincible = true;
                playerCharacter.Task.ClearAllImmediately();
                currentProstitute.Task.ClearAllImmediately();
            }
        }

        /// <summary>
        /// Phase 2: Move characters to interaction position.
        /// </summary>
        private static void MoveCharactersFrame()
        {
            if (phaseTimer == 1)
            {
                ProstitutionMod.Utils.Helper.Log("[Cutscene] Phase 2: Movement");
                movementInitialized = false;
            }

            if (!movementInitialized)
            {
                Vector3 playerPos = playerCharacter.Position;
                float heading = playerCharacter.Heading;
                Vector3 offset = new Vector3((float)Math.Cos(heading * Math.PI / 180.0), (float)Math.Sin(heading * Math.PI / 180.0), 0);
                targetPos = playerPos + offset * 1.5f;
                currentProstitute.Task.GoStraightToCoord(targetPos, 1.0f, 5000, 0.5f, 0);
                movementInitialized = true;
            }

            if (Vector3.Distance(currentProstitute.Position, targetPos) <= 1.5f)
            {
                movementInitialized = false;
            }
        }

        /// <summary>
        /// Phase 3: Play interaction animations.
        /// </summary>
        private static void InteractionFrame()
        {
            if (phaseTimer == 1)
            {
                ProstitutionMod.Utils.Helper.Log("[Cutscene] Phase 3: Interaction");
                currentProstitute.Face(playerCharacter);
                playerCharacter.Face(currentProstitute);
                currentProstitute.Task.PlayAnimation("misscarsteal4@actor", "actor_react_succeed", 8.0f, 3000, AnimationFlags.None);
                playerCharacter.Task.PlayAnimation("combat@damage@rb_writhe", "rb_writhe_loop", 8.0f, 3000, AnimationFlags.None);
            }
        }

        /// <summary>
        /// Phase 4: Camera control and audio playback.
        /// </summary>
        private static void CameraAndAudioFrame()
        {
            if (phaseTimer == 1)
            {
                ProstitutionMod.Utils.Helper.Log("[Cutscene] Phase 4: Camera & Audio");
                cameraInitialized = false;
                audioPhase = 0;
            }

            // Initialize camera once
            if (!cameraInitialized)
            {
                try
                {
                    Vector3 midpoint = Vector3.Add(playerCharacter.Position, currentProstitute.Position) / 2.0f;
                    Vector3 cameraPos = midpoint + new Vector3(0, 0, 2.0f);
                    cutsceneCamera = World.CreateCamera(cameraPos, new Vector3(0, 0, 0), 50.0f);
                    cutsceneCamera.PointAt(midpoint);
                    World.RenderingCamera = cutsceneCamera;
                    cameraInitialized = true;
                }
                catch (Exception ex)
                {
                    ProstitutionMod.Utils.Helper.Log("ERROR setting up camera: " + ex.Message);
                }
            }

            // Audio triggers at specific frame intervals
            if (phaseTimer == 20 && audioPhase == 0)
            {
                PlayGameSound(GROAN_SOUND_SET, "FEMALE_AROUSAL_0");
                audioPhase++;
            }
            else if (phaseTimer == 50 && audioPhase == 1)
            {
                PlayGameSound(GROAN_SOUND_SET, "FEMALE_AROUSAL_1");
                audioPhase++;
            }
            else if (phaseTimer == 80 && audioPhase == 2)
            {
                PlayGameSound(GROAN_SOUND_SET, "FEMALE_AROUSAL_2");
                audioPhase++;
            }
            else if (phaseTimer == 120 && audioPhase == 3)
            {
                PlayGameSound(GROAN_SOUND_SET, "FEMALE_AROUSAL_0");
                audioPhase++;
            }
        }

        /// <summary>
        /// Play a game audio sound from in-game catalog.
        /// </summary>
        private static void PlayGameSound(string soundSet, string soundName)
        {
            try
            {
                Function.Call(Hash.PLAY_SOUND_FRONTEND, soundSet, soundName, true, 0);
                ProstitutionMod.Utils.Helper.Log($"Playing sound: {soundSet}/{soundName}");
            }
            catch (Exception ex)
            {
                ProstitutionMod.Utils.Helper.Log("ERROR playing sound: " + ex.Message);
            }
        }

        /// <summary>
        /// Cleanup after cutscene completes.
        /// </summary>
        private static void CleanupCutscene()
        {
            try
            {
                if (playerCharacter != null && playerCharacter.Exists())
                {
                    playerCharacter.IsInvincible = false;
                    playerCharacter.Task.ClearAllImmediately();
                }

                if (currentProstitute != null && currentProstitute.Exists())
                {
                    currentProstitute.Task.ClearAllImmediately();
                }

                if (cutsceneCamera != null)
                {
                    cutsceneCamera.IsActive = false;
                    World.RenderingCamera = null;
                    cutsceneCamera.Delete();
                    cutsceneCamera = null;
                }

                ProstitutionMod.Utils.Helper.Log("Cutscene cleanup complete");
            }
            catch (Exception ex)
            {
                ProstitutionMod.Utils.Helper.Log("ERROR during cleanup: " + ex.Message);
            }
            finally
            {
                cutsceneActive = false;
                currentProstitute = null;
                playerCharacter = null;
                ResetPhaseStates();
            }
        }

        /// <summary>
        /// Reset all phase-specific state variables.
        /// </summary>
        private static void ResetPhaseStates()
        {
            cutscenePhase = 0;
            phaseTimer = 0;
            movementInitialized = false;
            targetPos = Vector3.Zero;
            cameraInitialized = false;
            audioPhase = 0;
        }

        /// <summary>
        /// Check if cutscene is currently active.
        /// </summary>
        public static bool IsCutsceneActive()
        {
            return cutsceneActive;
        }

        /// <summary>
        /// Emergency stop for cutscene.
        /// </summary>
        public static void StopCutscene()
        {
            if (cutsceneActive)
            {
                ProstitutionMod.Utils.Helper.Log("Stopping cutscene (emergency)");
                CleanupCutscene();
            }
        }
    }
}
