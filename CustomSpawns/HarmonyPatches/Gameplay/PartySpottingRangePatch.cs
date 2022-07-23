using TaleWorlds.CampaignSystem;
using HarmonyLib;
using TaleWorlds.CampaignSystem.GameComponents;

namespace CustomSpawns.HarmonyPatches.Gameplay
{
    [HarmonyPatch(typeof(DefaultMapVisibilityModel), "GetPartySpottingRange")]
    static class PartySpottingRangePatch
    {

        public static int AdditionalSpottingRange { get; set; }

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
