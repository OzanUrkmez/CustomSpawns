using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem.Actions;

namespace CustomSpawns.Diplomacy
{
    public static class DiplomacyUtils
    {
        public static void DeclareWar(IFaction faction, IFaction provocator)
        {
            DeclareWarAction.Apply(faction, provocator);
        }
    }
}
