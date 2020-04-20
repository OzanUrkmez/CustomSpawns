using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;

namespace CustomSpawns.AI
{
    public class HourlyPatrolAroundSpawnBehaviour : CampaignBehaviorBase, IAIBehaviour
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
            List<Patroller> toRemove = new List<Patroller>();
            foreach(var patroller in registeredPatrollers)
            {
                if(patroller.patrollerParty == null || patroller.patrollerParty.MemberRoster.Count == 0 || patroller.patrolledSettlement == null)
                {
                    //ded or for some reason settlement dead
                    toRemove.Add(patroller);
                    return;
                }
                bool isPreOccupied = patroller.patrollerParty.DefaultBehavior == AiBehavior.EngageParty || patroller.patrollerParty.DefaultBehavior == AiBehavior.GoAroundParty ||
                     patroller.patrollerParty.DefaultBehavior == AiBehavior.JoinParty || patroller.patrollerParty.DefaultBehavior == AiBehavior.FleeToPoint;
                if (!isPreOccupied)
                {
                    patroller.patrollerParty.SetMovePatrolAroundSettlement(patroller.patrolledSettlement);
                }
            }
            for (int i = 0; i < toRemove.Count; i++){
                registeredPatrollers.Remove(toRemove[i]);
            }
        }

        private List<Patroller> registeredPatrollers = new List<Patroller>();

        private Patroller GetPatroller(MobileParty mb)
        {
            var patroller = registeredPatrollers.FirstOrDefault((p) => p.patrollerParty == mb);
            return patroller;
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

        #region Registration and AI Manager Integration

        public bool RegisterParty(MobileParty mb, Settlement s)
        {
            var behaviours = AI.AIManager.GetAIBehavioursForParty(mb);
            foreach (var b in behaviours)
            {
                if (!b.IsCompatible(this))
                    return false;
            }
            var patrollerInstance = GetPatroller(mb);
            if (patrollerInstance.patrollerParty != null && patrollerInstance.patrolledSettlement != s)
            {
                ErrorHandler.HandleException(new Exception("The same mobile party cannot patrol around two different settlements!"));
            }
            if (patrollerInstance.patrolledSettlement == s)
                return false;
            registeredPatrollers.Add(new Patroller(mb, s));
            mb.HomeSettlement = s;
            ModDebug.ShowMessage(mb.StringId + " is now engaged in patrol behaviour around " + s.Name);
            AIManager.RegisterAIBehaviour(mb, this);
            return true;
        }

        #endregion

        #region IAIBehaviour Implementation

        public bool IsCompatible(IAIBehaviour AIBehaviour, bool secondCall)
        {
            if (AIBehaviour is PatrolAroundClosestLestInterruptedAndSwitchBehaviour || AIBehaviour is HourlyPatrolAroundSpawnBehaviour)
                return false;
            return secondCall? true : AIBehaviour.IsCompatible(this, true);
        }

        #endregion
    }
}
