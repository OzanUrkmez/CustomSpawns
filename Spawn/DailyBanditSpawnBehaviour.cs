using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;

namespace BanditInfestation.Spawn
{
    class DailyBanditSpawnBehaviour : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, SpawnBanditAtRandomSettlement);
        }

        public override void SyncData(IDataStore dataStore)
        {

        }

        private void SpawnBanditAtRandomSettlement()
        {

        }
    }
}
