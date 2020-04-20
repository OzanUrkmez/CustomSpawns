using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
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

        private List<PatrolAroundClosestLestInterruptedAndSwitchBehaviourData> instances = new List<PatrolAroundClosestLestInterruptedAndSwitchBehaviourData>();

        private void hourlyAI()
        {
            List<PatrolAroundClosestLestInterruptedAndSwitchBehaviourData> toRemove = new List<PatrolAroundClosestLestInterruptedAndSwitchBehaviourData>();
            for(int i = 0; i < instances.Count; i++)
            {
                var dat = instances[i];
                if(dat.party == null || !dat.party.IsActive || dat.party.MemberRoster.Count == 0)
                {
                    toRemove.Add(dat);
                    continue;
                }
                bool isPreOccupied = dat.party.DefaultBehavior == AiBehavior.EngageParty || dat.party.DefaultBehavior == AiBehavior.GoAroundParty ||
                dat.party.DefaultBehavior == AiBehavior.JoinParty || dat.party.DefaultBehavior == AiBehavior.FleeToPoint;
                if(!isPreOccupied)
                    isPreOccupied = dat.party.ShortTermBehavior == AiBehavior.EngageParty || dat.party.ShortTermBehavior == AiBehavior.GoAroundParty ||
                dat.party.ShortTermBehavior == AiBehavior.JoinParty || dat.party.ShortTermBehavior == AiBehavior.FleeToPoint;
                if (isPreOccupied)
                {
                    dat.currentDays = 0; 
                    instances[i] = dat;
                    continue;
                }
                if(dat.currentPatrolledSettlement == null)
                {
                    dat.currentPatrolledSettlement = GetSettlementToPatrol(dat);
                    if (dat.currentPatrolledSettlement == null)
                        continue;
                    dat.currentDays = 0;
                    dat.currentRolledDays = GetRolledDay(dat);
                }
                dat.party.SetMovePatrolAroundSettlement(dat.currentPatrolledSettlement);
                dat.currentDays += (1f / 24f);
                if(dat.currentDays >= dat.currentRolledDays)
                {
                    dat.currentDays = 0;
                    dat.currentPatrolledSettlement = GetSettlementToPatrol(dat);
                    if (dat.currentPatrolledSettlement == null)
                    {
                        instances[i] = dat;
                        continue;
                    }
                    dat.party.SetMovePatrolAroundSettlement(dat.currentPatrolledSettlement);
                    dat.currentRolledDays = GetRolledDay(dat);
                }
                instances[i] = dat;
            }

            foreach (var dat in toRemove)
            {
                instances.Remove(dat);
            }
        }

        private Settlement GetSettlementToPatrol(PatrolAroundClosestLestInterruptedAndSwitchBehaviourData dat)
        {
            return CampaignUtils.GetClosestNonHostileCityAmong(dat.party, dat.preferredSettlements, dat.currentPatrolledSettlement);
        }

        private float GetRolledDay(PatrolAroundClosestLestInterruptedAndSwitchBehaviourData dat)
        {
            return MBRandom.RandomFloatRanged(dat.minStablePatrolDays, dat.maxStablePatrolDays);
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
            instances.Add(data);
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
            public Settlement currentPatrolledSettlement;
            public MobileParty party;
            public float currentDays;
            public float currentRolledDays;
            public float minStablePatrolDays;
            public float maxStablePatrolDays;
            public bool isValidData;
            public List<Data.SpawnSettlementType> preferredSettlements;

            public PatrolAroundClosestLestInterruptedAndSwitchBehaviourData(MobileParty party, float minDays, float maxDays, List<Data.SpawnSettlementType> preferredSettlements)
            {
                this.party = party;
                minStablePatrolDays = minDays;
                maxStablePatrolDays = maxDays;
                isValidData = true;
                this.preferredSettlements = preferredSettlements;
                currentDays = 0;
                currentPatrolledSettlement = null;
                currentRolledDays = 0;
            }

            public PatrolAroundClosestLestInterruptedAndSwitchBehaviourData(MobileParty party, PatrolAroundClosestLestInterruptedAndSwitchBehaviourData data)
            {
                this.party = party;
                minStablePatrolDays = data.minStablePatrolDays;
                maxStablePatrolDays = data.maxStablePatrolDays;
                isValidData = true;
                this.preferredSettlements = data.preferredSettlements;
                currentDays = 0;
                currentPatrolledSettlement = null;
                currentRolledDays = 0;
            }
        }
    }
}
