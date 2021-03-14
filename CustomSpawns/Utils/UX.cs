using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Library;
using CustomSpawns.Data;

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

        public static void DisplayInquiry(InquiryData inqDat, bool pause)
        {
            InformationManager.ShowInquiry(inqDat, pause);
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

        public static void ShowParseSpawnInquiryMessage(InquiryData inqData, string spawnPlaceName, bool pauseInquiry)
        {
            string title = inqData.TitleText;
            string body = inqData.Text;
            string[] codesTitle = title.Split(new string[] { "[", "]" }, StringSplitOptions.None);
            string[] codesBody = body.Split(new string[] { "[", "]" }, StringSplitOptions.None);
            if (codesTitle.Length == 1 && codesBody.Length == 1)
            {
                DisplayInquiry(inqData, pauseInquiry);
                return;
            }
            for (int i = 0; i < codesTitle.Length; i++)
            {
                switch (codesTitle[i].ToLower())
                {
                    case "spawnplace":
                        codesTitle[i] = spawnPlaceName;
                        break;
                }
            }
            for (int i = 0; i < codesBody.Length; i++)
            {
                switch (codesBody[i].ToLower())
                {
                    case "spawnplace":
                        codesBody[i] = spawnPlaceName;
                        break;
                }
            }
            inqData.TitleText = string.Join("", codesTitle);
            inqData.Text = string.Join("", codesBody);
            DisplayInquiry(inqData, pauseInquiry);
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
            {"error", "#FF2300FF" },
            {"relief",  "#65BF22FF" }
        };

        public static string GetMessageColour(string flag)
        {
            if (flagToMessageColour.ContainsKey(flag))
                return flagToMessageColour[flag];

            return "";
        }

    }
}
