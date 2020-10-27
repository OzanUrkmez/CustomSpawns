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
            if (!Main.isAPIMode)
            {
                string path = Path.Combine(BasePath.Name, "Modules", "CustomSpawns", "ModuleData", "Data", "CustomDialogue.xml");
                ConstructListFromXML(path);
            }
            foreach (var subMod in ModIntegration.SubModManager.dependentModsArray)
            {
                string path = Path.Combine(subMod.CustomSpawnsDirectoryPath, "CustomDialogue.xml");
                if (File.Exists(path))
                    ConstructListFromXML(path);
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

                foreach (XmlNode childNode in node)
                {
                    if (childNode.NodeType == XmlNodeType.Comment)
                        continue;

                    dat.IsPlayer = childNode["IsPlayerLine"] == null ? false : bool.Parse(childNode["IsPlayerLine"].InnerText);

                    string template = "";

                    if (childNode.Name == "DialogueLine")
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

                        template = childNode.Attributes["template"].InnerText;
                    }
                    else
                    {
                        throw new Exception("DialogueCondition node must exist for every dialogue!");
                    }

                    if (childNode.Name == "DialogueConsequence")
                    {
                        dat.Consequence = ParseConsequence(childNode.Attributes["type"].InnerText);
                    }
                    else
                    {
                        throw new Exception("DialogueConsequence node must exist for every dialogue!");
                    }

                    dat.Parameters = new CustomSpawnsDialogueParams(template);

                    data.Add(dat);
                }
            }
        }

        private CustomSpawnsDialogueBehavior.CSDialogueCondition ParseCondition(string text)
        {
            switch(text)
            {
                case "none":
                    return CustomSpawnsDialogueBehavior.CSDialogueCondition.None;
                case "PartyTemplate":
                    return CustomSpawnsDialogueBehavior.CSDialogueCondition.PartyTemplate;
                case "PartyTemplateAndDefenderHostile":
                    return CustomSpawnsDialogueBehavior.CSDialogueCondition.PartyTemplateAndDefenderHostile;
                case "PartyTemplateAndAttackerHostile":
                    return CustomSpawnsDialogueBehavior.CSDialogueCondition.PartyTemplateAndAttackerHostile;
                default:
                    throw new Exception("A dialogue condition type wasn't explicity defined! If it is supposed to be always true, use 'none' please!");
            }
        }

        private CustomSpawnsDialogueBehavior.CSDialogueConsequence ParseConsequence(string text)
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
                default:
                    throw new Exception("A dialogue consequence type wasn't explicity defined! If it is supposed to be always true, use 'none' please!");
            }
        }
    }

    public class DialogueData
    {
        public bool IsPlayer { get; set; }
        public string Id { get; set; }
        public string InToken { get; set; }
        public string OutToken { get; set; }
        public string DialogueText { get; set; }
        public int Priority { get; set; }
        public CustomSpawnsDialogueBehavior.CSDialogueCondition Condition { get; set; }
        public CustomSpawnsDialogueBehavior.CSDialogueConsequence Consequence { get; set; }
        public CustomSpawnsDialogueParams Parameters { get; set; }
    }
}
