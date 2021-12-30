using System;
using HarmonyLib;

namespace CustomSpawns.HarmonyPatches
{
    public class PatchManager
    {
        private static bool _isApplied = false;
        
        public static void ApplyPatches()
        {
            if (_isApplied)
            {
                return;
            }

            try
            {
                Harmony harmony = new Harmony("com.Questry.CustomSpawns");
                harmony.PatchAll();
                _isApplied = true;
            }
            catch (Exception e)
            {
                ErrorHandler.HandleException(e, "HARMONY PATCHES");
            }
        }
    }
}