using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.Localization;

namespace Banditlord.Data
{
    public class RegularBanditDailySpawnData
    {

        public Clan BanditClan { get; set; }
        public Clan OverridenSpawnClan { get; set; }
        public int MaximumOnMap { get; set; }

    }
}
