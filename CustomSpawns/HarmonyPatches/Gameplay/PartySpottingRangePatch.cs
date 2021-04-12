using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using HarmonyLib;
using TaleWorlds.CampaignSystem.Barterables;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Engine.InputSystem;
using TaleWorlds.InputSystem;
using SandBox.View.Map;
using System.Reflection;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;

namespace CustomSpawns.HarmonyPatches.Gameplay
{
    [HarmonyPatch(typeof(DefaultMapVisibilityModel), "GetPartySpottingRange")]
    static class PartySpottingRangePatch
    {

        public static int AdditionalSpottingRange { get; set; } = 0;

        static void Postfix(ref ExplainedNumber __result)
        {
            if(AdditionalSpottingRange == 0)
            {
                return;
            }
            __result.AddFactor(AdditionalSpottingRange, new TaleWorlds.Localization.TextObject("CustomSpawns HAX"));
        }

    }
}
