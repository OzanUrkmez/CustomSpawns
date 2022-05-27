using System;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace TaleWorldsCode
{
    class BanditsCampaignBehaviour
    {
        public static Hideout SelectARandomHideout(Clan faction, bool isInfestedHideoutNeeded, bool sameFactionIsNeeded, bool selectingFurtherToOthersNeeded = false)
        {
            int num = 0;
            float num2 = Campaign.AverageDistanceBetweenTwoTowns * 0.33f * Campaign.AverageDistanceBetweenTwoTowns * 0.33f;
            foreach (Settlement settlement in Campaign.Current.Settlements)
            {
                if (settlement.IsHideout && (settlement.Culture.Equals(faction.Culture) || !sameFactionIsNeeded) && ((isInfestedHideoutNeeded && ((Hideout)settlement.GetComponent(typeof(Hideout))).IsInfested) || (!isInfestedHideoutNeeded && !((Hideout)settlement.GetComponent(typeof(Hideout))).IsInfested)))
                {
                    int num3 = 1;
                    if (selectingFurtherToOthersNeeded)
                    {
                        float num4 = Campaign.MapDiagonal * Campaign.MapDiagonal;
                        float num5 = Campaign.MapDiagonal * Campaign.MapDiagonal;
                        foreach (Settlement settlement2 in Campaign.Current.Settlements)
                        {
                            if (settlement2.IsHideout && ((Hideout)settlement2.GetComponent(typeof(Hideout))).IsInfested)
                            {
                                float num6 = settlement.Position2D.DistanceSquared(settlement2.Position2D);
                                if (settlement.Culture == settlement2.Culture && num6 < num4)
                                {
                                    num4 = num6;
                                }
                                if (num6 < num5)
                                {
                                    num5 = num6;
                                }
                            }
                        }
                        num3 = (int)Math.Max(1f, num4 / num2 + 5f * (num5 / num2));
                    }
                    num += num3;
                }
            }
            int num7 = MBRandom.RandomInt(num);
            foreach (Settlement settlement3 in Campaign.Current.Settlements)
            {
                if (settlement3.IsHideout && (settlement3.Culture.Equals(faction.Culture) || !sameFactionIsNeeded) && ((isInfestedHideoutNeeded && ((Hideout)settlement3.GetComponent(typeof(Hideout))).IsInfested) || (!isInfestedHideoutNeeded && !((Hideout)settlement3.GetComponent(typeof(Hideout))).IsInfested)))
                {
                    int num8 = 1;
                    if (selectingFurtherToOthersNeeded)
                    {
                        float num9 = Campaign.MapDiagonal * Campaign.MapDiagonal;
                        float num10 = Campaign.MapDiagonal * Campaign.MapDiagonal;
                        foreach (Settlement settlement4 in Campaign.Current.Settlements)
                        {
                            if (settlement4.IsHideout && ((Hideout)settlement4.GetComponent(typeof(Hideout))).IsInfested)
                            {
                                float num11 = settlement3.Position2D.DistanceSquared(settlement4.Position2D);
                                if (settlement3.Culture == settlement4.Culture && num11 < num9)
                                {
                                    num9 = num11;
                                }
                                if (num11 < num10)
                                {
                                    num10 = num11;
                                }
                            }
                        }
                        num8 = (int)Math.Max(1f, num9 / num2 + 5f * (num10 / num2));
                    }
                    num7 -= num8;
                    if (num7 < 0)
                    {
                        return settlement3.GetComponent(typeof(Hideout)) as Hideout;
                    }
                }
            }
            return null;
        }

        public static int CalculateDistanceScore(float distance)
        {
            int result = 2;
            if (distance < 10000f)
            {
                result = 8;
            }
            else if (distance < 40000f)
            {
                result = 6;
            }
            else if (distance < 160000f)
            {
                result = 4;
            }
            else if (distance < 420000f)
            {
                result = 3;
            }
            return result;
        }

        // Token: 0x060020F7 RID: 8439 RVA: 0x00089110 File Offset: 0x00087310
        public static void CreatePartyTrade(MobileParty banditParty)
        {
            int initialGold = (int)(10f * (float)banditParty.Party.MemberRoster.TotalManCount * (0.5f + 1f * MBRandom.RandomFloat));
            banditParty.InitializePartyTrade(initialGold);
        }

        public static bool IsLooterFaction(IFaction faction)
        {
            return !faction.Culture.CanHaveSettlement;
        }

    }
}
