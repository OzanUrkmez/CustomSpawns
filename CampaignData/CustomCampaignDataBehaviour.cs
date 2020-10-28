using CustomSpawns.UtilityBehaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.TwoDimension;

namespace CustomSpawns.CampaignData
{
    public abstract class CustomCampaignDataBehaviour<T, C>: CampaignBehaviorBase where T : CustomCampaignDataBehaviour<T,C>, new() where C: class, ICampaignDataConfig, new()
    {

        private static T _singleton;

        protected C campaignConfig;

        public static T Singleton
        {
            get
            {
                if (_singleton == null)
                {
                    _singleton = new T();
                    _singleton.OnCustomCampaignDataBehaviourInitialized();
                }

                return _singleton;

            }
            private set
            {
                _singleton = value;
            }
        }

        private bool _initialized = false;

        public CustomCampaignDataBehaviour()
        {
            OnCustomCampaignDataBehaviourInitialized();
        }

        

        private void OnCustomCampaignDataBehaviourInitialized()
        {
            if (_initialized)
                return;
            campaignConfig = CampaignDataConfigLoader.Instance.GetConfig<C>();
            _initialized = true;
            OnSaveStartRunBehaviour.Singleton.RegisterFunctionToRunOnSaveStart(OnSaveStart);
        }

        public sealed override void RegisterEvents()
        {

            OnRegisterEvents();
        }

        public sealed override void SyncData(IDataStore dataStore)
        {
            OnSyncData(dataStore);
        }

        protected abstract void OnRegisterEvents();

        protected abstract void OnSyncData(IDataStore dataStore);

        protected abstract void OnSaveStart();

    }
}
