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

        private List<Patroller> registeredPatrollers = new List<Patroller>();

        private Patroller GetPatroller(MobileParty mb)
        {
            var patroller = registeredPatrollers.FirstOrDefault((p) => p.patrollerParty == mb);
            return patroller;
        }

        public void RegisterMobilePartyToPatrol(MobileParty mb, Settlement s)
        {
            var patrollerInstance = GetPatroller(mb);
            if (patrollerInstance.patrollerParty != null && patrollerInstance.patrolledSettlement != s)
            {
                ErrorHandler.HandleException(new Exception("The same mobile party cannot patrol around two different settlements!"));
            }
            if (patrollerInstance.patrolledSettlement == s)
                return;
            registeredPatrollers.Add(new Patroller(mb, s));
        }

        struct Patroller
        {
            public MobileParty patrollerParty;
            public Settlement patrolledSettlement;

            public Patroller(MobileParty mb, Settlement s)
            {
                patrollerParty = mb;
                patrolledSettlement = s;
            }
        }
    }
}
