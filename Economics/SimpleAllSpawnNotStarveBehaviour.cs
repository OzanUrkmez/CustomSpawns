using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace CustomSpawns.Economics
{
    class SimpleAllSpawnNotStarveBehaviour: CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, DailyTick);
        }

        public override void SyncData(IDataStore dataStore)
        {

        }

        private void DailyTick()
        {
            foreach (MobileParty mobileParty in MobileParty.All)
            {
                if (mobileParty.IsBandit && mobileParty.IsPartyTradeActive)
                {
                    mobileParty.PartyTradeGold = (int)((double)mobileParty.PartyTradeGold * 0.95 + (double)(50f * (float)mobileParty.Party.MemberRoster.TotalManCount * 0.05f));
                    if (MBRandom.RandomFloat < 0.03f && mobileParty.MapEvent != null)
                    {
                        foreach (ItemObject itemObject in ItemObject.All)
                        {
                            if (itemObject.IsFood)
                            {
                                int num =  12;
                                int num2 = MBRandom.RoundRandomized((float)mobileParty.MemberRoster.TotalManCount * (1f / (float)itemObject.Value) * (float)num * MBRandom.RandomFloat * MBRandom.RandomFloat * MBRandom.RandomFloat * MBRandom.RandomFloat);
                                if (num2 > 0)
                                {
                                    mobileParty.ItemRoster.AddToCounts(itemObject, num2, true);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
