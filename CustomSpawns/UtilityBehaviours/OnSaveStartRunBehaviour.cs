using System;
using TaleWorlds.CampaignSystem;

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

        public static void InitializeSave(CampaignGameStarter starter)
        {
            if(Singleton == null)
                Singleton = new OnSaveStartRunBehaviour();
            Singleton.alreadyRun = false;
            starter.AddBehavior(_singleton);
        }

        public override void RegisterEvents()
        {
            CampaignEvents.TickEvent.AddNonSerializedListener(this, TickBehaviour);
        }

        public override void SyncData(IDataStore dataStore)
        {
            
        }

        private Action run;
        private bool alreadyRun = false;
        private void TickBehaviour(float dt)
        {
            if (alreadyRun)
                return;
            alreadyRun = true;
            run();
        }

        public void RegisterFunctionToRunOnSaveStart(Action f)
        {
            run -= f;
            run += f;
        }
    }
}
