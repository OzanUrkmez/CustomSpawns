using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using HarmonyLib;
using TaleWorlds.CampaignSystem.Barterables;
using TaleWorlds.CampaignSystem.Actions;

namespace CustomSpawns.HarmonyPatches
{
    [HarmonyPatch(typeof(ChangeKingdomAction), "ApplyByJoinToKingdom")]
    public class ChangeKingdomActionApplyByJoinToKingdomPatch
    {

        static bool Prefix(Clan clan, Kingdom newKingdom, bool showNotification = true)
        {
            if(CampaignUtils.IsCustomSpawnsClan(clan))
            {
                ModDebug.ShowMessage("Prevented " + clan.StringId + "from joining kingdom " + newKingdom.StringId, DebugMessageType.Diplomacy);

                return false;
            }
            return true;
        }

    }

    [HarmonyPatch(typeof(ChangeKingdomAction), "ApplyByLeaveKingdom")]
    public class ChangeKingdomActionApplyByLeaveKingdomPatch
    {

        static bool Prefix(Clan clan, bool showNotification = true)
        {
            if (CampaignUtils.IsCustomSpawnsClan(clan))
            {
                ModDebug.ShowMessage("Prevented " + clan.StringId + "from leaving his kingdom.", DebugMessageType.Diplomacy);

                return false;
            }
            return true;
        }

    }

    [HarmonyPatch(typeof(ChangeKingdomAction), "ApplyByLeaveWithRebellionAgainstKingdom")]
    public class ChangeKingdomActionApplyByLeaveWithRebellionAgainstKingdomPatch
    {
        static bool Prefix(Clan clan, Kingdom newKingdom, bool showNotification = true)
        {
            if (CampaignUtils.IsCustomSpawnsClan(clan))
            {
                ModDebug.ShowMessage("Prevented " + clan.StringId + "from leaving his kingdom.", DebugMessageType.Diplomacy);

                return false;
            }
            return true;
        }

    }

    [HarmonyPatch(typeof(ChangeKingdomAction), "ApplyByJoinFactionAsMercenary")]
    public class ChangeKingdomActionApplyByJoinFactionAsMercenaryPatch
    {

        static bool Prefix(Clan clan, Kingdom newKingdom, int awardMultiplier = 50, bool showNotification = true)
        {
            if (CampaignUtils.IsCustomSpawnsClan(clan))
            {
                ModDebug.ShowMessage("Prevented " + clan.StringId + "from joining kingdom " + newKingdom.StringId, DebugMessageType.Diplomacy);

                return false;
            }
            return true;
        }

    }


    [HarmonyPatch(typeof(ChangeKingdomAction), "ApplyByLeaveKingdomAsMercenaryForNoPayment")]
    public class ChangeKingdomApplyByLeaveKingdomAsMercenaryForNoPaymentPatch
    {
        static bool Prefix(Clan mercenaryClan, Kingdom kingdom, bool showNotification = true)
        {
            if (CampaignUtils.IsCustomSpawnsClan(mercenaryClan))
            {
                ModDebug.ShowMessage("Prevented " + mercenaryClan.StringId + "from leaving his kingdom.", DebugMessageType.Diplomacy);

                return false;
            }
            return true;
        }

    }
}
