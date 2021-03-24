using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.Core;
using CustomSpawns.Dialogues;

using CustomSpawns.Dialogues.DialogueAlgebra;
using System.Text.RegularExpressions;

namespace CustomSpawns.Data
{
    public class DialogueDataManager
    {
        static DialogueDataManager _instance;

        public static DialogueDataManager Instance
        {
            get
            {
                return _instance ?? new DialogueDataManager();
            }
            private set
            {
                _instance = value;

            }
        }

        private List<DialogueData> data = new List<DialogueData>();

        public IList<DialogueData> Data
        {
            get
            {
                return data.AsReadOnly();
            }
        }

        public static void ClearInstance(Main caller)
        {
            if (caller == null)
                return;
            _instance = null;
        }

        private static int currentID = 0;

        private DialogueDataManager()
        {
            try
            {
                if (!Main.isAPIMode)
                {
                    string path = Path.Combine(BasePath.Name, "Modules", "CustomSpawns", "ModuleData", "Data", "CustomDialogue.xml"); // the usual path deal, located in the CustomSpawns or Data folder
                    ConstructListFromXML(path);
                }
                foreach (var subMod in ModIntegration.SubModManager.dependentModsArray)
                {
                    string path = Path.Combine(subMod.CustomSpawnsDirectoryPath, "CustomDialogue.xml");
                    if (File.Exists(path))
                        ConstructListFromXML(path);
                }
            }
            catch (Exception e)
            {
                ErrorHandler.HandleException(e, " Dialogue System XML loading");
            }


        }

        private void ConstructListFromXML(string filePath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            foreach (XmlNode node in doc.DocumentElement)
            {
                if (node.NodeType == XmlNodeType.Comment)
                    continue;

                DialogueData dat = new DialogueData();

                if (node.Attributes["condition"] != null)
                {
                    dat.Condition = ParseCondition(node.Attributes["condition"].Value);
                }

                dat.DialogueText = node.Attributes["text"]?.Value;

                dat.Dialogue_ID = "CS_Dialogue_" + currentID.ToString();

                data.Add(dat);

                currentID++;
            }
        }

        private DialogueCondition ParseCondition(string text)
        {
            //TODO: Parse AND, OR, etc to make use of the algebra system.

            try
            {

                //Simple as possible for now. No paranthesis support to group

                string[] tokens = text.Split(' ');

                if(tokens.Length == 0)
                {
                    //user has entered empty condition string
                    return new DialogueConditionBare((p) => true, "");
                }else if(tokens.Length == 1)
                {
                    //just a good old single function.
                    return ParseConditionToken(tokens[0]);
                }else if(tokens.Length % 2 == 0)
                {
                    throw new Exception("Invalid algebraic expression: " + text);
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
                            throw new Exception("Unrecognized logic keyword: " + tokens[i]);
                        }

                    }
                }

                return aggregate;

            }
            catch(Exception e)
            {
                ErrorHandler.ShowPureErrorMessage("Could not parse dialogue condition: \n" + text + "\n Error Message: \n" + e.Message);
                return null;
            }


        }

        private DialogueCondition ParseConditionToken(string token)
        {
            //function and its parameters
            string[] openPSplit = token.Split('(');

            string funcName = openPSplit[0];

            //get rid of trailing

            for (int j = 1; j < openPSplit.Length; j++)
            {
                openPSplit[j] = openPSplit[j].TrimEnd(',', ')');
            }


            switch (openPSplit.Length)
            {
                case 0:
                    throw new Exception("Can't parse " + token + ". It may be empty.");
                case 1:
                    //no params
                    return DialogueConditionsManager.GetDialogueCondition(funcName);
                case 2:
                    // 1 param
                    return DialogueConditionsManager.GetDialogueCondition(funcName, openPSplit[1]);
                case 3:
                    // 2 params
                    return DialogueConditionsManager.GetDialogueCondition(funcName, openPSplit[1], openPSplit[2]);
                case 4:
                    // 3 params
                    return DialogueConditionsManager.GetDialogueCondition(funcName, openPSplit[1], openPSplit[2], openPSplit[3]);
                default:
                    throw new Exception("Can't parse " + token + ". Possibly too many params.");
            }
        }
    }

    public class DialogueData
    {
        public string DialogueText { get; set; }
        public string Dialogue_ID { get; set; }
        public DialogueCondition Condition { get; set; }
    }
}
