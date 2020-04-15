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
            Dictionary<Data.RegularBanditDailySpawnData, int> oldValues = new Dictionary<Data.RegularBanditDailySpawnData, int>();
            foreach(Data.RegularBanditDailySpawnData dat in dataManager.Data)
            {
                oldValues.Add(dat, dat.GetNumberSpawned());
                dat.SetNumberSpawned(0);
            }

            foreach(MobileParty mb in MobileParty.All)
            {
                foreach (var dat in dataManager.Data) {
                    var split = mb.StringId.Split('_');
                    string compared = string.Join("_", Utils.Utils.TakeAllButLast<string>(split));
                    if (compared == dat.PartyTemplate.StringId)
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
                foreach (Data.RegularBanditDailySpawnData data in list)
                {
                    int j = 0;
                    for (int i = 0; i < data.RepeatSpawnRolls; i++)
                    {
                        if (data.CanSpawn() && (data.MinimumNumberOfDaysUntilSpawn < (int)Math.Ceiling(Campaign.Current.CampaignStartTime.ElapsedDaysUntilNow)))
                        {
                            if (ConfigLoader.Instance.Config.IsAllSpawnMode || (float)rand.NextDouble() < data.ChanceOfSpawn)
                            {
                                Clan spawnClan = data.BanditClan;
                                //deal with override of spawn clan.
                                if(data.OverridenSpawnClan.Count != 0)
                                {
                                    spawnClan = data.OverridenSpawnClan[rand.Next(0, data.OverridenSpawnClan.Count)];
                                }
                                //check for one hideout
                                Settlement firstHideout = null;
                                if (ConfigLoader.Instance.Config.SpawnAtOneHideout)
                                {
                                    foreach (Settlement s in Settlement.All)
                                    {
                                        if (s.IsHideout())
                                        {
                                            firstHideout = s;
                                            break;
                                        }
                                    }
                                }
                                //deal with town spawn
                                Settlement spawnOverride = null;
                                if(data.OverridenSpawnCultures.Count != 0)
                                {
                                    //spawn at overriden spawn instead!
                                    spawnOverride = CampaignUtils.PickRandomSettlementOfCulture(data.OverridenSpawnCultures);
                                }
                                //get settlement
                                Settlement spawnSettlement = ConfigLoader.Instance.Config.SpawnAtOneHideout ? firstHideout : (spawnOverride == null ? CampaignUtils.GetPreferableHideout(spawnClan) : spawnOverride);
                                //spawn nao!
                                MobileParty spawnedParty = Spawner.SpawnBanditAtHideout(spawnSettlement, data.BanditClan, data.PartyTemplate, new TextObject(data.Name));
                                data.IncrementNumberSpawned(); //increment for can spawn and chance modifications
                                j++;
                                //AI Checks!
                                HandleAIChecks(spawnedParty, data, spawnSettlement);
                                //accompanying spawns
                                foreach(var accomp in data.SpawnAlongWith)
                                {
                                    MobileParty juniorParty = Spawner.SpawnBanditAtHideout(spawnSettlement, data.BanditClan, accomp.templateObject, new TextObject(accomp.name));
                                    HandleAIChecks(juniorParty, data, spawnSettlement); //junior party has same AI behaviour as main party.
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

        private void HandleAIChecks(MobileParty mb, Data.RegularBanditDailySpawnData data, Settlement spawnedSettlement)
        {
            if (data.PatrolAroundSpawn)
                AI.AIManager.HourlyPatrolAroundSpawn.RegisterMobilePartyToPatrol(mb, spawnedSettlement);
        }
    }
}
