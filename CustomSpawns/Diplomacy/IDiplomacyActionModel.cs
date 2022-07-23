using TaleWorlds.CampaignSystem;

namespace CustomSpawns.Diplomacy
{
    public interface IDiplomacyActionModel
    {
        void DeclareWar(IFaction attacker, IFaction warTarget);

        void MakePeace(IFaction attacker, IFaction warTarget);

        bool IsAtWar(IFaction attacker, IFaction warTarget);
    }
}