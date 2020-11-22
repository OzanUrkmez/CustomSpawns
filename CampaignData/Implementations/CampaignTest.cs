using CustomSpawns.Data;
using CustomSpawns.UtilityBehaviours;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace CustomSpawns.CampaignData
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



        public CampaignTest()
        {
            
        }

 

    }
}
