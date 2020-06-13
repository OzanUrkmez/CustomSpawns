using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomSpawns.Data
{
    public static class DataUtils
    {

        public static void EnsureWarnIDQUalities(IList<SpawnData> data)
        {
            List<string> parsedIDs = new List<string>();
            List<string> problematicIDs = new List<string>();
            foreach(var d in data)
            {
                string id = d.PartyTemplate.StringId;
                if (parsedIDs.Contains(id))
                {
                    problematicIDs.Add(id);
                }
                parsedIDs.Add(id);
            }

            if(problematicIDs.Count != 0)
            {
                var msg = "DO NOT WORRY PLAYER, BUT MODDERS BEWARE! \n Duplicate party template IDs have been detected for different spawns. This will not lead to any crashes, but it might lead to behaviour " + 
                "that you may not have intended, especially regarding spawn numbers. Also, it is bad practice. The duplicate IDs are: \n";
                foreach(var pr in problematicIDs)
                {
                    msg += pr + "\n";
                }
                ErrorHandler.ShowPureErrorMessage(msg);
            }
        }

    }
}
