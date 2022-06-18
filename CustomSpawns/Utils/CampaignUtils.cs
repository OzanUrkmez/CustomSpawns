using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;


namespace CustomSpawns
{
    class CampaignUtils
    {

        public static Settlement GetPreferableHideout(Clan preferredFaction, bool secondCall = false)
        {
            Settlement settlement;
            Hideout hideout = SelectARandomHideout(preferredFaction, true, true, false);
            settlement = ((hideout != null) ? hideout.Owner.Settlement : null);
            if (settlement == null)
            {
                hideout = SelectARandomHideout(preferredFaction, false, true, false);
                settlement = ((hideout != null) ? hideout.Owner.Settlement : null);
                if (settlement == null)
                {
                    hideout = SelectARandomHideout(preferredFaction, false, false, false);
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
                int num2 = CalculateBanditDistanceScore(s.Position2D.DistanceSquared(MobileParty.MainParty.Position2D));
                num += num2;
            }
            int num3 = MBRandom.RandomInt(num);
            foreach (Settlement s in permissible)
            {
                int num4 = CalculateBanditDistanceScore(s.Position2D.DistanceSquared(MobileParty.MainParty.Position2D)); //makes it more likely that the spawn will be further to the player.
                num3 -= num4;
                if (num3 <= 0)
                {
                    return s;
                }
            }
            //ModDebug.ShowMessage("Unable to find proper settlement of" + c.ToString() + " for some reason.", DebugMessageType.Spawn);
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
            if (str.Contains('_'))
            {
                return str.Substring(0, str.LastIndexOf('_'));
            }

            return str;
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
        
        private static Hideout SelectARandomHideout(Clan faction, bool isInfestedHideoutNeeded, bool sameFactionIsNeeded, bool selectingFurtherToOthersNeeded = false)
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
        
        private static int CalculateBanditDistanceScore(float distance)
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

    }

    public struct PrisonerInfo
    {
        public CharacterObject prisoner;
        public int count;
        public PartyBase acquiringParty;
        public PartyBase prisonerParty;
    }
}
