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
    [Serializable]
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

        private NameSignifierData()
        {
            string path = Path.Combine(BasePath.Name, "Modules", "CustomSpawns", "ModuleData", "Data", "NameSignifiers.xml");
            ConstructFromXML(path);
        }

        private void ConstructFromXML(string path)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);

            foreach(XmlNode node in doc.DocumentElement)
            {
                if(node.Attributes["id"] == null || node.Attributes["value"] == null)
                {
                    ErrorHandler.HandleException(new Exception("There must be an id and value attribute defined for each element in NameSignifiers.xml"));
                    continue;
                }
                IDToName.Add(node.Attributes["id"].InnerText, node.Attributes["value"].InnerText);
            }
        }

        public string GetPartyNameFromID(string id)
        {
            return IDToName[id];
        }
    }
}
