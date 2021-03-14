using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace CustomSpawns.HarmonyPatches
{

    [HarmonyPatch(typeof(Scene), "GetNavMeshCenterPosition")]
    public class GetNavigationMeshCenterPositionCrashOnInvalidFaceFix
    {

        static bool Prefix(int faceIndex, ref Vec3 centerPosition)
        {

            if (faceIndex == int.MaxValue || faceIndex < 0) //this seems to be input when an invalid face is selected.
            {
                ErrorHandler.ShowPureErrorMessage("CustomSpawns has detected a possible mod conflict or an in-game bug!" +
                    " A face with an index that is not on the current navigation mesh of the map was tried to be accessed. " +
                    " This occurs often when the game" +
                    " is trying to deal with an invalid coordinate, which might be caused by a mod altering the map or the map" +
                    " being changed without a mod updating to accompany this." +
                    " KEEP IN MIND THAT YOUR GAME MIGHT BECOME UNSTABLE!" +
                    " CustomSpawns has suppressed the crash.");

                centerPosition = new Vec3();

                return false;
            }

            return true;
        }

    }

}
