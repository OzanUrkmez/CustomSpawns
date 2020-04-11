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
        public static readonly string version = "v1.0.2";
        public static CustomSpawnsCustomSpeedModel customSpeedModel;

        protected override void OnSubModuleLoad()
        {
            Config config = ConfigLoader.Instance.Config;
            customSpeedModel = new CustomSpawnsCustomSpeedModel();
        }

        public override void OnCampaignStart(Game game, object starterObject)
        {
            base.OnCampaignStart(game, starterObject);
            try
            {
                InitializeGame(game, (IGameStarter)starterObject);
            }catch(Exception e)
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

        private void InitializeGame(Game game, IGameStarter gameStarterObject)
        {
            try
            {
                UX.ShowMessage("CustomSpawns " + version + " is now enabled. Enjoy! :)", Color.ConvertStringToColor("#001FFFFF"));
                AddBehaviours(gameStarterObject as CampaignGameStarter);
                if(ConfigLoader.Instance.Config.ModifyPartySpeeds)
                    gameStarterObject.AddModel(customSpeedModel);
            }
            catch (Exception e)
            {
                ErrorHandler.HandleException(e);
            }
        }

        private void AddBehaviours(CampaignGameStarter starter)
        {
            starter.AddBehavior(new Spawn.DailyBanditSpawnBehaviour(Data.RegularBanditDailySpawnDataManager.Instance));
        }

    }
}
