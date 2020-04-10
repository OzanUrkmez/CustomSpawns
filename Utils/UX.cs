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
    public static class UX
    {
        public static void ShowMessage(string message, Color messageColor)
        {
            InformationManager.DisplayMessage(new InformationMessage(message, messageColor));
        }

        public static void ShowMessage(InformationMessage msg)
        {
            InformationManager.DisplayMessage(msg);
        }

        private static Dictionary<string, string> flagToMessageColour = new Dictionary<string, string>()
        {
            { "danger", "FF2300FF"}
        };

        public static string GetMessageColour(string flag)
        {
            if (flagToMessageColour.ContainsKey(flag))
                return flagToMessageColour[flag];

            return "";
        }

    }
}
