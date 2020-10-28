using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;

namespace CustomSpawns.Dialogues
{
    public class CustomSpawnsDialogueBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.AddCustomDialogues));
            DialogueManager.CustomSpawnsDialogueBehavior = this;
        }

        private Data.DialogueDataManager dataManager;

        // no method for saving data right now- it works on campaign launch but probably not once the save is reloaded

        public override void SyncData(IDataStore dataStore)
        {

        }

        // basic dialogue overview: 
        // ids are used to differentiate similar dialogues (must be unique)
        // tokens are like impulses, so an out token leads into an in token. 'magic tokens' are tokens which have special functions, they are start (in token) and close_window (out token)
        // conditions are delegates that must anonymously return a bool, which will be wheteher or not the line is displayed
        // consequences are void delegates, basically just pieces of code run after a line has been selected
        // i think? priority determines which line is shown if multiple lines meet the requirements, and also maybe what order player lines are displayed in (speculation)

        Dictionary<MobileParty, string> dict = new Dictionary<MobileParty, string>();

        public void AddCustomDialogues(CampaignGameStarter starter)
        {
            if(dataManager == null)
            {
                GetData();
            }
            foreach (Data.DialogueData d in dataManager.Data) // handle the dialogues
            {
                if(d.IsPlayer)
                {
                    starter.AddPlayerLine(d.Id, d.InToken, d.OutToken, d.DialogueText, // delegating in the loop so we don't interfere with the asynchronous magic
                    delegate
                    {
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
            switch (d.Condition)
            {
                case CSDialogueCondition.None:
                    return true;
                case CSDialogueCondition.PartyTemplateAndStance:
                    return this.party_template_condition_delegate(d.Parameters.c_partyTemplate, d.Parameters.c_isFriendly);
                case CSDialogueCondition.PartyTemplateAttackerAndStance:
                    return this.party_template_and_attacker_hostile_condition_delegate(d.Parameters.c_partyTemplate, d.Parameters.c_isFriendly);
                case CSDialogueCondition.PartyTemplateDefenderAndStance:
                    return this.party_template_and_defender_hostile_condition_delegate(d.Parameters.c_partyTemplate, d.Parameters.c_isFriendly);
                case CSDialogueCondition.GenericWar:
                    return this.generic_war_condition_delegate(d.Parameters.c_isFriendly);
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
                case CSDialogueConsequence.EndConversationSurrender: // dont use this pls, it doesn't work
                    this.surrender_consequence_delegate(d.Parameters.cs_isPlayerSurrender);
                    break;
            }
        }

        #region Condition Delegates

        private bool party_template_condition_delegate(string t, bool isFriendly) // generic party checking delegate
        {
            PartyBase encounteredParty = PlayerEncounter.EncounteredParty;
            if(isFriendly)
            {
                if (dict.ContainsKey(encounteredParty.MobileParty) && (dict[encounteredParty.MobileParty] == t) && !Hero.MainHero.MapFaction.IsAtWarWith(encounteredParty.MapFaction))
                {
                    return true;
                }

            }
            else if(!isFriendly)
            {
                if (dict.ContainsKey(encounteredParty.MobileParty) && (dict[encounteredParty.MobileParty] == t) && Hero.MainHero.MapFaction.IsAtWarWith(encounteredParty.MapFaction))
                {
                    return true;
                }
            }
            return false;
        }

        private bool party_template_and_defender_hostile_condition_delegate(string t, bool isFriendly) // checks for template, attitude and if the hostile party is defending (being engaged)
        {
            PartyBase encounteredParty = PlayerEncounter.EncounteredParty;
            if (isFriendly)
            {
                if (dict.ContainsKey(encounteredParty.MobileParty) && (dict[encounteredParty.MobileParty] == t) && PlayerEncounter.PlayerIsAttacker && !Hero.MainHero.MapFaction.IsAtWarWith(encounteredParty.MapFaction))
                {
                    return true;
                }

            }
            else if (!isFriendly)
            {
                if (dict.ContainsKey(encounteredParty.MobileParty) && (dict[encounteredParty.MobileParty] == t) && PlayerEncounter.PlayerIsAttacker && Hero.MainHero.MapFaction.IsAtWarWith(encounteredParty.MapFaction))
                {
                    return true;
                }
            }
            return false;
        }

        private bool party_template_and_attacker_hostile_condition_delegate(string t, bool isFriendly) // checks for template, attitude and if the party is attacking (engaging the player)
        {
            PartyBase encounteredParty = PlayerEncounter.EncounteredParty;
            if (isFriendly)
            {
                if (dict.ContainsKey(encounteredParty.MobileParty) && (dict[encounteredParty.MobileParty] == t) && !PlayerEncounter.PlayerIsAttacker && !Hero.MainHero.MapFaction.IsAtWarWith(encounteredParty.MapFaction))
                {
                    return true;
                }

            }
            else if (!isFriendly)
            {
                if (dict.ContainsKey(encounteredParty.MobileParty) && (dict[encounteredParty.MobileParty] == t) && !PlayerEncounter.PlayerIsAttacker && Hero.MainHero.MapFaction.IsAtWarWith(encounteredParty.MapFaction))
                {
                    return true;
                }
            }
            return false;
        }

        private bool generic_war_condition_delegate(bool isFriendly) // generic delegate to check if at war or not at war
        {
            if (isFriendly)
                return !PlayerEncounter.EncounteredParty.MapFaction.IsAtWarWith(Hero.MainHero.MapFaction);
            else if(!isFriendly)
                return PlayerEncounter.EncounteredParty.MapFaction.IsAtWarWith(Hero.MainHero.MapFaction);
            return false;
        }

        #endregion Condition Delegates

        #region Consequence Delegates

        private void  end_conversation_consequence_delegate()
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

        private void surrender_consequence_delegate(bool isPlayer) // doesn't work for some reason, no idea how it's supposed to work tbh
        {
            if(isPlayer)
                PlayerEncounter.PlayerSurrender = true;
            else if(!isPlayer)
                PlayerEncounter.EnemySurrender = true;
            PlayerEncounter.LeaveEncounter = true;
        }

        // TODO implement barter screen

        #endregion Consequence Delegates

        public void RegisterParty(MobileParty mb, string template)
        {
            ModDebug.ShowMessage("party of " + mb.StringId + " has registered for dialogue detection", DebugMessageType.Dialogue);
            dict.Add(mb, template);
        }

        private void GetData() // the classic
        {
            dataManager = Data.DialogueDataManager.Instance;
        }

        // final version uses no structs, yay!!! no more lazy copouts  (well, at least a reduced amount of them)

        /* public struct CustomSpawnsDialogueInstance
        {
            public bool isPlayer;
            public string id;
            public string tokenIn;
            public string tokenOut;
            public string text;
            public CSDialogueCondition conditionType;
            public CSDialogueConsequence consequenceType;
            public CustomSpawnsDialogueParams parameters;
            public int priority;

            public CustomSpawnsDialogueInstance(bool playerLine, string k, string inS, string outS, string body, CSDialogueCondition cond, CSDialogueConsequence cons, CustomSpawnsDialogueParams settings, int pri)
            {
                isPlayer = playerLine;
                id = k;
                tokenIn = inS;
                tokenOut = outS;
                text = body;
                conditionType = cond;
                consequenceType = cons;
                parameters = settings;
                priority = pri;
            }
        }   keeping this in case I need to rollback (most likely not) */

        public enum CSDialogueCondition
        {
            None,
            PartyTemplateAndStance,
            PartyTemplateDefenderAndStance,
            PartyTemplateAttackerAndStance,
            GenericWar
        }

        public enum CSDialogueConsequence
        {
            None,
            EndConversation,
            EndConversationInBattle,
            EndConversationSurrender,
            DeclareWar,
            DeclarePeace,
            BarterScreen,
        }

        /* private enum PlayerInteraction
        {
            Friendly,
            Hostile
        } was gonna used this but decided aginst it, maybe it'll find a use in the future */
    }
}
