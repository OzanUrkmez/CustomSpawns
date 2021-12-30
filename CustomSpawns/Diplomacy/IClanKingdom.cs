using TaleWorlds.CampaignSystem;

namespace Diplomacy
{
    public interface IClanKingdom
    {
        bool IsPartOfAKingdom(IFaction clan);

        IFaction Kingdom(IFaction clan);
    }
}