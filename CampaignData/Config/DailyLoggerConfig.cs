using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomSpawns.CampaignData
{
    [Serializable]
    public class DailyLoggerConfig : ICampaignDataConfig
    {

        public bool ShowConfigDebug { get; set; }

        public float MinimumSpawnLogValue { get; set; }
    }
}
