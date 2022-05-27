using CustomSpawns.UtilityBehaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.TwoDimension;

namespace CustomSpawns.CampaignData
{
    class DevestationMetricData : CustomCampaignDataBehaviour<DevestationMetricData, DevestationMetricConfig>
    {

        private Dictionary<Settlement, float> settlementToDevestation = new Dictionary<Settlement, float>();


        #region Custom Campaign Data Implementation


        protected override void OnRegisterEvents()
        {
            CampaignEvents.MapEventEnded.AddNonSerializedListener(this, OnMapEventEnded);
            CampaignEvents.VillageLooted.AddNonSerializedListener(this, OnVillageLooted);

            CampaignEvents.DailyTickSettlementEvent.AddNonSerializedListener(this, OnSettlementDaily);
        }

        protected override void SyncSaveData(IDataStore dataStore)
        {
            dataStore.SyncData("settlementToDevestation", ref settlementToDevestation);
        }

        protected override void OnSaveStart()
        {
            if (settlementToDevestation.Count != 0) //If you include non-village etc. or add new settlements this approach will break old saves.
            {
                return;
            }

            foreach (Settlement s in Settlement.All) //assuming no new settlements can be created mid-game.
            {
                if (!s.IsVillage)
                {
                    continue;
                }
                settlementToDevestation.Add(s, 0);
            }
        }

        public override void FlushSavedData()
        {
            settlementToDevestation.Clear();
        }


        #endregion

        #region Event Callbacks

        private void OnMapEventEnded(MapEvent e)
        {
            if (e == null)
                return;

            float increase = (e.AttackerSide.CasualtyStrength + e.DefenderSide.CasualtyStrength) * campaignConfig.FightOccuredDevestationPerPower;

            Settlement closestSettlement = CampaignUtils.GetClosestVillage(e.Position);

            if (!settlementToDevestation.ContainsKey(closestSettlement))
            {
                ErrorHandler.HandleException(new Exception("Devestation value for settlement could not be found!"));
                return;
            }

            ChangeDevestation(closestSettlement, increase);

            ModDebug.ShowMessage("Fight at " + closestSettlement.Name + ". Increasing devestation by " + increase + ". New devestation is: " + settlementToDevestation[closestSettlement], campaignConfig);
        }

        private void OnVillageLooted(Village v)
        {
            if (v == null)
                return;

            ChangeDevestation(v.Settlement, campaignConfig.DevestationPerTimeLooted);

            ModDebug.ShowMessage("Successful Looting at " + v.Name + ". Increasing devestation by " + campaignConfig.DevestationPerTimeLooted, campaignConfig);
        }

        private void OnSettlementDaily(Settlement s)
        {
            if (s == null || !s.IsVillage)
                return;

            if (!settlementToDevestation.ContainsKey(s))
            {
                ErrorHandler.HandleException(new Exception("Devastation value for settlement could not be found!"));
                return;
            }

            var presentInDay = MobilePartyTrackingBehaviour.Singleton.GetSettlementDailyMobilePartyPresences(s);

            float hostileDecay = 0;

            float friendlyGain = 0;

            foreach (var mb in presentInDay)
            {
                if (mb.IsBandit)
                {
                    hostileDecay += campaignConfig.HostilePresencePerPowerDaily * mb.Party.TotalStrength;
                }
                else if (s.OwnerClan.MapFaction.IsAtWarWith(mb.Party.MapFaction))
                {
                    hostileDecay += campaignConfig.HostilePresencePerPowerDaily * mb.Party.TotalStrength;
                }
                else if (s.OwnerClan.MapFaction == mb.Party.MapFaction)
                {
                    friendlyGain += campaignConfig.FriendlyPresenceDecayPerPowerDaily * mb.Party.TotalStrength;
                }
            }

            if(friendlyGain > 0)
            {
                //ModDebug.ShowMessage("Calculating friendly presence devestation decay in " + s.Name + ". Decreasing devestation by " + friendlyGain, campaignConfig);

                ChangeDevestation(s, -friendlyGain);
            }


            if(hostileDecay > 0)
            {
                ModDebug.ShowMessage("Calculating hostile presence devestation gain in " + s.Name + ". Increasing devestation by " + hostileDecay, campaignConfig);

                ChangeDevestation(s, hostileDecay);
            }

            if(GetDevestation(s) > 0)
            {
                ModDebug.ShowMessage("Calculating daily Devestation Decay in " + s.Name + ". Decreasing devestation by " + campaignConfig.DailyDevestationDecay, campaignConfig);
                ChangeDevestation(s, -campaignConfig.DailyDevestationDecay);
            }

            if(GetDevestation(s) != 0)
                ModDebug.ShowMessage("Current Devestation at " + s.Name + " is now " + settlementToDevestation[s], campaignConfig);
        }

        #endregion



        private void ChangeDevestation(Settlement s, float change)
        {
            if (!settlementToDevestation.ContainsKey(s))
            {
                ErrorHandler.HandleException(new Exception("Devastation value for settlement could not be found!"));
                return;
            }

            settlementToDevestation[s] += change;

            settlementToDevestation[s] = Mathf.Clamp(settlementToDevestation[s], campaignConfig.MinDevestationPerSettlement, campaignConfig.MaxDevestationPerSettlement);
        }

        public float GetDevestation(Settlement s)
        {
            if (!s.IsVillage)
            {
                return 0;
                ModDebug.ShowMessage("Non-village devestation data is not currently supported!", campaignConfig);
            }
            if (settlementToDevestation.ContainsKey(s))
                return settlementToDevestation[s];

            ErrorHandler.HandleException(new Exception("Devestation value for settlement could not be found!"));
            return 0;
        }

        public float GetMinimumDevestation()
        {
            return campaignConfig.MinDevestationPerSettlement;
        }

        public float GetMaximumDevestation()
        {
            return campaignConfig.MaxDevestationPerSettlement;
        }


        public float GetAverageDevestation()
        {
            return settlementToDevestation.Values.Aggregate((c,d) => c+d) / settlementToDevestation.Count; //TODO make more efficient
        }

        public float GetDevestationLerp()
        {
            return GetAverageDevestation() / GetMaximumDevestation();
        }


    }
}
