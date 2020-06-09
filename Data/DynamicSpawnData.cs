using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.Localization;


namespace CustomSpawns.Data
{
    static class DynamicSpawnData
    {

        private static Dictionary<MobileParty, CSPartyData> dynamicSpawnData = new Dictionary<MobileParty, CSPartyData>();

        public static void FlushSpawnData()
        {
            dynamicSpawnData.Clear();
        }

        public static void AddDynamicSpawnData(MobileParty mb, CSPartyData data)
        {
            dynamicSpawnData.Add(mb, data);
        }

        public static void RemoveDynamicSpawnData(MobileParty mb)
        {
            dynamicSpawnData.Remove(mb);
        }

    }

    public struct CSPartyData
    {
        public SpawnData spawnBaseData;
        public Settlement latestClosestSettlement;

        public CSPartyData(SpawnData spawnData, Settlement latestClosestSettlement)
        {
            this.spawnBaseData = spawnData;
            this.latestClosestSettlement = latestClosestSettlement;
        }
    }
}
