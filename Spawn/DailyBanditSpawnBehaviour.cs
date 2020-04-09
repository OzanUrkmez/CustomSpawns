using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.Localization;

namespace CustomSpawns.Spawn
{
    class DailyBanditSpawnBehaviour : CampaignBehaviorBase
    {

        Data.RegularBanditDailySpawnDataManager dataManager;

        public DailyBanditSpawnBehaviour(Data.RegularBanditDailySpawnDataManager data_manager)
        {
            dataManager = data_manager;
        }

        public void GetCurrentData()
        {
            foreach(Data.RegularBanditDailySpawnData dat in dataManager.Data)
            {
                dat.SetNumberSpawned(0);
            }
            foreach(MobileParty mb in MobileParty.All)
            {
                foreach (var dat in dataManager.Data) {
                    if (mb.StringId.Split('_')[0] == dat.PartyTemplate.StringId)
                    {
                        //increase count
                        dat.IncrementNumberSpawned();
                    }
                }
            }
            if (ConfigLoader.Instance.Config.IsDebugMode)
            {
                //display necessary debug message.
                foreach(var dat in dataManager.Data)
                {
                    ModDebug.ShowMessage(dat.Name + " count: " + dat.GetNumberSpawned());
                }
            }
        }

        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, DailyBehaviour);
            CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, HourlyBehaviour);
        }

        public override void SyncData(IDataStore dataStore)
        {

        }

        private bool spawnedToday = false;

        public void HourlyBehaviour()
        {
            GetCurrentData();
            if (!spawnedToday && Campaign.Current.IsNight)
            {
                RegularBanditSpawn();
                spawnedToday = true;
            }

        }

        private void DailyBehaviour()
        {
            spawnedToday = false;
        }

        private void RegularBanditSpawn()
        {
            try
            {
                var list = dataManager.Data;
                Random rand = new Random();
                foreach (Data.RegularBanditDailySpawnData data in list)
                {
                    if (data.CanSpawn())
                    {
                        if ((float)rand.NextDouble() < data.ChanceOfSpawn)
                        {
                            //spawn!
                            Spawner.SpawnBanditAtHideout(CampaignUtils.GetPreferableHideout(data.OverridenSpawnClan??data.BanditClan), data.BanditClan, data.PartyTemplate, new TextObject(data.Name));
                            data.IncrementNumberSpawned();
                        }
                    }
                }
            }
            catch(Exception e)
            {
                ErrorHandler.HandleException(e);
            }
        }
    }
}
