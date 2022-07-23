using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace CustomSpawns.Spawn
{
    public abstract class MobilePartySpawnFactory : IMobilePartySpawn
    {
        internal abstract MobileParty CreateParty(Settlement spawnedSettlement, Clan clan, PartyTemplateObject templateObject, TextObject partyName, float speed = 0f);

        public MobileParty SpawnParty(Settlement homeSettlement, TextObject partyName, Clan clan, PartyTemplateObject partyTemplate, float customPartyBaseSpeed = 0f)
        {
            MobileParty mobileParty = CreateParty(homeSettlement, clan, partyTemplate, partyName ?? clan.Name, customPartyBaseSpeed);
            
            mobileParty.InitializeMobilePartyAroundPosition(ConstructTroopRoster(partyTemplate, mobileParty.Party),
                new TroopRoster(mobileParty.Party), homeSettlement.GatePosition, 0);

            return mobileParty;
        }

        private static TroopRoster ConstructTroopRoster(PartyTemplateObject pt, PartyBase ownerParty, int troopNumberLimit = -1) //TODO implement troop number limit.
        {
            TroopRoster returned = new TroopRoster(ownerParty);
            float gameProcess = MiscHelper.GetGameProcess();
            float num = 0.25f + 0.75f * gameProcess;
            int num2 = MBRandom.RandomInt(2);
            float num3 = (num2 == 0) ? MBRandom.RandomFloat : (MBRandom.RandomFloat * MBRandom.RandomFloat * MBRandom.RandomFloat * 4f);
            float num4 = (num2 == 0) ? (num3 * 0.8f + 0.2f) : (1f + num3);
            float randomFloat = MBRandom.RandomFloat;
            for (int i = 0; i < pt.Stacks.Count; i++)
            {
                float f = (pt.Stacks.Count > 0) ? ((float)pt.Stacks[i].MinValue + num * num4 * randomFloat * (float)(pt.Stacks[i].MaxValue - pt.Stacks[i].MinValue)) : 0f;
                returned.AddToCounts(pt.Stacks[i].Character, MBRandom.RoundRandomized(f), false);
            }
            return returned;
        }
        
    }
}