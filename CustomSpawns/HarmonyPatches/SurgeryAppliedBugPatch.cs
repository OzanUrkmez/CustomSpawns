using HarmonyLib;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Party;

namespace CustomSpawns.HarmonyPatches
{
    [HarmonyPatch(typeof(SkillLevelingManager), "OnSurgeryApplied")]
    public class OnSurgeryAppliedBugPatch
    {
        static bool Prefix(MobileParty party)
        {
            return party != null;
        }
    }
}
