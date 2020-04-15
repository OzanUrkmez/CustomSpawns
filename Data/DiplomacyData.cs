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

namespace CustomSpawns.Data
{
    public class DiplomacyDataManager
    {

        static DiplomacyDataManager _instance;

        public static DiplomacyDataManager Instance
        {
            get
            {
                return _instance ?? new DiplomacyDataManager();
            }
            private set
            {
                _instance = value;

            }
        }

        private List<DiplomacyData> data = new List<DiplomacyData>();

        public IList<DiplomacyData> Data
        {
            get
            {
                return data.AsReadOnly();
            }
        }

        private DiplomacyDataManager()
        {
            if (!Main.isAPIMode)
            {
                string path = Path.Combine(BasePath.Name, "Modules", "CustomSpawns", "ModuleData", "Data", "Diplomacy.xml");
                ConstructListFromXML(path);
            }
            foreach (var subMod in ModIntegration.SubModManager.dependentModsArray)
            {
                string path = Path.Combine(subMod.CustomSpawnsDirectoryPath, "Diplomacy.xml");
                if (File.Exists(path))
                    ConstructListFromXML(path);
            }
        }

        private void ConstructListFromXML(string path)
        {

        }

    }

    public class DiplomacyData
    {

    }
}
