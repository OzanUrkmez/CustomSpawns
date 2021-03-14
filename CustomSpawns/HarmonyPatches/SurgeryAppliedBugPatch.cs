using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment.Managers;

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
