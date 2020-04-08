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


namespace Banditlord.Utils
{
    class CampaignUtils
    {

        public static Settlement ReturnPreferableHideout(Clan selectedFaction)
        {
            Settlement settlement;
            Hideout hideout = TaleWorldsCode.BanditsCampaignBehaviour.SelectARandomHideout(selectedFaction, true, true, false);
            settlement = ((hideout != null) ? hideout.Owner.Settlement : null);
            if (settlement == null)
            {
                hideout = TaleWorldsCode.BanditsCampaignBehaviour.SelectARandomHideout(selectedFaction, false, true, false);
                settlement = ((hideout != null) ? hideout.Owner.Settlement : null);
                if (settlement == null)
                {
                    hideout = TaleWorldsCode.BanditsCampaignBehaviour.SelectARandomHideout(selectedFaction, false, false, false);
                    settlement = ((hideout != null) ? hideout.Owner.Settlement : null);
                }
            }
            return settlement;
        }



    }
}
