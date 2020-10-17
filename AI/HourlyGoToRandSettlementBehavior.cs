using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem;

namespace CustomSpawns.AI
{
    public class HourlyGoToRandSettlementBehavior : CampaignBehaviorBase, IAIBehaviour
    {
        public override void RegisterEvents()
        {
            CampaignEvents.HourlyTickPartyEvent.AddNonSerializedListener(this, new Action<MobileParty>(this.HourlyTickParty));
            AIManager.HourlyGoToRandSettlementBehavior = this;
        }

        public override void SyncData(IDataStore dataStore)
        {

        }

        public Dictionary<RandSettlementBehaviorData, Settlement> dict = new Dictionary<RandSettlementBehaviorData, Settlement>();

        private List<RandSettlementBehaviorData> datInst = new List<RandSettlementBehaviorData>();

        private void HourlyTickParty(MobileParty mb)
        {
            List<RandSettlementBehaviorData> toRemove = new List<RandSettlementBehaviorData>();
            for (int i = 0; i < datInst.Count; i++)
            {
                var dat = datInst[i];
                MobileParty p = dat.party;
                if (p == null || !p.IsActive || p.MemberRoster.Count == 0)
                {
                    toRemove.Add(dat);
                    continue;
                }
                if (!Campaign.Current.GameStarted) //Don't execute for party if the game is paused
                {
                    continue;
                }
                if (p.MapEvent == null && (p.DefaultBehavior != AiBehavior.EngageParty) && (p.DefaultBehavior != AiBehavior.GoToSettlement)) //Lots of checks
                {
                    if ((p.ShortTermBehavior != AiBehavior.FleeToPoint) && !p.IsAlerted && (!p.Ai.DoNotMakeNewDecisions || p.IsCurrentlyUsedByAQuest)) //Just checking if the party is fleeing or quest involved
                    {
                        Settlement dest = this.ThinkDestination(dat.cultures, dat.preferredSettlements, dat.party);
                        if (dest != null)
                        {
                            p.SetMoveGoToSettlement(dest);
                            // p.Ai.SetAIState(AIState.VisitingNearbyTown, null); // Covering all the bases - this might cause problems down the line but I don't see any now
                            dict[dat] = dest;
                        }
                    }
                }
                Settlement destCheck = dict[dat];
                if (p.CurrentSettlement == null && destCheck != null && p.TargetSettlement != destCheck)
                {
                    p.SetMoveGoToSettlement(destCheck);
                    // p.Ai.SetAIState(AIState.VisitingNearbyTown, null);
                }
                dat.party.RecalculateShortTermAi(); //I think this is necessary? Forces the party to rethink their ShortTermAiBehavior to match their DefaultBehavior
            }
            for (int i = 0; i < toRemove.Count; i++)
            {
                datInst.Remove(toRemove[i]);
            }

        }

        private Settlement ThinkDestination(List<CultureCode> cultures, List<Data.SpawnSettlementType> types, MobileParty p)
        {
            Settlement result = null;
            Settlement s = CampaignUtils.PickRandomSettlementOfCulture(cultures, types, false);
            if ((s != p.CurrentSettlement) && !p.NeedTargetReset) //Make sure we aren't piling on the move orders and causing the AI to freak out
            {
                if (!s.IsUnderSiege && !s.IsRaided && !s.IsUnderRaid) //Probably a bad idea to go to a warzone
                {
                    result = s;
                }
            }
            return result;
        }

        #region Registration and AI Manager Integration

        public bool RegisterParty(MobileParty mb, RandSettlementBehaviorData data)
        {
            var behaviours = AI.AIManager.GetAIBehavioursForParty(mb);
            foreach (var b in behaviours)
            {
                if (!b.IsCompatible(this))
                    return false;
            }
            ModDebug.ShowMessage(mb.StringId + " will now go to random settlements", DebugMessageType.AI);
            AIManager.RegisterAIBehaviour(mb, this);
            datInst.Add(data);
            dict.Add(data, null);
            return true;
        }

        #endregion

        #region IAIBehaviour Implementation

        public bool IsCompatible(IAIBehaviour AIBehaviour, bool secondCall)
        {
            if (AIBehaviour is PatrolAroundClosestLestInterruptedAndSwitchBehaviour || AIBehaviour is HourlyPatrolAroundSpawnBehaviour || AIBehaviour is HourlyGoToRandSettlementBehavior)
                return false;
            return secondCall ? true : AIBehaviour.IsCompatible(this, true);
        }

        #endregion

        public struct RandSettlementBehaviorData
        {
            public MobileParty party;
            public List<CultureCode> cultures;
            public bool isValidData;
            public List<Data.SpawnSettlementType> preferredSettlements;

            public RandSettlementBehaviorData(MobileParty party, List<CultureCode> cList, List<Data.SpawnSettlementType> preferredSettlements)
            {
                this.party = party;
                this.preferredSettlements = preferredSettlements;
                this.cultures = cList;
                isValidData = true;
            }

            public RandSettlementBehaviorData(MobileParty party, RandSettlementBehaviorData data)
            {
                this.party = party;
                this.preferredSettlements = data.preferredSettlements;
                this.cultures = data.cultures;
                isValidData = true;
            }
        }
    }
}