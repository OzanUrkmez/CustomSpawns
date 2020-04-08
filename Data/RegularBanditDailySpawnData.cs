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

namespace Banditlord.Data
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
            string path = Path.Combine(BasePath.Name, "Modules", "Banditlord", "ModuleData", "Data", "RegularBanditDailySpawn.xml");
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

                    if(node.Attributes["overriden_spawn_clan"].Value == "")
                    {
                        dat.OverridenSpawnClan = null;
                    }
                    else
                    {
                        dat.OverridenSpawnClan = (Clan)MBObjectManager.Instance.ReadObjectReferenceFromXml("overriden_spawn_clan", typeof(Clan), node);
                    }

                    dat.MaximumOnMap = int.Parse(node["MaximumOnMap"].Value);
                    dat.ChanceOfSpawn = float.Parse(node["ChanceOfSpawn"].Value);
                    dat.Name = node["Name"].Value;

                    data.Add(dat);
                }
            }catch(Exception e)
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
        public float ChanceOfSpawn { get; set; }
        public PartyTemplateObject PartyTemplate { get; set; }
        public string Name { get; set; }

        private int numberSpawned = 0;

        private void IncrementNumberSpawned()
        {
            numberSpawned++;
        }

        private void DecrementNumberSpawned()
        {
            numberSpawned--;
        }

        public bool CanSpawn()
        {
            return numberSpawned < MaximumOnMap;
        }
    }

}
