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

            foreach(var mb in presentInDay)
            {
                if (mb.IsBandit)
                {
                     //TODO left here.
                }else if (s.OwnerClan.MapFaction.IsAtWarWith(mb.Party.MapFaction))
                {
                    
                }else if(s.OwnerClan.MapFaction == mb.Party.MapFaction)
                {

                }
            }

            ChangeDevestation(s, -campaignConfig.DailyDevestationDecay);
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
                ModDebug.ShowMessage("Non-village devestation data is not currently supported!", campaignConfig);
            }
            if (settlementToDevestation.ContainsKey(s))
                return settlementToDevestation[s];

            ErrorHandler.HandleException(new Exception("Devestation value for settlement could not be found!"));
            return 0;
        }

    }
}
