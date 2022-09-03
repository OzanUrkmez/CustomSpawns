using System.Collections.Generic;
using System.IO;
using System.Xml;
using CustomSpawns.Dialogues;
using CustomSpawns.Dialogues.DialogueAlgebra;
using CustomSpawns.Exception;
using CustomSpawns.Utils;
using TaleWorlds.Library;

namespace CustomSpawns.Data
{
    public class DialogueDataManager
    {
        private readonly DialogueConsequenceInterpretor _dialogueConsequenceInterpretor = new();
        private readonly DialogueConditionInterpretor _dialogueConditionInterpretor = new();
        
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
                DefaultDialogue defaultDialogue = new (_dialogueConditionInterpretor, _dialogueConsequenceInterpretor);
                rootDialogueData.AddRange(defaultDialogue.DefaultDialogueData);
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
                dat.Condition = _dialogueConditionInterpretor.ParseCondition(node.Attributes["condition"].Value);
            }

            if (node.Attributes["consequence"] != null)
            {
                dat.Consequence = _dialogueConsequenceInterpretor.ParseConsequence(node.Attributes["consequence"].Value);
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



        #endregion

        #region Consequence Parsing



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
