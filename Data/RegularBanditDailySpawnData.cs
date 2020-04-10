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
            string path = Path.Combine(BasePath.Name, "Modules", "CustomSpawns", "ModuleData", "Data", "RegularBanditDailySpawn.xml");
            ConstructListFromXML(path);
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

                    if (node.Attributes["overriden_spawn_clan"].Value == "")
                    {
                        dat.OverridenSpawnClan = null;
                    }
                    else
                    {
                        dat.OverridenSpawnClan = (Clan)MBObjectManager.Instance.ReadObjectReferenceFromXml("overriden_spawn_clan", typeof(Clan), node);
                    }

                    dat.MaximumOnMap = int.Parse(node["MaximumOnMap"].InnerText);
                    if(dat.MaximumOnMap == 0)
                    {
                        throw new Exception("the node 'MaximumOnMap' cannot be set to 0!");
                    }
                    dat.ChanceOfSpawn = float.Parse(node["ChanceOfSpawn"].InnerText);
                    dat.Name = node["Name"].InnerText;
                    dat.ChanceInverseConstant = float.Parse(node["ChanceInverseConstant"].InnerText);
                    dat.RepeatSpawnRolls = int.Parse(node["RepeatSpawnRolls"].InnerText);

                    //message
                    string msg = node["SpawnMessage"] == null? "" : node["SpawnMessage"].Value;
                    string color = node["SpawnMessageColor"] == null ? "" : node["SpawnMessageColor"].Value;

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
        public Clan OverridenSpawnClan { get; set; }
        public int MaximumOnMap { get; set; }
        private float chanceOfSpawn;
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

}
