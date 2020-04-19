using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;

namespace CustomSpawns.AI
{
    public interface IAIBehaviour
    {

        bool IsCompatible(IAIBehaviour AIBehaviour, bool secondCall = false);

    }
}
