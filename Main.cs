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

namespace Banditlord
{
    public class Main : MBSubModuleBase
    {

        protected override void OnSubModuleLoad()
        {
            Config config = ConfigLoader.Instance.Config;
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            if (!(game.GameType is Campaign))
                return;
            try
            {
                UX.ShowMessage("Banditlord is now enabled. Enjoy! :)", Color.ConvertStringToColor("#001FFFFF"));
                AddBehaviours(gameStarterObject as CampaignGameStarter);
            }
            catch (Exception e)
            {
                ErrorHandler.HandleException(e);
            }
            base.OnGameStart(game, gameStarterObject);
        }

        private void AddBehaviours(CampaignGameStarter starter)
        {
            starter.AddBehavior(new Spawn.DailyBanditSpawnBehaviour());
        }

    }
}
