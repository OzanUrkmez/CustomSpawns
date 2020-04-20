using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomSpawns.AI
{
    public class PatrolAroundClosestLestInterruptedAndSwitchBehaviour: IAIBehaviour
    {


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
