using System.Collections.Generic;
using CustomSpawns.Data;
using Data.Manager;
using TaleWorlds.CampaignSystem;

namespace Diplomacy
{
    public class CustomSpawnsClanDiplomacyModel
    {
        private const string PlayerFactionId = "player_faction";
        private readonly IClanKingdom _clanKingdom;
        private readonly IDictionary<string, DiplomacyData> _customSpawnsClanData;
        private readonly IDiplomacyActionModel _diplomacyModel; 

        public CustomSpawnsClanDiplomacyModel(IClanKingdom clanKingdom, IDiplomacyActionModel diplomacyModel, IDataManager<Dictionary<string,DiplomacyData>> diplomacyDataManager)
        {
            _clanKingdom = clanKingdom;
            _customSpawnsClanData = diplomacyDataManager.Data;
            _diplomacyModel = diplomacyModel;
        }
        
        /// <summary>
        /// Checks if war is possible between two factions.
        /// A faction can be a clan or a kingdom.
        /// </summary>
        /// <para>If both factions are non custom spawns faction then war is never possible.</para>
        /// <para>If one of the factions is a custom spawn factions (for instance the attacker), then war will be possible</para>
        /// <para>if one of the following conditions is met:</para>
        /// <para>1. The attacker forced war behaviour is not specified</para>
        /// <para>2. The attacker forced war behaviour is specified and</para>
        /// <para>2.1 The attacker is a clan and is not part of a kingdom and</para>
        /// <para>2.1.1 warTarget is a kingdom and is not in the kingdom exception list</para>
        /// <para>2.1.2 warTarget is a clan in a kingdom which is not in the kingdom exception list</para>
        /// <para>2.1.3 warTarget is a clan and is not in the clan exception list</para>
        /// <para>2.2 The attacker is a clan in a kingdom and</para>
        /// <para>2.2.1 warTarget is a clan in a different kingdom which is not in the kingdom exception list</para>
        /// <para>2.2.2 warTarget is a clan not in a kingdom and is not in the clan exception list</para>
        /// <br/>
        /// <para>If both factions are custom spawn factions, then both factions need to at least meet a condition as attacker as
        /// described above.</para>
        /// <br/>
        /// <para>War is not possible if one of the factions is eliminated or if the faction is "test_clan" or "neutral"
        /// or if both factions are already at war.</para>
        /// <br/>
        /// <param name="attacker">Clan or kingdom faction</param>
        /// <param name="warTarget">Clan or kingdom faction</param>
        /// <returns>true if war is possible between both factions or not</returns>
        ///
        public bool IsWarDeclarationPossible(IFaction attacker, IFaction warTarget)
        {
            if (attacker == null || warTarget == null)
            {
                return false;
            }

            var hasAttackerForcedWarPeaceBehaviour = attacker != null && _customSpawnsClanData.ContainsKey(attacker.StringId) &&
                                                     _customSpawnsClanData[attacker.StringId].ForcedWarPeaceDataInstance != null;
            var hasWarTargetForcedWarPeaceBehaviour = warTarget != null && _customSpawnsClanData.ContainsKey(warTarget.StringId) &&
                                                      _customSpawnsClanData[warTarget.StringId].ForcedWarPeaceDataInstance != null;

            if (IsPlayerActionAgainstCustomSpawnFaction(attacker, warTarget))
            {
                return false;
            }

            if (hasAttackerForcedWarPeaceBehaviour && hasWarTargetForcedWarPeaceBehaviour)
            {
                return IsCustomSpawnClanWarDeclarationPossible(attacker, warTarget)
                       && IsCustomSpawnClanWarDeclarationPossible(warTarget, attacker);
            }

            if (hasAttackerForcedWarPeaceBehaviour)
            {
                return IsCustomSpawnClanWarDeclarationPossible(attacker, warTarget);
            }
            
            if (hasWarTargetForcedWarPeaceBehaviour)
            {
                return IsCustomSpawnClanWarDeclarationPossible(warTarget, attacker);
            }
            
            if (IsPlayerActionAgainstCustomSpawnFaction(attacker, warTarget))
            {
                return false;
            }

            return false;
        }
        
        private bool IsCustomSpawnClanWarDeclarationPossible(IFaction customFaction, IFaction warTarget)
        {
            if (customFaction == null || warTarget == null || customFaction == warTarget || customFaction.IsEliminated || warTarget.IsEliminated ||
                customFaction.IsBanditFaction || warTarget.IsBanditFaction || GetHardCodedExceptionClans().Contains(customFaction.StringId) || GetHardCodedExceptionClans().Contains(warTarget.StringId) || _diplomacyModel.IsAtWar(customFaction, warTarget))
            {
                return false;
            }
            
            if (!_customSpawnsClanData.ContainsKey(customFaction.StringId) || 
                _customSpawnsClanData[customFaction.StringId].ForcedWarPeaceDataInstance == null)
            {
                return false;
            }

            var forcedWarPeaceInstance = _customSpawnsClanData[customFaction.StringId].ForcedWarPeaceDataInstance;

            if (customFaction.IsClan && !_clanKingdom.IsPartOfAKingdom(customFaction))
            {
                if(warTarget.IsKingdomFaction
                   && !forcedWarPeaceInstance.ExceptionKingdoms.Contains(warTarget.StringId)
                   || (warTarget.IsClan
                       && _clanKingdom.IsPartOfAKingdom(warTarget)
                       && !forcedWarPeaceInstance.ExceptionKingdoms.Contains(_clanKingdom.Kingdom(warTarget).StringId))
                   || (warTarget.IsClan
                       && !forcedWarPeaceInstance.AtPeaceWithClans.Contains(warTarget.StringId))
                       && !_clanKingdom.IsPartOfAKingdom(warTarget))
                {
                    return true;
                }   
            } else if (customFaction.IsClan
                       && _clanKingdom.IsPartOfAKingdom(customFaction)
                       && warTarget.IsClan
                       && _clanKingdom.IsPartOfAKingdom(warTarget)
                       && !_clanKingdom.Kingdom(warTarget).StringId.Equals(_clanKingdom.Kingdom(customFaction).StringId)
                       && !forcedWarPeaceInstance.ExceptionKingdoms.Contains(_clanKingdom.Kingdom(warTarget).StringId)
                   || (customFaction.IsClan
                       && _clanKingdom.IsPartOfAKingdom(customFaction)
                       && warTarget.IsClan
                       && !_clanKingdom.IsPartOfAKingdom(warTarget)
                       && !forcedWarPeaceInstance.AtPeaceWithClans.Contains(warTarget.StringId)))
            {
                return true;
            } else if (customFaction.IsKingdomFaction)
            {
                // Custom Spawn Kingdoms mechanic is not supported yet. Does it even make sense ?
                return true;
            } 

            return false;
        }

        /// <summary>
        /// Checks if peace is possible between two factions.
        /// A faction can be a clan or a kingdom.
        /// </summary>
        /// <para>If both factions are non custom spawns factions then peace is never possible.</para>
        /// <para>If one of the factions is a custom spawn faction (for instance the attacker), then peace will be possible</para>
        /// <para>if one of the following conditions is met:</para>
        /// <para>1. The attacker forced war behaviour is not specified</para>
        /// <para>2. The attacker forced war behaviour is specified and</para>
        /// <para>2.1 The attacker is a clan and is not part of a kingdom and</para>
        /// <para>2.1.1 peaceTarget is a kingdom and is in the kingdom exception list</para>
        /// <para>2.1.2 peaceTarget is a clan in a kingdom which is in the kingdom exception list</para>
        /// <para>2.1.3 peaceTarget is a clan and is in the clan exception list</para>
        /// <para>2.2 The attacker is a clan in a kingdom and</para>
        /// <para>2.2.1 peaceTarget is a clan in a different kingdom which is in the kingdom exception list</para>
        /// <para>2.2.2 peaceTarget is a clan not in a kingdom and is in the clan exception list</para>
        /// <br/>
        /// <para>If both factions are custom spawn factions, then both factions need to at least meet a condition as attacker as
        /// described above.</para>
        /// <br/>
        /// <para>Peace is not possible if one of the factions is eliminated or if the faction is "test_clan" or "neutral"
        /// or if both factions are already at peace</para>
        /// <br/>
        /// <param name="attacker">Clan or kingdom faction</param>
        /// <param name="peaceTarget">Clan or kingdom faction</param>
        /// <returns>true if peace is possible between both factions or not</returns>
        public bool IsPeaceDeclarationPossible(IFaction attacker, IFaction warTarget)
        {
            if (attacker == null || warTarget == null)
            {
                return false;
            }

            var hasAttackerForcedWarPeaceBehaviour = attacker != null && _customSpawnsClanData.ContainsKey(attacker.StringId) &&
                                                     _customSpawnsClanData[attacker.StringId].ForcedWarPeaceDataInstance != null;
            var hasWarTargetForcedWarPeaceBehaviour = warTarget != null && _customSpawnsClanData.ContainsKey(warTarget.StringId) &&
                                                      _customSpawnsClanData[warTarget.StringId].ForcedWarPeaceDataInstance != null;

            if (IsPlayerActionAgainstCustomSpawnFaction(attacker, warTarget))
            {
                return false;
            }

            if (hasAttackerForcedWarPeaceBehaviour && hasWarTargetForcedWarPeaceBehaviour)
            {
                return IsCustomSpawnsClanPeaceDeclarationPossible(attacker, warTarget)
                    && IsCustomSpawnsClanPeaceDeclarationPossible(warTarget, attacker);
            }

            if (hasAttackerForcedWarPeaceBehaviour)
            {
                return IsCustomSpawnsClanPeaceDeclarationPossible(attacker, warTarget);
            }
            
            if (hasWarTargetForcedWarPeaceBehaviour)
            {
                return IsCustomSpawnsClanPeaceDeclarationPossible(warTarget, attacker);
            }

            return false;
        }

        private bool IsPlayerActionAgainstCustomSpawnFaction(IFaction attacker, IFaction warTarget)
        {
            return _customSpawnsClanData.ContainsKey(warTarget.StringId)
                && !_clanKingdom.IsPartOfAKingdom(warTarget)
                && (attacker.StringId.Equals(PlayerFactionId) 
                || attacker.IsKingdomFaction && attacker.Leader == Hero.MainHero);
        }
        
        private bool IsCustomSpawnsClanPeaceDeclarationPossible(IFaction attacker, IFaction peaceTarget)
        {
            if (attacker == null || peaceTarget == null || attacker == peaceTarget || attacker.IsEliminated || peaceTarget.IsEliminated ||
                attacker.IsBanditFaction || peaceTarget.IsBanditFaction || GetHardCodedExceptionClans().Contains(attacker.StringId) || GetHardCodedExceptionClans().Contains(peaceTarget.StringId) ||
                !_diplomacyModel.IsAtWar(attacker, peaceTarget))
            {
                return false;
            }
            
            if (!_customSpawnsClanData.ContainsKey(attacker.StringId) || 
                _customSpawnsClanData[attacker.StringId].ForcedWarPeaceDataInstance == null)
            {
                return false;
            } 
            
            var forcedWarPeaceInstance = _customSpawnsClanData[attacker.StringId].ForcedWarPeaceDataInstance;

            if (attacker.IsClan && !_clanKingdom.IsPartOfAKingdom(attacker))
            {
                if(peaceTarget.IsKingdomFaction
                   && forcedWarPeaceInstance.ExceptionKingdoms.Contains(peaceTarget.StringId)
                   || (peaceTarget.IsClan
                       && _clanKingdom.IsPartOfAKingdom(peaceTarget)
                       && forcedWarPeaceInstance.ExceptionKingdoms.Contains(_clanKingdom.Kingdom(peaceTarget).StringId))
                   || (peaceTarget.IsClan
                       && forcedWarPeaceInstance.AtPeaceWithClans.Contains(peaceTarget.StringId)
                       && !_clanKingdom.IsPartOfAKingdom(peaceTarget)))
                {
                    return true;
                }   
            } else if (attacker.IsClan
                       && _clanKingdom.IsPartOfAKingdom(attacker)
                       && peaceTarget.IsClan
                       && _clanKingdom.IsPartOfAKingdom(peaceTarget)
                       && (_clanKingdom.Kingdom(peaceTarget).StringId.Equals(_clanKingdom.Kingdom(attacker).StringId)
                       || !_clanKingdom.Kingdom(peaceTarget).StringId.Equals(_clanKingdom.Kingdom(attacker).StringId)
                       && forcedWarPeaceInstance.ExceptionKingdoms.Contains(_clanKingdom.Kingdom(peaceTarget).StringId))
                   || (attacker.IsClan
                       && _clanKingdom.IsPartOfAKingdom(attacker)
                       && peaceTarget.IsClan
                       && !_clanKingdom.IsPartOfAKingdom(peaceTarget)
                       && forcedWarPeaceInstance.AtPeaceWithClans.Contains(peaceTarget.StringId)))
            {
                return true;
            } else if (attacker.IsKingdomFaction)
            {
                // Custom Spawn Kingdoms mechanic is not supported yet. Does it even make sense ?
                return true;
            } 

            return false;
        }

        private static IList<string> GetHardCodedExceptionClans()
        {
            return new[]
            {
                "test_clan", "neutral"
            };
        }
    }
}