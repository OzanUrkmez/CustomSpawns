using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using CustomSpawns.Data;
using CustomSpawns.Dialogues.DialogueAlgebra;
using CustomSpawns.Dialogues.DialogueAlgebra.Condition;
using CustomSpawns.Exception;
using CustomSpawns.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Party;

namespace CustomSpawns.Dialogues
{
    public class DialogueConditionInterpretor
    {

        public DialogueConditionInterpretor()
        {
            allMethods = typeof(DialogueConditionInterpretor).GetMethods(BindingFlags.Static | BindingFlags.NonPublic).
                 Where((m) => m.GetCustomAttributes(typeof(DialogueConditionImplementorAttribute), false).Count() > 0).ToList();
        }

        #region Getters

        //TODO: cache at constructor and thus optimize if need be with a dictionary. Might not be needed though since this only runs once at the start of campaign.

        //just realized: can probably implement this more elegantly with just an array of strings than copying 4 functions! Consider if you want more params

        private static List<MethodInfo> allMethods;

        public DialogueCondition ParseCondition(string text)
        {
            if(text.Length == 0)
            {
                return null;
            }

            try
            {

                //Simple as possible for now. No paranthesis support to group

                string[] tokens = text.Split(' ');

                if(tokens.Length == 0)
                {
                    //user has entered empty condition string
                    return null;
                }else if(tokens.Length == 1)
                {
                    //just a good old single function.
                    return ParseConditionToken(tokens[0]);
                }else if(tokens.Length % 2 == 0)
                {
                    throw new TechnicalException("Invalid algebraic expression: " + text);
                }

                //tokens.Length is thus at least 3.

                DialogueCondition aggregate = null;
                DialogueCondition cur = null;
                 
                aggregate = ParseConditionToken(tokens[0]);

                for (int i = 2; i < tokens.Length; i += 2)
                {
                    if (i % 2 == 0)
                    {

                        cur = ParseConditionToken(tokens[i]);

                        i -= 3; //we will add 2 to this and so we will get to the logic keyword.
                    }
                    else
                    {
                        //logic keyword AND OR 

                        if(tokens[i] == "AND" || tokens[i] == "&" || tokens[i] == "&&")
                        {
                            aggregate = aggregate & cur;
                        }
                        else if(tokens[i] == "OR" || tokens[i] == "|" || tokens[i] == "||")
                        {
                            aggregate = aggregate | cur;
                        }
                        else
                        {
                            throw new TechnicalException("Unrecognized logic keyword: " + tokens[i]);
                        }

                        i += 1; // we will add 2 to this and so we will get to next token.

                    }
                }

                return aggregate;

            }
            catch(System.Exception e)
            {
                ErrorHandler.ShowPureErrorMessage("Could not parse dialogue condition: \n" + text + "\n Error Message: \n" + e.Message);
                return null;
            }


        }

        private DialogueCondition ParseConditionToken(string token)
        {
            //function and its parameters
            List<string> openPSplit = token.Split('(', ',').ToList();

            string funcName = openPSplit[0];

            //get rid of trailing

            for (int j = 1; j < openPSplit.Count; j++)
            {
                openPSplit[j] = openPSplit[j].TrimEnd(',', ')');
                //remove empty
                if(openPSplit[j].Length == 0)
                {
                    openPSplit.RemoveAt(j);
                    j--;
                }
            }


            bool negationFlag = false;

            if(funcName[0] == '!')
            {
                negationFlag = true;
                funcName = funcName.TrimStart('!');
            }

            DialogueCondition returned = null;

            switch (openPSplit.Count)
            {
                case 0:
                    throw new TechnicalException("Can't parse " + token + ". It may be empty.");
                case 1:
                    //no params
                    returned = GetDialogueCondition(funcName);
                    break;
                case 2:
                    // 1 param
                    returned = GetDialogueCondition(funcName, openPSplit[1]);
                    break;
                case 3:
                    // 2 params
                    returned = GetDialogueCondition(funcName, openPSplit[1], openPSplit[2]);
                    break;
                case 4:
                    // 3 params
                    returned = GetDialogueCondition(funcName, openPSplit[1], openPSplit[2], openPSplit[3]);
                    break;
                default:
                    throw new TechnicalException("Can't parse " + token + ". Possibly too many params.");
            }

            if (negationFlag)
            {
                returned = !returned;
            }

            return returned;
        }
        
        private DialogueCondition GetDialogueCondition(string implementor)
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

            throw new TechnicalException("There is no function with name " + implementor + " that takes no parameters.");
        }

        private DialogueCondition GetDialogueCondition(string implementor, string param)
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

            throw new TechnicalException("There is no function with name " + implementor + " that takes one parameter.");
        }

        private DialogueCondition GetDialogueCondition(string implementor, string param1, string param2)
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

            throw new TechnicalException("There is no function with name " + implementor + " that takes two parameters.");
        }

        private DialogueCondition GetDialogueCondition(string implementor, string param1, string param2, string param3)
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

            throw new TechnicalException("There is no function with name " + implementor + " that takes three parameters.");
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

            return CampaignUtils.IsolateMobilePartyStringID(param.AdversaryParty).StartsWith(ID);
        }

        [DialogueConditionImplementor("PartyIDEndsWith")]
        private static bool PartyIDEndsWith(DialogueParams param, string ID)
        {
            if (param.AdversaryParty == null)
                return false;

            return CampaignUtils.IsolateMobilePartyStringID(param.AdversaryParty).EndsWith(ID);
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

        [DialogueConditionImplementor("IsCustomSpawnParty")]
        private static bool IsCustomSpawnParty(DialogueParams param)
        {
            // TODO refactor when these "managers" are not singletons anymore. 
            return SpawnDataManager.Instance.AllSpawnData().Keys.Any(partyId => param?.AdversaryParty?.StringId?.StartsWith(partyId) ?? false);
        }

        [DialogueConditionImplementor("IsPlayerEncounterInsideSettlement")]
        private static bool IsPlayerEncounterInsideSettlement(DialogueParams param)
        {
            return PlayerEncounter.InsideSettlement;
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
