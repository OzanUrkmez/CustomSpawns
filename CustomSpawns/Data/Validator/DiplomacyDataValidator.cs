using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace CustomSpawns.Data.Validator
{
    // TODO create data validator system
    public class DiplomacyDataValidator : IValidator<Dictionary<string,DiplomacyData>>
    {
        public IList<string> ValidateData(Dictionary<string,DiplomacyData> data)
        {
            var errors = new List<string>();
            var clans = Clan.All;
            if (clans.IsEmpty())
            {
                errors.Add("Clans have not been initialised yet (clan xml file was not read yet and clans were not registered " +
                           "in the MBObjectManager). Make sure that the validator is being called after that the clan initialisation.");
                return errors;
            }

            foreach(KeyValuePair<string,DiplomacyData> clanData in data)
            {
                if (clans.Count(clan => clan.StringId.Equals(clanData.Key)) == 0)
                {
                    errors.Add("Unknown clan " + clanData.Key + ". The clan id does not match any of the loaded clan ids");
                }

                errors.AddRange(ValidateDiplomacyData(clanData.Key, clanData.Value));
            }

            return errors;
        }

        public IList<string> ValidateDiplomacyData(string clanId, DiplomacyData data)
        {
            var errors = new List<string>();
            var kingdoms = Kingdom.All;
            var clans = Clan.All;
            if (kingdoms.IsEmpty())
            {
                errors.Add("Kingdoms have not been initialised yet (kingdoms xml file was not read yet and kingdoms were not registered " +
                           "in the MBObjectManager). Make sure that the validator is being called after that the kingdom initialisation.");
                return errors;
            }

            foreach (string kingdomId in data.ForcedWarPeaceDataInstance?.ExceptionKingdoms ?? new List<string>())
            {
                if (kingdoms.Count(kingdom => kingdom.StringId.Equals(kingdomId)) == 0)
                {
                    errors.Add("For clan " + clanId + ": the kingdom id " + kingdomId + " in the kingdom exception " +
                               "list does not match any kingdom ids loaded in the game");
                }
            }
            
            foreach (string clanAtPeaceId in data.ForcedWarPeaceDataInstance?.AtPeaceWithClans ?? new List<string>())
            {
                if (clans.Count(clan => clan.StringId.Equals(clanAtPeaceId)) == 0)
                {
                    errors.Add("For clan " + clanId + ": the clan id " + clanAtPeaceId + " in the clan exception " +
                               "list does not match any clan ids loaded in the game");
                }
            }

            return errors;
        }
    }
}
