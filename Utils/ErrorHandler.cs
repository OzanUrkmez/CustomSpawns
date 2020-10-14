using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CustomSpawns.MCMv3;
using TaleWorlds.Library;

namespace CustomSpawns
{
    class ErrorHandler
    {

        private static Dictionary<string, int> numberOfTimesShown = new Dictionary<string, int>();

        public static void HandleException(Exception e, string during = "")
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

            if (numberOfTimesShown[errorMessage] > CsSettings.SameErrorShowUntil)
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
