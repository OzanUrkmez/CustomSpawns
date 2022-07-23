using System.Collections.Generic;
using CustomSpawns.Exception;
using CustomSpawns.Utils;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using static TaleWorlds.CampaignSystem.Actions.ChangeKingdomAction.ChangeKingdomActionDetail;

namespace CustomSpawns.Diplomacy
{
    public class TrackClanKingdom : IClanKingdom
    {
        
        private static readonly Dictionary<string, Kingdom> ClanInKingdoms = new Dictionary<string, Kingdom>();
        
        public void Init()
        {
            foreach (var clan in Clan.All)
            {
                ClanInKingdoms.Add(clan.StringId, clan.Kingdom);
            }
        }

        public void Clear()
        {
            ClanInKingdoms.Clear();
        }
        
        [HarmonyPatch(typeof(ChangeKingdomAction), "ApplyInternal")]
        public class ChangeKingdomActionApplyByLeaveWithRebellionAgainstKingdomPatch
        {
            static bool Prefix(Clan clan, Kingdom newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail detail)
            {
                // newKingdom can be null
                if (clan != null)
                {
                    if (ClanInKingdoms.ContainsKey(clan.StringId))
                    {
                        ClanInKingdoms.Remove(clan.StringId);
                    }

                    if (detail == LeaveKingdom || detail == LeaveAsMercenary || detail == LeaveByClanDestruction)
                    {
                        ClanInKingdoms.Add(clan.StringId, null!);
                    } else if(detail == CreateKingdom || detail == JoinKingdom || detail == JoinAsMercenary || detail == LeaveWithRebellion)
                    {
                        ClanInKingdoms.Add(clan.StringId, newKingdom);
                    }
                    else
                    {
                        ErrorHandler.ShowPureErrorMessage("Unexpected change kingdom action \"" + detail + "\". " +
                                                          " for clan " + clan.StringId + "/" + clan.Name + "A new " +
                                                          "update added a new state. It needs fixing.");
                    }
                }
                return true;
            }
    
        }
        
        public bool IsPartOfAKingdom(IFaction clan)
        {
            return !clan.IsEliminated && (clan.IsKingdomFaction || clan.IsClan && ClanInKingdoms.ContainsKey(clan.StringId) && ClanInKingdoms[clan.StringId] != null);
        }

        public IFaction Kingdom(IFaction clan)
        {
            if (clan == null)
            {
                throw new TechnicalException("Cannot get the kingdom of a null clan");
            }
            if (clan.IsEliminated)
            {
                throw new FunctionalException("The faction with id + " + clan.StringId + " and name "+ clan.Name 
                                              + " has been eliminated. Therefore, the kingdom for this faction can not be retrieved");
            }
            
            if (clan.IsKingdomFaction)
            {
                return clan;
            }

            if (clan.IsClan && ClanInKingdoms.ContainsKey(clan.StringId))
            {
                return ClanInKingdoms[clan.StringId];
            }

            throw new TechnicalException("Clan " + clan.StringId + " can not be found in the tracked clans list from the TrackClanKingdomBehaviour object." 
                                         + "Normally all clans should be initialised in the tracked clans list."
                                         + " Some behaviour in the mod has deleted this clan from this list. Maybe the list was not initialised at all and is empty ?");
        }
    }
}