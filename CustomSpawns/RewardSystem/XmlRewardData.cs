using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using CustomSpawns.RewardSystem.Models;
using TaleWorlds.Library;

namespace CustomSpawns.RewardSystem
{
    public class XmlRewardData
    {
        private static XmlRewardData _instance = null;

        public List<PartyReward> PartyRewards { get; set; } = new List<PartyReward>();

        private XmlRewardData()
        {
            LoadDataFromXml();
        }

        public static XmlRewardData GetInstance()
        {
            if (_instance == null)
            {
                _instance = new XmlRewardData();
            }

            return _instance;
        }

        private void LoadDataFromXml()
        {
            string pathToTamplate = "";
            string pathToSchema = "";
            if (!Main.isAPIMode)
            {
                pathToTamplate = Path.Combine(BasePath.Name, "Modules", "CustomSpawns", "ModuleData", "Data",
                    "PartyRewards.xml");
                pathToSchema = Path.Combine(BasePath.Name, "Modules", "CustomSpawns", "Schema",
                    "PartyRewardTemplateSchema.xsd");
            }
            else
            {
                pathToTamplate = Path.Combine(BasePath.Name, "Modules", "CustomSpawnsCleanAPI", "ModuleData", "Data",
                    "PartyRewards.xml");
                pathToSchema = Path.Combine(BasePath.Name, "Modules", "CustomSpawnsCleanAPI", "Schema",
                    "PartyRewardTemplateSchema.xsd");
            }

            // load xml schema for validation
            try
            {
                using (var streamSchema = new StreamReader(pathToSchema))
                {
                    var schema = XmlSchema.Read(streamSchema, new ValidationEventHandler(ValidationEventHandler));
                    var settings = new XmlReaderSettings();
                    settings.Schemas.Add(schema);
                    settings.ValidationType = ValidationType.Schema;
                    // define settings for xml doc reading and validating
                    using (var streamDocument = XmlReader.Create(pathToTamplate, settings))
                    {
                        // read the xml document
                        var document = new XmlDocument();
                        document.Load(streamDocument);
                        // validate the xml document
                        var eventHandler = new ValidationEventHandler(ValidationEventHandler);
                        document.Validate(eventHandler);

                        foreach (XmlNode partyRewardNode in document.DocumentElement)
                        {
                            if (partyRewardNode.NodeType == XmlNodeType.Comment)
                                continue;
                            string partyId;
                            List<Reward> rewardsList;
                            if (partyRewardNode.FirstChild.Name == "PartyId")
                            {
                                partyId = partyRewardNode.FirstChild.InnerText;
                                rewardsList = RewardLoader(partyRewardNode.LastChild);
                            }
                            else
                            {
                                partyId = partyRewardNode.LastChild.InnerText;
                                rewardsList = RewardLoader(partyRewardNode.FirstChild);
                            }
                            // add the new partyReword
                            this.PartyRewards.Add(new PartyReward(partyId, rewardsList));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ErrorHandler.HandleException(e);
            }

        }

        static void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            ErrorHandler.HandleException(e.Exception);
        }

        private List<Reward> RewardLoader(XmlNode node)
        {
            var rewardsList = new List<Reward>();
            foreach (XmlNode rewards in node)
            {
                var reward = new Reward();
                foreach (XmlNode rewardNode in rewards.ChildNodes)
                {
                    switch (rewardNode.Name)
                    {
                        case "Type":
                            reward.Type = this.getRewardType(rewardNode.InnerText) ?? RewardType.Money;
                            break;
                        case "RenownInfluenceMoneyAmount":
                            reward.RenownInfluenceMoneyAmount = Convert.ToInt32(rewardNode.InnerText);
                            break;
                        case "Chance":
                            reward.Chance = Convert.ToDecimal(rewardNode.InnerText);
                            break;
                        case "ItemId":
                            reward.ItemId = rewardNode.InnerText;
                            break;
                    }
                }
                rewardsList.Add(reward);
            }

            return rewardsList;
        }

        private RewardType? getRewardType(string type)
        {
            switch (type)
            {
                case "Money":
                    return RewardType.Money;
                case "Renown":
                    return RewardType.Renown;
                    break;
                case "Influence":
                    return RewardType.Influence;
                case "Item":
                    return RewardType.Item;
            }

            return null;
        }
    }
}