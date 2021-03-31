using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TaleWorlds;
using TaleWorlds.CampaignSystem;

using CustomSpawns.Dialogues.DialogueAlgebra;
using TaleWorlds.CampaignSystem.Barterables;

namespace CustomSpawns.Dialogues
{
    public static class DialogueConsequencesManager
    {

        static DialogueConsequencesManager()
        {
            allMethods = typeof(DialogueConsequencesManager).GetMethods(BindingFlags.Static | BindingFlags.NonPublic).
                 Where((m) => m.GetCustomAttributes(typeof(DialogueConsequenceImplementorAttribute), false).Count() > 0).ToList();
        }

        #region Getters

        //TODO: cache at constructor and thus optimize if need be with a dictionary. Might not be needed though since this only runs once at the start of campaign.

        //just realized: can probably implement this more elegantly with just an array of strings than copying 4 functions! Consider if you want more params

        private static List<MethodInfo> allMethods;

        public static DialogueConsequence GetDialogueConsequence(string implementor)
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

            throw new Exception("There is no function with name " + implementor + " that takes no parameters.");
        }

        public static DialogueConsequence GetDialogueConsequence(string implementor, string param)
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

            throw new Exception("There is no function with name " + implementor + " that takes one parameter.");
        }

        public static DialogueConsequence GetDialogueConsequence(string implementor, string param1, string param2)
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

            throw new Exception("There is no function with name " + implementor + " that takes two parameters.");
        }

        public static DialogueConsequence GetDialogueConsequence(string implementor, string param1, string param2, string param3)
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

            throw new Exception("There is no function with name " + implementor + " that takes three parameters.");
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
            PlayerEncounter.Current.IsEnemy = true;
        }

        [DialogueConsequenceImplementorAttribute("War")]
        private static void declare_war_consequence_delegate(DialogueParams param)
        {
            PartyBase encounteredParty = PlayerEncounter.EncounteredParty;
            Diplomacy.DiplomacyUtils.DeclareWarOverProvocation(Hero.MainHero.MapFaction, encounteredParty.MapFaction);
        }

        [DialogueConsequenceImplementorAttribute("Peace")]
        private static void declare_peace_consequence_delegate()
        {
            PartyBase encounteredParty = PlayerEncounter.EncounteredParty;
            Diplomacy.DiplomacyUtils.MakePeace(Hero.MainHero.MapFaction, encounteredParty.MapFaction);
        }

        [DialogueConsequenceImplementorAttribute("Surrender")]
        private static void surrender_consequence_delegate(string isPlayer) // for surrenders you need to update the player encounter- not sure if this closes the window or not
        {
            isPlayer = isPlayer.ToLower();

            if (isPlayer == "true")
            {
                PlayerEncounter.PlayerSurrender = true;
            }
            else if (isPlayer == "false")
            {
                PlayerEncounter.EnemySurrender = true;
            }
            else
            {
                ErrorHandler.ShowPureErrorMessage("Can't interpret " + isPlayer.ToString() + " as a bool. Possible typo in the XML consequence 'Surrender'");
            }
            PlayerEncounter.Update();
        }

        [DialogueConsequenceImplementorAttribute("BarterPeace")]
        private static void barter_for_peace_consequence_delegate() // looks like a lot, I just stole most of this from tw >_>
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
        private static void conversation_set_up_safe_passage_barter_on_consequence() //taken from LordConversationsCampaignBehaviour
        {
            BarterManager instance = BarterManager.Instance;
            Hero mainHero = Hero.MainHero;
            Hero oneToOneConversationHero = Hero.OneToOneConversationHero;
            PartyBase mainParty = PartyBase.MainParty;
            MobileParty conversationParty = MobileParty.ConversationParty;
            PartyBase otherParty = (conversationParty != null) ? conversationParty.Party : null;
            Hero beneficiaryOfOtherHero = null;
            BarterManager.BarterContextInitializer initContext = new BarterManager.BarterContextInitializer(BarterManager.Instance.InitializeSafePassageBarterContext);
            int persuasionCostReduction = 0;
            bool isAIBarter = false;
            Barterable[] array = new Barterable[2];
            int num = 0;
            Hero oneToOneConversationHero2 = Hero.OneToOneConversationHero;
            Hero mainHero2 = Hero.MainHero;
            MobileParty conversationParty2 = MobileParty.ConversationParty;
            array[num] = new SafePassageBarterable(oneToOneConversationHero2, mainHero2, (conversationParty2 != null) ? conversationParty2.Party : null, PartyBase.MainParty);
            int num2 = 1;
            Hero mainHero3 = Hero.MainHero;
            Hero oneToOneConversationHero3 = Hero.OneToOneConversationHero;
            PartyBase mainParty2 = PartyBase.MainParty;
            MobileParty conversationParty3 = MobileParty.ConversationParty;
            array[num2] = new NoAttackBarterable(mainHero3, oneToOneConversationHero3, mainParty2, (conversationParty3 != null) ? conversationParty3.Party : null, CampaignTime.Days(5f));
            instance.StartBarterOffer(mainHero, oneToOneConversationHero, mainParty, otherParty, beneficiaryOfOtherHero, initContext, persuasionCostReduction, isAIBarter, array);
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
