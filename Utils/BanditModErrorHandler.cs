using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BanditLord
{
    class BanditModErrorHandler
    {

        public static void HandleException(Exception e)
        {
            MessageBox.Show(e.Message);
        }

    }
}
