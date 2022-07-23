using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.TwoDimension;

namespace CustomSpawns.Economics
{
    public static class PartyEconomicUtils
    {

        public static void PartyReplenishFood(MobileParty mobileParty)
        {
            if (mobileParty.MapEvent == null && mobileParty.Food < Mathf.Abs(mobileParty.FoodChange * 2))
            {
                foreach (ItemObject itemObject in Items.All)
                {
                    if (itemObject.IsFood)
                    {
                        int num = 12;
                        int num2 = MBRandom.RoundRandomized(mobileParty.MemberRoster.TotalManCount * (1f / itemObject.Value) * num * MBRandom.RandomFloat * MBRandom.RandomFloat * MBRandom.RandomFloat * MBRandom.RandomFloat);
                        if (num2 > 0)
                        {
                            mobileParty.ItemRoster.AddToCounts(itemObject, num2);
                        }
                    }
                }
            }
        }

    }
}
