using TaleWorlds.CampaignSystem;

namespace Diplomacy
{
    public class ConstantWarDiplomacyActionModel : IDiplomacyActionModel
    {
        public void DeclareWar(IFaction attacker, IFaction warTarget)
        {
            FactionManager.DeclareWar(attacker, warTarget, true);
            FactionManager.SetStanceTwoSided(attacker, warTarget, -70);
        }

        public void MakePeace(IFaction attacker, IFaction warTarget)
        {
            FactionManager.SetNeutral(attacker, warTarget);
        }

        public bool IsAtWar(IFaction attacker, IFaction warTarget)
        {
            return FactionManager.IsAtWarAgainstFaction(attacker, warTarget);
        }
    }
}
