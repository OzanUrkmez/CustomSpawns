using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Localization;

namespace CustomSpawns.Spawn
{
    public abstract class MobilePartySpawnFactory : IMobilePartySpawn
    {
        internal abstract MobileParty CreateParty(Settlement spawnedSettlement, Clan clan, PartyTemplateObject templateObject, TextObject partyName, float speed = 0f);

        public MobileParty SpawnParty(Settlement homeSettlement, TextObject partyName, Clan clan, PartyTemplateObject partyTemplate, float customPartyBaseSpeed = 0f)
        {
            MobileParty mobileParty = CreateParty(homeSettlement, clan, partyTemplate, partyName ?? clan.Name, customPartyBaseSpeed);
            mobileParty.InitializeMobilePartyAroundPosition(partyTemplate, homeSettlement.GatePosition, 10f);
            return mobileParty;
        }

    }
}