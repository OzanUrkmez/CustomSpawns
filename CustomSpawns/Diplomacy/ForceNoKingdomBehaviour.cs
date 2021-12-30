using System;
using System.Collections.Generic;
using CustomSpawns;
using CustomSpawns.CampaignData;
using CustomSpawns.Data;
using Data.Manager;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.Library;
using static TaleWorlds.CampaignSystem.Actions.ChangeKingdomAction.ChangeKingdomActionDetail;

namespace Diplomacy
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

        private void MakeSureNoClanJoinAKingdom(Clan c, Kingdom k1, Kingdom k2, ChangeKingdomAction.ChangeKingdomActionDetail details, Boolean b)
        {
            if (details != JoinAsMercenary && details != JoinKingdom && details != CreateKingdom)
            {
                return;
            }
            
            if (_dataManager.Data.ContainsKey(c.StringId))
            {
                if(_dataManager.Data[c.StringId].ForceNoKingdom && c.Kingdom != null)
                {
                    ChangeKingdomAction.ApplyByLeaveKingdom(c, false);
                    _dailyLogger.Info(c.StringId + " has forcefully been removed from parent kingdom " + k2.Name);
                    InformationManager.DisplayMessage(new InformationMessage("The " + c.Name + " no longer serves " + k1.Name, Colors.Gray));
                }
            }
        }
    }
}
