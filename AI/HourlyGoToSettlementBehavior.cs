using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using CustomSpawns.Data;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;

namespace CustomSpawns.AI
{
    public class HourlyGoToSettlementBehaviour : CampaignBehaviorBase, IAIBehaviour
    {
        public override void RegisterEvents()
        {
            CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, HourlyCheckSettlementTick);
            AIManager.HourlyGoToSettlement = this;
        }

        public override void SyncData(IDataStore dataStore)
        {

        }

        private void HourlyCheckSettlementTick()
        {
            List<Traveller> toRemove = new List<Traveller>();
            foreach (var traveller in registeredTravellers)
            {
                if (traveller.travellerParty == null || traveller.travellerParty.MemberRoster.Count == 0)
                {
                    //if party is dead, remove
                    toRemove.Add(traveller);
                    return;
                }

                bool isPreOccupied = traveller.travellerParty.DefaultBehavior == AiBehavior.EngageParty || traveller.travellerParty.DefaultBehavior == AiBehavior.GoAroundParty ||
                     traveller.travellerParty.DefaultBehavior == AiBehavior.JoinParty || traveller.travellerParty.DefaultBehavior == AiBehavior.FleeToPoint;
                if (!isPreOccupied)
                {
                    isPreOccupied = traveller.travellerParty.ShortTermBehavior == AiBehavior.EngageParty || traveller.travellerParty.ShortTermBehavior == AiBehavior.GoAroundParty ||
                         traveller.travellerParty.ShortTermBehavior == AiBehavior.JoinParty || traveller.travellerParty.ShortTermBehavior == AiBehavior.FleeToPoint;
                }
                bool partyIsTraveling = traveller.travellerParty.IsGoingToSettlement;
                if (!partyIsTraveling)
                {
                    partyIsTraveling = traveller.travellerParty.IsCurrentlyGoingToSettlement;
                }
                if (!partyIsTraveling)
                {
                    List<CultureCode> spawnSettlementCultureList = new List<CultureCode>();
                    spawnSettlementCultureList.Add(traveller.homeSettlement.Culture.GetCultureCode());
                    Settlement settlementGoingTo = CampaignUtils.PickRandomSettlementOfCulture(spawnSettlementCultureList);
                    if (settlementGoingTo.IsRaided || settlementGoingTo.IsUnderSiege) // if settlement that it's pathing to is raided or under siege, change target
                    {
                        List<CultureCode> fallbackSettlementCultureList = new List<CultureCode>();
                        fallbackSettlementCultureList.Add(traveller.homeSettlement.Culture.GetCultureCode());
                        Settlement fallbackSettlement = CampaignUtils.PickRandomSettlementOfCulture(fallbackSettlementCultureList);
                        traveller.travellerParty.SetMoveGoToSettlement(fallbackSettlement);
                    }
                    else if (!isPreOccupied)
                    {
                        traveller.travellerParty.SetMoveGoToSettlement(settlementGoingTo);
                    }
                }

            }
            for (int i = 0; i < toRemove.Count; i++){
                registeredTravellers.Remove(toRemove[i]);
            }
        }

        private List<Traveller> registeredTravellers = new List<Traveller>();

        private Traveller GetTraveller(MobileParty mb)
        {
            var traveller = registeredTravellers.FirstOrDefault((p) => p.travellerParty == mb);
            return traveller;
        }

        struct Traveller
        {
            public MobileParty travellerParty;
            public Settlement homeSettlement;

            public Traveller(MobileParty mb, Settlement s)
            {
                travellerParty = mb;
                homeSettlement = s;
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
            var travellerInstance = GetTraveller(mb);
            // might need this in the future, cant be assed right now
            //if (travellerInstance.travellerParty != null && travellerInstance.settlementGoingTo == s)
            //{
            //    ErrorHandler.HandleException(new Exception("The same mobile party cannot patrol around two different settlements!"));
            //}
            if (travellerInstance.homeSettlement == s)
                return false;
            registeredTravellers.Add(new Traveller(mb, s));
            ModDebug.ShowMessage(mb.StringId + " is attempting to go to settlement " + s.Name, DebugMessageType.AI);
            AIManager.RegisterAIBehaviour(mb, this);
            return true;
        }

        #endregion

        #region IAIBehaviour Implementation

        public bool IsCompatible(IAIBehaviour AIBehaviour, bool secondCall)
        {
            if (AIBehaviour is PatrolAroundClosestLestInterruptedAndSwitchBehaviour || AIBehaviour is HourlyPatrolAroundSpawnBehaviour || AIBehaviour is HourlyGoToSettlementBehaviour)
                return false;
            return secondCall? true : AIBehaviour.IsCompatible(this, true);
        }

        #endregion
    }
}
