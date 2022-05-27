using System;
using TaleWorlds.CampaignSystem;

namespace CustomSpawns.UtilityBehaviours
{
    public class OnCampaignStartRunBehaviour : CampaignBehaviorBase
    {

        private static OnCampaignStartRunBehaviour _singleton;

        private bool gameIsNew = true;

        public static OnCampaignStartRunBehaviour Singleton
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
            if (Singleton == null)
                Singleton = new OnCampaignStartRunBehaviour();
            Singleton.alreadyRun = false;
            starter.AddBehavior(_singleton);
        }

        public override void RegisterEvents()
        {
            if (gameIsNew)
            {
                OnSaveStartRunBehaviour.Singleton.RegisterFunctionToRunOnSaveStart(OnRun);
            }
        }

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("gameIsNew", ref gameIsNew);
        }

        private Action run;
        private bool alreadyRun = false;

        public void RegisterFunctionToRunOnCampaignStart(Action f)
        {
            run -= f;
            run += f;
        }

        private void OnRun()
        {
            gameIsNew = false;
        }
    }
}
