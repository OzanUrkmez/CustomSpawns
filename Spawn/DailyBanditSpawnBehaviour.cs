using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.Localization;

namespace Banditlord.Spawn
{
    class DailyBanditSpawnBehaviour : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, DailyBehaviour);
            CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, HourlyBehaviour);
        }

        public override void SyncData(IDataStore dataStore)
        {

        }

        private bool spawnedToday = false;

        public void HourlyBehaviour()
        {
            if (!spawnedToday && Campaign.Current.IsNight)
            {
                SpawnBanditAtRandomHideout();
                spawnedToday = true;
            }

        }

        private void DailyBehaviour()
        {
            spawnedToday = false;
        }

        private void SpawnBanditAtRandomHideout()
        {
            try
            {
                Clan clan = CampaignUtils.GetClanWithName("Mountain Bandits");
                Spawner.SpawnBanditAtHideout(CampaignUtils.GetPreferableHideout(clan), clan, clan.DefaultPartyTemplate, new TextObject("Mountain Raiders"));
            }
            catch(Exception e)
            {
                ErrorHandler.HandleException(e);
            }
        }
    }
}
