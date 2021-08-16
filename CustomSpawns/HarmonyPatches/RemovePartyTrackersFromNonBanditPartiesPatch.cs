using HarmonyLib;
using SandBox.ViewModelCollection.MobilePartyTracker;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment.Managers;

namespace CustomSpawns.HarmonyPatches
{
    [HarmonyPatch(typeof(MobilePartyTrackerVM), "CanAddParty")]
    public class RemovePartyTrackersFromNonBanditPartiesPatch
    {
        static void Postfix(MobileParty party, ref bool __result)
        {
            if (Utils.Utils.IsCustomSpawnsStringID(party.StringId))
                __result = false;
        }
    }
}