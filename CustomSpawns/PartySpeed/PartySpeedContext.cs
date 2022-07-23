using System.Collections.Generic;
using CustomSpawns.Config;
using CustomSpawns.Exception;

namespace CustomSpawns.PartySpeed
{
    public class PartySpeedContext
    {
        private readonly Dictionary<string, float> _partyIdToExtraSpeed = new();
        private readonly Dictionary<string, float> _partyIdToBaseSpeed = new();
        private readonly Dictionary<string, float> _partyIdToMinimumSpeed = new();
        private readonly Dictionary<string, float> _partyIdToMaximumSpeed = new();

        public void RegisterPartyExtraBonusSpeed(string partyBaseId, float extraSpeed)
        {
            if (_partyIdToExtraSpeed.ContainsKey(partyBaseId) || !ConfigLoader.Instance.Config.ModifyPartySpeeds)
                return;
            _partyIdToExtraSpeed.Add(partyBaseId, extraSpeed);
        }

        public void RegisterPartyMinimumSpeed(string partyBaseId, float minimumSpeed)
        {
            if (_partyIdToMinimumSpeed.ContainsKey(partyBaseId) || !ConfigLoader.Instance.Config.ModifyPartySpeeds)
                return;
            _partyIdToMinimumSpeed.Add(partyBaseId, minimumSpeed);
        }

        public void RegisterPartyMaximumSpeed(string partyBaseId, float maximumSpeed)
        {
            if (_partyIdToMaximumSpeed.ContainsKey(partyBaseId) || !ConfigLoader.Instance.Config.ModifyPartySpeeds)
                return;
            _partyIdToMaximumSpeed.Add(partyBaseId, maximumSpeed);
        }

        public void RegisterPartyBaseSpeed(string partyBaseId, float maximumSpeed)
        {
            if (_partyIdToBaseSpeed.ContainsKey(partyBaseId) || !ConfigLoader.Instance.Config.ModifyPartySpeeds)
                return;
            _partyIdToBaseSpeed.Add(partyBaseId, maximumSpeed);
        }

        public bool IsPartySpeedBonusAllowedByUser() => ConfigLoader.Instance.Config.ModifyPartySpeeds;
        
        public bool IsBasePartySpeedOverriden(string partyBaseId) => _partyIdToBaseSpeed.ContainsKey(partyBaseId);
        
        public bool IsPartyEligibleForExtraSpeed(string partyBaseId) => _partyIdToExtraSpeed.ContainsKey(partyBaseId);
        
        public bool IsPartyMinimumSpeedOverriden(string partyBaseId) => _partyIdToMinimumSpeed.ContainsKey(partyBaseId);
        
        public bool IsPartyMaximumSpeedOverriden(string partyBaseId) => _partyIdToMaximumSpeed.ContainsKey(partyBaseId);
        
        public float GetBaseSpeed(string partyBaseId)
        {
            if (!_partyIdToBaseSpeed.ContainsKey(partyBaseId))
                throw new TechnicalException("Invalid partyId ! Party should have been registered beforehand.");
            _partyIdToBaseSpeed.TryGetValue(partyBaseId, out float baseSpeed);
            return baseSpeed;
        }
        
        public float GetSpeedWithExtraBonus(string partyBaseId)
        {
            if (!_partyIdToExtraSpeed.ContainsKey(partyBaseId))
                throw new TechnicalException("Invalid partyId ! Party should have been registered beforehand.");
            _partyIdToExtraSpeed.TryGetValue(partyBaseId, out float speedWithExtraBonus);
            return speedWithExtraBonus;
        }
        
        public float GetMinimumSpeed(string partyBaseId)
        {
            if (!_partyIdToMinimumSpeed.ContainsKey(partyBaseId))
                throw new TechnicalException("Invalid partyId ! Party should have been registered beforehand.");
            _partyIdToMinimumSpeed.TryGetValue(partyBaseId, out float minimumSpeed);
            return minimumSpeed;
        }
        
        public float GetMaximumSpeed(string partyBaseId)
        {
            if (!_partyIdToMaximumSpeed.ContainsKey(partyBaseId))
                throw new TechnicalException("Invalid partyId ! Party should have been registered beforehand.");
            _partyIdToMaximumSpeed.TryGetValue(partyBaseId, out float maximumSpeed);
            return maximumSpeed;
        }
    }
}