using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.Localization;
using TaleWorlds.TwoDimension;
using CustomSpawns.UtilityBehaviours;

namespace CustomSpawns.PrisonerRecruitment
{

    class PrisonerRecruitmentBehaviour : CampaignBehaviorBase
    {

        public PrisonerRecruitmentBehaviour()
        {
            Config = PrisonerRecruitmentConfigLoader.Instance.Config;
            OnSaveStartRunBehaviour.Singleton.RegisterFunctionToRunOnSaveStart(OnSaveStart);
        }

        private PrisonerRecruitmentConfig Config;

        private void OnSaveStart()
        {
            //deal with corrupted settlement parties from pre-1.4.1
            foreach(Settlement s in Settlement.All)
            {
                if (!s.IsTown && !s.IsCastle)
                    continue;
                PartyBase settlementParty = null, garrisonParty = null;
                settlementParty = s.Town.Owner;
                foreach(var party in ((SettlementComponent)s.Town).Settlement.Parties)
                {
                    if (party.IsGarrison)
                        garrisonParty = party.Party;
                    if (party.Party.IsSettlement)
                        settlementParty = party.Party;
                }

                if (settlementParty == null || garrisonParty == null)
                    continue;

                List<TroopRosterElement> elements = new List<TroopRosterElement>();

                foreach (var troopRosterElement in settlementParty.MemberRoster)
                {
                    elements.Add(troopRosterElement);
                }

                foreach (var troopRosterElement in elements)
                {
                    settlementParty.MemberRoster.RemoveTroop(troopRosterElement.Character, troopRosterElement.Number);
                    garrisonParty.MemberRoster.AddToCounts(troopRosterElement.Character, troopRosterElement.Number);
                }
            }
        }

        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickPartyEvent.AddNonSerializedListener(this, DailyPrisonerRecruitmentEvent);
            CampaignEvents.DailyTickSettlementEvent.AddNonSerializedListener(this, DailyGarrisonRecruitmentEvent);
        }

        public override void SyncData(IDataStore dataStore)
        {

        }

        private void DailyPrisonerRecruitmentEvent(MobileParty mb)
        {
            try
            {
                if (Config.Enabled == false || mb.IsMainParty)
                    return;
                if (!Config.ApplyToVanilla && !Utils.Utils.IsCustomSpawnsStringID(mb.StringId))
                    return;
                if (mb.Party.MapEvent != null)
                    return;
                if (mb.IsGarrison)
                    return;
                float recruitChance = 0;
                float devalueChance = 0;
                int capTimes = 0;

                Random rand = new Random();

                recruitChance = Config.BaseRecruitChance;
                devalueChance = Config.BaseDevalueChance;
                recruitChance += Config.MercifulTraitModifier * (mb.Leader == null ? 0 : mb.Leader.GetTraitLevel(DefaultTraits.Mercy));
                devalueChance -= Config.MercifulTraitModifier * (mb.Leader == null ? 0 : mb.Leader.GetTraitLevel(DefaultTraits.Mercy));
                capTimes = (int)((float)mb.PrisonRoster.Count / ((float)mb.MemberRoster.Count * Config.PrisonerPartyPercentageCap));
                recruitChance *= (float)Math.Pow(Config.CapReverseFinalCoefficientPerCap, capTimes);

                List<CharacterObject> recruited = new List<CharacterObject>();
                List<CharacterObject> devalued = new List<CharacterObject>();
                foreach (CharacterObject c in mb.PrisonRoster.Troops)
                {
                    if (c.IsHero || c.IsPlayerCharacter)
                        continue;
                    float particularRecruitChance = recruitChance - (c.Level * Config.PrisonerLevelReverseModifierPerLevel);
                    float particularDevalueChance = devalueChance + (c.Level * Config.PrisonerLevelDevalueModifierPerLevel);
                    if (c.Culture != null && mb.Party.Culture != null && c.Culture != mb.Party.Culture)
                    {
                        particularRecruitChance -= Config.DifferentCultureReverseModifier;
                        particularDevalueChance -= Config.DifferentCultureReverseModifier;
                    }
                    particularRecruitChance = Math.Max(particularRecruitChance, Config.FinalMinimumChance);
                    particularDevalueChance = Math.Max(particularDevalueChance, Config.FinalMinimumChance);
                    int troopCount = mb.PrisonRoster.GetTroopCount(c);
                    for (int i = 0; i < troopCount; i++) {
                        if (rand.NextDouble() <= particularRecruitChance)
                        {
                            //recruit!
                            recruited.Add(c);
                        }
                        if (rand.NextDouble() <= particularDevalueChance)
                        {
                            //recruit!
                            devalued.Add(c);
                        }
                    }
                }
                recruited.ForEach((CharacterObject c) => PartyRecruitAndRemovePrisoner(mb.Party, c));
                devalued.ForEach((CharacterObject c) => PartyDevaluePrisoner(mb.Party, c));
            }
            catch (Exception e)
            {
                ErrorHandler.HandleException(e, " daily prisoner recruitment event of " + mb.StringId);
            }
        }

        private void DailyGarrisonRecruitmentEvent(Settlement s)
        {
            try
            {
                if (s.IsCastle || s.IsTown)
                {
                    Town t = s.Town;
                    if (t.IsUnderSiege)
                        return;
                    var prisoners = CampaignUtils.GetPrisonersInSettlement(t);
                    if (prisoners.Count > 0)
                    {
                        int total = Utils.Utils.GetTotalPrisonerCounts(prisoners);
                        int totalGarrison = CampaignUtils.GetGarrisonCountInSettlement(t);
                        float recruitChance = 0;
                        float devalueChance = 0;
                        int capTimes = 0;

                        Random rand = new Random();

                        recruitChance = Config.BaseRecruitChance;
                        devalueChance = Config.BaseDevalueChance;
                        recruitChance += Config.MercifulTraitModifier * (s.OwnerClan.Leader == null ? 0 : s.OwnerClan.Leader.GetTraitLevel(DefaultTraits.Mercy));
                        devalueChance -= Config.MercifulTraitModifier * (s.OwnerClan.Leader == null ? 0 : s.OwnerClan.Leader.GetTraitLevel(DefaultTraits.Mercy));
                        capTimes = (int)((float)total / ((float)totalGarrison * Config.PrisonerPartyPercentageCap));
                        recruitChance *= (float)Math.Pow(Config.CapReverseFinalCoefficientPerCap, capTimes);

                        List<PrisonerInfo> recruited = new List<PrisonerInfo>();
                        List<PrisonerInfo> devalued = new List<PrisonerInfo>();
                        foreach (var p in prisoners)
                        {
                            if (p.prisoner.IsHero || p.prisoner.IsPlayerCharacter)
                                continue;
                            float particularRecruitChance = recruitChance - (p.prisoner.Level * Config.PrisonerLevelReverseModifierPerLevel);
                            float particularDevalueChance = devalueChance + (p.prisoner.Level * Config.PrisonerLevelDevalueModifierPerLevel);
                            if (p.prisoner.Culture != null && t.Culture != null && p.prisoner.Culture != t.Culture)
                            {
                                particularRecruitChance -= Config.DifferentCultureReverseModifier;
                                particularDevalueChance -= Config.DifferentCultureReverseModifier;
                            }
                            particularRecruitChance = Math.Max(particularRecruitChance, Config.FinalMinimumChance);
                            particularDevalueChance = Math.Max(particularDevalueChance, Config.FinalMinimumChance);
                            particularRecruitChance *= Config.GarrisonFinalCoefficient;
                            particularDevalueChance *= Config.GarrisonFinalCoefficient;
                            int troopCount = p.count;

                            int recruitedCount = 0;
                            int devaluedCount = 0;
                            for (int i = 0; i < troopCount; i++)
                            {
                                if (rand.NextDouble() <= particularRecruitChance)
                                {
                                    //recruit!
                                    recruitedCount++;
                                }
                                if (rand.NextDouble() <= particularDevalueChance)
                                {
                                    //recruit!
                                    devaluedCount++;
                                }
                            }
                            if (recruitedCount > 0)
                            {
                                recruited.Add(new PrisonerInfo()
                                {
                                    count = recruitedCount,
                                    prisoner = p.prisoner,
                                    acquiringParty = t.GarrisonParty.Party,
                                    prisonerParty = p.prisonerParty
                                });
                            }
                            if (devaluedCount > 0)
                            {
                                devalued.Add(new PrisonerInfo()
                                {
                                    count = devaluedCount,
                                    prisoner = p.prisoner,
                                    acquiringParty = p.prisonerParty,
                                    prisonerParty = p.prisonerParty
                                });
                            }
                        }
                        recruited.ForEach((PrisonerInfo p) => PartyRecruitAndRemovePrisoner(p.acquiringParty, p.prisonerParty, p.prisoner, p.count));
                        //devalued.ForEach((PrisonerInfo p) => PartyDevaluePrisoner(p.acquiringParty, p.prisonerParty, p.prisoner, p.count)); IMPLEMENT!
                    }
                }
            }catch(Exception e)
            {
                ErrorHandler.HandleException(e, "daily garrison recruitment event of " + s.StringId);
            }
        }

        #region Recruiting

        private void PartyRecruitAndRemovePrisoner(PartyBase mb, CharacterObject c)
        {
            ModDebug.ShowMessage("recruiting " + c.StringId + " from prisoners of party " + mb.Id, DebugMessageType.Prisoner);
            mb.PrisonRoster.RemoveTroop(c, 1);
            mb.AddElementToMemberRoster(c, 1);
        }

        private void PartyRecruitAndRemovePrisoner(PartyBase acquiringParty, PartyBase prisonerParty, CharacterObject c)
        {
            ModDebug.ShowMessage("recruiting " + c.StringId + " from prisoners of party " + prisonerParty.Id + " to the party " + acquiringParty.Id, DebugMessageType.Prisoner);
            prisonerParty.PrisonRoster.RemoveTroop(c, 1);
            acquiringParty.AddElementToMemberRoster(c, 1);
        }


        private void PartyRecruitAndRemovePrisoner(PartyBase mb, CharacterObject c, int times)
        {
            ModDebug.ShowMessage("recruiting " + c.StringId + " from prisoners of party " + mb.Id, DebugMessageType.Prisoner);
            mb.PrisonRoster.RemoveTroop(c, times);
            mb.AddElementToMemberRoster(c, times);
        }

        private void PartyRecruitAndRemovePrisoner(PartyBase acquiringParty, PartyBase prisonerParty, CharacterObject c, int times)
        {
            ModDebug.ShowMessage("recruiting " + c.StringId + " from prisoners of party " + prisonerParty.Id + " to the party " + acquiringParty.Id, DebugMessageType.Prisoner);
            prisonerParty.PrisonRoster.RemoveTroop(c, times);
            acquiringParty.AddElementToMemberRoster(c, times);
        }

        #endregion

        #region Devaluing

        private void PartyDevaluePrisoner(PartyBase mb, CharacterObject c, int times)
        {
            for (int i = 0; i < times; i++)
            {
                PartyDevaluePrisoner(mb, c);
            }
        }

        private void PartyDevaluePrisoner(PartyBase mb, CharacterObject c)
        {
            //TODO devalue only if not recruited! there is a possibility it doesnt exist anymore!
        }

        #endregion

    }
}
