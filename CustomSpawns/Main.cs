using System;
using System.IO;
using CustomSpawns.AI;
using CustomSpawns.Data;
using CustomSpawns.Dialogues;
using CustomSpawns.HarmonyPatches;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using CustomSpawns.PartySpeed;
using CustomSpawns.PrisonerRecruitment;
using CustomSpawns.RewardSystem;
using CustomSpawns.Spawn;
//using CustomSpawns.MCMv3;
using CustomSpawns.UtilityBehaviours;
using Data.Manager;
using Diplomacy;

namespace CustomSpawns
{
    public class Main : MBSubModuleBase
    {
        public static PartySpeedContext PartySpeedContext;

        private IDiplomacyActionModel _diplomacyActionModel;
        private TrackClanKingdom _clanKingdomTrackable;
        private CustomSpawnsClanDiplomacyModel _customSpawnsClanDiplomacyModel;
        private BanditPartySpawnFactory _banditPartySpawnFactory;
        private CustomPartySpawnFactory _customPartySpawnFactory;
        private Spawner _spawner;

        private static bool removalMode = false;

        #region Taleworlds Sub Mod Callbacks

        protected override void OnSubModuleLoad()
        {
            ModIntegration.SubModManager.LoadAllValidDependentMods();
            if (ConfigLoader.Instance.Config.IsRemovalMode)
            {
                removalMode = true;
                return;
            }
            removalMode = false;

            try
            {
                // Spawn Data Init (Read from XML)
                // ClearLastInstances();
                DiplomacyDataManager.Init();
            }
            catch (Exception e)
            {
                ErrorHandler.HandleException(e, "Could not create an instance of DiplomacyDataManager");
            }

            PartySpeedContext = new PartySpeedContext();
            _diplomacyActionModel = new ConstantWarDiplomacyActionModel();
            _clanKingdomTrackable = new TrackClanKingdom();
            _customSpawnsClanDiplomacyModel = new CustomSpawnsClanDiplomacyModel(_clanKingdomTrackable, _diplomacyActionModel, DiplomacyDataManager.Instance);
            _banditPartySpawnFactory = new BanditPartySpawnFactory();
            _customPartySpawnFactory = new CustomPartySpawnFactory();
            _spawner = new Spawner(_banditPartySpawnFactory, _customPartySpawnFactory);
        }

        protected override void InitializeGameStarter(Game game, IGameStarter gameStarterObject)
        {
            if (!(gameStarterObject is CampaignGameStarter) || !(game.GameType is Campaign))
            {
                return;
            }
            
            AddBehaviours((CampaignGameStarter) gameStarterObject);
            LoadXmlFiles((CampaignGameStarter) gameStarterObject);
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot() //assure player :) also myself lol
        {
            UX.ShowMessage("CustomSpawns is now enabled. Enjoy! :)", Color.ConvertStringToColor("#001FFFFF"));
            AI.AIManager.FlushRegisteredBehaviours(); //forget old behaviours to allocate space. 
            foreach (var subMod in ModIntegration.SubModManager.dependentModsArray)
            {
                UX.ShowMessage( subMod.SubModuleName + " is now integrated into the CustomSpawns API.", Color.ConvertStringToColor("#001FFFFF"));
            }
            //ConfigLoader.Instance.Config.GetInstance();
        }

        #endregion

        private void AddBehaviours(CampaignGameStarter starter)
        {
            if (!removalMode)
            {
                OnSaveStartRunBehaviour.InitializeSave(starter);

                starter.AddBehavior(new SpawnBehaviour(_spawner));
                starter.AddBehavior(new HourlyPatrolAroundSpawnBehaviour());
                starter.AddBehavior(new AttackClosestIfIdleForADayBehaviour());
                starter.AddBehavior(new PatrolAroundClosestLestInterruptedAndSwitchBehaviour());
                starter.AddBehavior(new PrisonerRecruitmentBehaviour());
                starter.AddBehavior(new CustomSpawnsDialogueBehavior());
                starter.AddBehavior(new SpawnRewardBehavior());
                starter.AddBehavior(new ForcedWarPeaceBehaviour(_diplomacyActionModel, _clanKingdomTrackable, _customSpawnsClanDiplomacyModel));
                starter.AddBehavior(new ForceNoKingdomBehaviour(DiplomacyDataManager.Instance));

                //campaign behaviours
                starter.AddBehavior(CampaignData.DevestationMetricData.Singleton);
                starter.AddBehavior(CampaignData.DailyLogger.Singleton);
                starter.AddBehavior(CampaignData.CampaignTest.Singleton);

                //these come last! assuming those that are added last are also run last.
                starter.AddBehavior(MobilePartyTrackingBehaviour.Singleton);
            }
            else
            {
                starter.AddBehavior(new Utils.RemoverBehaviour());
            }
        }
        
        private void LoadXmlFiles(CampaignGameStarter starter)
        {
#if !API_MODE
            starter.LoadGameTexts(Path.Combine(BasePath.Name, "Modules", "CustomSpawns", "ModuleData", "CraftingTemplateNames.xml"));
#endif
        }



        public override void OnGameInitializationFinished(Game game) {
            base.OnGameInitializationFinished(game);
            if(!(game.GameType is Campaign))
            {
                return;
            }

            PatchManager.ApplyPatches();
            
            try
            {
                // TODO Check how to handle the ClearLastInstances
                
                // Spawn Data Init (Read from XML)
                // ClearLastInstances();
                SpawnDataManager.ClearInstance(this);
                NameSignifierData.ClearInstance(this);
                DynamicSpawnData.ClearInstance(this);
                SpawnDataManager.Init();
                DynamicSpawnData.Init();
            }
            catch (Exception e)
            {
                ErrorHandler.HandleException(e, "Could not create an instance of SpawnDataManager. Might have encountered an " +
                                                "issue while parsing the XML file or invalid parameters/values have been found");
            }
        }

    }
}
