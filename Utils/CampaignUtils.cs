using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Barterables;
using TaleWorlds.CampaignSystem.CharacterDevelopment.Managers;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.CampaignSystem;
using Helpers;


namespace CustomSpawns
{
    class CampaignUtils
    {

        public static Settlement GetPreferableHideout(Clan preferredFaction, bool secondCall = false)
        {
            Settlement settlement;
            Hideout hideout = TaleWorldsCode.BanditsCampaignBehaviour.SelectARandomHideout(preferredFaction, true, true, false);
            settlement = ((hideout != null) ? hideout.Owner.Settlement : null);
            if (settlement == null)
            {
                hideout = TaleWorldsCode.BanditsCampaignBehaviour.SelectARandomHideout(preferredFaction, false, true, false);
                settlement = ((hideout != null) ? hideout.Owner.Settlement : null);
                if (settlement == null)
                {
                    hideout = TaleWorldsCode.BanditsCampaignBehaviour.SelectARandomHideout(preferredFaction, false, false, false);
                    settlement = ((hideout != null) ? hideout.Owner.Settlement : null);
                    if(settlement == null && !secondCall) //in case the selected faction is invalid
                    {
                        List<Clan> clans = Clan.BanditFactions.ToList();
                        Random rnd = new Random();
                        Clan chosen = clans[rnd.Next(0, clans.Count)];
                        return GetPreferableHideout(chosen, true);
                    }
                }
            }
            return settlement;
        }

        public static Settlement PickRandomSettlementOfCulture(List<CultureCode> c)
        {
            int num = 0;
            List<Settlement> permissible = new List<Settlement>();
            foreach (Settlement s in Settlement.All)
            {
                if((s.IsTown || s.IsVillage) && (c.Contains(s.Culture.GetCultureCode())))
                {
                    permissible.Add(s);
                }
            }
            foreach (Settlement s in permissible)
            {
                    int num2 = TaleWorldsCode.BanditsCampaignBehaviour.CalculateDistanceScore(s.Position2D.DistanceSquared(MobileParty.MainParty.Position2D));
                    num += num2;
            }
            int num3 = MBRandom.RandomInt(num);
            foreach (Settlement s in permissible)
            {
                    int num4 = TaleWorldsCode.BanditsCampaignBehaviour.CalculateDistanceScore(s.Position2D.DistanceSquared(MobileParty.MainParty.Position2D)); //makes it more likely that the spawn will be closer to the player.
                    num3 -= num4;
                    if (num3 <= 0)
                    {
                        return s;
                    }
            }
            ModDebug.ShowMessage("Unable to find proper settlement of" + c.ToString() + " for some reason.");
            return permissible.Count == 0? Settlement.All[0]: permissible[0];
        }

        public static Clan GetClanWithName(string name)
        {
            foreach(Clan c in Clan.All)
            {
                if (c.Name.ToString().ToLower() == name.ToLower())
                    return c;
            }
            return null;
        }

        public static Settlement PickRandomSettlementAmong(List<Settlement> list)
        {
            Random rand = new Random();
            return list[rand.Next(0, list.Count)];
        }

        public static Settlement GetClosestHostileSettlement(MobileParty mb)
        {

        }

    }
}
