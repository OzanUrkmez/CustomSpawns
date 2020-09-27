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
            ModDebug.ShowMessage("Yes", DebugMessageType.Development);
        }

        #endregion

        #region Campaign Behavior Base Abstract Implementation



        public override void RegisterEvents()
        {
            CampaignEvents.CommonAreaFightOccured.AddNonSerializedListener(this, OnCommonAreaFightOccured);
            CampaignEvents.VillageLooted.AddNonSerializedListener(this, OnVillageLooted);
            CampaignEvents.MobilePartyDestroyed.AddNonSerializedListener(this, OnMobilePartyDestroyed);
            CampaignEvents.HourlyTickPartyEvent.AddNonSerializedListener(this, OnMobilePartyHourlyTick);

            CampaignEvents.OnGameLoadFinishedEvent.AddNonSerializedListener(this, OnGameLoadFinished);
        }

        public override void SyncData(IDataStore dataStore)
        {
           
        }


        #endregion

        #region Event Callbacks
    
        private void OnGameLoadFinished()
        {
            ModDebug.ShowMessage("game loaded!", DebugMessageType.Development);
        }

        private void OnCommonAreaFightOccured(MobileParty p1, MobileParty p2, Hero h, Settlement s)
        {

        }

        private void OnVillageLooted(Village v)
        {
        }

        private void OnMobilePartyDestroyed(MobileParty mb, PartyBase mbbase)
        {

        }

        private void OnMobilePartyHourlyTick(MobileParty mb)
        {
            
        }

        #endregion

        public float GetDevestation(Settlement s)
        {
            if (settlementToDevestation.ContainsKey(s))
                return settlementToDevestation[s];

            ErrorHandler.HandleException(new Exception("Devestation value for settlement could not be found!"));
            return 0;
        }

    }
}
