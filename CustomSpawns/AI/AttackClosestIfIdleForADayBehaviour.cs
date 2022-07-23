using Helpers;
using System.Collections.Generic;
using CustomSpawns.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace CustomSpawns.AI
{
    public class AttackClosestIfIdleForADayBehaviour : CampaignBehaviorBase, IAIBehaviour
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
                    mb.SetMoveGoToPoint(MobilePartyHelper.FindReachablePointAroundPosition(closestHostile.GatePosition, 10));
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

        #region Registration and AI Manager Integration

        public bool RegisterParty(MobileParty mb)
        {
            var behaviours = AIManager.GetAIBehavioursForParty(mb);
            foreach(var b in behaviours)
            {
                if (!b.IsCompatible(this))
                    return false;
            }
            registeredParties.Add(mb);
            ModDebug.ShowMessage(mb.StringId + " is now prevented from idling for 2 days. If it does idle for 2 days it will head to closest hostile settlement.", DebugMessageType.AI);
            AIManager.RegisterAIBehaviour(mb, this);
            return true;
        }

        #endregion

        #region IAIBehaviour Implementation

        public bool IsCompatible(IAIBehaviour AIBehaviour, bool secondCall)
        {
            if (AIBehaviour is AttackClosestIfIdleForADayBehaviour)
                return false;
            return secondCall ? true : AIBehaviour.IsCompatible(this, true);
        }

        #endregion
    }
}
