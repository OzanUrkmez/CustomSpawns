using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
//using CustomSpawns.MCMv3;
using TaleWorlds.Library;

namespace CustomSpawns.PrisonerRecruitment
{
    class PrisonerRecruitmentConfigLoader
    {

        private static PrisonerRecruitmentConfigLoader _instance = null;
        public PrisonerRecruitmentConfig Config { get; private set; }

        public static PrisonerRecruitmentConfigLoader Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PrisonerRecruitmentConfigLoader();
                }

                return _instance;
            }
        }

        private PrisonerRecruitmentConfigLoader()
        {
            string path = "";
            if (!Main.isAPIMode)
            {
                path = Path.Combine(BasePath.Name, "Modules", "CustomSpawns", "ModuleData", "prisoner_recruitment_config.xml");
            }
            else
            {
                path = Path.Combine(BasePath.Name, "Modules", "CustomSpawnsCleanAPI", "ModuleData", "prisoner_recruitment_config.xml");
            }
            Config = getConfig(path);
        }

        private PrisonerRecruitmentConfig getConfig(String filePath)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(PrisonerRecruitmentConfig));
                using (var reader = new StreamReader(filePath))
                {
                    PrisonerRecruitmentConfig returned = (PrisonerRecruitmentConfig)serializer.Deserialize(reader);
                    HandleConfig(returned);
                    return returned;
                }
            }
            catch (Exception e)
            {
                ErrorHandler.HandleException(e);
                PrisonerRecruitmentConfig config = new PrisonerRecruitmentConfig();
                return config;
            }
        }

        private void HandleConfig(PrisonerRecruitmentConfig config)
        {
            if (config.PrisonRecruitmentDebugEnabled && !ConfigLoader.Instance.Config.IsDebugMode)
                config.PrisonRecruitmentDebugEnabled = false;
        }
    }
}

