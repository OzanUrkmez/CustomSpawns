using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.ObjectSystem;

namespace CustomSpawns.UtilityBehaviours
{
    class MobilePartyTrackingBehaviour: CampaignBehaviorBase
    {

        #region Singleton Architecture
        private static MobilePartyTrackingBehaviour _singleton;

        public static MobilePartyTrackingBehaviour Singleton
        {
            get
            {
                if (_singleton == null)
                    _singleton = new MobilePartyTrackingBehaviour();
                return _singleton;
            }
            private set
            {
                _singleton = value;
            }
        }

        private MobilePartyTrackingBehaviour()
        {
            OnSaveStartRunBehaviour.Singleton.RegisterFunctionToRunOnSaveStart(OnGameStart);
        }

        #endregion

        #region Taleworlds Implementations

        public override void RegisterEvents()
        {
            CampaignEvents.HourlyTickPartyEvent.AddNonSerializedListener(this, OnMobilePartyHourlyTick);
            CampaignEvents.MobilePartyDestroyed.AddNonSerializedListener(this, OnMobilePartyDestroyed);
            CampaignEvents.AfterDailyTickEvent.AddNonSerializedListener(this, LateDailyTick);

            
        }

        public override void SyncData(IDataStore dataStore) //maybe make this work between loads too? it is just a daily event tho.
        {
            
        }

        #endregion

        private Dictionary<MobileParty, List<Settlement>> dailyPresences = new Dictionary<MobileParty, List<Settlement>>();
        private Dictionary<Settlement, List<MobileParty>> settlementDailyPresences = new Dictionary<Settlement, List<MobileParty>>();


        private List<MobileParty> toBeRemoved = new List<MobileParty>();

        private void OnGameStart()
        {
            foreach(Settlement s in Settlement.All)
            {
                settlementDailyPresences.Add(s, new List<MobileParty>());
            }
        }

        private void OnMobilePartyHourlyTick(MobileParty mb)
        {
            if (mb == null)
                return;

            Settlement closest = CampaignUtils.GetClosestSettlement(mb);

            if (!dailyPresences.ContainsKey(mb))
                dailyPresences.Add(mb, new List<Settlement>());

            if (!dailyPresences[mb].Contains(closest))
            {
                dailyPresences[mb].Add(closest);
                settlementDailyPresences[closest].Add(mb);
            }
        }

        private void OnMobilePartyDestroyed(MobileParty mb, PartyBase pb)
        {
            toBeRemoved.Add(mb);
        }

        private void LateDailyTick() 
        {
            foreach(var mb in toBeRemoved)
            {
                dailyPresences.Remove(mb);
            }

            toBeRemoved.Clear();

            foreach(var l in settlementDailyPresences.Values)
            {
                l.Clear();
            }

            foreach(var l in dailyPresences.Values)
            {
                l.Clear();
            }
        }

        /// <summary>
        /// Returns a list of all settlements that this party has been closest to within the day.
        /// </summary>
        /// <param name="mb"></param>
        /// <returns></returns>
        public List<Settlement> GetMobilePartyDailyPresences(MobileParty mb)
        {
            if (dailyPresences.ContainsKey(mb))
            {
                return dailyPresences[mb];
            }
            else
            {
                ModDebug.ShowMessage("Tried to get daily presences of an invalid mobile party!", DebugMessageType.Development);
                return new List<Settlement>();
            }
        }

        /// <summary>
        /// Gets a list of all mobile parties that have visited this settlement today. 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public List<MobileParty> GetSettlementDailyMobilePartyPresences(Settlement s)
        {
            if (settlementDailyPresences.ContainsKey(s))
            {
                return settlementDailyPresences[s];
            }
            else
            {
                ModDebug.ShowMessage("Tried to get daily presences of an invalid settlement!", DebugMessageType.Development);
                return new List<MobileParty>();
            }
        }

    }
}
