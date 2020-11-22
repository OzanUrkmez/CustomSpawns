using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomSpawns.CampaignData
{
    public class CampaignTestConfig : ICampaignDataConfig
    {

        public bool OverrideGameSpeed { get; set; }

        public float OverridenGameSpeed { get; set; }

        public bool ShowConfigDebug { get; set; }
    }
}
