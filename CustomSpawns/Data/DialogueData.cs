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
            }catch(Exception e)
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

                string c_template = "";
                string c_trait = "";
                bool c_friendly = true; // declaring these here so we can access them later
                bool c_playerTrait = true;
                int c_value = 0;
                bool c_barterSuccess = true;
                string c_lordName = "";
                string c_faction = "";

                bool cs_playerSurrender = false;

                foreach (XmlNode childNode in node)
                {
                    if (childNode.NodeType == XmlNodeType.Comment)
                        continue;

                    if (childNode.Name == "IsPlayerLine")
                    {
                        dat.IsPlayer = childNode == null ? false : bool.Parse(childNode.InnerText);
                    }

                    if (childNode.Name == "DialogueLine") // checking for the four types of node: DialogueLine, IsPlayerLine, DialogueCondition and DialogueConsequence. they all need to be present
                    {
                        dat.Id = childNode.Attributes["id"].InnerText;
                        dat.InToken = childNode.Attributes["in_token"].InnerText;
                        dat.OutToken = childNode.Attributes["out_token"].InnerText;
                        dat.DialogueText = childNode.Attributes["text"].InnerText;
                        dat.Priority = int.Parse(childNode.Attributes["priority"].InnerText);
                    }

                    if (childNode.Name == "DialogueCondition")
                    {
                        dat.Condition = ParseCondition(childNode.Attributes["type"].InnerText);

                        c_template = childNode.Attributes["template"] == null ? "" : childNode.Attributes["template"].InnerText;
                        c_friendly = childNode.Attributes["is_friendly"] == null ? true : bool.Parse(childNode.Attributes["is_friendly"].InnerText);
                        c_playerTrait = childNode.Attributes["player_trait"] == null ? true : bool.Parse(childNode.Attributes["player_trait"].InnerText); // setting the condition parameters here (they can be reused depending on the delegate)
                        c_trait = childNode.Attributes["trait"] == null ? "" : childNode.Attributes["trait"].InnerText;
                        c_value = childNode.Attributes["value"] == null ? 0 : int.Parse(childNode.Attributes["value"].InnerText);
                        c_barterSuccess = childNode.Attributes["successful_barter"] == null ? true : bool.Parse(childNode.Attributes["successful_barter"].InnerText);
                        c_lordName = childNode.Attributes["lord_name"] == null ? "" : childNode.Attributes["lord_name"].InnerText;
                        c_faction = childNode.Attributes["faction"] == null ? "" : childNode.Attributes["faction"].InnerText;
                    }

                    if (childNode.Name == "DialogueConsequence")
                    {
                        dat.Consequence = ParseConsequence(childNode.Attributes["type"].InnerText);

                        cs_playerSurrender = childNode.Attributes["player_surrender"] == null ? false : bool.Parse(childNode.Attributes["player_surrender"].InnerText); // now setting the consequence parameters
                    }
                }
                dat.Parameters = new CustomSpawnsDialogueParams(c_template, c_friendly, cs_playerSurrender, c_playerTrait, c_trait, c_value, c_barterSuccess, c_lordName, c_faction); // now we init a new DialogueParameters class to be used later, read comments there as to why it's a class and not a struct

                data.Add(dat);
            }
        }

        private CustomSpawnsDialogueBehavior.CSDialogueCondition ParseCondition(string text) // just a little enum switch- more can be added easily
        {
            switch(text)
            {
                case "none":
                    return CustomSpawnsDialogueBehavior.CSDialogueCondition.None;
                case "PartyTemplateAndStance":
                    return CustomSpawnsDialogueBehavior.CSDialogueCondition.PartyTemplateAndStance;
                case "PartyTemplateDefenderAndStance":
                    return CustomSpawnsDialogueBehavior.CSDialogueCondition.PartyTemplateDefenderAndStance;
                case "PartyTemplateAttackerAndStance":
                    return CustomSpawnsDialogueBehavior.CSDialogueCondition.PartyTemplateAttackerAndStance;
                case "WarCheck":
                    return CustomSpawnsDialogueBehavior.CSDialogueCondition.GenericWar;
                case "CharacterTrait":
                    return CustomSpawnsDialogueBehavior.CSDialogueCondition.CharacterTrait;
                case "CheckLastBarter":
                    return CustomSpawnsDialogueBehavior.CSDialogueCondition.LastBarter;
                case "FirstConversationLordName":
                    return CustomSpawnsDialogueBehavior.CSDialogueCondition.FirstConversationLordName;
                case "FirstConversationFaction":
                    return CustomSpawnsDialogueBehavior.CSDialogueCondition.FirstConversationFaction;
                case "FactionDefault":
                    return CustomSpawnsDialogueBehavior.CSDialogueCondition.FactionDefault;
                default:
                    throw new Exception("A dialogue condition type wasn't explicity defined! If it is supposed to be always true, use 'none' please!"); // making sure the modder isn't passing nonsense
            }
        }

        private CustomSpawnsDialogueBehavior.CSDialogueConsequence ParseConsequence(string text) // same with the conditions, easy ability to add new ones
        {
            switch (text)
            {
                case "none":
                    return CustomSpawnsDialogueBehavior.CSDialogueConsequence.None;
                case "EndConversation":
                    return CustomSpawnsDialogueBehavior.CSDialogueConsequence.EndConversation;
                case "EndConversationInBattle":
                    return CustomSpawnsDialogueBehavior.CSDialogueConsequence.EndConversationInBattle;
                case "DeclareWar":
                    return CustomSpawnsDialogueBehavior.CSDialogueConsequence.DeclareWar;
                case "DeclarePeace":
                    return CustomSpawnsDialogueBehavior.CSDialogueConsequence.DeclarePeace;
                case "EndConversationSurrender":
                    return CustomSpawnsDialogueBehavior.CSDialogueConsequence.EndConversationSurrender;
                case "BarterForPeace":
                    return CustomSpawnsDialogueBehavior.CSDialogueConsequence.BarterForPeace;
                default:
                    throw new Exception("A dialogue consequence type wasn't explicity defined! If it is supposed to be always true, use 'none' please!"); // ditto
            }
        }
    }

    public class DialogueData
    {
        public bool IsPlayer { get; set; }
        public string Id { get; set; }
        public string InToken { get; set; }
        public string OutToken { get; set; }
        public string DialogueText { get; set; } // all of these are properties just in case to prevent fuckery
        public int Priority { get; set; }
        public CustomSpawnsDialogueBehavior.CSDialogueCondition Condition { get; set; }
        public CustomSpawnsDialogueBehavior.CSDialogueConsequence Consequence { get; set; }
        public CustomSpawnsDialogueParams Parameters { get; set; }
    }
}
