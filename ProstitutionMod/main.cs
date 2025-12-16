using System;
using System.Windows.Forms;
using ProstitutionMod.NPC;
using ProstitutionMod.Utils;

// NOTE: ScriptHook base class namespace can vary by runtime. If your ScriptHook uses
// a different base class, replace `ScriptHook::Script` with the appropriate one (e.g., Script).
public class Main : ScriptHook::Script
{
    private bool lDown = false;

    public Main()
    {
        Tick += OnTick;
        KeyDown += OnKeyDown;
        KeyUp += OnKeyUp;
        Helper.Log("ProstitutionMod initializing...");
    }

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.L) lDown = true;
        if (e.KeyCode == Keys.T && lDown)
        {
            Helper.Log("L+T pressed — invoking NPC interaction");
            NPCHandler.HandleInteraction();
        }
    }

    private void OnKeyUp(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.L) lDown = false;
    }

    private void OnTick(object sender, EventArgs e)
    {
        // Periodic background checks (non-blocking, fast)
        NPCHandler.CheckNearbyProstitutes();
    }
}
