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

        public static Settlement PickRandomSettlementOfCulture(List<CultureCode> c, List<Data.SpawnSettlementType> preferredTypes = null)
        {
            int num = 0;
            List<Settlement> permissible = new List<Settlement>();
            if (preferredTypes != null)
            {
                foreach (Settlement s in Settlement.All)
                {
                    foreach (var type in preferredTypes)
                    {
                        if (SettlementIsOfValidType(s, type))
                        {
                            permissible.Add(s);
                            break;
                        }
                    }
                }
            }
            if (permissible.Count == 0)
            {
                foreach (Settlement s in Settlement.All)
                {
                    if ((s.IsTown || s.IsVillage) && (c.Contains(s.Culture.GetCultureCode())))
                    {
                        permissible.Add(s);
                    }
                }
            }
            permissible.Randomize();
            foreach (Settlement s in permissible)
            {
                    int num2 = TaleWorldsCode.BanditsCampaignBehaviour.CalculateDistanceScore(s.Position2D.DistanceSquared(MobileParty.MainParty.Position2D));
                    num += num2;
            }
            int num3 = MBRandom.RandomInt(num);
            foreach (Settlement s in permissible)
            {
                    int num4 = TaleWorldsCode.BanditsCampaignBehaviour.CalculateDistanceScore(s.Position2D.DistanceSquared(MobileParty.MainParty.Position2D)); //makes it more likely that the spawn will be further to the player.
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

        public static Settlement PickRandomSettlementAmong(List<Settlement> list, List<Data.SpawnSettlementType> preferredTypes = null, Random rand = null)
        {
            if(rand == null)
                rand = new Random();
            var preferred = new List<Settlement>();
            if (preferredTypes != null)
            {
                foreach (Settlement s in list)
                {
                    foreach (var type in preferredTypes) {
                        if (SettlementIsOfValidType(s, type))
                        {
                            preferred.Add(s);
                            break;
                        }
                    }
                }
            }
            if (preferred.Count == 0)
            {
                return list[rand.Next(0, list.Count)];
            }
            else
            {
                return preferred[rand.Next(0, preferred.Count)];
            }
        }

        public static Settlement GetClosestHostileSettlement(MobileParty mb)
        {
            Settlement min = null;
            float minDistance = float.MaxValue;
            foreach(Settlement s in Settlement.All)
            {
                float dist = mb.Position2D.Distance(s.GatePosition);
                if (FactionManager.IsAtWarAgainstFaction(mb.MapFaction, s.MapFaction) && dist < minDistance)
                {
                    minDistance = dist;
                    min = s;
                }
            }
            return min;
        }

        public static string IsolateMobilePartyStringID(MobileParty mobileParty)
        {
            return string.Join("_", Utils.Utils.TakeAllButLast<string>(mobileParty.StringId.Split('_')).ToArray<string>()); 
        }

        public static bool SettlementIsOfValidType(Settlement s, Data.SpawnSettlementType t)
        {
            switch (t)
            {
                case Data.SpawnSettlementType.Castle:
                    return s.IsCastle;
                case Data.SpawnSettlementType.Town:
                    return s.IsTown;
                case Data.SpawnSettlementType.Village:
                    return s.IsVillage;
            }
            return false;
        }

    }
}
