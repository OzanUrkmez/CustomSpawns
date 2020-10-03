using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using TaleWorlds.Library;

namespace CustomSpawns.CampaignData
{
    class CampaignDataConfigLoader
    {

        private static CampaignDataConfigLoader _instance = null;


        public static CampaignDataConfigLoader Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CampaignDataConfigLoader();
                }

                return _instance;
            }
        }

        private CampaignDataConfigLoader()
        {
            string path = "";
            if (!Main.isAPIMode)
            {
                path = Path.Combine(BasePath.Name, "Modules", "CustomSpawns", "ModuleData", "custom_spawns_campaign_data_config.xml");
            }
            else
            {
                path = Path.Combine(BasePath.Name, "Modules", "CustomSpawnsCleanAPI", "ModuleData", "custom_spawns_campaign_data_config.xml");
            }
            ConstructConfigs(path);
        }

        private void ConstructConfigs(string xmlPath)
        {

        }

        private static T DeserializeNode<T>(XNode data) where T : class, new()
        {
            if (data == null)
                return null;

            var ser = new XmlSerializer(typeof(T));
            return (T)ser.Deserialize(data.CreateReader());
        }
    }

}

