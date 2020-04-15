using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CustomSpawns
{
    class ErrorHandler
    {

        public static void HandleException(Exception e, string during = "")
        {
            MessageBox.Show("CustomSpawns error has occured, please report to mod developer: " + e.Message + " AT " + e.Source + "DURING " + during);
        }

    }
}
