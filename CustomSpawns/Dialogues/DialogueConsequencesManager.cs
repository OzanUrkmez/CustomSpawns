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
    public static class DialogueConsequencesManager
    {

        static DialogueConsequencesManager()
        {
            allMethods = typeof(DialogueConditionsManager).GetMethods(BindingFlags.Static | BindingFlags.NonPublic).
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
