using System;
using GTA;
using GTA.Math;
using GTA.Native;
using ProstitutionMod.NPC;
using ProstitutionMod.Utils;
using ProstitutionMod.Cutscenes;

public class Main : Script
{
    public Main()
    {
        Tick += OnTick;
        Helper.Log("ProstitutionMod initializing...");
    }

    private void OnTick(object sender, EventArgs e)
    {
        try
        {
            // Cutscene system per-frame updates
            AcceptScene.OnTick();

            // Check for LT + D-Pad Right (Accept)
            bool ltHeld = Game.IsControlPressed(Control.TakeScreenshot) || Function.Call<bool>(Hash.GET_CONTROL_NORMAL, 2, 174);
            bool dpadRight = Game.IsControlJustPressed(Control.VehicleSelectNextWeapon) || Function.Call<bool>(Hash.GET_CONTROL_NORMAL, 2, 175);
            
            if (ltHeld && dpadRight)
            {
                Helper.Log("LT + DPad Right detected — Accept");
                NPCHandler.AcceptInteraction();
            }

            // Check for B button (Decline) - FrontendCancel is B/Circle
            if (Game.IsControlJustPressed(Control.FrontendCancel))
            {
                Helper.Log("B button detected — Decline");
                NPCHandler.DeclineInteraction();
            }

            // Periodic background checks (non-blocking, fast)
            NPCHandler.CheckNearbyProstitutes();
        }
        catch (Exception ex)
        {
            Helper.Log("Error in OnTick: " + ex.Message);
        }
    }
}
