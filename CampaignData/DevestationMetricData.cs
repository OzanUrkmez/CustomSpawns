using CustomSpawns.UtilityBehaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;

namespace CustomSpawns.CampaignData
{
    class DevestationMetricData : CampaignBehaviorBase
    {

        private Dictionary<Settlement, float> settlementToDevestation = new Dictionary<Settlement, float>();

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

        }

        #endregion

        #region Campaign Behavior Base Abstract Implementation



        public override void RegisterEvents()
        {
            CampaignEvents.CommonAreaFightOccured.AddNonSerializedListener(this, OnCommonAreaFightOccured);
            CampaignEvents.VillageLooted.AddNonSerializedListener(this, OnVillageLooted);
            CampaignEvents.MobilePartyDestroyed.AddNonSerializedListener(this, OnMobilePartyDestroyed);
            CampaignEvents.HourlyTickPartyEvent.AddNonSerializedListener(this, OnMobilePartyHourlyTick);
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

        private void OnCommonAreaFightOccured(MobileParty p1, MobileParty p2, Hero h, Settlement s)
        {
            if (p1 == null || p2 == null || s==null)
                return;

            ModDebug.ShowMessage("Fight at " + s.Name, DebugMessageType.Development);
        }

        private void OnVillageLooted(Village v)
        {
            if (v == null)
                return;
        }

        private void OnMobilePartyDestroyed(MobileParty mb, PartyBase mbbase)
        {
            if (mb == null)
                return;

        }

        private void OnMobilePartyHourlyTick(MobileParty mb)
        {
            if (mb == null || mb.IsBandit)
                return;
        }

        #endregion

        public float GetDevestation(Settlement s)
        {
            if (!s.IsVillage)
            {
                ModDebug.ShowMessage("Non-village devestation data is not currently supported!", DebugMessageType.Development);
            }
            if (settlementToDevestation.ContainsKey(s))
                return settlementToDevestation[s];

            ErrorHandler.HandleException(new Exception("Devestation value for settlement could not be found!"));
            return 0;
        }

    }
}
