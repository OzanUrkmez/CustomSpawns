using CustomSpawns.CampaignData.Config;
using TaleWorlds.CampaignSystem;

namespace CustomSpawns.CampaignData.Implementations
{
    class CampaignTest : CustomCampaignDataBehaviour<CampaignTest, CampaignTestConfig>
    {

        public override void FlushSavedData()
        {

        }

        protected override void OnSaveStart()
        {
            if (campaignConfig.OverrideGameSpeed)
            {
                Campaign.Current.SpeedUpMultiplier = campaignConfig.OverridenGameSpeed;
            }
        }

        protected override void SyncSaveData(IDataStore dataStore)
        {

        }

        protected override void OnRegisterEvents()
        {

        }

    }
}
