using System;
using System.Collections.Generic;
using System.Linq;
using CustomSpawns.CampaignData.Implementations;
using CustomSpawns.Data;
using CustomSpawns.Data.Manager;
using CustomSpawns.Exception;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using static TaleWorlds.CampaignSystem.Actions.ChangeKingdomAction.ChangeKingdomActionDetail;

namespace CustomSpawns.Diplomacy
{
    class ForcedWarPeaceBehaviour : CampaignBehaviorBase
    {
        private readonly DailyLogger _dailyLogger = new();
        private readonly IDiplomacyActionModel _customSpawnDiplomacyActionModel;
        private readonly TrackClanKingdom _clanKingdomTrackable;
        private static CustomSpawnsClanDiplomacyModel _customSpawnsClanDiplomacyModel;

        public ForcedWarPeaceBehaviour(IDiplomacyActionModel diplomacyActionModel, TrackClanKingdom clanKingdomTrackable, CustomSpawnsClanDiplomacyModel clanDiplomacyModel)
        {
            _customSpawnDiplomacyActionModel = diplomacyActionModel;
            _clanKingdomTrackable = clanKingdomTrackable;
            _customSpawnsClanDiplomacyModel = clanDiplomacyModel;
        }

        public override void RegisterEvents()
        {
            CampaignEvents.MakePeace.AddNonSerializedListener(this, OnMakePeace);
            CampaignEvents.WarDeclared.AddNonSerializedListener(this, OnWarDeclared);
            CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, s => { _clanKingdomTrackable.Clear(); _clanKingdomTrackable.Init(); });
            CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener(this, OnNewGameCreated);
            CampaignEvents.ClanChangedKingdom.AddNonSerializedListener(this, ResetDiplomacyDataWhenClanLeavesKingdom);
        }

        public override void SyncData(IDataStore dataStore) { }
        
        private void OnWarDeclared(IFaction attacker, IFaction warTarget)
        {
            SetPeaceIfPossible(attacker, warTarget);
        }

        private void OnMakePeace(IFaction attacker, IFaction warTarget)
        {
            SetWarIfPossible(attacker, warTarget);
        }

        private void OnNewGameCreated(CampaignGameStarter starter)
        {
            _clanKingdomTrackable.Clear();
            _clanKingdomTrackable.Init();
            SetWarsDescribedByDiplomacyData();
        }

        private void ResetDiplomacyDataWhenClanLeavesKingdom(Clan c, Kingdom k1, Kingdom k2, ChangeKingdomAction.ChangeKingdomActionDetail details, Boolean b)
        {
            if (details != LeaveWithRebellion && details != LeaveKingdom && details != LeaveAsMercenary)
            {
                return;
            }

            SetWarsDescribedByDiplomacyData();
        }
        
        private static IList<IFaction> GetDiplomacyFromValidClans(IDataManager<Dictionary<string,DiplomacyData>> diplomacyDataManager, ref List<string> clanIdErrors)
        {
            IList<IFaction> clanDiplomacy = new List<IFaction>();
            IDictionary<string,DiplomacyData> diplomacy = diplomacyDataManager.Data;
            foreach (KeyValuePair<string,DiplomacyData> clanData in diplomacy)
            {
                if (!Clan.All.Any(clan => clan.StringId == clanData.Key))
                {
                    clanIdErrors.Add(clanData.Key);
                }

                IFaction clan = Clan.All.First(clan1 => clan1.StringId == clanData.Key);
                clanDiplomacy.Add(clan);
            }

            return clanDiplomacy;
        }

        private void SetWarsDescribedByDiplomacyData()
        {
            List<string> clanIdErrors = new();
            IList<IFaction> customSpawnsClans =
                GetDiplomacyFromValidClans(DiplomacyDataManager.Instance, ref clanIdErrors);

            if (clanIdErrors.Count > 0)
            {
                throw new TechnicalException("Could not find " + String.Join(", ", clanIdErrors) +
                                    " clan ids after the loading of all clans into the game. " +
                                    "The consequence is that the wars for these clans could not be set.");
            }

            foreach (var customSpawnClan in customSpawnsClans)
            {
                foreach (var clan in Clan.All)
                {
                    SetWarIfPossible(customSpawnClan, clan);
                }
            }
        }

        private void SetWarIfPossible(IFaction attacker, IFaction warTarget)
        {
            IFaction enemy = warTarget;
            if (_clanKingdomTrackable.IsPartOfAKingdom(warTarget))
            {
                enemy = _clanKingdomTrackable.Kingdom(warTarget);
            }

            if (_customSpawnsClanDiplomacyModel.IsWarDeclarationPossible(attacker, enemy))
            {
                _customSpawnDiplomacyActionModel.DeclareWar(attacker, enemy);
                _dailyLogger.Info("Forcing " + attacker.Name + " and " + enemy.Name + " to make war after peace was made due to diplomacy data");
            }
        }

        private void SetPeaceIfPossible(IFaction attacker, IFaction warTarget)
        {
            IFaction enemy = warTarget;
            if (_clanKingdomTrackable.IsPartOfAKingdom(warTarget))
            {
                enemy = _clanKingdomTrackable.Kingdom(warTarget);
            }

            if (_customSpawnsClanDiplomacyModel.IsPeaceDeclarationPossible(attacker, enemy))
            {
                _customSpawnDiplomacyActionModel.MakePeace(attacker, enemy);
                _dailyLogger.Info("Forcing " + attacker.Name + " and " + enemy.Name + " to make peace after war broke out due to diplomacy data");
            }
        }
    }
}
