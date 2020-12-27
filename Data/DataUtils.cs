using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.TwoDimension;

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
                if (parsedIDs.Contains(id) && !problematicIDs.Contains(id))
                {
                    problematicIDs.Add(id);
                }
                parsedIDs.Add(id);
            }

            if(problematicIDs.Count != 0)
            {
                var msg = "DO NOT WORRY PLAYER, BUT MODDERS BEWARE! \n Duplicate party template IDs have been detected for different spawns. This will not lead to any crashes, but it might lead to behaviour " + 
                "that you may not have intended, especially regarding spawn numbers. Also, it is bad practice. \n In short, You should have only one party template for one spawn type. The duplicate IDs are: \n";
                foreach(var pr in problematicIDs)
                {
                    msg += pr + "\n";
                }
                ErrorHandler.ShowPureErrorMessage(msg);
            }
        }

        public static float GetCurrentDynamicSpawnCoeff(float period)
        { 

            float cur = (Campaign.Current.CampaignStartTime.ElapsedDaysUntilNow % period) * (2.9f / period);

            return Math.Max((cur * cur * cur) - 2 * (cur * cur) - (1 / 2) * cur + 1, 0);
        }

    }
}
