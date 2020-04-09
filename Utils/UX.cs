using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Library;

namespace CustomSpawns
{
    class UX
    {
        public static void ShowMessage(string message, Color messageColor)
        {
            InformationManager.DisplayMessage(new InformationMessage(message, messageColor));
        }

    }
}
