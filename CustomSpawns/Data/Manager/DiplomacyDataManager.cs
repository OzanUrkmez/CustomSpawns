using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using CustomSpawns;
using CustomSpawns.Data;
using MonoMod.Utils;
using TaleWorlds.Library;

namespace Data.Manager
{
    public class DiplomacyDataManager : AbstractDataManager<DiplomacyDataManager, Dictionary<string,DiplomacyData>>
    {
        private DiplomacyDataManager()
        {
            string path = "";
            var diplomacyData = new Dictionary<string,DiplomacyData>();
            try
            {
#if !API_MODE
                path = Path.Combine(BasePath.Name, "Modules", "CustomSpawns", "ModuleData", "Data", "Diplomacy.xml");
                diplomacyData = ConstructListFromXML(path);
#endif
                foreach (var subMod in ModIntegration.SubModManager.dependentModsArray)
                {
                    path = Path.Combine(subMod.CustomSpawnsDirectoryPath, "Diplomacy.xml");
                    if (File.Exists(path))
                        diplomacyData.AddRange(ConstructListFromXML(path));
                }

                Data = diplomacyData;
            }
            catch (Exception e)
            {
                throw new TechnicalException("Diplomacy Data Parsing of " + path, e);
            }
        }

        private Dictionary<string,DiplomacyData> ConstructListFromXML(string path)
        {
            Dictionary<string, DiplomacyData> data = new Dictionary<string, DiplomacyData>();
            XmlDocument doc = new XmlDocument();
            doc.Load(path);

            foreach (XmlNode node in doc.DocumentElement)
            {
                if (node.NodeType == XmlNodeType.Comment)
                    continue;
                DiplomacyData diplomacyData = new DiplomacyData();
                if(node.Attributes["target"] == null || node.Attributes["target"].InnerText == "")
                {
                    throw new TechnicalException("Each diplomacy data instance must have a target faction!");
                }
                diplomacyData.clanString = node.Attributes["target"].InnerText;
                if (node["ForceWarPeaceBehaviour"] != null)
                {
                    //handle forced war peace data.
                    diplomacyData.ForcedWarPeaceDataInstance = new DiplomacyData.ForcedWarPeaceData();
                    XmlElement forceNode = node["ForceWarPeaceBehaviour"];
                    HandleForcedWarPeaceBehaviourData(forceNode, diplomacyData);
                }
                if(node["ForceNoKingdom"] != null)
                {
                    //handle forcing of no parent kingdoms.
                    bool result;
                    if(!bool.TryParse(node["ForceNoKingdom"].InnerText, out result))
                    {
                        throw new TechnicalException("ForceNoKingdom must be a boolean value!");
                    }
                    diplomacyData.ForceNoKingdom = result;
                }

                data.Add(diplomacyData.clanString, diplomacyData);
            }

            return data;
        }

        private void HandleForcedWarPeaceBehaviourData(XmlElement forceNode, DiplomacyData diplomacyData)
        {
            foreach (XmlNode forceNodeChild in forceNode)
            {
                if (forceNodeChild.NodeType == XmlNodeType.Comment)
                    continue;
                if (forceNodeChild.Name == "ForcedWarSpecial")
                {
                    //handle forced war special.
                    if (forceNodeChild.Attributes["flag"] == null)
                    {
                        throw new TechnicalException("Each forced war special data must have a flag.");
                    }
                    List<string> exceptionClans = new List<string>();
                    List<string> exceptionKingdoms = new List<string>();
                    string flag = forceNodeChild.Attributes["flag"].InnerText;
                    switch (flag)
                    {
                        case "all": //handle case where All clans except maybe some are designated as enemies.
                            int j = 0;
                            string st = "but";
                            while (true)
                            {
                                string s1 = st + "_" + j.ToString();
                                if (forceNodeChild.Attributes[s1] == null || forceNodeChild.Attributes[s1].InnerText == "")
                                {
                                    break;
                                }
                                else
                                {
                                    exceptionClans.Add(forceNodeChild.Attributes[s1].InnerText);
                                }
                                j++;
                            }
                            j = 0;
                            st = "but_kingdom";
                            while (true)
                            {
                                string s1 = st + "_" + j.ToString();
                                if (forceNodeChild.Attributes[s1] == null || forceNodeChild.Attributes[s1].InnerText == "")
                                {
                                    break;
                                }
                                else
                                {
                                    exceptionKingdoms.Add(forceNodeChild.Attributes[s1].InnerText);
                                }
                                j++;
                            }
                            break;
                        default:
                            throw new TechnicalException("Invalid forced war special data flag detected");
                    }

                    diplomacyData.ForcedWarPeaceDataInstance.AtPeaceWithClans = exceptionClans;
                    diplomacyData.ForcedWarPeaceDataInstance.ExceptionKingdoms = exceptionKingdoms;
                }
            }
        }

    }
}