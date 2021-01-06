using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using System.Windows.Forms;
//using CustomSpawns.MCMv3;
using StoryMode;
using CustomSpawns.UtilityBehaviours;
using HarmonyLib;

namespace CustomSpawns
{
    public class Main : MBSubModuleBase
    {
        public static readonly string version = "v1.4.1";
        public static readonly bool isAPIMode = false;
        public static CustomSpawnsCustomSpeedModel customSpeedModel;

        private static bool removalMode = false;

        #region Taleworlds Sub Mod Callbacks

        protected override void OnSubModuleLoad()
        {
            try
            {
                Harmony harmony = new Harmony("com.Questry.CustomSpawns");
                harmony.PatchAll();
            }
            catch (Exception e)
            {
                ErrorHandler.HandleException(e, "HARMONY PATCHES");
            }


            ModIntegration.SubModManager.LoadAllValidDependentMods();
            if (ConfigLoader.Instance.Config.IsRemovalMode)
            {
                removalMode = true;
                return;
            }
            removalMode = false;
            customSpeedModel = new CustomSpawnsCustomSpeedModel();
        }

        public override void OnCampaignStart(Game game, object starterObject)
        {
            base.OnCampaignStart(game, starterObject);
            if (!(game.GameType is Campaign))
                return;
            try
            {
                InitializeGame(game, (IGameStarter)starterObject);
            }
            catch (Exception e)
            {
                ErrorHandler.HandleException(e);
            }
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);
            if (!(game.GameType is Campaign) || ((StoryMode.CampaignStoryMode)game.GameType).CampaignGameLoadingType == Campaign.GameLoadingType.NewCampaign)
                return;
            InitializeGame(game, gameStarterObject);
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot() //assure player :) also myself lol
        {
            UX.ShowMessage("CustomSpawns " + version + " is now enabled. Enjoy! :)", Color.ConvertStringToColor("#001FFFFF"));
            AI.AIManager.FlushRegisteredBehaviours(); //forget old behaviours to allocate space. 
            foreach (var subMod in ModIntegration.SubModManager.dependentModsArray)
            {
                UX.ShowMessage( subMod.SubModuleName + " is now integrated into the CustomSpawns API.", Color.ConvertStringToColor("#001FFFFF"));
            }
            //ConfigLoader.Instance.Config.GetInstance();
        }

        #endregion

        private void InitializeGame(Game game, IGameStarter gameStarterObject)
        {
            try
            {
                ClearLastInstances();
                AddBehaviours(gameStarterObject as CampaignGameStarter);
                //do overrides
                if (ConfigLoader.Instance.Config.ModifyPartySpeeds && !removalMode)
                    gameStarterObject.AddModel(customSpeedModel);
            }
            catch (Exception e)
            {
                ErrorHandler.HandleException(e);
            }
        }

        private void ClearLastInstances()
        {
            Data.DiplomacyDataManager.ClearInstance(this);
            Data.SpawnDataManager.ClearInstance(this);
            Data.NameSignifierData.ClearInstance(this);
        }

        private void AddBehaviours(CampaignGameStarter starter)
        {
            if (!removalMode)
            {

                OnSaveStartRunBehaviour.InitializeSave(starter);
                OnSaveStartRunBehaviour.Singleton.RegisterFunctionToRunOnSaveStart(OnSaveStart);

                starter.AddBehavior(new Spawn.SpawnBehaviour(Data.SpawnDataManager.Instance));
                starter.AddBehavior(new AI.HourlyPatrolAroundSpawnBehaviour());
                starter.AddBehavior(new AI.AttackClosestIfIdleForADayBehaviour());
                starter.AddBehavior(new AI.PatrolAroundClosestLestInterruptedAndSwitchBehaviour());
                starter.AddBehavior(new Diplomacy.ForcedWarPeaceBehaviour());
                starter.AddBehavior(new Diplomacy.ForceNoKingdomBehaviour());
                starter.AddBehavior(new PrisonerRecruitment.PrisonerRecruitmentBehaviour());



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

        private void OnSaveStart()
        {
            //restore lost AI behaviours!
            try
            {
                var partyIDToData = Data.SpawnDataManager.Instance.PartyIDToData;
                foreach (MobileParty mb in MobileParty.All)
                {
                    string id = CampaignUtils.IsolateMobilePartyStringID(mb);
                    if(id != "" && partyIDToData.ContainsKey(id))
                    {
                        var spawnData = partyIDToData[id];
                        Spawn.Spawner.HandleAIChecks(mb, spawnData, mb.HomeSettlement);
                    }

                }
            }catch(Exception e)
            {
                ErrorHandler.HandleException(e, " reconstruction of save custom spawns mobile party data");
            }
        }

    }
}
