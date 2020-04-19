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

        private List<MobileParty> yesterdayIdleParties = new List<MobileParty>();
        private void DailyCheckBehaviour(MobileParty mb)
        {
            if (!registeredParties.Contains(mb))
                return;
            if (mb.Ai.AiState == AIState.Undefined || mb.Ai.AiState == AIState.WaitingAtSettlement)
            {
                if (yesterdayIdleParties.Contains(mb))
                {
                    Settlement closestHostile = CampaignUtils.GetClosestHostileSettlement(mb);
                    if (closestHostile == null)
                        return;
                    mb.SetMoveGoToPoint(mb.FindReachablePointAroundPosition(closestHostile.GatePosition, 10));
                    yesterdayIdleParties.Remove(mb);
                }
                else
                {
                    yesterdayIdleParties.Add(mb);
                }
            }
            else
            {
                yesterdayIdleParties.Remove(mb);
            }
        }

        public void RegisterParty(MobileParty mb)
        {
            registeredParties.Add(mb);
        }
    }
}
