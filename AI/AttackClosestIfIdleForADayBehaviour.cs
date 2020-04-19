using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;

namespace CustomSpawns.AI
{
    public class AttackClosestIfIdleForADayBehaviour : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            AIManager.AttackClosestIfIdleForADayBehaviour = this;
            CampaignEvents.DailyTickPartyEvent.AddNonSerializedListener(this, DailyCheckBehaviour);
        }

        public override void SyncData(IDataStore dataStore)
        {
            
        }

        private List<MobileParty> registeredParties = new List<MobileParty>();

        private void DailyCheckBehaviour(MobileParty mb)
        {
            if (!registeredParties.Contains(mb))
                return;

        }

        public void RegisterParty(MobileParty mb)
        {
            registeredParties.Add(mb);
        }
    }
}
