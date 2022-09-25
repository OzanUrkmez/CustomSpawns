using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;


namespace CustomSpawns.Utils
{
    public class CampaignUtils
    {
        public static Settlement PickRandomSettlementOfCulture(List<CultureCode> c, Func<Settlement, bool> exceptionPredicate, List<Data.SpawnSettlementType> preferredTypes = null)
        {
            int num = 0;
            List<Settlement> permissible = new();

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

        public static Settlement? GetNearestSettlement(List<Settlement>? settlements,
            List<IMapPoint>? mapPositions)
        {
            if (settlements == null || settlements.IsEmpty() || mapPositions == null)
            {
                return null;
            }

            if (mapPositions.IsEmpty())
            {
                return settlements[0]; 
            }

            Settlement? settlement = mapPositions
                .Select(mapPosition => GetNearestSettlement(mapPosition, settlements))
                .GroupBy(h => h)
                .Select(group => new
                {
                    Settlement = group.Key,
                    Count = group.Count()
                })
                .Aggregate((leftSettlement, rightSettlement) => leftSettlement.Count >= rightSettlement.Count ? leftSettlement : rightSettlement)?
                .Settlement;

            if (settlement == null)
            {
                return settlements[0];
            }

            return settlement;
        }
        
        private static Settlement GetNearestSettlement(IMapPoint party, List<Settlement> settlements)
        {
            return settlements.Select(settlement => new Tuple<Settlement, float>(settlement, party.Position2D.DistanceSquared(settlement.GatePosition)))
                .Aggregate((closestHideout, hideout) => closestHideout.Item2 < hideout.Item2 ? closestHideout : hideout)
                .Item1;
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
}
