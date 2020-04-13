using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;

namespace CustomSpawns.AI
{
    public class HourlyPatrolAroundSpawnBehaviour : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            AIManager.HourlyPatrolAroundSpawn = this;
            CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, HourlyPatrolTick);
        }

        public override void SyncData(IDataStore dataStore)
        {

        }

        private void HourlyPatrolTick()
        {

        }

        public void RegisterMobilePartyToPatrol()
        {

        }
    }
}
