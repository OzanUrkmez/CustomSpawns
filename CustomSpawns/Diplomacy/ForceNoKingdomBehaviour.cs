using System;
using System.Collections.Generic;
using CustomSpawns.CampaignData.Implementations;
using CustomSpawns.Data;
using CustomSpawns.Data.Manager;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using static TaleWorlds.CampaignSystem.Actions.ChangeKingdomAction.ChangeKingdomActionDetail;

namespace CustomSpawns.Diplomacy
{
    class ForceNoKingdomBehaviour : CampaignBehaviorBase
    {
        private readonly DailyLogger _dailyLogger = new();
        private readonly IDataManager<Dictionary<string,DiplomacyData>> _dataManager;

        public ForceNoKingdomBehaviour(IDataManager<Dictionary<string,DiplomacyData>> dataManager)
        {
            _dataManager = dataManager;
        }

        public override void RegisterEvents()
        {
            CampaignEvents.ClanChangedKingdom.AddNonSerializedListener(this, MakeSureNoClanJoinAKingdom);
        }

        public override void SyncData(IDataStore dataStore)
        {

        }

        private void MakeSureNoClanJoinAKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail details, Boolean b)
        {
            if (clan == null || details != JoinAsMercenary && details != JoinKingdom && details != CreateKingdom)
            {
                return;
            }
            
            if (_dataManager.Data.ContainsKey(clan.StringId))
            {
                if(_dataManager.Data[clan.StringId].ForceNoKingdom && clan.Kingdom != null)
                {
                    ChangeKingdomAction.ApplyByLeaveKingdom(clan, true);
                    _dailyLogger.Info(clan.StringId + " has forcefully been removed from parent kingdom " + oldKingdom?.Name ?? "");
                }
            }
        }
    }
}
