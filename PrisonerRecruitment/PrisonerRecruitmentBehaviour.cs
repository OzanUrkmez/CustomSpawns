using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.Localization;
using TaleWorlds.TwoDimension;

namespace CustomSpawns.PrisonerRecruitment
{
    class PrisonerRecruitmentBehaviour : CampaignBehaviorBase
    {

        public PrisonerRecruitmentBehaviour()
        {
            Config = PrisonerRecruitmentConfigLoader.Instance.Config;
        }

        private PrisonerRecruitmentConfig Config;

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
                    if (c.Culture != null && mb.Party.Culture != null && c.Culture != mb.Party.Culture)
                    {
                        recruitChance -= Config.DifferentCultureReverseModifier;
                        devalueChance -= Config.DifferentCultureReverseModifier;
                    }
                    float particularRecruitChance = recruitChance - (c.Level * Config.PrisonerLevelReverseModifierPerLevel);
                    float particularDevalueChance = devalueChance + (c.Level * Config.PrisonerLevelDevalueModifierPerLevel);
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
                recruited.ForEach((CharacterObject c) => PartyRecruitAndRemovePrisoner(mb, c));
                devalued.ForEach((CharacterObject c) => PartyDevaluePrisoner(mb, c));
            }
            catch (Exception e)
            {
                ErrorHandler.HandleException(e, " daily prisoner recruitment event of " + mb.StringId);
            }
        }

        private void DailyGarrisonRecruitmentEvent(Settlement s)
        {
            if (s.IsCastle || s.IsTown)
            {
                Town t = s.Town;
                var prisoners = CampaignUtils.GetPrisonersInSettlement(t);
                if(prisoners.Count > 0)
                {
                   
                }
            }
        }

        private void PartyRecruitAndRemovePrisoner(MobileParty mb, CharacterObject c)
        {
            if (Config.PrisonRecruitmentDebugEnabled)
            {
                ModDebug.ShowMessage("recruiting " + c.StringId + " from prisoners of party " + mb.StringId);
            }
            mb.PrisonRoster.RemoveTroop(c, 1);
            mb.AddElementToMemberRoster(c, 1);
        }

        private void PartyDevaluePrisoner(MobileParty mb, CharacterObject c)
        {

        }
    }
}
