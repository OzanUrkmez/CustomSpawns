using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Library;


namespace Banditlord
{
    class ModDebug
    {

        public static void ShowMessage(string message)
        {
            if (!ConfigLoader.Instance.Config.IsDebugMode)
                return;
            InformationManager.DisplayMessage(new InformationMessage(message, Color.ConvertStringToColor("red")));
        }

    }
}
