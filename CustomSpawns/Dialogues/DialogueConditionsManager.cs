using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TaleWorlds;
using TaleWorlds.CampaignSystem;

using CustomSpawns.Dialogues.DialogueAlgebra;

namespace CustomSpawns.Dialogues
{
    public static class DialogueConditionsManager
    {

        static DialogueConditionsManager()
        {
            allMethods = typeof(DialogueConditionsManager).GetMethods(BindingFlags.Static | BindingFlags.NonPublic).
                 Where((m) => m.GetCustomAttributes(typeof(DialogueConditionImplementorAttribute), false).Count() > 0).ToList();
        }

        #region Getters

        //TODO: cache at constructor and thus optimize if need be with a dictionary. Might not be needed though since this only runs once at the start of campaign.

        //just realized: can probably implement this more elegantly with just an array of strings than copying 4 functions! Consider if you want more params

        private static List<MethodInfo> allMethods;

        public static DialogueCondition GetDialogueCondition(string implementor)
        {

            foreach (var m in allMethods)
            {
                var a = m.GetCustomAttribute<DialogueConditionImplementorAttribute>();
                if (a.ExposedName == implementor)
                {
                    if (m.GetParameters().Length != 1)
                        continue;

                    return new DialogueConditionBare
                        ((x) => ((Func<DialogueParams, bool>)m.CreateDelegate(typeof(Func<DialogueParams, bool>)))(x),
                        a.ExposedName);
                }
            }

            throw new Exception("There is no function with name " + implementor + " that takes no parameters.");
        }

        public static DialogueCondition GetDialogueCondition(string implementor, string param)
        {

            foreach (var m in allMethods)
            {
                var a = m.GetCustomAttribute<DialogueConditionImplementorAttribute>();
                if (a.ExposedName == implementor)
                {
                    if (m.GetParameters().Length != 2)
                        continue;

                    return new DialogueConditionWithExtraStaticParams<string>
                        ((Func<DialogueParams, string, bool>)m.CreateDelegate(typeof(Func<DialogueParams, string, bool>)),
                        param, a.ExposedName + "(" + param + ")");
                }
            }

            throw new Exception("There is no function with name " + implementor + " that takes one parameter.");
        }

        public static DialogueCondition GetDialogueCondition(string implementor, string param1, string param2)
        {

            foreach (var m in allMethods)
            {
                var a = m.GetCustomAttribute<DialogueConditionImplementorAttribute>();
                if (a.ExposedName == implementor)
                {
                    if (m.GetParameters().Length != 3)
                        continue;

                    return new DialogueConditionWithExtraStaticParams<string, string>
                        ((Func<DialogueParams, string, string, bool>)m.CreateDelegate(typeof(Func<DialogueParams, string, string, bool>)),
                        param1, param2, a.ExposedName + "(" + param1 + ", " + param2 + ")");
                }
            }

            throw new Exception("There is no function with name " + implementor + " that takes two parameters.");
        }

        public static DialogueCondition GetDialogueCondition(string implementor, string param1, string param2, string param3)
        {

            foreach (var m in allMethods)
            {
                var a = m.GetCustomAttribute<DialogueConditionImplementorAttribute>();
                if (a.ExposedName == implementor)
                {
                    if (m.GetParameters().Length != 4)
                        continue;

                    return new DialogueConditionWithExtraStaticParams<string, string,string>
                        ((Func<DialogueParams, string, string, string, bool>)m.CreateDelegate(typeof(Func<DialogueParams, string, string, string, bool>)),
                        param1, param2, param3, a.ExposedName + "(" + param1 + ", " + param2 + ", " + param3 +")");
                } 
            }

            throw new Exception("There is no function with name " + implementor + " that takes three parameters.");
        }

        #endregion

        #region Conditions

        [DialogueConditionImplementor("HasPartyID")]
        private static bool HasPartyID(DialogueParams param, string ID)
        {
            if (param.AdversaryParty == null)
                return false;

            if (CampaignUtils.IsolateMobilePartyStringID(param.AdversaryParty.Party.MobileParty) == ID)
            {
                return true;
            }
            return false;
        }

        [DialogueConditionImplementor("PartyIDStartsWith")]
        private static bool PartyIDStartsWith(DialogueParams param, string ID)
        {
            if (param.AdversaryParty == null)
                return false;

            return param.AdversaryParty.Party.Id.StartsWith(ID);
        }

        [DialogueConditionImplementor("PartyIDEndsWith")]
        private static bool PartyIDEndsWith(DialogueParams param, string ID)
        {
            if (param.AdversaryParty == null)
                return false;

            return param.AdversaryParty.Party.Id.EndsWith(ID);
        }

        [DialogueConditionImplementor("PartyIsInFaction")]
        private static bool PartyIsInFaction(DialogueParams param, string factionID)
        {
            if (param.AdversaryParty == null)
                return false;

            return param.AdversaryParty.MapFaction.StringId.ToString() == factionID;
        }

        [DialogueConditionImplementor("IsHero")]
        private static bool IsHero(DialogueParams param, string ID)
        {
            return Hero.OneToOneConversationHero != null && Hero.OneToOneConversationHero.StringId == ID;
        }

        [DialogueConditionImplementor("FirstTimeTalkWithHero")]
        private static bool FirstTimeTalkWithHero(DialogueParams param)
        {
            return Hero.OneToOneConversationHero != null && !Hero.OneToOneConversationHero.HasMet;
        }

        [DialogueConditionImplementor("IsFriendly")]
        private static bool IsFriendly(DialogueParams param)
        {
            if (param.AdversaryParty == null)
                return false;

            return !param.AdversaryParty.MapFaction.IsAtWarWith(param.PlayerParty.MapFaction);
        }


        [DialogueConditionImplementor("IsHostile")]
        private static bool IsHostile(DialogueParams param)
        {
            if (param.AdversaryParty == null)
                return false;

            return param.AdversaryParty.MapFaction.IsAtWarWith(param.PlayerParty.MapFaction);
        }


        [DialogueConditionImplementor("IsDefending")]
        private static bool IsDefending(DialogueParams param)
        {
            if (param.AdversaryParty == null)
                return false;

            return PlayerEncounter.PlayerIsAttacker;
        }

        [DialogueConditionImplementor("IsAttacking")]
        private static bool IsAttacking(DialogueParams param)
        {
            if (param.AdversaryParty == null)
                return false;

            return PlayerEncounter.PlayerIsDefender;
        }

        [DialogueConditionImplementor("BarterSuccessful")]
        private static bool conversation_barter_successful_on_condition(DialogueParams param)
        {
            return Campaign.Current.BarterManager.LastBarterIsAccepted;
        }

        #endregion

        [AttributeUsage(AttributeTargets.Method)]
        private class DialogueConditionImplementorAttribute : Attribute
        {
            public string ExposedName { get; private set; }

            public DialogueConditionImplementorAttribute(string name)
            {
                ExposedName = name;
            }
        }
    }

    public class DialogueParams
    {
        public MobileParty AdversaryParty;
        public MobileParty PlayerParty;
    }
}
