using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using CustomSpawns.Dialogues.DialogueAlgebra;
using CustomSpawns.Diplomacy;
using CustomSpawns.Exception;
using CustomSpawns.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.BarterSystem;
using TaleWorlds.CampaignSystem.BarterSystem.Barterables;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Party;

namespace CustomSpawns.Dialogues
{
    public class DialogueConsequenceInterpretor
    {
        private readonly ConstantWarDiplomacyActionModel _diplomacyModel;

        // TODO properly manage dependencies but all managers are static and are singletons which makes it difficult to manage
        public DialogueConsequenceInterpretor()
        {
            _diplomacyModel = new ConstantWarDiplomacyActionModel();
            allMethods = typeof(DialogueConsequenceInterpretor).GetMethods(BindingFlags.Static | BindingFlags.NonPublic).
                Where((m) => m.GetCustomAttributes(typeof(DialogueConsequenceImplementorAttribute), false).Count() > 0).ToList();
        }

        public DialogueConsequence ParseConsequence(string text)
        {

            if(text.Length == 0)
            {
                return null;
            }

            try
            {

                //Simple as possible for now. No paranthesis support to group

                string[] tokens = text.Split(' ');

                if (tokens.Length == 0)
                {
                    //user has entered empty condition string
                    return null;
                }
                else if (tokens.Length == 1)
                {
                    //just a good old single function.
                    return ParseConsequenceToken(tokens[0]);
                }
                else if (tokens.Length % 2 == 0)
                {
                    throw new TechnicalException("Invalid consequence expression: " + text);
                }

                //tokens.Length is thus at least 3.

                DialogueConsequence aggregate = null;
                DialogueConsequence cur = null;

                aggregate = ParseConsequenceToken(tokens[0]);

                for (int i = 2; i < tokens.Length; i += 2)
                {
                    if (i % 2 == 0)
                    {

                        cur = ParseConsequenceToken(tokens[i]);

                        i -= 3; //we will add 2 to this and so we will get to the logic keyword.
                    }
                    else
                    {
                        //logic keyword AND OR 

                        if (tokens[i] == "AND" || tokens[i] == "&" || tokens[i] == "&&" || tokens[i] == "+")
                        {
                            aggregate = aggregate + cur;
                        }
                        else
                        {
                            throw new TechnicalException("Unrecognized logic keyword for consequences: " + tokens[i]);
                        }

                        i += 1; // we will add 2 to this and so we will get to next token.
                    }
                }

                return aggregate;

            }
            catch (System.Exception e)
            {
                ErrorHandler.ShowPureErrorMessage("Could not parse dialogue consequnce: \n" + text + "\n Error Message: \n" + e.Message);
                return null;
            }


        }

        private DialogueConsequence ParseConsequenceToken(string token)
        {
            //function and its parameters
            List<string> openPSplit = token.Split('(', ',').ToList();

            string funcName = openPSplit[0];

            //get rid of trailing

            for (int j = 1; j < openPSplit.Count; j++)
            {
                openPSplit[j] = openPSplit[j].TrimEnd(',', ')');
                //remove empty
                if (openPSplit[j].Length == 0)
                {
                    openPSplit.RemoveAt(j);
                    j--;
                }
            }

            if (funcName[0] == '!')
            {
                throw new TechnicalException("Consequences cannot be negated: " + token);
            }

            DialogueConsequence returned = null;

            switch (openPSplit.Count)
            {
                case 0:
                    throw new TechnicalException("Can't parse " + token + ". It may be empty.");
                case 1:
                    //no params
                    returned = GetDialogueConsequence(funcName);
                    break;
                case 2:
                    // 1 param
                    returned = GetDialogueConsequence(funcName, openPSplit[1]);
                    break;
                case 3:
                    // 2 params
                    returned = GetDialogueConsequence(funcName, openPSplit[1], openPSplit[2]);
                    break;
                case 4:
                    // 3 params
                    returned = GetDialogueConsequence(funcName, openPSplit[1], openPSplit[2], openPSplit[3]);
                    break;
                default:
                    throw new TechnicalException("Can't parse " + token + ". Possibly too many params.");
            }

            return returned;
        }
        
        #region Getters

        //TODO: cache at constructor and thus optimize if need be with a dictionary. Might not be needed though since this only runs once at the start of campaign.

        //just realized: can probably implement this more elegantly with just an array of strings than copying 4 functions! Consider if you want more params

        private static List<MethodInfo> allMethods;

        private DialogueConsequence GetDialogueConsequence(string implementor)
        {
            foreach (var m in allMethods)
            {
                var a = m.GetCustomAttribute<DialogueConsequenceImplementorAttribute>();
                if (a.ExposedName == implementor)
                {
                    if (m.GetParameters().Length != 1)
                        continue;

                    return new DialogueConsequenceBare
                        ((x) => ((Action<DialogueParams>)m.CreateDelegate(typeof(Action<DialogueParams>)))(x),
                        a.ExposedName);
                }
            }

            throw new TechnicalException("There is no function with name " + implementor + " that takes no parameters.");
        }

        private DialogueConsequence GetDialogueConsequence(string implementor, string param)
        {

            foreach (var m in allMethods)
            {
                var a = m.GetCustomAttribute<DialogueConsequenceImplementorAttribute>();
                if (a.ExposedName == implementor)
                {
                    if (m.GetParameters().Length != 2)
                        continue;

                    return new DialogueConsequenceWithExtraStaticParams<string>
                        ((Action<DialogueParams, string>)m.CreateDelegate(typeof(Action<DialogueParams, string>)),
                        param, a.ExposedName + "(" + param + ")");
                }
            }

            throw new TechnicalException("There is no function with name " + implementor + " that takes one parameter.");
        }

        private DialogueConsequence GetDialogueConsequence(string implementor, string param1, string param2)
        {

            foreach (var m in allMethods)
            {
                var a = m.GetCustomAttribute<DialogueConsequenceImplementorAttribute>();
                if (a.ExposedName == implementor)
                {
                    if (m.GetParameters().Length != 3)
                        continue;

                    return new DialogueConsequenceWithExtraStaticParams<string, string>
                        ((Action<DialogueParams, string, string>)m.CreateDelegate(typeof(Action<DialogueParams, string, string>)),
                        param1, param2, a.ExposedName + "(" + param1 + ", " + param2 + ")");
                }
            }

            throw new TechnicalException("There is no function with name " + implementor + " that takes two parameters.");
        }

        private DialogueConsequence GetDialogueConsequence(string implementor, string param1, string param2, string param3)
        {

            foreach (var m in allMethods)
            {
                var a = m.GetCustomAttribute<DialogueConsequenceImplementorAttribute>();
                if (a.ExposedName == implementor)
                {
                    if (m.GetParameters().Length != 4)
                        continue;

                    return new DialogueConsequenceWithExtraStaticParams<string, string, string>
                        ((Action<DialogueParams, string, string, string>)m.CreateDelegate(typeof(Action<DialogueParams, string, string, string>)),
                        param1, param2, param3, a.ExposedName + "(" + param1 + ", " + param2 + ", " + param3 + ")");
                }
            }

            throw new TechnicalException("There is no function with name " + implementor + " that takes three parameters.");
        }

        #endregion

        #region Consequences

        [DialogueConsequenceImplementorAttribute("Leave")]
        private static void end_conversation_consequence_delegate(DialogueParams param)
        {
            PlayerEncounter.LeaveEncounter = true;
        }

        [DialogueConsequenceImplementorAttribute("Battle")]
        private static void end_conversation_battle_consequence_delegate(DialogueParams param)
        {
            if(PlayerEncounter.Current == null)
            {
                return;
            }

            PlayerEncounter.Current.IsEnemy = true;
        }

        [DialogueConsequenceImplementorAttribute("War")]
        private void declare_war_consequence_delegate(DialogueParams param)
        {
            PartyBase encounteredParty = PlayerEncounter.EncounteredParty;

            if(encounteredParty == null)
            {
                return;
            }
            _diplomacyModel.DeclareWar(Hero.MainHero.MapFaction, encounteredParty.MapFaction);
        }

        [DialogueConsequenceImplementorAttribute("Peace")]
        private void declare_peace_consequence_delegate()
        {
            PartyBase encounteredParty = PlayerEncounter.EncounteredParty;

            if (encounteredParty == null)
            {
                return;
            }

            _diplomacyModel.MakePeace(Hero.MainHero.MapFaction, encounteredParty.MapFaction);
        }

        [DialogueConsequenceImplementorAttribute("Surrender")]
        private static void surrender_consequence_delegate(DialogueParams param, string isPlayer) // for surrenders you need to update the player encounter- not sure if this closes the window or not
        {
            isPlayer = isPlayer.ToLower();

            if (isPlayer == "true")
            {
                PlayerEncounter.PlayerSurrender = true;
            }
            else if (isPlayer == "false")
            {   //Interestingly, PlayerEncounter.EnemySurrender = true; results in a null reference crash.
                param.PlayerParty.Party.AddPrisoners(param.AdversaryParty.Party.MemberRoster);
                param.AdversaryParty.RemoveParty();
                UX.ShowMessage("You have taken your enemies prisoner.", TaleWorlds.Library.Colors.Green);
                end_conversation_consequence_delegate(param);
            }
            else
            {
                ErrorHandler.ShowPureErrorMessage("Can't interpret " + isPlayer.ToString() + " as a bool. Possible typo in the XML consequence 'Surrender'");
            }
            PlayerEncounter.Update();
        }

        [DialogueConsequenceImplementorAttribute("BarterPeace")]
        private static void barter_for_peace_consequence_delegate(DialogueParams param) // looks like a lot, I just stole most of this from tw >_>
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

        [DialogueConsequenceImplementorAttribute("BarterNoAttack")]
        private static void conversation_set_up_safe_passage_barter_on_consequence(DialogueParams param)
        {
            BarterManager instance = BarterManager.Instance;
            Hero oneToOneConversationHero = Hero.OneToOneConversationHero;
            PartyBase mainParty = PartyBase.MainParty;
            PartyBase otherParty = MobileParty.ConversationParty?.Party;
            BarterManager.BarterContextInitializer initContext = 
                new BarterManager.BarterContextInitializer(BarterManager.Instance.InitializeSafePassageBarterContext);
            int persuasionCostReduction = 0;
            bool isAIBarter = false;


            if (Hero.OneToOneConversationHero == null)
            {
                Barterable[] array = new Barterable[1];
                array[0] = new SafePassageBarterable(null, Hero.MainHero, otherParty, PartyBase.MainParty);
                instance.StartBarterOffer(Hero.MainHero, oneToOneConversationHero, mainParty, otherParty, null, initContext,
                    persuasionCostReduction, isAIBarter, array);
            }
            else
            {
                Barterable[] array = new Barterable[2];
                array[0] = new SafePassageBarterable(oneToOneConversationHero, Hero.MainHero, otherParty, PartyBase.MainParty);
                array[1] = new NoAttackBarterable(Hero.MainHero, oneToOneConversationHero, mainParty,
                    otherParty, CampaignTime.Days(5f));
                instance.StartBarterOffer(Hero.MainHero, oneToOneConversationHero, mainParty, otherParty, null,
                    initContext, persuasionCostReduction, isAIBarter, array);
            }
        }

        #endregion

        [AttributeUsage(AttributeTargets.Method)]
        private class DialogueConsequenceImplementorAttribute : Attribute
        {
            public string ExposedName { get; private set; }

            public DialogueConsequenceImplementorAttribute(string name)
            {
                ExposedName = name;
            }
        }
    }
}
