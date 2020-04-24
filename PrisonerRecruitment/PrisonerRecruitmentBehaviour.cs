using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.Localization;

namespace CustomSpawns.PrisonerRecruitment
{
    class PrisonerRecruitmentBehaviour : CampaignBehaviorBase
    {

        public PrisonerRecruitmentBehaviour()
        {
            Config = PrisonerRecruitmentConfigLoader.Instance.Config;
        }

        private PrisonerRecruitmentConfig Config;

        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, DailyPrisonerRecruitmentEvent);
        }

        public override void SyncData(IDataStore dataStore)
        {
           
        }

        private void DailyPrisonerRecruitmentEvent()
        {

        }
    }
}
