using System;
using System.Collections.Generic;
using ProstitutionMod.Utils;

// Note: This file uses common ScriptHook-style APIs. Adjust native calls to your
// runtime if required (RDR2 ScriptHook API variations exist).
namespace ProstitutionMod.NPC
{
    public static class NPCHandler
    {
        private static List<object> nearbyProstitutes = new List<object>();

        public static void CheckNearbyProstitutes()
        {
            try
            {
                // Placeholder: query nearby peds from the game engine
                // Replace with your runtime's World.GetNearbyPeds or equivalent.
                var peds = GameBridge.GetNearbyPeds(12.0f);
                nearbyProstitutes.Clear();
                foreach (var ped in peds)
                {
                    if (IsProstitute(ped)) nearbyProstitutes.Add(ped);
                }

                if (nearbyProstitutes.Count > 0)
                    Helper.Log($"Found {nearbyProstitutes.Count} nearby prostitute(s)");
            }
            catch (Exception ex)
            {
                Helper.Log("Error in CheckNearbyProstitutes: " + ex.Message);
            }
        }

        private static bool IsProstitute(object ped)
        {
            // Minimal placeholder detection logic. Replace with model/relgroup checks.
            try
            {
                string modelName = GameBridge.GetPedModelName(ped)?.ToLower() ?? "";
                if (modelName.Contains("prostitute")) return true;
                // Fallback heuristic: female pedestrian types
                var sex = GameBridge.GetPedSex(ped);
                return sex == GameBridge.PedSex.Female && GameBridge.IsPedHuman(ped);
            }
            catch
            {
                return false;
            }
        }

        public static void HandleInteraction()
        {
            try
            {
                var cfg = Config.Load();
                if (nearbyProstitutes.Count == 0)
                {
                    Helper.Log("No prostitute nearby to interact with.");
                    return;
                }

                var target = nearbyProstitutes[0];
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
                Helper.Log("Error in HandleInteraction: " + ex.Message);
            }
        }

        private static void AcceptProposal(object ped)
        {
            Helper.Log("Accepting proposal with ped: " + GameBridge.GetPedDebugName(ped));
            // Trigger placeholder animation and cutscene
            Helper.TriggerAnimation("acceptAnim");
            Helper.TriggerCutscene("acceptScene");

            // Minimal in-game animation call (runtime-specific placeholder)
            GameBridge.PlayEmote(ped, "accept_base");
        }
    }
}
