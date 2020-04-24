using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.Localization;


namespace CustomSpawns.Diplomacy
{
    class ForceNoKingdomBehaviour : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickClanEvent.AddNonSerializedListener(this, DailyMakeSureNoKingdom);
        }

        private Data.DiplomacyDataManager dataManager;

        public override void SyncData(IDataStore dataStore)
        {

        }

        private void DailyMakeSureNoKingdom(Clan c)
        {
            if (c == null || c.IsDeactivated || DiplomacyUtils.GetHardCodedExceptionClans().Contains(c.StringId))
                return;
            try
            {
                if (dataManager == null)
                {
                    GetData();
                }
                if (dataManager.Data.ContainsKey(c.StringId))
                {
                    if(dataManager.Data[c.StringId].ForceNoKingdom && c.Kingdom != null)
                    {
                        c.ClanLeaveKingdom(true);
                        ModDebug.ShowMessage(c.StringId + " has forcefully been removed from parent kingdom");
                    }
                }
            }
            catch (Exception e)
            {
                ErrorHandler.HandleException(e, "daily make sure no kingdom process");
            }
        }


        private void GetData()
        {
            dataManager = Data.DiplomacyDataManager.Instance;
        }
    }
}
