using System;
using System.Collections.Generic;
using System.Linq;
using CustomSpawns.CampaignData.Implementations;
using CustomSpawns.Config;
using CustomSpawns.Data;
using CustomSpawns.Economics;
using CustomSpawns.UtilityBehaviours;
using CustomSpawns.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.Localization;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace CustomSpawns.Spawn
{
    class SpawnBehaviour : CampaignBehaviorBase
    {

        private readonly Spawner _spawner;
        
        #region Data Management

        private int _lastRedundantDataUpdate = 0;

        public SpawnBehaviour(Spawner spawner)
        {
            _spawner = spawner;
            _lastRedundantDataUpdate = 0;
            OnSaveStartRunBehaviour.Singleton.RegisterFunctionToRunOnSaveStart(OnSaveStart);
        }

        private void HourlyCheckData()
        {
            if (_lastRedundantDataUpdate < ConfigLoader.Instance.Config.UpdatePartyRedundantDataPerHour + 1) // + 1 to give leeway and make sure every party gets updated. 
            {
                _lastRedundantDataUpdate++;
            }
            else
            {
                _lastRedundantDataUpdate = 0;
            }

            //Now for data checking?
        }

        #endregion


        #region MB API-Registered Behaviours

        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, DailyBehaviour);
            CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, HourlyBehaviour);
            CampaignEvents.HourlyTickPartyEvent.AddNonSerializedListener(this, HourlyPartyBehaviour);
            CampaignEvents.OnPartyRemovedEvent.AddNonSerializedListener(this, OnPartyRemoved);
        }

        public override void SyncData(IDataStore dataStore)
        {

        }

        private bool _spawnedToday = false;

        private void OnSaveStart()
        {
            //restore lost AI behaviours!
            try
            {
                var allSpawnData = SpawnDataManager.Instance.AllSpawnData();
                foreach (MobileParty mb in MobileParty.All)
                {
                    string id = CampaignUtils.IsolateMobilePartyStringID(mb);
                    if(id != "" && allSpawnData.ContainsKey(id))
                    {
                        var spawnData = allSpawnData[id];
                        HandleAIChecks(mb, spawnData, mb.HomeSettlement);
                    }

                }
            } catch(System.Exception e)
            {
                ErrorHandler.HandleException(e, " reconstruction of save custom spawns mobile party data");
            }
        }

        private void HourlyBehaviour()
        {
            HourlyCheckData();
            if (!_spawnedToday && Campaign.Current.IsNight)
            {
                RegularSpawn();
                _spawnedToday = true;
            }
        }

        //deal with our parties being removed! Also this is more efficient ;)
        private void OnPartyRemoved(PartyBase p)
        {
            MobileParty mb = p.MobileParty;
            if (mb == null)
                return;

            CSPartyData partyData = DynamicSpawnData.Instance.GetDynamicSpawnData(mb);
            if (partyData != null)
            {
                partyData.spawnBaseData.DecrementNumberSpawned();
                //this is a custom spawns party!!
                OnPartyDeath(mb, partyData);
                ModDebug.ShowMessage(mb.StringId + " has died at " + partyData.latestClosestSettlement + ", reducing the total number to: " + partyData.spawnBaseData.GetNumberSpawned(), DebugMessageType.DeathTrack);
                DynamicSpawnData.Instance.RemoveDynamicSpawnData(mb);
            }
        }

        private void HourlyPartyBehaviour(MobileParty mb)
        {
            if (DynamicSpawnData.Instance.GetDynamicSpawnData(mb) == null) //check if it is a custom spawns party
                return;
            DynamicSpawnData.Instance.UpdateDynamicData(mb);
            if (_lastRedundantDataUpdate >= ConfigLoader.Instance.Config.UpdatePartyRedundantDataPerHour)
            {
                DynamicSpawnData.Instance.UpdateRedundantDynamicData(mb);
            }
            //for now for all
            PartyEconomicUtils.PartyReplenishFood(mb);
        }

        private void DailyBehaviour()
        {
            _spawnedToday = false;
        }

        #endregion

        private void RegularSpawn()
        {
            try
            {
                var list = SpawnDataManager.Instance.Data;
                Random rand = new();
                foreach (SpawnData data in list)
                {
                    for (int i = 0; i < data.RepeatSpawnRolls; i++)
                    {
                        if (data.CanSpawn() && (data.MinimumNumberOfDaysUntilSpawn < (int)Math.Ceiling(Campaign.Current.CampaignStartTime.ElapsedDaysUntilNow)))
                        {
                            float currentChanceOfSpawn = data.ChanceOfSpawn;
                            if (!ConfigLoader.Instance.Config.IsAllSpawnMode && 
                                (float)rand.NextDouble() >= currentChanceOfSpawn * ConfigLoader.Instance.Config.SpawnChanceFlatMultiplier)
                                continue;

                            var spawnSettlement = GetSpawnSettlement(data, (s => data.MinimumDevestationToSpawn > DevestationMetricData.Singleton.GetDevestation(s)), rand);
                            //spawn nao!

                            if (spawnSettlement == null)
                            {
                                //no valid spawn settlement

                                break;
                            }

                            MobileParty spawnedParty = _spawner.SpawnParty(spawnSettlement, data.SpawnClan, data.PartyTemplate, data.BaseSpeedOverride, new TextObject(data.Name));
                            if (spawnedParty == null)
                                return;
                            data.IncrementNumberSpawned(); //increment for can spawn and chance modifications
                                                           //dynamic data registration
                            //dynamic spawn tracking
                            DynamicSpawnData.Instance.AddDynamicSpawnData(spawnedParty, new CSPartyData(data, spawnSettlement));
                            //AI Checks!
                            HandleAIChecks(spawnedParty, data, spawnSettlement);
                            //accompanying spawns
                            foreach (var accomp in data.SpawnAlongWith)
                            {
                                MobileParty juniorParty = _spawner.SpawnParty(spawnSettlement, data.SpawnClan, accomp.templateObject, data.BaseSpeedOverride, new TextObject(accomp.name));
                                if (juniorParty == null)
                                    continue;
                                HandleAIChecks(juniorParty, data, spawnSettlement); //junior party has same AI behaviour as main party. TODO in future add some junior party AI and reconstruction.
                            }
                            //message if available
                            if (data.spawnMessage != null)
                            {
                                UX.ShowParseSpawnMessage(data.spawnMessage, spawnSettlement.Name.ToString());
                                //if (data.SoundEvent != -1 && !isSpawnSoundPlaying && ConfigLoader.Instance.Config.SpawnSoundEnabled)
                                //{
                                //    var sceneEmpty = Scene.CreateNewScene(false);
                                //    SoundEvent sound = SoundEvent.CreateEvent(data.SoundEvent, sceneEmpty);
                                //    sound.Play();
                                //    isSpawnSoundPlaying = true;
                                //}
                            }
                            DailyLogger.ReportSpawn(spawnedParty, currentChanceOfSpawn);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                ErrorHandler.HandleException(e);
            }
        }

        private void OnPartyDeath(MobileParty mb, CSPartyData dynamicData)
        {
            HandleDeathMessage(mb, dynamicData);
        }

        #region Behaviour Handlers

        private void HandleDeathMessage(MobileParty mb, CSPartyData dynamicData)
        {
            if (dynamicData.spawnBaseData.deathMessage != null)
            {
                UX.ShowParseDeathMessage(dynamicData.spawnBaseData.deathMessage, dynamicData.latestClosestSettlement.ToString());
            }
        }

        #endregion
        
        private void HandleAIChecks(MobileParty mb, SpawnData data, Settlement spawnedSettlement) //TODO handle sub parties being reconstructed!
        {
            try
            {
                bool invalid = false;
                Dictionary<string, bool> aiRegistrations = new();
                if (data.PatrolAroundSpawn)
                {
                    bool success = AI.AIManager.HourlyPatrolAroundSpawn.RegisterParty(mb, spawnedSettlement);
                    aiRegistrations.Add("Patrol around spawn behaviour: ", success);
                    invalid = invalid ? true : !success;
                }
                if (data.AttackClosestIfIdleForADay)
                {
                    bool success = AI.AIManager.AttackClosestIfIdleForADayBehaviour.RegisterParty(mb);
                    aiRegistrations.Add("Attack Closest Settlement If Idle for A Day Behaviour: ", success);
                    invalid = invalid ? true : !success;
                }
                if (data.PatrolAroundClosestLestInterruptedAndSwitch.isValidData)
                {
                    bool success = AI.AIManager.PatrolAroundClosestLestInterruptedAndSwitchBehaviour.RegisterParty(mb, 
                        new AI.PatrolAroundClosestLestInterruptedAndSwitchBehaviour.PatrolAroundClosestLestInterruptedAndSwitchBehaviourData(mb, data.PatrolAroundClosestLestInterruptedAndSwitch));
                    aiRegistrations.Add("Patrol Around Closest Lest Interrupted And Switch Behaviour: ", success);
                    invalid = invalid ? true : !success;
                }
                if (invalid && ConfigLoader.Instance.Config.IsDebugMode)
                {
                    ErrorHandler.ShowPureErrorMessage("Custom Spawns AI XML registration error has occured. The party being registered was: " + mb.StringId +
                        "\n Here is more info about the behaviours being registered: \n" + String.Join("\n", aiRegistrations.Keys));
                }
            }
            catch (System.Exception e)
            {
                ErrorHandler.HandleException(e);
            }
        }
        private Settlement GetSpawnSettlement(Data.SpawnData data, Func<Settlement , bool> exceptionPredicate, Random rand = null)
        {
            if (rand == null)
                rand = new Random();


            Clan spawnClan = data.SpawnClan;
            //deal with override of spawn clan.
            if (data.OverridenSpawnClan.Count != 0)
            {
                spawnClan = data.OverridenSpawnClan[rand.Next(0, data.OverridenSpawnClan.Count)];
            }
            //check for one hideout
            Settlement firstHideout = null;
            if (ConfigLoader.Instance.Config.SpawnAtOneHideout)
            {
                foreach (Settlement s in Settlement.All)
                {
                    if (s.IsHideout)
                    {
                        firstHideout = s;
                        break;
                    }
                }
            }

            //deal with town spawn
            Settlement spawnOverride = null;
            if (data.OverridenSpawnSettlements.Count != 0)
            {
                spawnOverride = CampaignUtils.PickRandomSettlementAmong(new List<Settlement>(data.OverridenSpawnSettlements.Where(s => !exceptionPredicate(s))),
                    data.TrySpawnAtList, rand);
            }

            if (spawnOverride == null && data.OverridenSpawnCultures.Count != 0)
            {
                //spawn at overriden spawn instead!
                spawnOverride = CampaignUtils.PickRandomSettlementOfCulture(data.OverridenSpawnCultures, exceptionPredicate, data.TrySpawnAtList);
            }

            if (spawnOverride != null)
                return spawnOverride;

            //get settlement
            List<IMapPoint> parties = spawnClan.WarPartyComponents.Select(warParty => warParty.MobileParty).ToList<IMapPoint>();
            List<Settlement> hideouts = Settlement.All.Where(settlement => settlement.IsHideout).ToList();
            Settlement spawnSettlement = ConfigLoader.Instance.Config.SpawnAtOneHideout ? firstHideout : (data.TrySpawnAtList.Count == 0 ? CampaignUtils.GetNearestSettlement(hideouts, parties) : null);
            return spawnSettlement;
        }
    }
}
