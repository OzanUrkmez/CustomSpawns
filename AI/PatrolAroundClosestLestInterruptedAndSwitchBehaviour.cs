using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;

namespace CustomSpawns.AI
{
    public class PatrolAroundClosestLestInterruptedAndSwitchBehaviour: CampaignBehaviorBase, IAIBehaviour
    {

        public override void RegisterEvents()
        {
            CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, hourlyAI);
            AIManager.PatrolAroundClosestLestInterruptedAndSwitchBehaviour = this;
        }

        public override void SyncData(IDataStore dataStore)
        {
            
        }

        private void hourlyAI()
        {

        }


        #region Registration and AI Manager Integration

        public bool RegisterParty(MobileParty mb, PatrolAroundClosestLestInterruptedAndSwitchBehaviourData data)
        {
            var behaviours = AI.AIManager.GetAIBehavioursForParty(mb);
            foreach (var b in behaviours)
            {
                if (!b.IsCompatible(this))
                    return false;
            }
            ModDebug.ShowMessage(mb.StringId + " will now patrol around closest settlement, unless interrupted, and, if has not been interrupted, will" +
                " switch settlements every " + data.minStablePatrolDays.ToString() + " to " + 
                data.maxStablePatrolDays.ToString() + " days.");
            AIManager.RegisterAIBehaviour(mb, this);
            return true;
        }

        #endregion


        #region IAIBehaviour Implementation

        public bool IsCompatible(IAIBehaviour AIBehaviour, bool secondCall)
        {
            if (AIBehaviour is HourlyPatrolAroundSpawnBehaviour || AIBehaviour is PatrolAroundClosestLestInterruptedAndSwitchBehaviour)
                return false;
            return secondCall ? true : AIBehaviour.IsCompatible(this, true);
        }


        #endregion

        public struct PatrolAroundClosestLestInterruptedAndSwitchBehaviourData
        {
            public float minStablePatrolDays;
            public float maxStablePatrolDays;
            public bool isValidData;
            public List<Data.SpawnSettlementType> preferredSettlements;

            public PatrolAroundClosestLestInterruptedAndSwitchBehaviourData(float minDays, float maxDays, List<Data.SpawnSettlementType> preferredSettlements)
            {
                minStablePatrolDays = minDays;
                maxStablePatrolDays = maxDays;
                isValidData = true;
                this.preferredSettlements = preferredSettlements;
            }
        }
    }
}
