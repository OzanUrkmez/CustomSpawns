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
                    if (settlement == null && !secondCall) //in case the selected faction is invalid
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

        public static Settlement PickRandomSettlementOfCulture(List<CultureCode> c, List<Data.SpawnSettlementType> preferredTypes = null, List<Settlement> exceptions = null)
        {
            int num = 0;
            List<Settlement> permissible = new List<Settlement>();

            if (exceptions == null)
                exceptions = new List<Settlement>();

            if (preferredTypes != null)
            {
                foreach (Settlement s in Settlement.All)
                {
                    foreach (var type in preferredTypes)
                    {
                        if (SettlementIsOfValidType(s, type) && !exceptions.Contains(s))
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
                    if (!exceptions.Contains(s) && (s.IsTown || s.IsVillage) && (c.Contains(s.Culture.GetCultureCode())))
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
            //ModDebug.ShowMessage("Unable to find proper settlement of" + c.ToString() + " for some reason.", DebugMessageType.Spawn);
            return null;
        }

        public static Settlement PickRandomSettlementOfCulture(List<CultureCode> c, Func<Settlement, bool> exceptionPredicate, List<Data.SpawnSettlementType> preferredTypes = null)
        {
            int num = 0;
            List<Settlement> permissible = new List<Settlement>();

            if (preferredTypes != null && preferredTypes.Count != 0)
            {
                foreach (Settlement s in Settlement.All)
                {
                    foreach (var type in preferredTypes)
                    {
                        if (SettlementIsOfValidType(s, type) && !exceptionPredicate(s))
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
                    if (!exceptionPredicate(s) && (s.IsTown || s.IsVillage) && (c.Contains(s.Culture.GetCultureCode())))
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
            //ModDebug.ShowMessage("Unable to find proper settlement of" + c.ToString() + " for some reason.", DebugMessageType.Spawn);
            return null;
        }

        public static Settlement PickRandomSettlementOfKingdom(List<Kingdom> f, List<Data.SpawnSettlementType> preferredTypes = null) //instead of PicKRandomSettlementOfCulture, does not prioritize types over kingdom
        {
            int num = 0;
            List<Settlement> permissible = new List<Settlement>();
            if (preferredTypes.Count != 0)
            {
                foreach (Settlement s in Settlement.All)
                {
                    if (f.Contains(s.MapFaction))
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
            }
            if (permissible.Count == 0)
            {
                if (preferredTypes.Count != 0)
                {
                    ModDebug.ShowMessage("Spawn type checking for kingdom spawn did not find any valid settlements. Falling back to kingdom.", DebugMessageType.Spawn);
                }
                foreach (Settlement s in Settlement.All)
                {
                    if ((s.IsTown || s.IsVillage) && f.Contains(s.MapFaction))
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
            ModDebug.ShowMessage("Unable to find proper faction settlement of" + f.ToString() + " for some reason.", DebugMessageType.Spawn);
            return permissible.Count == 0 ? Settlement.All[0] : permissible[0];
        }

        public static Clan GetClanWithName(string name)
        {
            foreach (Clan c in Clan.All)
            {
                if (c.Name.ToString().ToLower() == name.ToLower())
                    return c;
            }
            return null;
        }

        public static Settlement PickRandomSettlementAmong(List<Settlement> list, List<Data.SpawnSettlementType> preferredTypes = null, Random rand = null)
        {
            if (rand == null)
                rand = new Random();
            if (list.Count == 0)
                return null;

            var preferred = new List<Settlement>();
            if (preferredTypes != null)
            {
                foreach (Settlement s in list)
                {
                    foreach (var type in preferredTypes)
                    {
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
            foreach (Settlement s in Settlement.All)
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

        public static Settlement GetClosestHabitedSettlement(MobileParty mb)
        {
            Settlement min = null;
            float minDistance = float.MaxValue;
            foreach (Settlement s in Settlement.All)
            {
                if (!(s.IsTown || s.IsCastle || s.IsVillage))
                    continue;
                float dist = mb.Position2D.Distance(s.GatePosition);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    min = s;
                }
            }
            return min;
        }

        public static Settlement GetClosestSettlement(MobileParty mb)
        {
            Settlement min = null;
            float minDistance = float.MaxValue;
            foreach (Settlement s in Settlement.All)
            {
                float dist = mb.Position2D.Distance(s.GatePosition);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    min = s;
                }
            }
            return min;
        }

        public static Settlement GetClosestSettlement(Vec2 pos)
        {
            Settlement min = null;
            float minDistance = float.MaxValue;
            foreach (Settlement s in Settlement.All)
            {
                float dist = pos.Distance(s.GatePosition);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    min = s;
                }
            }
            return min;
        }

        public static Settlement GetClosestVillage(MobileParty mb)
        {
            Settlement min = null;
            float minDistance = float.MaxValue;
            foreach (Settlement s in Settlement.All.Where<Settlement>(x => x.IsVillage))
            {
                float dist = mb.Position2D.Distance(s.GatePosition);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    min = s;
                }
            }
            return min;
        }

        public static Settlement GetClosestVillage(Vec2 pos)
        {
            Settlement min = null;
            float minDistance = float.MaxValue;
            foreach (Settlement s in Settlement.All.Where<Settlement>(x => x.IsVillage))
            {
                float dist = pos.Distance(s.GatePosition);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    min = s;
                }
            }
            return min;
        }

        public static Settlement GetClosestNonHostileCityAmong(MobileParty mb, List<Data.SpawnSettlementType> preferredTypes = null, Settlement exception = null)
        {
            List<Settlement> viableSettlements = new List<Settlement>();
            foreach (Settlement s in Settlement.All)
            {
                foreach (var type in preferredTypes)
                {
                    if (SettlementIsOfValidType(s, type))
                    {
                        viableSettlements.Add(s);
                        break;
                    }
                }
            }
            Settlement min = null;
            float minDistance = float.MaxValue;
            if (viableSettlements.Count == 0)
            {
                foreach (Settlement s in Settlement.All)
                {
                    if (s == exception)
                        continue;
                    float dist = mb.Position2D.Distance(s.GatePosition);
                    if ((FactionManager.IsNeutralWithFaction(mb.MapFaction, s.MapFaction) || FactionManager.IsAlliedWithFaction(mb.MapFaction, s.MapFaction)) && dist < minDistance)
                    {
                        minDistance = dist;
                        min = s;
                    }
                }
            }
            else
            {
                foreach (Settlement s in viableSettlements)
                {
                    if (s == exception)
                        continue;
                    float dist = mb.Position2D.Distance(s.GatePosition);
                    if ((FactionManager.IsNeutralWithFaction(mb.MapFaction, s.MapFaction) || FactionManager.IsAlliedWithFaction(mb.MapFaction, s.MapFaction)) && dist < minDistance)
                    {
                        minDistance = dist;
                        min = s;
                    }
                }
            }
            return min;
        }

        public static string IsolateMobilePartyStringID(MobileParty mobileParty)
        {
            var str = mobileParty.StringId;
            return str.Substring(0, str.LastIndexOf('_'));
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

        public static List<PrisonerInfo> GetPrisonersInSettlement(SettlementComponent sc)
        {
            List<PartyBase> list = new List<PartyBase>
            {
                sc.Owner
            };
            foreach (MobileParty mobileParty in sc.Owner.Settlement.Parties)
            {
                if (mobileParty.IsCommonAreaParty || mobileParty.IsGarrison)
                {
                    list.Add(mobileParty.Party);
                }
            }
            List<PrisonerInfo> list2 = new List<PrisonerInfo>();
            foreach (PartyBase partyBase in list)
            {
                for (int i = 0; i < partyBase.PrisonRoster.Count; i++)
                {
                    list2.Add(new PrisonerInfo()
                    {
                        prisoner = partyBase.PrisonRoster.GetCharacterAtIndex(i),
                        count = partyBase.PrisonRoster.GetTroopCount(partyBase.PrisonRoster.GetCharacterAtIndex(i)),
                        acquiringParty = null,
                        prisonerParty = partyBase
                    });
                }
            }
            return list2;
        }

        public static int GetGarrisonCountInSettlement(SettlementComponent sc)
        {
            int returned = 0;
            foreach (MobileParty mobileParty in sc.Owner.Settlement.Parties)
            {
                if (mobileParty.IsCommonAreaParty || mobileParty.IsGarrison)
                {
                    returned += mobileParty.MemberRoster.TotalManCount;
                }
            }
            return returned;
        }

    }

    public struct PrisonerInfo
    {
        public CharacterObject prisoner;
        public int count;
        public PartyBase acquiringParty;
        public PartyBase prisonerParty;
    }
}
