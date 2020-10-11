using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomSpawns.CampaignData
{
    [Serializable]
    public class DevestationMetricConfig: ICampaignDataConfig
    {
        public float MaxDevestationPerSettlement { get; set; }
        public float MinDevestationPerSettlement { get; set; }
        public float DailyDevestationDecay { get; set; }



        public float DevestationPerTimeLooted { get; set; }

        public float HostilePresencePerPowerDaily { get; set; }

        public float FightOccuredDevestationPerPower { get; set; }

        public float FriendlyPresenceDecayPerPowerDaily { get; set; }

        public float TradePerGoldValueDevestationDecay { get; set; }


    }
}
