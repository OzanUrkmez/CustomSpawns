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
        }

        public override void SyncData(IDataStore dataStore)
        {

        }

        List<CustomSpawnsDialogueData> datas = new List<CustomSpawnsDialogueData>();

        List<CustomSpawnsDialogueInstance> freeDialogues = new List<CustomSpawnsDialogueInstance>();

        public void AddCustomDialogues(CampaignGameStarter starter)
        {
            foreach (CustomSpawnsDialogueData d in datas) // handle starting lines
            {
                foreach (CustomSpawnsDialogueInstance inst in d.dialogues) // no checking for player line since afaik it's impossible to start dialogue with a player choice
                {
                    starter.AddDialogLine(inst.key, inst.tokenIn, inst.tokenOut, inst.text, // we are delegating these in the loop so they can have parameters passed without interfering with reciever
                    delegate // needs to return bool anonymously, the reciever handles the rest
                    {
                        return this.EvaluateStartCondition(d, inst);
                    },
                    delegate // doesn't need return since it's void
                    {
                        this.EvaluateDialogueConsequence(inst);
                    } );
                }
            }
            foreach (CustomSpawnsDialogueInstance d in freeDialogues) // handle the dialogues that don't start conversations
            {
                if(d.isPlayer)
                {
                    starter.AddPlayerLine(d.key, d.tokenIn, d.tokenOut, d.text, // again, delegating in the loop
                    delegate
                    {
                        return true; // TODO implement conditions
                    },
                    delegate
                    {
                        this.EvaluateDialogueConsequence(d);
                    });
                }
                else
                {
                    starter.AddDialogLine(d.key, d.tokenIn, d.tokenOut, d.text, // etc etc, reusing code is bad but what can I do ¯\_(ツ)_/¯
                    delegate
                    {
                        return true; // TODO implement conditions
                    },
                    delegate
                    {
                        this.EvaluateDialogueConsequence(d);
                    });
                }
            }
        }

        private bool EvaluateStartCondition(CustomSpawnsDialogueData d, CustomSpawnsDialogueInstance inst)
        {
            switch (inst.conditionType)
            {
                case CSDialogueCondition.PartyTemplate:
                    return this.party_template_condition_delegate(d.templateName);
                case CSDialogueCondition.PartyTemplateAndAttackerHostile:
                    return this.party_template_and_attacker_condition_delegate(d.templateName);
                case CSDialogueCondition.PartyTemplateAndDefenderHostile:
                    return this.party_template_and_defender_condition_delegate(d.templateName);
                default:
                    return false;
            }
        }

        private void EvaluateDialogueConsequence(CustomSpawnsDialogueInstance d)
        {
            switch (d.consequenceType)
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
            }
        }

        #region Condition Delegates

        private bool party_template_condition_delegate(string t) // generic party checking delegate, for starting lines
        {
            PartyBase encounteredParty = PlayerEncounter.EncounteredParty;
            foreach (CustomSpawnsDialogueData d in datas)
            {
                if((encounteredParty.MobileParty == d.mb) && d.templateName == t)
                {
                    return true;
                }
            }
            return false;
        }

        private bool party_template_and_defender_condition_delegate(string t) // checks both for template and if the party is defending (being engaged)
        {
            PartyBase encounteredParty = PlayerEncounter.EncounteredParty;
            foreach (CustomSpawnsDialogueData d in datas)
            {
                if ((encounteredParty.MobileParty == d.mb) && (d.templateName == t) && PlayerEncounter.PlayerIsAttacker)
                {
                    return true;
                }
            }
            return false;
        }

        private bool party_template_and_attacker_condition_delegate(string t) // checks both for template and if the party is attacking (engaging the player)
        {
            PartyBase encounteredParty = PlayerEncounter.EncounteredParty;
            foreach (CustomSpawnsDialogueData d in datas)
            {
                if ((encounteredParty.MobileParty == d.mb) && (d.templateName == t) && PlayerEncounter.PlayerIsDefender)
                {
                    return true;
                }
            }
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

        // TODO implement barter screen

        #endregion Consequence Delegates

        public void RegisterDialogues(CustomSpawnsDialogueData dat, List<CustomSpawnsDialogueInstance> list)
        {
            ModDebug.ShowMessage("party template of [" + dat.templateName + "] has registered starting dialogue", DebugMessageType.Dialogue);
            datas.Add(dat);
            freeDialogues = list;
        }

        public struct CustomSpawnsDialogueData
        {
            public MobileParty mb;
            public string templateName;
            public List<CustomSpawnsDialogueInstance> dialogues;

            public CustomSpawnsDialogueData(MobileParty p, string temp, List<CustomSpawnsDialogueInstance> list)
            {
                mb = p;
                templateName = temp;
                dialogues = list;
            }
        }

        public struct CustomSpawnsDialogueInstance
        {
            public bool isPlayer;
            public string key;
            public string tokenIn;
            public string tokenOut;
            public string text;
            public CSDialogueCondition conditionType;
            public CSDialogueConsequence consequenceType;
            public int priority;

            public CustomSpawnsDialogueInstance(bool playerLine, string k, string inS, string outS, string body, CSDialogueCondition cond, CSDialogueConsequence cons, int pri)
            {
                isPlayer = playerLine;
                key = k;
                tokenIn = inS;
                tokenOut = outS;
                text = body;
                conditionType = cond;
                consequenceType = cons;
                priority = pri;
            }
        }

        public enum CSDialogueCondition
        {
            None,
            PartyTemplate,
            PartyTemplateAndDefenderHostile,
            PartyTemplateAndAttackerHostile,
        }

        public enum CSDialogueConsequence
        {
            None,
            EndConversation,
            EndConversationInBattle,
            DeclareWar,
            DeclarePeace,
            BarterScreen,
        }

        /* private enum PlayerInteraction
        {
            None,
            Friendly,
            Hostile
        } might have to use this at some point as well */
    }
}
