using System;
using TaleWorlds.Localization;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace CustomSpawns.Spawn
{
    public class Spawner
    {
        private readonly BanditPartySpawnFactory _banditPartySpawnFactory;
        private readonly CustomPartySpawnFactory _customPartySpawnFactory;

        public Spawner(BanditPartySpawnFactory banditPartySpawnFactory, CustomPartySpawnFactory customPartySpawnFactory)
        {
            _banditPartySpawnFactory = banditPartySpawnFactory;
            _customPartySpawnFactory = customPartySpawnFactory;
        }

        public MobileParty SpawnParty(Settlement spawnedSettlement, Clan clan, PartyTemplateObject templateObject, float speed, TextObject partyName = null)
        {
            try
            {

                if(templateObject == null)
                {
                    ErrorHandler.ShowPureErrorMessage(
                        "Party Template with ID " + templateObject.StringId + " possibly does not exist. It was tried to be assigned to "
                        + templateObject.StringId);
                    return null;
                }

                //get name and show message.
                TextObject name = partyName ?? clan.Name;
                ModDebug.ShowMessage("CustomSpawns: Spawning " + name + " at " + spawnedSettlement.GatePosition + " in settlement " + spawnedSettlement.Name.ToString(), DebugMessageType.Spawn);

                if (clan.IsBanditFaction)
                {
                    return _banditPartySpawnFactory.SpawnParty(spawnedSettlement, name, clan, templateObject, speed);
                }
                else
                {
                    return _customPartySpawnFactory.SpawnParty(spawnedSettlement, name, clan, templateObject, speed);
                }
            }
            catch (Exception e) {
                ErrorHandler.ShowPureErrorMessage("Possible invalid spawn data. Spawning of party terminated.");
                ErrorHandler.HandleException(e, "party spawning");
                return null;
            }

        }
    }
}
