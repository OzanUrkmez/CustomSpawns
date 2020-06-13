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

        public static void ShowParseSpawnMessage(InformationMessage msg, string spawnPlaceName)
        {
            string s = msg.Information;
            string[] codes = s.Split(new string[] { "[", "]"}, StringSplitOptions.None);
            if (codes.Length == 1) {
                ShowMessage(msg);
                return;
            }
            for(int i = 0; i < codes.Length; i++)
            {
                switch (codes[i].ToLower())
                {
                    case "spawnplace":
                        codes[i] = spawnPlaceName;
                        break;
                }
            }
            msg.Information = string.Join("", codes);
            ShowMessage(msg);
        }

        public static void ShowParseDeathMessage(InformationMessage msg, string deathClosestPlaceName)
        {
            string s = msg.Information;
            string[] codes = s.Split(new string[] { "[", "]" }, StringSplitOptions.None);
            if (codes.Length == 1)
            {
                ShowMessage(msg);
                return;
            }
            for (int i = 0; i < codes.Length; i++)
            {
                switch (codes[i].ToLower())
                {
                    case "deathplace":
                        codes[i] = deathClosestPlaceName;
                        break;
                }
            }
            msg.Information = string.Join("", codes);
            ShowMessage(msg);
        }

        private static Dictionary<string, string> flagToMessageColour = new Dictionary<string, string>()
        {
            { "danger", "#FF2300FF"},
            {"relief", "#70DB22FF" }
        };

        public static string GetMessageColour(string flag)
        {
            if (flagToMessageColour.ContainsKey(flag))
                return flagToMessageColour[flag];

            return "";
        }

    }
}
