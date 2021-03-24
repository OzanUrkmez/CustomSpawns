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
                        ((x) => ((Func<DialogueConditionParams, bool>)m.CreateDelegate(typeof(Func<DialogueConditionParams, bool>)))(x),
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
                        ((Func<DialogueConditionParams, string, bool>)m.CreateDelegate(typeof(Func<DialogueConditionParams, string, bool>)),
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
                        ((Func<DialogueConditionParams, string, string, bool>)m.CreateDelegate(typeof(Func<DialogueConditionParams, string, string, bool>)),
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
                        ((Func<DialogueConditionParams, string, string, string, bool>)m.CreateDelegate(typeof(Func<DialogueConditionParams, string, string, string, bool>)),
                        param1, param2, param3, a.ExposedName + "(" + param1 + ", " + param2 + ", " + param3 +")");
                } 
            }

            throw new Exception("There is no function with name " + implementor + " that takes three parameters.");
        }

        #endregion

        [DialogueConditionImplementor("HasPartyID")]
        private static bool HasPartyID(DialogueConditionParams param, string ID)
        {
            if (param.Adversary == null)
                return false;

            if (CampaignUtils.IsolateMobilePartyStringID(param.Adversary.Party.MobileParty) == ID)
            {
                return true;
            }
            return false;
        }

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

    public class DialogueConditionParams
    {
        public MobileParty Adversary;
        public MobileParty PlayerParty;
    }
}
