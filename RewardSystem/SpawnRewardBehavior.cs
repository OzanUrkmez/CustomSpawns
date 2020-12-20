using System;
using System.Collections.Generic;
using System.Linq;
using CustomSpawns.RewardSystem.Models;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace CustomSpawns.RewardSystem
{
    public class SpawnRewardBehavior : CampaignBehaviorBase
    {
        public SpawnRewardBehavior() : base()
        {
            XmlRewardData.GetInstance();
        }
        
        public override void RegisterEvents()
        {
            CampaignEvents.OnPlayerBattleEndEvent.AddNonSerializedListener(this, new Action<MapEvent>(
                mapEvent =>
                {
                    if (mapEvent.GetLeaderParty(BattleSideEnum.Attacker).Id == "player_party" &&
                        mapEvent.WinningSide == BattleSideEnum.Attacker)
                    {
                        CalculateReward(mapEvent.DefenderSide.Parties, mapEvent.GetLeaderParty(BattleSideEnum.Attacker));
                    }
                    else if (mapEvent.GetLeaderParty(BattleSideEnum.Defender).Id == "player_party" &&
                             mapEvent.WinningSide == BattleSideEnum.Defender)
                    {
                        CalculateReward(mapEvent.AttackerSide.Parties, mapEvent.GetLeaderParty(BattleSideEnum.Defender));
                    }
                })
            );
        }

        public override void SyncData(IDataStore dataStore){}

        private void CalculateReward(IReadOnlyCollection<PartyBase> defeatedParties, PartyBase playerParty)
        {
            foreach (var party in defeatedParties)
            {
                var partyRewards = XmlRewardData.GetInstance().PartyRewards;
                var partyReward = partyRewards.FirstOrDefault(el => party.Id.Contains(el.PartyId));
                if (partyReward != null)
                {
                    foreach (var reward in partyReward.Rewards)
                    {
                        switch (reward.Type)
                        {
                            case RewardType.Influence:
                                if (reward.RenownInfluenceMoneyAmount != null)
                                {
                                    playerParty.Owner.AddInfluenceWithKingdom(Convert.ToSingle(reward.RenownInfluenceMoneyAmount));
                                }
                                break;
                            case RewardType.Money:
                                if (reward.RenownInfluenceMoneyAmount != null)
                                {
                                    playerParty.LeaderHero.ChangeHeroGold(Convert.ToInt32(reward.RenownInfluenceMoneyAmount));
                                }
                                break;
                            case RewardType.Item:
                                if (reward.ItemId != null)
                                {
                                    var itemToAdd = ItemObject.All.FirstOrDefault(obj => obj.StringId == reward.ItemId);
                                    playerParty.ItemRoster.AddToCounts(itemToAdd, 1);
                                }
                                break;
                            case RewardType.Renown:
                                if (reward.RenownInfluenceMoneyAmount != null)
                                {
                                    playerParty.LeaderHero.Clan.AddRenown(Convert.ToSingle(reward.RenownInfluenceMoneyAmount));
                                }
                                break;
                        }
                    }
                }
            }
        }
    }
}