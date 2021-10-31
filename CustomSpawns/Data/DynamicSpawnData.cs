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
    public class DynamicSpawnData
    {
        private static DynamicSpawnData _instance;

        public static DynamicSpawnData Instance
        {
            get
            {
                return _instance;
            }
            private set
            {
                _instance = value;
                
            }
        }

        public static void ClearInstance(Main caller)
        {
            if (caller == null)
                return;
            _dynamicSpawnData.Clear();
            _instance = null;
        }

        public static void Init()
        {
            if (_instance != null)
            {
                throw new Exception("DynamicSpawnData has already been initialised!");
            }
            _instance = new DynamicSpawnData();
        }

        private DynamicSpawnData()
        {
            foreach (MobileParty mb in MobileParty.All)
            {
                if (mb == null)
                    return;
                foreach (var dat in SpawnDataManager.Instance.Data)
                {
                    if (CampaignUtils.IsolateMobilePartyStringID(mb) == dat.PartyTemplate.StringId) //TODO could deal with sub parties in the future as well!
                    {
                        //this be a custom spawns party :O
                        AddDynamicSpawnData(mb, new CSPartyData(dat, null));
                        dat.IncrementNumberSpawned();
                        UpdateDynamicData(mb);
                        UpdateRedundantDynamicData(mb);
                    }
                }
            }
        }

        private static Dictionary<MobileParty, CSPartyData> _dynamicSpawnData = new Dictionary<MobileParty, CSPartyData>();

        public void FlushSpawnData()
        {
            _dynamicSpawnData.Clear();
        }

        public void AddDynamicSpawnData(MobileParty mb, CSPartyData data)
        {
            if (_dynamicSpawnData.ContainsKey(mb))
            {
                return;
            }
            _dynamicSpawnData.Add(mb, data);
        }

        public bool RemoveDynamicSpawnData(MobileParty mb)
        {
            return _dynamicSpawnData.Remove(mb);
        }

        public CSPartyData GetDynamicSpawnData(MobileParty mb)
        {
            if (!_dynamicSpawnData.ContainsKey(mb))
                return null;
            return _dynamicSpawnData[mb];
        }

        public void UpdateDynamicData(MobileParty mb)
        {

        }

        public void UpdateRedundantDynamicData(MobileParty mb)
        {
            GetDynamicSpawnData(mb).latestClosestSettlement = CampaignUtils.GetClosestHabitedSettlement(mb);
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
