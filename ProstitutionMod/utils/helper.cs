using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Collections.Generic;

namespace ProstitutionMod.Utils
{
    public class ConfigData
    {
        public bool EnableProstituteAccept { get; set; } = true;
        public bool EnableGenericNPCAccept { get; set; } = false;
        public List<string> GenericNPCList { get; set; } = new List<string>();
    }

    public static class Config
    {
        private static ConfigData cached = null;

        public static ConfigData Load()
        {
            if (cached != null) return cached;
            try
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory ?? Directory.GetCurrentDirectory();
                // Try a few likely locations relative to the runtime directory
                string[] tries = new string[] {
                    Path.Combine(baseDir, "ProstitutionMod", "config.json"),
                    Path.Combine(baseDir, "scripts", "ProstitutionMod", "config.json"),
                    Path.Combine(baseDir, "config.json")
                };

                foreach (var p in tries)
                {
                    if (File.Exists(p))
                    {
                        var txt = File.ReadAllText(p);
                        cached = JsonSerializer.Deserialize<ConfigData>(txt, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                        Helper.Log("Loaded config from: " + p);
                        return cached;
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.Log("Failed to load config.json: " + ex.Message);
            }

            // Fallback defaults
            cached = new ConfigData() { EnableProstituteAccept = true, EnableGenericNPCAccept = false, GenericNPCList = new List<string> { "femalePedestrianType1", "femalePedestrianType2" } };
            Helper.Log("Using default config values.");
            return cached;
        }
    }

    public static class Helper
    {
        private static readonly string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory ?? Directory.GetCurrentDirectory(), "ProstitutionMod.log");

        public static void Log(string message)
        {
            try
            {
                string line = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " - " + message;
                File.AppendAllText(logPath, line + Environment.NewLine);
                Console.WriteLine(line);
                // Try to use in-game notification if available (ScriptHook-specific)
                InGameNotify(message);
            }
            catch { }
        }

        public static void WaitMs(int ms)
        {
            try { Thread.Sleep(ms); } catch { }
        }

        public static void TriggerAnimation(string animName)
        {
            Log("TriggerAnimation: " + animName);
            // Runtime-specific: animation playback should be implemented here.
            // This is a placeholder that logs and can be detected by other systems.
        }

        public static void TriggerCutscene(string sceneName)
        {
            Log("TriggerCutscene: " + sceneName);
            // Runtime-specific cutscene trigger placeholder.
        }

        public static void RegisterRampageHook(Action onStart)
        {
            // Placeholder for a 'rampage' or mission hook registration
            Log("RegisterRampageHook called (placeholder)");
        }

        public static void UnregisterRampageHook()
        {
            Log("UnregisterRampageHook called (placeholder)");
        }

        private static void InGameNotify(string message)
        {
            try
            {
                // Many ScriptHook runtimes expose a UI/Screen API. Guarded call.
                dynamic ui = Type.GetType("GTA.UI.Screen, GTA");
                if (ui != null)
                {
                    // This is intentionally loose to avoid compile/runtime errors in varying runtimes.
                }
            }
            catch { }
        }
    }
}

// Lightweight runtime bridge used by the script files above. This is a thin
// indirection so that the script can remain portable; replace the internals
// of GameBridge with direct calls to your ScriptHook runtime if necessary.
public static class GameBridge
{
    public enum PedSex { Unknown = 0, Male = 1, Female = 2 }

    public static IEnumerable<object> GetNearbyPeds(float radius)
    {
        // Placeholder: return an empty list. Implement using your runtime's API.
        return new List<object>();
    }

    public static string GetPedModelName(object ped) { return ""; }
    public static PedSex GetPedSex(object ped) { return PedSex.Unknown; }
    public static bool IsPedHuman(object ped) { return true; }
    public static void PlayEmote(object ped, string emote) { }
    public static string GetPedDebugName(object ped) { return ped?.ToString() ?? "unknown"; }
}
