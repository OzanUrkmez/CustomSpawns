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
using StoryMode;

namespace CustomSpawns
{
    public class Main : MBSubModuleBase
    {
        public static readonly string version = "v1.0.6";
        public static readonly bool isAPIMode = false;
        public static CustomSpawnsCustomSpeedModel customSpeedModel;

        private static bool removalMode = false;

        protected override void OnSubModuleLoad()
        {
            Config config = ConfigLoader.Instance.Config;
            ModIntegration.SubModManager.LoadAllValidDependentMods();
            if (config.IsRemovalMode)
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
            foreach(var subMod in ModIntegration.SubModManager.dependentModsArray)
            {
                UX.ShowMessage( subMod.SubModuleName + " is now integrated into the CustomSpawns API.", Color.ConvertStringToColor("#001FFFFF"));
            }
        }

        private void InitializeGame(Game game, IGameStarter gameStarterObject)
        {
            try
            {
                AddBehaviours(gameStarterObject as CampaignGameStarter);
                if (ConfigLoader.Instance.Config.ModifyPartySpeeds && !removalMode)
                    gameStarterObject.AddModel(customSpeedModel);
            }
            catch (Exception e)
            {
                ErrorHandler.HandleException(e);
            }
        }

        private void AddBehaviours(CampaignGameStarter starter)
        {
            if (!removalMode)
            {
                starter.AddBehavior(new Spawn.DailyBanditSpawnBehaviour(Data.RegularBanditDailySpawnDataManager.Instance));
                starter.AddBehavior(new AI.HourlyPatrolAroundSpawnBehaviour());
            }
            else
            {
                starter.AddBehavior(new Utils.RemoverBehaviour());
            }
        }

    }
}
