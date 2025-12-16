using System;

namespace ProstitutionMod.NPC
{
    // Placeholder for future NPC expansion. Implement detection and interaction
    // logic here for generic NPC types listed in config.json.
    public static class GenericNPCHandler
    {
        public static void Initialize()
        {
            // Future initialization logic
        }

        public static void HandleGenericAccept(object ped)
        {
            // Placeholder: will be implemented in future updates
            ProstitutionMod.Utils.Helper.Log("GenericNPCHandler.HandleGenericAccept called for: " + GameBridge.GetPedDebugName(ped));
        }
    }
}
