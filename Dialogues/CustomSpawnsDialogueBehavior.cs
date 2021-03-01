using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Barterables;
using TaleWorlds.SaveSystem;

namespace CustomSpawns.Dialogues
{
    //TODO Improve upon delegate logic. add more options. 
    public class CustomSpawnsDialogueBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.AddCustomDialogues));
            DialogueManager.CustomSpawnsDialogueBehavior = this;
        }

        private Data.DialogueDataManager dataManager;

        Dictionary<MobileParty, string> dialoguePartyRef = new Dictionary<MobileParty, string>();

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData<Dictionary<MobileParty, string>>("dialoguePartyRef", ref dialoguePartyRef);
        }

        // basic dialogue overview: 
        // ids are used to differentiate similar dialogues (must be unique)
        // tokens are like impulses, so an out token leads into an in token. 'magic tokens' are tokens which have special functions, they are start (in token) and close_window (out token)
        // conditions are delegates that must anonymously return a bool, which will be wheteher or not the line is displayed
        // consequences are void delegates, just pieces of code run after a line has been selected
        // i think? priority determines which line is shown if multiple lines meet the requirements, and also maybe what order player lines are displayed in (speculation)

        public void AddCustomDialogues(CampaignGameStarter starter)
        {
            if (dataManager == null)
            {
                GetData();
            }
            foreach (Data.DialogueData d in dataManager.Data) // handle the dialogues
            {
                if (d.IsPlayer)
                {
                    starter.AddPlayerLine(d.Id, d.InToken, d.OutToken, d.DialogueText, // delegating in the loop so we don't interfere with the asynchronous magic
                    delegate
                    {
                        if(PlayerEncounter.InsideSettlement)
                        {
                            //TODO is this supposed to return false?
                        }
                        return this.EvaluateDialogueCondition(d);
                    },
                    delegate
                    {
                        this.EvaluateDialogueConsequence(d);
                    });
                }
                else
                {
                    starter.AddDialogLine(d.Id, d.InToken, d.OutToken, d.DialogueText, // etc etc, reusing code is bad but there's no other way ¯\_(ツ)_/¯
                    delegate
                    {
                        return this.EvaluateDialogueCondition(d);
                    },
                    delegate
                    {
                        this.EvaluateDialogueConsequence(d);
                    });
                }
            }
        }

        private bool EvaluateDialogueCondition(Data.DialogueData d)
        {
            CustomSpawnsDialogueParams p = d.Parameters;

            if(d.Condition == CSDialogueCondition.None)
            {
                return true;
            }

            PartyBase encounteredParty = PlayerEncounter.EncounteredParty;

            //party template conditions:
            if (dialoguePartyRef.ContainsKey(encounteredParty.MobileParty) &&
                (dialoguePartyRef[encounteredParty.MobileParty] == p.c_partyTemplate))
            {
                switch (d.Condition)
                {
                    case CSDialogueCondition.PartyTemplateAndStance:
                        return this.party_template_condition_delegate(encounteredParty, p.c_isFriendly);
                    case CSDialogueCondition.PartyTemplateAttackerAndStance:
                        return this.party_template_and_attacker_hostile_condition_delegate(encounteredParty, p.c_isFriendly);
                    case CSDialogueCondition.PartyTemplateDefenderAndStance:
                        return this.party_template_and_defender_hostile_condition_delegate(encounteredParty, p.c_isFriendly);
                }
            }

            //other conditions

            switch (d.Condition)
            {
                case CSDialogueCondition.GenericWar:
                    return this.generic_war_condition_delegate(p.c_isFriendly);
                case CSDialogueCondition.CharacterTrait:
                    return this.hero_trait_condition_delegate(p.c_isPlayerTrait, p.c_traitToCheck, p.c_value);
                case CSDialogueCondition.LastBarter:
                    return this.barter_check_condition_delegate(p.c_barterSuccessful);
                case CSDialogueCondition.FirstConversationLordName:
                    return this.first_and_lord_name_condition_delegate(p.c_lordName); // these two untested
                case CSDialogueCondition.FirstConversationFaction:
                    return this.first_and_faction_condition_delegate(p.c_faction); // ^
                default:
                    return false;
            }
        }

        private void EvaluateDialogueConsequence(Data.DialogueData d)
        {
            switch (d.Consequence)
            {
                case CSDialogueConsequence.EndConversation:
                    this.end_conversation_consequence_delegate();
                    break;
                case CSDialogueConsequence.EndConversationInBattle:
                    this.end_conversation_battle_consequence_delegate();
                    break;
                case CSDialogueConsequence.DeclareWar:
                    this.declare_war_consequence_delegate();
                    break;
                case CSDialogueConsequence.DeclarePeace:
                    this.declare_peace_consequence_delegate();
                    break;
                case CSDialogueConsequence.EndConversationSurrender:
                    this.surrender_consequence_delegate(d.Parameters.cs_isPlayerSurrender);
                    break;
                case CSDialogueConsequence.BarterForPeace:
                    this.barter_for_peace_consequence_delegate();
                    break;
            }
        }

        #region Condition Delegates

        private bool party_template_condition_delegate(PartyBase encounteredParty, bool isFriendly) // generic party checking delegate
        {
            if (encounteredParty.MobileParty != null)
            {
                if (isFriendly)
                {
                    if (!Hero.MainHero.MapFaction.IsAtWarWith(encounteredParty.MapFaction))
                    {
                        return true;
                    }

                }
                else if (!isFriendly)
                {
                    if (Hero.MainHero.MapFaction.IsAtWarWith(encounteredParty.MapFaction))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool party_template_and_defender_hostile_condition_delegate(PartyBase encounteredParty, bool isFriendly) // checks for template, attitude and if the party is defending (being engaged)
        {
            if (encounteredParty.MobileParty != null)
            {
                if (isFriendly)
                {
                    if (PlayerEncounter.PlayerIsAttacker && !Hero.MainHero.MapFaction.IsAtWarWith(encounteredParty.MapFaction))
                    {
                        return true;
                    }

                }
                else if (!isFriendly)
                {
                    if (PlayerEncounter.PlayerIsAttacker && Hero.MainHero.MapFaction.IsAtWarWith(encounteredParty.MapFaction))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool party_template_and_attacker_hostile_condition_delegate(PartyBase encounteredParty, bool isFriendly) // checks for template, attitude and if the party is attacking (engaging the player)
        {
            if (encounteredParty.MobileParty != null)
            {
                if (isFriendly)
                {
                    if (!PlayerEncounter.PlayerIsAttacker && !Hero.MainHero.MapFaction.IsAtWarWith(encounteredParty.MapFaction))
                    {
                        return true;
                    }

                }
                else if (!isFriendly)
                {
                    if (!PlayerEncounter.PlayerIsAttacker && Hero.MainHero.MapFaction.IsAtWarWith(encounteredParty.MapFaction))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool first_and_lord_name_condition_delegate(string name)
        {
            if ((Hero.OneToOneConversationHero != null) && !Hero.OneToOneConversationHero.HasMet)
            {
                return Hero.OneToOneConversationHero.Name.ToString() == name;
            }
            return false;
        }

        private bool first_and_faction_condition_delegate(string faction)
        {
            if(PlayerEncounter.EncounteredParty.MobileParty != null)
                if ((PlayerEncounter.EncounteredParty.MobileParty.MapFaction.StringId == faction) && !Hero.OneToOneConversationHero.HasMet)
                    return true;

            return false;
        }

        private bool generic_war_condition_delegate(bool isFriendly) // generic delegate to check if at war or not at war
        {
            if (isFriendly)
                return !PlayerEncounter.EncounteredParty.MapFaction.IsAtWarWith(Hero.MainHero.MapFaction);
            else if (!isFriendly)
                return PlayerEncounter.EncounteredParty.MapFaction.IsAtWarWith(Hero.MainHero.MapFaction);
            return false;
        }

        private bool hero_trait_condition_delegate(bool isPlayer, string trait, int value) // for traits, not really a good idea to use since it only checks for a single value rather than inequality
        {
            Hero checker;
            if (isPlayer)
                checker = Hero.MainHero;
            else
                checker = Hero.OneToOneConversationHero;

            switch (trait)
            {
                case "valor":
                    return ((checker.GetTraitLevel(DefaultTraits.Valor)) == value);
                case "honor":
                    return ((checker.GetTraitLevel(DefaultTraits.Honor)) == value);
                case "mercy":
                    return ((checker.GetTraitLevel(DefaultTraits.Mercy)) == value);
                case "calculating":
                    return ((checker.GetTraitLevel(DefaultTraits.Calculating)) == value);
            }
            return false;
        }

        private bool barter_check_condition_delegate(bool successful)
        {
            if (successful)
                return Campaign.Current.BarterManager.LastBarterIsAccepted;
            else if (!successful)
                return !Campaign.Current.BarterManager.LastBarterIsAccepted;

            return false;
        }

        #endregion Condition Delegates

        #region Consequence Delegates

        private void end_conversation_consequence_delegate()
        {
            PlayerEncounter.LeaveEncounter = true;
        }

        private void end_conversation_battle_consequence_delegate()
        {
            PlayerEncounter.Current.IsEnemy = true;
        }

        private void declare_war_consequence_delegate()
        {
            PartyBase encounteredParty = PlayerEncounter.EncounteredParty;
            Diplomacy.DiplomacyUtils.DeclareWarOverProvocation(Hero.MainHero.MapFaction, encounteredParty.MapFaction);
        }

        private void declare_peace_consequence_delegate()
        {
            PartyBase encounteredParty = PlayerEncounter.EncounteredParty;
            Diplomacy.DiplomacyUtils.MakePeace(Hero.MainHero.MapFaction, encounteredParty.MapFaction);
        }

        private void surrender_consequence_delegate(bool isPlayer) // for surrenders you need to update the player encounter- not sure if this closes the window or not
        {
            if (isPlayer)
                PlayerEncounter.PlayerSurrender = true;
            else if (!isPlayer)
                PlayerEncounter.EnemySurrender = true;
            PlayerEncounter.Update();
        }

        private void barter_for_peace_consequence_delegate() // looks like a lot, I just stole most of this from tw >_>
        {
            BarterManager instance = BarterManager.Instance;
            Hero mainHero = Hero.MainHero;
            Hero oneToOneConversationHero = Hero.OneToOneConversationHero;
            PartyBase mainParty = PartyBase.MainParty;
            MobileParty conversationParty = MobileParty.ConversationParty;
            PartyBase otherParty = (conversationParty != null) ? conversationParty.Party : null;
            Hero beneficiaryOfOtherHero = null;
            BarterManager.BarterContextInitializer initContext = new BarterManager.BarterContextInitializer(BarterManager.Instance.InitializeMakePeaceBarterContext);
            int persuasionCostReduction = 0;
            bool isAIBarter = false;
            Barterable[] array = new Barterable[1];
            int num = 0;
            Hero originalOwner = conversationParty.MapFaction.Leader;
            Hero mainHero2 = Hero.MainHero;
            MobileParty conversationParty2 = MobileParty.ConversationParty;
            array[num] = new PeaceBarterable(originalOwner, conversationParty.MapFaction, mainHero.MapFaction, CampaignTime.Years(1f));
            instance.StartBarterOffer(mainHero, oneToOneConversationHero, mainParty, otherParty, beneficiaryOfOtherHero, initContext, persuasionCostReduction, isAIBarter, array);
        }

        #endregion Consequence Delegates

        public void RegisterParty(MobileParty mb, string partyTemplateID)
        {
            ModDebug.ShowMessage("party of " + mb.StringId + " has registered for dialogue detection", DebugMessageType.Dialogue);
            dialoguePartyRef.Add(mb, partyTemplateID);
        }

        private void GetData() // the classic
        {
            dataManager = Data.DialogueDataManager.Instance;
        }

        // final version uses no structs, yay!!! no more lazy copouts (well, at least a reduced amount of them)

        public enum CSDialogueCondition
        {
            None,
            PartyTemplateAndStance,
            PartyTemplateDefenderAndStance,
            PartyTemplateAttackerAndStance,
            GenericWar,
            CharacterTrait,
            FirstConversationLordName,
            FirstConversationFaction,
            LastBarter
        }

        public enum CSDialogueConsequence
        {
            None,
            EndConversation,
            EndConversationInBattle,
            EndConversationSurrender,
            DeclareWar,
            DeclarePeace,
            BarterForPeace,
            StandardBarter
        }

        public class CustomSpawnsDialogueBehaviorTypeDefiner : CampaignBehaviorBase.SaveableCampaignBehaviorTypeDefiner
        {
            public CustomSpawnsDialogueBehaviorTypeDefiner() : base(51255)
            {
            }

            protected override void DefineContainerDefinitions()
            {
                base.ConstructContainerDefinition(typeof(Dictionary<MobileParty, string>));
            }
        }

        /* private enum PlayerInteraction
        {
            Friendly,
            Hostile
        } was gonna used this but decided aginst it, maybe it'll find a use in the future */
    }
}
