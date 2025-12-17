using System;
using System.Collections.Generic;
using System.Linq;
using GTA;
using GTA.Native;
using ProstitutionMod.Utils;
using ProstitutionMod.Cutscenes;

namespace ProstitutionMod.NPC
{
    public static class NPCHandler
    {
        private static List<Ped> nearbyProstitutes = new List<Ped>();

        public static void CheckNearbyProstitutes()
        {
            try
            {
                if (Game.Player.Character == null || !Game.Player.Character.Exists())
                    return;

                nearbyProstitutes.Clear();
                var peds = World.GetNearbyPeds(Game.Player.Character, 12.0f);
                
                foreach (var ped in peds)
                {
                    if (ped != null && ped.Exists() && IsProstitute(ped))
                    {
                        nearbyProstitutes.Add(ped);
                    }
                }

                if (nearbyProstitutes.Count > 0)
                    Helper.Log($"Found {nearbyProstitutes.Count} nearby prostitute(s)");
            }
            catch (Exception ex)
            {
                Helper.Log("Error in CheckNearbyProstitutes: " + ex.Message);
            }
        }

        private static bool IsProstitute(Ped ped)
        {
            try
            {
                // Check model name for prostitute indicator
                string modelName = ped.Model.Name?.ToLower() ?? "";
                if (modelName.Contains("prostitute")) return true;
                
                // Check for female pedestrian types via relationship group
                if (ped.IsFemale && ped.IsHuman && !ped.IsPlayer)
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        // Backwards-compatible single-call handler
        public static void HandleInteraction()
        {
            AcceptInteraction();
        }

        // Called when LT + DPad Right is pressed
        public static void AcceptInteraction()
        {
            try
            {
                var cfg = Config.Load();
                if (nearbyProstitutes.Count == 0)
                {
                    Helper.Log("No prostitute nearby to accept with.");
                    return;
                }

                Ped target = nearbyProstitutes[0];
                if (cfg.EnableProstituteAccept)
                {
                    AcceptProposal(target);
                }
                else
                {
                    Helper.Log("Prostitute accept disabled in config.json");
                }
            }
            catch (Exception ex)
            {
                Helper.Log("Error in AcceptInteraction: " + ex.Message);
            }
        }

        // Called when B is pressed
        public static void DeclineInteraction()
        {
            try
            {
                if (nearbyProstitutes.Count == 0)
                {
                    Helper.Log("No prostitute nearby to decline with.");
                    return;
                }

                Ped target = nearbyProstitutes[0];
                Helper.Log("Declining interaction with ped: " + target.Handle);
                // Play decline animation
                target.Task.PlayAnimation("amb@prop_human_seat_female@backhand_idle@base", "base", 8.0f, -1, AnimationFlags.Loop);
            }
            catch (Exception ex)
            {
                Helper.Log("Error in DeclineInteraction: " + ex.Message);
            }
        }

        private static void AcceptProposal(Ped ped)
        {
            Helper.Log("Accepting proposal with ped: " + ped.Handle);
            // Trigger cutscene system
            AcceptScene.TriggerAcceptCutscene(ped);
        }
    }
}
