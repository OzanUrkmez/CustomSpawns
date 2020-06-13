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
            if (dynamicSpawnData.ContainsKey(mb))
            {
                ErrorHandler.ShowPureErrorMessage("A mobile party key that already was dynamically allocated was tried to be added! Report to developer and just keep playing, no issues will occur... probably :)");
                return;
            }
            dynamicSpawnData.Add(mb, data);
        }

        public static bool RemoveDynamicSpawnData(MobileParty mb)
        {
            return dynamicSpawnData.Remove(mb);
        }

        public static CSPartyData GetDynamicSpawnData(MobileParty mb)
        {
            if (!dynamicSpawnData.ContainsKey(mb))
                return null;
            return dynamicSpawnData[mb];
        }

    }

    public class CSPartyData
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
