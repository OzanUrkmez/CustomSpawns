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

namespace Banditlord
{
    public class Main : MBSubModuleBase
    {
        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);
            if (!(game.GameType is Campaign))
                return;
            try
            {
                AddBehaviours(gameStarterObject as CampaignGameStarter);
                UX.ShowMessage("Banditlord is now enabled. Enjoy! :)", Color.ConvertStringToColor("Green"));
            }
            catch (Exception e)
            {
                ErrorHandler.HandleException(e);
            }
        }

        private void AddBehaviours(CampaignGameStarter starter)
        {
            starter.AddBehavior(new Spawn.DailyBanditSpawnBehaviour());
        }

    }
}
