using TaleWorlds.CampaignSystem;

namespace CustomSpawns.Diplomacy
{
    public interface IClanKingdom
    {
        bool IsPartOfAKingdom(IFaction clan);

        IFaction Kingdom(IFaction clan);
    }
}