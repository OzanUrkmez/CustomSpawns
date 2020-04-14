using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.Core;

namespace CustomSpawns.Data
{
    [Serializable]
    public class RegularBanditDailySpawnDataManager
    {

        static RegularBanditDailySpawnDataManager _instance;

        public static RegularBanditDailySpawnDataManager Instance
        {
            get
            {
                return _instance ?? new RegularBanditDailySpawnDataManager();
            }
            private set
            {
                _instance = value;
                
            }
        }

        private List<RegularBanditDailySpawnData> data = new List<RegularBanditDailySpawnData>();

        public IList<RegularBanditDailySpawnData> Data
        {
            get
            {
                return data.AsReadOnly();
            }
        }

        private RegularBanditDailySpawnDataManager()
        {
            if (!Main.isAPIMode)
            {
                string path = Path.Combine(BasePath.Name, "Modules", "CustomSpawns", "ModuleData", "Data", "RegularBanditDailySpawn.xml");
                ConstructListFromXML(path);
            }
            foreach (var subMod in ModIntegration.SubModManager.dependentModsArray)
            {
                ConstructListFromXML(Path.Combine(subMod.CustomSpawnsDirectoryPath, "RegularBanditDailySpawn.xml"));
            }
        }

        private void ConstructListFromXML(string filePath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);
            try
            {
                foreach (XmlNode node in doc.DocumentElement)
                {
                    RegularBanditDailySpawnData dat = new RegularBanditDailySpawnData();

                    dat.PartyTemplate = (PartyTemplateObject)MBObjectManager.Instance.ReadObjectReferenceFromXml("party_template", typeof(PartyTemplateObject), node);
                    dat.BanditClan = (Clan)MBObjectManager.Instance.ReadObjectReferenceFromXml("bandit_clan", typeof(Clan), node);

                    //have bannerlord read attributes

                    int i = 0;
                    string s = "overriden_spawn_clan";
                    while (true)
                    {
                        string s1 = s + "_" + i.ToString();
                        if (node.Attributes[s1] == null || node.Attributes[s1].InnerText == "")
                        {
                            break;
                        }
                        else
                        {
                            dat.OverridenSpawnClan.Add((Clan)MBObjectManager.Instance.ReadObjectReferenceFromXml(s1, typeof(Clan), node));
                        }
                        i++;
                    }

                    int j = 0;
                    string st = "overriden_spawn_culture";
                    while (true)
                    {
                        string s1 = st + "_" + j.ToString();
                        if (node.Attributes[s1] == null || node.Attributes[s1].InnerText == "")
                        {
                            break;
                        }
                        else
                        {
                            dat.OverridenSpawnCultures.Add(((CultureObject)MBObjectManager.Instance.ReadObjectReferenceFromXml(s1, typeof(CultureObject), node)).GetCultureCode());
                        }
                        j++;
                    }

                    //get elements
                    dat.MaximumOnMap = node["MaximumOnMap"] == null? 0 : int.Parse(node["MaximumOnMap"].InnerText);
                    if (dat.MaximumOnMap < 1)
                    {
                        throw new Exception("the node 'MaximumOnMap' cannot be less than 1!");
                    }

                    dat.ChanceOfSpawn = node["ChanceOfSpawn"] == null? 1 : float.Parse(node["ChanceOfSpawn"].InnerText);
                    dat.Name = node["Name"] == null ? "Unnamed" : node["Name"].InnerText;
                    dat.ChanceInverseConstant = node["ChanceInverseConstant"] == null? 0 : float.Parse(node["ChanceInverseConstant"].InnerText);
                    dat.RepeatSpawnRolls = node["RepeatSpawnRolls"] == null? 1 : int.Parse(node["RepeatSpawnRolls"].InnerText);

                    dat.PatrolAroundSpawn = node["PatrolAroundSpawn"] == null ? false : bool.Parse(node["PatrolAroundSpawn"].InnerText);
                    dat.MinimumNumberOfDaysUntilSpawn = node["MinimumNumberOfDaysUntilSpawn"] == null ? -1 : int.Parse(node["MinimumNumberOfDaysUntilSpawn"].InnerText);

                    //message
                    string msg = node["SpawnMessage"] == null? "" : node["SpawnMessage"].InnerText;
                    string color = node["SpawnMessageColor"] == null ? "" : node["SpawnMessageColor"].InnerText;

                    if(msg != "")
                    {
                        if(color == "")
                        {
                            dat.spawnMessage = new InformationMessage(msg, Color.Black);
                        }
                        else
                        {
                            Color c = UX.GetMessageColour(color) == "" ? (color[0] == '#'? Color.ConvertStringToColor(color) : Color.Black) : Color.ConvertStringToColor(UX.GetMessageColour(color));
                            dat.spawnMessage = new InformationMessage(msg, c);
                        }
                    }

                    //handle extra linear speed.
                    if(node["ExtraLinearSpeed"] != null)
                    {
                        float extra = float.Parse(node["ExtraLinearSpeed"].InnerText);
                        Main.customSpeedModel.RegisterPartyExtraSpeed(dat.PartyTemplate.StringId, extra);
                    }

                    //Spawn along with
                    int k = 0;
                    string str = "spawn_along_with";
                    while (true)
                    {
                        string s1 = str + "_" + k.ToString();
                        if (node.Attributes[s1] == null || node.Attributes[s1].InnerText == "")
                        {
                            break;
                        }
                        else
                        {
                            PartyTemplateObject pt = (PartyTemplateObject)MBObjectManager.Instance.ReadObjectReferenceFromXml(s1, typeof(PartyTemplateObject), node);
                            dat.SpawnAlongWith.Add(new AccompanyingParty(pt, NameSignifierData.Instance.GetPartyNameFromID(pt.StringId)));
                            Main.customSpeedModel.RegisterPartyExtraSpeed(pt.StringId, NameSignifierData.Instance.GetSpeedModifierFromID(pt.StringId));
                        }
                        k++;
                    }

                    data.Add(dat);
                }
            }
            catch (Exception e)
            {
                ErrorHandler.HandleException(e);
            }
        }

    }
    public class RegularBanditDailySpawnData
    {
        public Clan BanditClan { get; set; }
        public List<Clan> OverridenSpawnClan = new List<Clan>();
        public List<CultureCode> OverridenSpawnCultures = new List<CultureCode>();
        public List<AccompanyingParty> SpawnAlongWith = new List<AccompanyingParty>();
        public int MaximumOnMap { get; set; }
        private float chanceOfSpawn;
        public int MinimumNumberOfDaysUntilSpawn { get; set; }
        public float ChanceOfSpawn
        {
            get
            {
                return chanceOfSpawn + ChanceInverseConstant * (float)(MaximumOnMap - numberSpawned) / (float)(MaximumOnMap);
            }
            set
            {
                chanceOfSpawn = value;
            }
        }
        public float ChanceInverseConstant { private get; set; }
        public PartyTemplateObject PartyTemplate { get; set; }
        public string Name { get; set; }
        public int RepeatSpawnRolls { get; set; }
        public InformationMessage spawnMessage { get; set; }
        public bool PatrolAroundSpawn { get; set; }
        private int numberSpawned = 0;

        public void IncrementNumberSpawned()
        {
            numberSpawned++;
        }

        public void DecrementNumberSpawned()
        {
            numberSpawned--;
        }

        public void SetNumberSpawned(int num)
        {
            numberSpawned = num;
        }

        public int GetNumberSpawned()
        {
            return numberSpawned;
        }

        public bool CanSpawn()
        {
            return numberSpawned < MaximumOnMap;
        }


    }

    public struct AccompanyingParty
    {
        public PartyTemplateObject templateObject;
        public string name;

        public AccompanyingParty(PartyTemplateObject pt, string n)
        {
            templateObject = pt;
            name = n;
        }
    }

}
