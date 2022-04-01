using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace Diplomacy
{
    public class ConstantWarDiplomacyActionModel : IDiplomacyActionModel
    {
        public void DeclareWar(IFaction attacker, IFaction warTarget)
        {
            FactionManager.DeclareWar(attacker, warTarget);
            // use mutable code as all Stance methods are internal
            SetConstantWar(attacker, warTarget);
        }

        public void MakePeace(IFaction attacker, IFaction warTarget)
        {
            FactionManager.SetNeutral(attacker, warTarget);
        }

        public bool IsAtWar(IFaction attacker, IFaction warTarget)
        {
            return FactionManager.IsAtWarAgainstFaction(attacker, warTarget);
        }

        private List<StanceLink> getStances(IFaction faction, IFaction attacker, IFaction warTarget)
        {
            return faction.Stances.Where(link =>
                link.Faction1.StringId.Equals(attacker.StringId) && link.Faction2.StringId.Equals(warTarget.StringId)).ToList();
        }

        private void SetConstantWar(IFaction attacker, IFaction warTarget)
        {
            List<StanceLink> attackerLinks = getStances(attacker , attacker, warTarget);
            List<StanceLink> warTargetLinks = getStances(warTarget , attacker, warTarget);

            if (!warTargetLinks.IsEmpty())
                warTargetLinks.ElementAt(0).IsAtConstantWar = true;

            if (!attackerLinks.IsEmpty())
                attackerLinks.ElementAt(0).IsAtConstantWar = true;
        }
    }
}
