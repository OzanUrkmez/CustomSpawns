using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace CustomSpawns.Spawn
{
    public class BanditPartySpawnFactory : MobilePartySpawnFactory
    {
        internal override MobileParty CreateParty(Settlement spawnedSettlement, Clan clan, PartyTemplateObject templateObject)
        {
            return BanditPartyComponent.CreateBanditParty(templateObject.StringId + "_" + 1, clan,
                spawnedSettlement.Hideout, false);
        }

        internal override MobileParty InitParty(MobileParty mobileParty, TextObject partyName, Settlement homeSettlement, Clan clan,
            float speed = 0)
        {
            if (clan.Leader != null)
            {
                mobileParty.Party.SetCustomOwner(clan.Leader);
            }
            else if (clan.Heroes.Count > 0)
            {
                mobileParty.Party.SetCustomOwner(clan.Heroes.First());
            }

            if (clan.Leader?.HomeSettlement == null)
            {
                clan.UpdateHomeSettlement(homeSettlement);
            }
            mobileParty.Party.Visuals.SetMapIconAsDirty();
            mobileParty.SetCustomName(partyName);
            mobileParty.ActualClan = clan;
            mobileParty.SetCustomHomeSettlement(homeSettlement);

            TaleWorldsCode.BanditsCampaignBehaviour.CreatePartyTrade(mobileParty);
            foreach (ItemObject itemObject in Items.All)
            {
                if (itemObject.IsFood)
                {
                    int num = TaleWorldsCode.BanditsCampaignBehaviour.IsLooterFaction(mobileParty.MapFaction) ? 8 : 16;
                    int num2 = MBRandom.RoundRandomized(mobileParty.MemberRoster.TotalManCount * (1f / itemObject.Value) * num * MBRandom.RandomFloat * MBRandom.RandomFloat * MBRandom.RandomFloat * MBRandom.RandomFloat);
                    if (num2 > 0)
                    {
                        mobileParty.ItemRoster.AddToCounts(itemObject, num2);
                    }
                }
            }
            
            return mobileParty;
        }
    }
}