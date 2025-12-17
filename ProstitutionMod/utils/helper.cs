using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Collections.Generic;
using GTA;
using GTA.Native;

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
                // Try to show in-game notification
                InGameNotify(message);
            }
            catch { }
        }

        public static void WaitMs(int ms)
        {
            try { Script.Wait(ms); } catch { }
        }

        public static void TriggerAnimation(string animName)
        {
            Log("TriggerAnimation: " + animName);
            // Log the animation trigger for debugging
        }

        public static void TriggerCutscene(string sceneName)
        {
            Log("TriggerCutscene: " + sceneName);
            // Cutscene trigger logged
        }

        public static void RegisterRampageHook(Action onStart)
        {
            // Rampage hook registration
            Log("RegisterRampageHook called");
        }

        public static void UnregisterRampageHook()
        {
            Log("UnregisterRampageHook called");
        }

        private static void InGameNotify(string message)
        {
            try
            {
                if (Game.Player != null && Game.Player.Character != null)
                {
                    UI.ShowSubtitle(message, 2000);
                }
            }
            catch { }
        }
    }
}
