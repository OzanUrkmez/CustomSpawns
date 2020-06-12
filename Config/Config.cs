using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomSpawns
{
    [Serializable]
    public class Config
    {

        public bool IsDebugMode { get; set; }
        public bool SpawnAtOneHideout { get; set; }
        public bool ModifyPartySpeeds { get; set; }
        public bool IsRemovalMode { get; set; }
        public bool IsAllSpawnMode { get; set; }
        public int UpdatePartyRedundantDataPerHour { get; set; }
    }
}
