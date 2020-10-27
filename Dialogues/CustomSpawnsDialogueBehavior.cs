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

        private Data.DialogueDataManager dataManager;

        public override void SyncData(IDataStore dataStore)
        {

        }

        Dictionary<MobileParty, string> dict = new Dictionary<MobileParty, string>();

        List<CustomSpawnsDialogueInstance> dialogues = new List<CustomSpawnsDialogueInstance>();

        public void AddCustomDialogues(CampaignGameStarter starter)
        {
            if(dataManager == null)
            {
                GetData();
            }
            dialogues = (List<CustomSpawnsDialogueInstance>)dataManager.Data;
            foreach (CustomSpawnsDialogueInstance d in dialogues) // handle the dialogues that don't start conversations
            {
                if(d.isPlayer)
                {
                    starter.AddPlayerLine(d.id, d.tokenIn, d.tokenOut, d.text, // delegating in the loop so we don't interfere with the asynchronous magic
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
                    starter.AddDialogLine(d.id, d.tokenIn, d.tokenOut, d.text, // etc etc, reusing code is bad but what can I do ¯\_(ツ)_/¯
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

        private bool EvaluateDialogueCondition(CustomSpawnsDialogueInstance inst)
        {
            switch (inst.conditionType)
            {
                case CSDialogueCondition.None:
                    return true;
                case CSDialogueCondition.PartyTemplate:
                    return this.party_template_condition_delegate(inst.parameters.c_partyTemplate);
                case CSDialogueCondition.PartyTemplateAndAttackerHostile:
                    return this.party_template_and_attacker_condition_delegate(inst.parameters.c_partyTemplate);
                case CSDialogueCondition.PartyTemplateAndDefenderHostile:
                    return this.party_template_and_defender_condition_delegate(inst.parameters.c_partyTemplate);
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
            if(dict.ContainsKey(encounteredParty.MobileParty) && (dict[encounteredParty.MobileParty] == t))
            {
                return true;
            }
            return false;
        }

        private bool party_template_and_defender_condition_delegate(string t) // checks both for template and if the hostile party is defending (being engaged)
        {
            PartyBase encounteredParty = PlayerEncounter.EncounteredParty;
            if (dict.ContainsKey(encounteredParty.MobileParty) && (dict[encounteredParty.MobileParty] == t) && PlayerEncounter.PlayerIsAttacker)
            {
                return true;
            }
            return false;
        }

        private bool party_template_and_attacker_condition_delegate(string t) // checks both for template and if the hostile party is attacking (engaging the player)
        {
            PartyBase encounteredParty = PlayerEncounter.EncounteredParty;
            if (dict.ContainsKey(encounteredParty.MobileParty) && (dict[encounteredParty.MobileParty] == t) && PlayerEncounter.PlayerIsDefender)
            {
                return true;
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

        public void RegisterParty(MobileParty mb, string template)
        {
            ModDebug.ShowMessage("party of " + mb.StringId + " has registered for dialogue detection", DebugMessageType.Dialogue);
            dict.Add(mb, template);
        }

        private void GetData()
        {
            dataManager = Data.DialogueDataManager.Instance;
        }

        public struct CustomSpawnsDialogueInstance
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
        } might have to use this at some point */
    }
}
