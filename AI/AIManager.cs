using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;

namespace CustomSpawns.AI
{
    public static class AIManager
    {

        public static HourlyPatrolAroundSpawnBehaviour HourlyPatrolAroundSpawn { get; set; }

        public static AttackClosestIfIdleForADayBehaviour AttackClosestIfIdleForADayBehaviour { get; set; }

        public static HourlyGoToRandSettlementBehavior HourlyGoToRandSettlementBehavior { get; set; }

        public static PatrolAroundClosestLestInterruptedAndSwitchBehaviour PatrolAroundClosestLestInterruptedAndSwitchBehaviour { get; set; }

        private static Dictionary<MobileParty, List<IAIBehaviour>> partyBehaviours = new Dictionary<MobileParty, List<IAIBehaviour>>();

        public static void RegisterAIBehaviour(MobileParty mb, IAIBehaviour behaviour)
        {
            if (!partyBehaviours.ContainsKey(mb))
                partyBehaviours.Add(mb, new List<IAIBehaviour>());
            partyBehaviours[mb].Add(behaviour);
        }

        public static List<IAIBehaviour> GetAIBehavioursForParty(MobileParty mb)
        {
            if (!partyBehaviours.ContainsKey(mb))
                partyBehaviours.Add(mb, new List<IAIBehaviour>());
            return partyBehaviours[mb];
        }

        public static void FlushRegisteredBehaviours()
        {
            partyBehaviours.Clear();
        }
    }
}
