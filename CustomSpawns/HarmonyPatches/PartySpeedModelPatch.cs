using CustomSpawns.Utils;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Localization;

namespace CustomSpawns.HarmonyPatches
{
    [HarmonyPatch(typeof(DefaultPartySpeedCalculatingModel), "CalculateFinalSpeed")]
    public class PartySpeedModelPatch
    {
        private static readonly TextObject ExtraSpeedExplanationText = new("Custom Spawns Extra Speed Modification");

        static bool Prefix(MobileParty mobileParty, ref ExplainedNumber finalSpeed)
        {
            if (!Main.PartySpeedContext.IsPartySpeedBonusAllowedByUser())
            {
                return true;
            }

            string partyId = CampaignUtils.IsolateMobilePartyStringID(mobileParty); //TODO if this is non-trivial make it more efficient
            if (Main.PartySpeedContext.IsPartyEligibleForExtraSpeed(partyId))
            {
                float extraSpeed = Main.PartySpeedContext.GetSpeedWithExtraBonus(partyId);
                finalSpeed.Add(extraSpeed, ExtraSpeedExplanationText);
            }
            else if (Main.PartySpeedContext.IsBasePartySpeedOverriden(partyId))
            {
                float overridenBaseSpeed = Main.PartySpeedContext.GetBaseSpeed(partyId);
                finalSpeed = new ExplainedNumber(overridenBaseSpeed);
                return true;
            }

            return true;
        }

        static void Postfix(MobileParty mobileParty, ref ExplainedNumber __result)
        {
            if (!Main.PartySpeedContext.IsPartySpeedBonusAllowedByUser())
            {
                return;
            }

            string partyId = CampaignUtils.IsolateMobilePartyStringID(mobileParty); //TODO if this is non-trivial make it more efficient

            if (Main.PartySpeedContext.IsPartyMinimumSpeedOverriden(partyId)) //minimum adjustment
                __result.LimitMin(Main.PartySpeedContext.GetMinimumSpeed(partyId));

            if (Main.PartySpeedContext.IsPartyMaximumSpeedOverriden(partyId))//maximum adjustment
                __result.LimitMax(Main.PartySpeedContext.GetMaximumSpeed(partyId));
        }
    }
}