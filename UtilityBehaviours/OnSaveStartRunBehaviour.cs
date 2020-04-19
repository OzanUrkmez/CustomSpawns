using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.Localization;

namespace CustomSpawns.UtilityBehaviours
{
    class OnSaveStartRunBehaviour : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, HourlyBehaviour);
        }

        public override void SyncData(IDataStore dataStore)
        {
            
        }

        private Action run;
        private bool alreadyRun = false;
        private void HourlyBehaviour()
        {
            if (alreadyRun)
                return;
            alreadyRun = true;
            run();
        }

        public void RegisterFunctionToRun(Action f)
        {
            run += f;
        }
    }
}
