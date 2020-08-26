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

        public static void ApplyExtremeHatred(IFaction f, IFaction f2)
        {
            FactionManager.SetStanceTwoSided(f, f2, -70);
        }

        public static void SetNeutral(IFaction f1, IFaction f2)
        {
            MakePeaceAction.Apply(f1, f2);
            FactionManager.SetStanceTwoSided(f1, f2, 0);
        }

        public static string[] GetHardCodedExceptionClans()
        {
            return new string[]
            {
                "test_clan",
                "neutral"
            };
        }
    }
}
