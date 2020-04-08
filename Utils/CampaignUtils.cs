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


namespace Banditlord
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

        public static Clan GetClanWithName(string name)
        {
            foreach(Clan c in Clan.All)
            {
                if (c.Name.ToString().ToLower() == name.ToLower())
                    return c;
            }
            return null;
        }

    }
}
