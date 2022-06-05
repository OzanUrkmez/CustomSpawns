using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Localization;

namespace CustomSpawns.Spawn
{
    public interface IMobilePartySpawn
    {
        MobileParty SpawnParty(Settlement homeSettlement, TextObject partyName, Clan clan, PartyTemplateObject partyTemplate, float partySpeed);
    }
}