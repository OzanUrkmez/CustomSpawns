using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Engine;

namespace CustomSpawns.HarmonyPatches
{

    [HarmonyPatch(typeof(Scene), "GetNavMeshCenterPosition")]
    class GetNavigationMeshCenterPositionCrashOnInvalidFaceFix
    {

        static Exception Finalizer(Exception exception)
        {
            if(exception != null)
            {
                ErrorHandler.ShowPureErrorMessage("CustomSpawns has detected a possible mod conflict or an in-game bug!" +
                    " A position that is outside of the bounds of the current map was tried to be accessed. This occurs often when the game" +
                    " is trying to spawn something at an inacessible coordinate. KEEP IN MIND THAT YOUR GAME MIGHT BECOME UNSTABLE!" +
                    " CustomSpawns has suppressed the crash but the game was meant to crash.");
            }

            return null;
        }

    }
}
