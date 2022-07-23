using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Linq;
using CustomSpawns.Dialogues;
using CustomSpawns.Dialogues.DialogueAlgebra;
using CustomSpawns.Exception;
using CustomSpawns.Utils;
using TaleWorlds.Library;

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

        private List<DialogueData> rootDialogueData = new List<DialogueData>();

        public IList<DialogueData> Data
        {
            get
            {
                return rootDialogueData.AsReadOnly();
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
                string path = "";
#if !API_MODE
                path = Path.Combine(BasePath.Name, "Modules", "CustomSpawns", "ModuleData", "Data", "CustomDialogue.xml"); // the usual path deal, located in the CustomSpawns or Data folder
                ParseDialogueFile(path);
#endif
                foreach (var subMod in ModIntegration.SubModManager.LoadAllValidDependentMods())
                {
                    path = Path.Combine(subMod.CustomSpawnsDirectoryPath, "CustomDialogue.xml");
                    if (File.Exists(path))
                        ParseDialogueFile(path);
                }
            }
            catch (System.Exception e)
            {
                ErrorHandler.HandleException(e, " Dialogue System XML loading");
            }


        }

        private void ParseDialogueFile(string filePath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            foreach (XmlNode node in doc.DocumentElement)
            {
                
                ParseDialogueNode(node, null);

            }
        }

        private void ParseDialogueNode(XmlNode node, DialogueData XMLParent)
        {
            if (node.NodeType == XmlNodeType.Comment)
                return;

            DialogueData dat;

            if(node.Name == "AlternativeDialogue")
            {
                //Dialogue alternative to parent.
                if(XMLParent == null)
                {
                    throw new TechnicalException(node.Name + " is not a valid Custom Spawns Dialogue Token!");
                }

                dat = InitializeDialogueNode(node, XMLParent.Parent, XMLParent); // initialize with same parameters as our XML parent.

                return;
            }

            if (node.Name != "Dialogue")
            {
                throw new TechnicalException(node.Name + " is not a valid Custom Spawns Dialogue Token!");
            }

            //regular dialogue node.

            dat = InitializeDialogueNode(node, XMLParent, null);

            //Now process children.

            foreach (XmlNode child in node)
            {
                ParseDialogueNode(child, dat);
            }
        }

        private DialogueData InitializeDialogueNode(XmlNode node, DialogueData XMLParent, DialogueData alternativeTarget)
        {
            DialogueData dat = new DialogueData();

            dat.InjectedToTaleworlds = false;

            //NODE RELATIONS

            dat.Parent = XMLParent;

            if(dat.Parent == null)
            {
                rootDialogueData.Add(dat);
            }
            else
            {
                XMLParent.Children.Add(dat);
            }

            if(alternativeTarget == null)
            {
                dat.Children = new List<DialogueData>();

                dat.Dialogue_ID = "CS_Dialogue_" + currentID.ToString();

                currentID++;
            }
            else
            {
                dat.Children = alternativeTarget.Children;

                dat.Dialogue_ID = alternativeTarget.Dialogue_ID;
            }

            //NODE PROPERTIES

            if (node.Attributes["condition"] != null)
            {
                dat.Condition = ParseCondition(node.Attributes["condition"].Value);
            }

            if (node.Attributes["consequence"] != null)
            {
                dat.Consequence = ParseConsequence(node.Attributes["consequence"].Value);
            }

            bool isPlayerDialogue;

            if (!bool.TryParse(node.Attributes["player"]?.Value, out isPlayerDialogue))
            {
                dat.IsPlayerDialogue = false;
            }
            else
            {
                dat.IsPlayerDialogue = isPlayerDialogue;
            }

            dat.DialogueText = node.Attributes["text"]?.Value;


            return dat;
        }



        #region Condition Parsing

        private DialogueCondition ParseCondition(string text)
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
                    returned = DialogueConditionsManager.GetDialogueCondition(funcName);
                    break;
                case 2:
                    // 1 param
                    returned = DialogueConditionsManager.GetDialogueCondition(funcName, openPSplit[1]);
                    break;
                case 3:
                    // 2 params
                    returned = DialogueConditionsManager.GetDialogueCondition(funcName, openPSplit[1], openPSplit[2]);
                    break;
                case 4:
                    // 3 params
                    returned = DialogueConditionsManager.GetDialogueCondition(funcName, openPSplit[1], openPSplit[2], openPSplit[3]);
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

        #endregion

        #region Consequence Parsing

        private DialogueConsequence ParseConsequence(string text)
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
                    returned = DialogueConsequencesManager.GetDialogueConsequence(funcName);
                    break;
                case 2:
                    // 1 param
                    returned = DialogueConsequencesManager.GetDialogueConsequence(funcName, openPSplit[1]);
                    break;
                case 3:
                    // 2 params
                    returned = DialogueConsequencesManager.GetDialogueConsequence(funcName, openPSplit[1], openPSplit[2]);
                    break;
                case 4:
                    // 3 params
                    returned = DialogueConsequencesManager.GetDialogueConsequence(funcName, openPSplit[1], openPSplit[2], openPSplit[3]);
                    break;
                default:
                    throw new TechnicalException("Can't parse " + token + ". Possibly too many params.");
            }

            return returned;
        }

        #endregion
    }

    public class DialogueData
    {
        public string DialogueText { get; set; }
        public string Dialogue_ID { get; set; }
        public DialogueCondition Condition { get; set; }
        public DialogueConsequence Consequence { get; set; }
        public bool IsPlayerDialogue { get; set; }

        public List<DialogueData> Children { get; set; }

        public DialogueData Parent { get; set; }

        public bool InjectedToTaleworlds { get; set; }
    }
}
