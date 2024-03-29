﻿using System.Collections.Generic;
using System.Windows.Forms;
using CustomSpawns.Config;
using TaleWorlds.Library;

namespace CustomSpawns.Utils
{
    class ErrorHandler
    {

        private static Dictionary<string, int> numberOfTimesShown = new Dictionary<string, int>();

        public static void HandleException(System.Exception e, string during = "")
        {
            string shown = "CustomSpawns error has occured, please report to mod developer: " + e.Message + " AT " + e.Source + "DURING " + during + " TRACE: " + e.StackTrace;

            ShowPureErrorMessage(shown);


        }

        public static void ShowPureErrorMessage(string errorMessage)
        {
            if (!numberOfTimesShown.ContainsKey(errorMessage))
            {
                numberOfTimesShown.Add(errorMessage, 1);
            }
            else
            {
                numberOfTimesShown[errorMessage]++;
            }

            if (numberOfTimesShown[errorMessage] > ConfigLoader.Instance.Config.SameErrorShowUntil)
            {
                UX.ShowMessage(errorMessage, Color.ConvertStringToColor(UX.GetMessageColour("error")));
            }
            else
            {
                MessageBox.Show(errorMessage);
            }
        }

    }
}
