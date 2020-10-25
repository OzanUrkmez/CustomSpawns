using CustomSpawns.UtilityBehaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.TwoDimension;

namespace CustomSpawns.CampaignData
{
    class DevestationMetricData : CampaignBehaviorBase
    {

        private Dictionary<Settlement, float> settlementToDevestation = new Dictionary<Settlement, float>();

        private DevestationMetricConfig campaignConfig;

        #region Singleton and Initialization

        private static DevestationMetricData _singleton;

        public static DevestationMetricData Singleton
        {
            get
            {
                if (_singleton == null)
                {
                    _singleton = new DevestationMetricData();
                }

                return _singleton;

            }
            private set
            {
                _singleton = value;
            }
        }

        public DevestationMetricData()
        {

            OnSaveStartRunBehaviour.Singleton.RegisterFunctionToRunOnSaveStart(OnGameInitialization);
            campaignConfig = CampaignDataConfigLoader.Instance.GetConfig<DevestationMetricConfig>();
        }

        #endregion

        #region Campaign Behavior Base Abstract Implementation



        public override void RegisterEvents()
        {
            CampaignEvents.MapEventEnded.AddNonSerializedListener(this, OnMapEventEnded);
            CampaignEvents.VillageLooted.AddNonSerializedListener(this, OnVillageLooted);

            CampaignEvents.DailyTickSettlementEvent.AddNonSerializedListener(this, OnSettlementDaily);

        }

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("settlementToDevestation", ref settlementToDevestation);
        }


        #endregion

        private void OnGameInitialization()
        {
            foreach(Settlement s in Settlement.All) //assuming no new settlements can be created mid-game.
            {
                if (!s.IsVillage)
                {
                    continue;
                }
                settlementToDevestation.Add(s, 0);
            }
        }

        #region Event Callbacks

       private void OnMapEventEnded(MapEvent e)
        {
            if (e == null)
                return;

            float increase = (e.AttackerSide.CasualtyStrength + e.DefenderSide.CasualtyStrength) * campaignConfig.FightOccuredDevestationPerPower;

            Settlement closestSettlement = CampaignUtils.GetClosestVillage(e.Position);

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
