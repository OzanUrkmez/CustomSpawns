using HarmonyLib;
using TaleWorlds.CampaignSystem.BarterSystem.Barterables;
using TaleWorlds.CampaignSystem.Party;

namespace CustomSpawns.HarmonyPatches
{
    [HarmonyPatch(typeof(SafePassageBarterable), "GetUnitValueForFaction")]
    public class GetUnitValueForFactionPatch
    {

        //TODO make this alterable.
        static void Postfix(ref int __result)
        {
            MobileParty other = MobileParty.ConversationParty;
            if (other == null || !Utils.Utils.IsCustomSpawnsStringID(other.StringId))
            {
                return;
            }

            __result /= 8;
        }

    }
}
