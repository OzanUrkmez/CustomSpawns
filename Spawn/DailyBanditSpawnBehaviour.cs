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

        Data.SpawnDataManager dataManager;

        public DailyBanditSpawnBehaviour(Data.SpawnDataManager data_manager)
        {
            dataManager = data_manager;
        }

        public void GetCurrentData()
        {
            Dictionary<Data.SpawnData, int> oldValues = new Dictionary<Data.SpawnData, int>();
            foreach(Data.SpawnData dat in dataManager.Data)
            {
                oldValues.Add(dat, dat.GetNumberSpawned());
                dat.SetNumberSpawned(0);
            }

            foreach(MobileParty mb in MobileParty.All)
            {
                foreach (var dat in dataManager.Data) {
                    if (CampaignUtils.IsolateMobilePartyStringID(mb) == dat.PartyTemplate.StringId)
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
                    if(oldValues[dat] != dat.GetNumberSpawned()) //leave a log only if a change has occured. TODO we can also detect death with these and create custom behaviour/spawn schedules accordingly ;)
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
                foreach (Data.SpawnData data in list)
                {
                    int j = 0;
                    for (int i = 0; i < data.RepeatSpawnRolls; i++)
                    {
                        if (data.CanSpawn() && (data.MinimumNumberOfDaysUntilSpawn < (int)Math.Ceiling(Campaign.Current.CampaignStartTime.ElapsedDaysUntilNow)))
                        {
                            if (ConfigLoader.Instance.Config.IsAllSpawnMode || (float)rand.NextDouble() < data.ChanceOfSpawn)
                            {
                                var spawnSettlement = Spawner.GetSpawnSettlement(data, rand);
                                //spawn nao!
                                MobileParty spawnedParty = Spawner.SpawnParty(spawnSettlement, data.SpawnClan, data.PartyTemplate, data.PartyType, new TextObject(data.Name));
                                data.IncrementNumberSpawned(); //increment for can spawn and chance modifications
                                j++;
                                //AI Checks!
                                Spawner.HandleAIChecks(spawnedParty, data, spawnSettlement);
                                //accompanying spawns
                                foreach(var accomp in data.SpawnAlongWith)
                                {
                                    MobileParty juniorParty = Spawner.SpawnParty(spawnSettlement, data.SpawnClan, accomp.templateObject, data.PartyType, new TextObject(accomp.name));
                                    Spawner.HandleAIChecks(juniorParty, data, spawnSettlement); //junior party has same AI behaviour as main party. TODO in future add some junior party AI and reconstruction.
                                }
                                //message if available
                                if(data.spawnMessage != null)
                                {
                                    UX.ShowParseSpawnMessage(data.spawnMessage, spawnSettlement.Name.ToString());
                                }
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    data.SetNumberSpawned(data.GetNumberSpawned() - j); //make sure that only the hourly checker really tells number spawned.
                }
            }
            catch(Exception e)
            {
                ErrorHandler.HandleException(e);
            }
        }
    }
}
