using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using TaleWorlds.Core;
using TaleWorlds.Library;


namespace CustomSpawns.Data
{
    public class NameSignifierData
    {

        static NameSignifierData _instance;

        public static NameSignifierData Instance
        {
            get
            {
                return _instance ?? new NameSignifierData();
            }
            private set
            {
                _instance = value;

            }
        }


        private Dictionary<string, string> IDToName = new Dictionary<string, string>();
        private Dictionary<string, float> IDToSpeedModifier = new Dictionary<string, float>();
        private NameSignifierData()
        {
            if (!Main.isAPIMode)
            {
                string path = Path.Combine(BasePath.Name, "Modules", "CustomSpawns", "ModuleData", "Data", "NameSignifiers.xml");
                ConstructFromXML(path);
            }
            foreach(var subMod in ModIntegration.SubModManager.dependentModsArray)
            {
                string path = Path.Combine(subMod.CustomSpawnsDirectoryPath, "NameSignifiers.xml");
                if (File.Exists(path))
                    ConstructFromXML(path);
            }

        }

        private void ConstructFromXML(string path)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(path);

                foreach (XmlNode node in doc.DocumentElement)
                {
                    if (node.NodeType == XmlNodeType.Comment)
                        continue;
                    if (node.Attributes["id"] == null || node.Attributes["value"] == null)
                    {
                        ErrorHandler.HandleException(new Exception("There must be an id and value attribute defined for each element in NameSignifiers.xml"));
                        continue;
                    }
                    string id = node.Attributes["id"].InnerText;
                    if (!IDToName.ContainsKey(id))
                    {
                        IDToName.Add(id, node.Attributes["value"].InnerText);
                        if (node.Attributes["speed_modifier"] != null)
                        {

                            float result;
                            if (!float.TryParse(node.Attributes["speed_modifier"].InnerText, out result))
                            {
                                throw new Exception("Please enter a valid float for the speed modifier!");
                            }
                            if(!IDToSpeedModifier.ContainsKey(id))
                            IDToSpeedModifier.Add(id, result);
                        }
                    }
                }

            }
            catch (Exception e)
            {
                ErrorHandler.HandleException(e, "Name Signifier Data Parsing of " + path);
            }
        }

        public string GetPartyNameFromID(string id)
        {
            return IDToName[id];
        }

        public float GetSpeedModifierFromID(string id)
        {
            return IDToSpeedModifier[id];
        }
    }
}
