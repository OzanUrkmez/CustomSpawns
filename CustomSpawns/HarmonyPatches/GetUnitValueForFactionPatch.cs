using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Barterables;

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
