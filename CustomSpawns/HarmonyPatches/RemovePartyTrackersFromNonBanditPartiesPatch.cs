using System.Linq;
using CustomSpawns.Data;
using CustomSpawns.Utils;
using HarmonyLib;
using SandBox.ViewModelCollection.Map;
using TaleWorlds.CampaignSystem.Party;

namespace CustomSpawns.HarmonyPatches
{
    [HarmonyPatch(typeof(MapMobilePartyTrackerVM), "CanAddParty")]
    public class RemovePartyTrackersFromNonBanditPartiesPatch
    {
        static void Postfix(MobileParty party, ref bool __result)
        {
            if (__result)
            {
                var isolatedPartyStringId = CampaignUtils.IsolateMobilePartyStringID(party);
                if (SpawnDataManager.Instance.Data.Any(spawnData =>
                    isolatedPartyStringId.Equals(spawnData.PartyTemplate.GetName().ToString())))
                    __result = false;
            }
        }
    }
}