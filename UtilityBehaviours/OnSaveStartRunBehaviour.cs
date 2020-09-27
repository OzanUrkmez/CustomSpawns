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
    public class OnSaveStartRunBehaviour : CampaignBehaviorBase
    {

        private static OnSaveStartRunBehaviour _singleton;

        public static OnSaveStartRunBehaviour Singleton
        {
            get
            {
                return _singleton;
            }
            private set
            {
                _singleton = value;
            }
        }

        public static void InitializeSingleton(CampaignGameStarter starter)
        {
            _singleton = new OnSaveStartRunBehaviour();
            starter.AddBehavior(_singleton);
        }

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

        public void RegisterFunctionToRunOnSaveStart(Action f)
        {
            run += f;
        }
    }
}
